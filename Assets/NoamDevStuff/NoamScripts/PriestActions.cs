using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PriestActions : MonoBehaviour
{
    [Header("Priest")]
    [SerializeField] private FpsMovement priestFpsMovement;

    [Header("Raycast (center of screen)")]
    [SerializeField] private Camera viewCamera;
    [SerializeField] private float interactDistance = 4f;
    [SerializeField] private LayerMask rayMask = ~0;

    [Header("Hiding Lerp")]
    [SerializeField] private float lerpDuration = 0.6f;

    [Header("Disable Input While Busy")]
    [Tooltip("Drag the PlayerInput component from the priest here (the component, not the GameObject).")]
    [SerializeField] private PlayerInput priestInput;

    [Header("Movement Script")]
    [Tooltip("Drag your FpsMovement component here (must have public bool IsUserBusyWalking).")]
    [SerializeField] private FpsMovement fpsMovement;

    [Header("State")]
    // True while entering (lerping), while hiding (staying inside), and while exiting (lerping).
    // False only after fully exiting the hiding spot.
    public bool IsHiding { get; private set; }

    [Header("Gizmos")]
    [SerializeField] private Color gizmoRayColor = new Color(1f, 0.2f, 0.2f, 1f);

    private bool _isTransitioning;
    private Coroutine _lerpRoutine;

    private Transform _lastHitTransform;
    private Transform _currentHidableRoot;

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

        // Not hiding at the start
        IsHiding = false;
    }

    private void Update()
    {
        UpdateLookRaycast();

        if (Mouse.current != null && Mouse.current.rightButton.wasPressedThisFrame)
            OnRightClick();
    }

    private void UpdateLookRaycast()
    {
        _lastHitTransform = null;

        if (!viewCamera) return;

        Ray ray = viewCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, rayMask, QueryTriggerInteraction.Ignore))
        {
            _lastHitTransform = hit.collider.transform;
        }
    }

    public void OnRightClick()
    {
        if (_isTransitioning) return;

        // If already in hiding, right click exits.
        // We use IsUserBusyWalking as the state for movement blocking,
        // and IsHiding as the overall "hiding system" state.
        if (IsBusy())
        {
            if (_currentHidableRoot == null) return;

            Transform exitSpot = FindDeepChildWithTagIncludingSelf(_currentHidableRoot, "ExitHidingSpot");
            if (exitSpot == null) return;

            // Exiting: IsHiding stays true while we lerp out.
            StartLerpTo(exitSpot, onCompleteExit: true);
            return;
        }

        // Not busy: try to enter
        if (_lastHitTransform == null) return;

        Transform root = _lastHitTransform.parent != null ? _lastHitTransform.parent : _lastHitTransform;

        Transform enterSpot = FindDeepChildWithTagIncludingSelf(root, "EnterHidingSpot");
        if (enterSpot == null) return;

        _currentHidableRoot = root;

        // Entering begins: IsHiding becomes true immediately (includes the lerp-in).
        IsHiding = true;

        // While entering, player becomes "busy" and input is disabled only during the lerp.
        SetBusy(true);
        StartLerpTo(enterSpot, onCompleteExit: false);
    }

    private void StartLerpTo(Transform target, bool onCompleteExit)
    {
        if (target == null) return;

        if (_lerpRoutine != null)
            StopCoroutine(_lerpRoutine);

        _lerpRoutine = StartCoroutine(LerpRoutine(target, onCompleteExit));
        priestFpsMovement.LockViewTo(target);
    }

    private IEnumerator LerpRoutine(Transform target, bool onCompleteExit)
    {
        _isTransitioning = true;

        float duration = Mathf.Max(0.0001f, lerpDuration);

        // Disable input ONLY while lerping
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

        // Re-enable input after lerp (but keep Busy=true while inside hiding spot)
        SetInputEnabled(true);

        if (onCompleteExit)
        {
            // Fully exited -> Busy ends here, and IsHiding becomes false ONLY here.
            SetBusy(false);
            _currentHidableRoot = null;
            IsHiding = false;
        }
        else
        {
            // Arrived inside hiding -> remain Busy + IsHiding true until user exits later.
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

        Gizmos.color = gizmoRayColor;

        Ray ray = viewCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        Gizmos.DrawRay(ray.origin, ray.direction * interactDistance);
        Gizmos.DrawWireSphere(ray.origin + ray.direction * interactDistance, 0.08f);
    }
}
