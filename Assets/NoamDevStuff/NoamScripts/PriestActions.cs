using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PriestActions : MonoBehaviour
{
    [Header("Priest")]
    [SerializeField] private FpsMovement priestFpsMovement;

    [Header("Raycast (center of screen)")]
    [SerializeField] private Camera viewCamera;
    [SerializeField] private float interactDistance = 10f;

    [Tooltip("Layers considered for ray hits (usually everything except ignore-raycast layers).")]
    [SerializeField] private LayerMask rayMask = ~0;

    [Header("Objective Ghost Prop Filter")]
    [Tooltip("The TAG on the ghost prop you spawn (e.g. 'Objective').")]
    [SerializeField] private string objectiveGhostTag = "Objective";

    [Header("Hiding Lerp")]
    [SerializeField] private float lerpDuration = 0.6f;

    [Header("Disable Input While Busy")]
    [SerializeField] private PlayerInput priestInput;

    [Header("Movement Script")]
    [SerializeField] private FpsMovement fpsMovement;

    [Header("State")]
    public bool IsHiding { get; private set; }

    [Header("Events")]
    [SerializeField] private UnityEvent onInteractEvent;

    [Header("Gizmos")]
    [SerializeField] private Color gizmoRayColor = new Color(1f, 0.2f, 0.2f, 1f);

    [Tooltip("When true, draws the interact ray and shows what it hits in the Scene view.")]
    [SerializeField] private bool drawInteractGizmo = true;

    [SerializeField] private Color interactMissColor = new Color(1f, 0.8f, 0.2f, 1f);
    [SerializeField] private Color interactHitColor  = new Color(0.2f, 1f, 0.2f, 1f);

    private bool _isTransitioning;
    private Coroutine _lerpRoutine;

    private Transform _lastHitTransform;
    private Transform _currentHidableRoot;

    // Cache last interact-ray result for gizmos (so gizmos reflects the same logic)
    private bool _lastInteractHit;
    private Vector3 _lastInteractHitPoint;
    private bool _lastInteractWasObjective;
    private string _lastInteractHitName;

    private void Awake()
    {
        if (!viewCamera) viewCamera = Camera.main;
        if (!priestInput) priestInput = GetComponent<PlayerInput>();
        if (!fpsMovement) fpsMovement = GetComponent<FpsMovement>();
    }

    private void Start()
    {
        SetBusy(false);
        SetInputEnabled(true);
        IsHiding = false;
    }

    private void Update()
    {
        UpdateLookRaycast();
        UpdateInteractGizmoCache(); // NEW: updates the gizmo cache every frame
    }

    private void UpdateLookRaycast()
    {
        _lastHitTransform = null;
        if (!viewCamera) return;

        Ray ray = viewCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, rayMask, QueryTriggerInteraction.Ignore))
            _lastHitTransform = hit.collider.transform;
    }

    // NEW: Interact ray logic (shared by OnInteract + gizmos)
    private bool TryGetObjectiveGhostHit(out RaycastHit bestHit)
    {
        bestHit = default;

        if (!viewCamera) return false;

        Ray ray = viewCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        // IMPORTANT: Collide so trigger colliders (your ghost prop) are included
        RaycastHit[] hits = Physics.RaycastAll(ray, interactDistance, rayMask, QueryTriggerInteraction.Collide);
        if (hits == null || hits.Length == 0) return false;

        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        for (int i = 0; i < hits.Length; i++)
        {
            Collider col = hits[i].collider;
            if (!col) continue;

            // Only accept trigger colliders (the ghost prop collider isTrigger)
            if (!col.isTrigger) continue;

            // Tag can be on the collider object OR on the root parent
            if (col.CompareTag(objectiveGhostTag) || col.transform.root.CompareTag(objectiveGhostTag))
            {
                bestHit = hits[i];
                return true;
            }
        }

        return false;
    }


    // Press E -> if aiming at ghost prop with Objective tag -> invoke event
    public void OnInteract()
    {
        if (_isTransitioning) return;

        if (TryGetObjectiveGhostHit(out _))
            onInteractEvent?.Invoke();
    }

    // NEW: Cache gizmo info for the interact ray
    private void UpdateInteractGizmoCache()
    {
        _lastInteractHit = false;
        _lastInteractWasObjective = false;
        _lastInteractHitName = null;
        _lastInteractHitPoint = Vector3.zero;

        if (!drawInteractGizmo) return;
        if (!viewCamera) return;

        Ray ray = viewCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        // Find the first hit (any collider) for showing where the ray is blocked
        if (Physics.Raycast(ray, out RaycastHit firstHit, interactDistance, rayMask, QueryTriggerInteraction.Ignore))
        {
            _lastInteractHit = true;
            _lastInteractHitPoint = firstHit.point;
            _lastInteractHitName = firstHit.collider ? firstHit.collider.name : null;
        }

        // Also check whether there is an objective ghost hit (might be behind blockers if using RaycastAll)
        if (TryGetObjectiveGhostHit(out RaycastHit objectiveHit))
        {
            _lastInteractWasObjective = true;
            _lastInteractHit = true;
            _lastInteractHitPoint = objectiveHit.point;
            _lastInteractHitName = objectiveHit.collider ? objectiveHit.collider.name : null;
        }
    }

    public void OnRightClick()
    {
        if (_isTransitioning) return;

        if (IsBusy())
        {
            if (_currentHidableRoot == null) return;

            Transform exitSpot = FindDeepChildWithTagIncludingSelf(_currentHidableRoot, "ExitHidingSpot");
            if (exitSpot == null) return;

            StartLerpTo(exitSpot, onCompleteExit: true);
            return;
        }

        if (_lastHitTransform == null) return;

        Transform root = _lastHitTransform.parent != null ? _lastHitTransform.parent : _lastHitTransform;

        Transform enterSpot = FindDeepChildWithTagIncludingSelf(root, "EnterHidingSpot");
        if (enterSpot == null) return;

        _currentHidableRoot = root;

        IsHiding = true;
        SetBusy(true);
        StartLerpTo(enterSpot, onCompleteExit: false);
    }

    private void StartLerpTo(Transform target, bool onCompleteExit)
    {
        if (target == null) return;

        if (_lerpRoutine != null)
            StopCoroutine(_lerpRoutine);

        _lerpRoutine = StartCoroutine(LerpRoutine(target, onCompleteExit));

        if (priestFpsMovement != null)
            priestFpsMovement.LockViewTo(target);
    }

    private IEnumerator LerpRoutine(Transform target, bool onCompleteExit)
    {
        _isTransitioning = true;

        float duration = Mathf.Max(0.0001f, lerpDuration);

        SetInputEnabled(false);

        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;

        Vector3 endPos = target.position;
        Quaternion endRot = target.rotation;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            float a = Mathf.Clamp01(t);

            transform.position = Vector3.Lerp(startPos, endPos, a);
            transform.rotation = Quaternion.Slerp(startRot, endRot, a);

            yield return null;
        }

        transform.position = endPos;
        transform.rotation = endRot;

        _isTransitioning = false;
        _lerpRoutine = null;

        SetInputEnabled(true);

        if (onCompleteExit)
        {
            SetBusy(false);
            _currentHidableRoot = null;
            IsHiding = false;
        }
        else
        {
            SetBusy(true);
            IsHiding = true;
        }
    }

    private bool IsBusy()
    {
        return fpsMovement != null && fpsMovement.IsUserBusyWalking;
    }

    private void SetBusy(bool busy)
    {
        if (fpsMovement != null)
            fpsMovement.IsUserBusyWalking = busy;
    }

    private void SetInputEnabled(bool enabled)
    {
        if (priestInput != null)
            priestInput.enabled = enabled;
    }

    private static Transform FindDeepChildWithTagIncludingSelf(Transform parent, string tag)
    {
        if (parent == null) return null;

        if (parent.CompareTag(tag))
            return parent;

        for (int i = 0; i < parent.childCount; i++)
        {
            Transform c = parent.GetChild(i);
            Transform r = FindDeepChildWithTagIncludingSelf(c, tag);
            if (r != null)
                return r;
        }

        return null;
    }

    private void OnDrawGizmos()
    {
        if (!viewCamera) return;

        // Existing generic ray gizmo
        Gizmos.color = gizmoRayColor;
        Ray baseRay = viewCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        Gizmos.DrawRay(baseRay.origin, baseRay.direction * interactDistance);
        Gizmos.DrawWireSphere(baseRay.origin + baseRay.direction * interactDistance, 0.08f);

        // NEW: Interact gizmo (uses interact logic)
        if (!drawInteractGizmo) return;

        Ray ray = viewCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        // Draw line in different color depending on whether we're aiming at an objective ghost
        Gizmos.color = _lastInteractWasObjective ? interactHitColor : interactMissColor;
        Gizmos.DrawRay(ray.origin, ray.direction * interactDistance);

        // If something is hit, mark the hit point
        if (_lastInteractHit)
        {
            Gizmos.DrawWireSphere(_lastInteractHitPoint, 0.12f);
        }
    }
}
