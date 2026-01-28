using UnityEngine;
using UnityEngine.InputSystem;

public class PriestMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CharacterController controller;
    [SerializeField] private Transform cameraPivot;
    
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    
    [Header("Look")]
    [SerializeField] private float mouseSensitivity = 0.12f;
    [SerializeField] private float minPitch = -80f;
    [SerializeField] private float maxPitch = 80f;
    [SerializeField] private bool invertY;
    
    [Header("Cursor")]
    [SerializeField] private bool lockCursorOnStart = true;
    
    
    // fields
    private Vector2 _moveInput;
    private Vector2 _lookInput;
    private float _pitch;
    private bool _isOnAngle;
    
    private void Awake()
    {
        // Starting the internal pitch value at whatever the camera is already looking at,
        // and expressing it in a sane -180° to +180° format so clamping works.
        if (cameraPivot != null)
        {
            _pitch = cameraPivot.localEulerAngles.x;
            if (_pitch > 180f)
                _pitch -= 360f;
        }
    }
    private void Start()
    {
        
        // making priest to snap onto the cursor location
        if (lockCursorOnStart)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
    private void Update()
    {
        // cant preform movement inputs like walking and looking if on angel
        if (!_isOnAngle)
        {
            WalkingUpdateLogic();
            LookingUpdateLogic();    
        }
        // put gravity logic last
        GravityLogic();
    }
    private void LookingUpdateLogic()
    {
        var yaw = _lookInput.x * mouseSensitivity;
        var pitchDelta = _lookInput.y * mouseSensitivity * (invertY ? 1f : -1f);

        // rotate body (yaw)
        transform.Rotate(0f, yaw, 0f, Space.Self);
        
        if (cameraPivot == null) return;
        
        // rotate camera (pitch)
        _pitch = Mathf.Clamp(_pitch + pitchDelta, minPitch, maxPitch);
        cameraPivot.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
    }
    private void WalkingUpdateLogic()
    {
        var move =
            transform.right * _moveInput.x +
            transform.forward * _moveInput.y;

        controller.Move(move * (moveSpeed * Time.deltaTime));
    }

    private void GravityLogic()
    {
        // gravity
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
    // switching to the angle and back
    public void OnSwitchCamera(InputAction.CallbackContext context)
    {
        _isOnAngle = !_isOnAngle;
    }
}


