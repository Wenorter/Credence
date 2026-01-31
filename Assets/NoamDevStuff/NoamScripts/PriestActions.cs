using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PriestActions : MonoBehaviour
{
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

        // If already in hiding (busy), right click exits.
        // We use IsUserBusyWalking as the state instead of IsHiding.
        if (IsBusy())
        {
            if (_currentHidableRoot == null) return;

            Transform exitSpot = FindDeepChildWithTagIncludingSelf(_currentHidableRoot, "ExitHidingSpot");
            if (exitSpot == null) return;

            StartLerpTo(exitSpot, onCompleteExit: true);
            return;
        }

        // Not busy: try to enter
        if (_lastHitTransform == null) return;

        // Use parent if exists, otherwise the collider transform itself (works either way)
        Transform root = _lastHitTransform.parent != null ? _lastHitTransform.parent : _lastHitTransform;

        Transform enterSpot = FindDeepChildWithTagIncludingSelf(root, "EnterHidingSpot");
        if (enterSpot == null) return;

        _currentHidableRoot = root;

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
            // Fully exited -> Busy ends here.
            SetBusy(false);
            _currentHidableRoot = null;
        }
        else
        {
            // Arrived inside hiding -> remain Busy until user exits later.
            SetBusy(true);
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

        // Check self first
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
