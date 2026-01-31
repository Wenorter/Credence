using UnityEngine;
using UnityEngine.InputSystem;

public class FpsMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CharacterController controller;
    [SerializeField] private Transform cameraPivot;         // camera pitch pivot
    [SerializeField] private Transform cameraTransform;     // actual Camera transform (child of pivot)

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private bool isEffectedByGravity = true;

    [Header("Look")]
    [SerializeField] private float mouseSensitivity = 0.12f;
    [SerializeField] private float minPitch = -80f;
    [SerializeField] private float maxPitch = 80f;
    [SerializeField] private bool invertY;

    [Header("Lerp")]
    [SerializeField] private bool isLerping;          // smooth movement & look when true
    [SerializeField] private float moveLerpSpeed = 10f;
    [SerializeField] private float lookLerpSpeed = 15f;

    [Header("Cursor")]
    [SerializeField] private bool lockCursorOnStart = true;

    [Header("Camera Collision")]
    [SerializeField] private bool enableCameraCollision = true;

    [Tooltip("Layers that can block the camera (exclude Player).")]
    [SerializeField] private LayerMask cameraCollisionMask = ~0;

    [Tooltip("Sphere radius for camera collision checks.")]
    [SerializeField] private float cameraCollisionRadius = 0.2f;

    [Tooltip("Extra gap from the wall to avoid buzzing.")]
    [SerializeField] private float cameraCollisionBuffer = 0.06f;

    [Tooltip("How quickly camera distance changes (lower = smoother, less flicker).")]
    [SerializeField] private float cameraDistanceSmoothTime = 0.06f;

    [Tooltip("Optional: prevents camera from going fully to 0 distance (helps extreme close-ups).")]
    [SerializeField] private float minCameraDistance = 0f;

    [Header("Busy")]
    public bool IsUserBusyWalking;
    public bool IsUserBusyLooking;

    // input state
    private Vector2 _moveInput;
    private Vector2 _lookInput;

    // look state
    private float _pitch;
    private float _currentYaw;
    private float _targetPitch;
    private float _targetYaw;

    // movement state
    private Vector3 _currentMove;

    // camera collision state
    private Vector3 _defaultCamLocalPos;
    private Vector3 _camLocalDir;
    private float _defaultCamDistance;

    private float _camDistanceCurrent;
    private float _camDistanceTarget;
    private float _camDistanceVel;

    private void Awake()
    {
        // Auto-find cameraTransform if not assigned
        if (cameraTransform == null && cameraPivot != null)
        {
            var cam = cameraPivot.GetComponentInChildren<Camera>();
            if (cam != null) cameraTransform = cam.transform;
        }

        if (cameraPivot != null)
        {
            _pitch = cameraPivot.localEulerAngles.x;
            if (_pitch > 180f) _pitch -= 360f;
        }

        _currentYaw = transform.eulerAngles.y;

        _targetPitch = _pitch;
        _targetYaw = _currentYaw;

        // Cache camera default offset + direction
        if (cameraTransform != null && cameraPivot != null)
        {
            _defaultCamLocalPos = cameraTransform.localPosition;
            _defaultCamDistance = _defaultCamLocalPos.magnitude;
            _camLocalDir = (_defaultCamDistance > 0.0001f) ? (_defaultCamLocalPos / _defaultCamDistance) : Vector3.back;

            _camDistanceCurrent = _defaultCamDistance;
            _camDistanceTarget = _defaultCamDistance;
        }
    }

    private void Start()
    {
        if (lockCursorOnStart)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void Update()
    {
        WalkingUpdateLogic();
        LookingUpdateLogic();
        GravityLogic();
    }

    private void LateUpdate()
    {
        // Important: do collision in LateUpdate after controller.Move to avoid jitter/flicker.
        if (enableCameraCollision)
            UpdateCameraCollisionSmoothed();
    }

    private void LookingUpdateLogic()
    {
        // If you actually want to block looking when busy, uncomment:
        // if (IsUserBusyLooking) return;

        var yawDelta = _lookInput.x * mouseSensitivity;
        var pitchDelta = _lookInput.y * mouseSensitivity * (invertY ? 1f : -1f);

        _targetYaw += yawDelta;
        _targetPitch = Mathf.Clamp(_targetPitch + pitchDelta, minPitch, maxPitch);

        if (isLerping)
        {
            _currentYaw = Mathf.Lerp(_currentYaw, _targetYaw, lookLerpSpeed * Time.deltaTime);
            _pitch = Mathf.Lerp(_pitch, _targetPitch, lookLerpSpeed * Time.deltaTime);
        }
        else
        {
            _currentYaw = _targetYaw;
            _pitch = _targetPitch;
        }

        transform.rotation = Quaternion.Euler(0f, _currentYaw, 0f);

        if (cameraPivot != null)
            cameraPivot.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
    }

    private void UpdateCameraCollisionSmoothed()
    {
        if (cameraPivot == null || cameraTransform == null) return;
        if (_defaultCamDistance < 0.0001f) return;

        float desiredDistance = _defaultCamDistance;

        Vector3 pivotWorldPos = cameraPivot.position;
        Vector3 desiredWorldPos = cameraPivot.TransformPoint(_camLocalDir * desiredDistance);

        Vector3 toDesired = desiredWorldPos - pivotWorldPos;
        float castDistance = toDesired.magnitude;

        float targetDistance = desiredDistance;

        if (castDistance > 0.0001f)
        {
            Vector3 castDir = toDesired / castDistance;

            // SphereCast from pivot to desired camera position
            if (Physics.SphereCast(
                    pivotWorldPos,
                    cameraCollisionRadius,
                    castDir,
                    out RaycastHit hit,
                    castDistance,
                    cameraCollisionMask,
                    QueryTriggerInteraction.Ignore))
            {
                float safeDist = hit.distance - (cameraCollisionRadius + cameraCollisionBuffer);
                targetDistance = Mathf.Clamp(safeDist, minCameraDistance, desiredDistance);
            }
        }

        _camDistanceTarget = targetDistance;

        // Smoothly change camera distance -> kills flicker when hit/no-hit toggles frame to frame.
        _camDistanceCurrent = Mathf.SmoothDamp(
            _camDistanceCurrent,
            _camDistanceTarget,
            ref _camDistanceVel,
            cameraDistanceSmoothTime
        );

        // Apply local position along the original local direction
        cameraTransform.localPosition = _camLocalDir * _camDistanceCurrent;
    }

    private void WalkingUpdateLogic()
    {
        if (IsUserBusyWalking) return;

        Vector3 targetMove;

        if (isEffectedByGravity)
        {
            targetMove =
                transform.right * _moveInput.x +
                transform.forward * _moveInput.y;
        }
        else
        {
            Vector3 lookDirection = cameraPivot != null ? cameraPivot.forward : transform.forward;

            Vector3 right = Vector3.Cross(Vector3.up, lookDirection).normalized;
            Vector3 forward = lookDirection.normalized;

            targetMove =
                right * _moveInput.x +
                forward * _moveInput.y;
        }

        if (isLerping)
        {
            _currentMove = Vector3.Lerp(_currentMove, targetMove, moveLerpSpeed * Time.deltaTime);
        }
        else
        {
            _currentMove = targetMove;
        }

        controller.Move(_currentMove * (moveSpeed * Time.deltaTime));
    }

    private void GravityLogic()
    {
        if (!isEffectedByGravity) return;

        if (!controller.isGrounded)
        {
            controller.Move(Physics.gravity * Time.deltaTime);
        }
    }

    public void LockViewTo(Transform viewTarget, bool lockCursor = true, bool hideCursor = true)
    {
        if (viewTarget == null) return;

        float yaw = viewTarget.eulerAngles.y;
        transform.rotation = Quaternion.Euler(0f, yaw, 0f);

        if (cameraPivot != null)
        {
            float pitch = viewTarget.eulerAngles.x;
            if (pitch > 180f) pitch -= 360f;

            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
            cameraPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);

            _pitch = pitch;
            _targetPitch = pitch;
        }

        _currentYaw = yaw;
        _targetYaw = yaw;

        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = !hideCursor;
        }

        _lookInput = Vector2.zero;

        // Re-run collision once after snapping
        if (enableCameraCollision)
            UpdateCameraCollisionSmoothed();
    }

    // PlayerInput Event: Move
    public void OnMove(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
    }

    // PlayerInput Event: Look
    public void OnLook(InputAction.CallbackContext context)
    {
        _lookInput = context.ReadValue<Vector2>();
    }
}
