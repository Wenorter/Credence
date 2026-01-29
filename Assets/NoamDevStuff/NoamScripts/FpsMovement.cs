using UnityEngine;
using UnityEngine.InputSystem;

public class FpsMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CharacterController controller;
    [SerializeField] private Transform cameraPivot; // camera pitch pivot
    
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
    
    // input state
    private Vector2 _moveInput;
    private Vector2 _lookInput;

    // look state
    private float _pitch;        // current vertical camera angle (smoothed)
    private float _currentYaw;   // current horizontal rotation (smoothed)

    private float _targetPitch;  // target vertical camera angle
    private float _targetYaw;    // target horizontal rotation

    // movement state
    private Vector3 _currentMove; // smoothed movement vector
    
    private void Awake()
    {
        if (cameraPivot != null)
        {
            _pitch = cameraPivot.localEulerAngles.x;
            if (_pitch > 180f)
                _pitch -= 360f;
        }

        _currentYaw = transform.eulerAngles.y;

        // start targets at current values so there's no snap on start
        _targetPitch = _pitch;
        _targetYaw = _currentYaw;
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

    private void LookingUpdateLogic()
    {
        // read mouse deltas
        var yawDelta = _lookInput.x * mouseSensitivity;
        var pitchDelta = _lookInput.y * mouseSensitivity * (invertY ? 1f : -1f);

        // build targets instantly (this is what makes smoothing actually work)
        _targetYaw += yawDelta;
        _targetPitch = Mathf.Clamp(_targetPitch + pitchDelta, minPitch, maxPitch);

        if (isLerping)
        {
            // smooth current values toward targets
            _currentYaw = Mathf.Lerp(_currentYaw, _targetYaw, lookLerpSpeed * Time.deltaTime);
            _pitch = Mathf.Lerp(_pitch, _targetPitch, lookLerpSpeed * Time.deltaTime);

            transform.rotation = Quaternion.Euler(0f, _currentYaw, 0f);

            if (cameraPivot == null) return;
            cameraPivot.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
        }
        else
        {
            // instant values match targets
            _currentYaw = _targetYaw;
            _pitch = _targetPitch;

            transform.rotation = Quaternion.Euler(0f, _currentYaw, 0f);

            if (cameraPivot == null) return;
            cameraPivot.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
        }
    }

    private void WalkingUpdateLogic()
    {
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
            _currentMove = Vector3.Lerp(
                _currentMove,
                targetMove,
                moveLerpSpeed * Time.deltaTime
            );
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
