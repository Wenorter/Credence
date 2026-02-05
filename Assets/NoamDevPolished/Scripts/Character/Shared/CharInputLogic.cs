using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
[RequireComponent(typeof(CharacterController))]
public class CharInputLogic : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Camera (or a camera pivot) under this character. Pitch is applied here (local X).")]
    [SerializeField] private Transform characterCamera;

    [Header("Movement")]
    [Tooltip("Meters per second on flat ground.")]
    [SerializeField] private float moveSpeed = 5.5f;

    [Tooltip("If false, we never apply gravity/vertical motion (useful for flying/debug).")]
    [SerializeField] private bool useGravity = true;

    [Tooltip("Downward acceleration (m/s^2).")]
    [SerializeField] private float gravity = 18f;

    [Tooltip("Small constant downward velocity while grounded to prevent 'hovering' on slopes.")]
    [SerializeField] private float groundedStickVelocity = 2f;

    [Header("Look")]
    [Tooltip("Multiplier for look input (mouse delta / stick).")]
    [SerializeField] private float lookSensitivity = 0.12f;

    [Tooltip("Lowest pitch allowed (looking down). Keep it within [-90..-5] so we never flip.")]
    [Range(-90f, -5f)]
    [SerializeField] private float minPitch = -80f;

    [Tooltip("Highest pitch allowed (looking up). Keep it within [5..90] so we never flip.")]
    [Range(5f, 90f)]
    [SerializeField] private float maxPitch = 80f;

    [Tooltip("Invert Y look if it feels more natural).")]
    [SerializeField] private bool invertY;

    [Header("Mouse Lock")]
    [Tooltip("If true, locks the mouse cursor to the center and hides it (standard FPS).")]
    [SerializeField] private bool lockMouseCursor = true;

    [Tooltip("If true, pressing Escape releases the cursor (useful while testing).")]
    [SerializeField] private bool escapeUnlocksCursor = true;

    private CharacterController _characterController;
    private Vector2 _moveInput;
    private Vector2 _lookInput;

    private float _pitch;
    private float _verticalVelocity;

    private void Reset()
    {
        if (characterCamera == null)
            characterCamera = GetComponentInChildren<Camera>(true)?.transform;
    }

    private void OnValidate()
    {
        if (minPitch > -5f) minPitch = -5f;
        if (maxPitch < 5f) maxPitch = 5f;
        if (minPitch > maxPitch) minPitch = maxPitch - 1f;
    }

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();

        if (characterCamera == null)
            characterCamera = GetComponentInChildren<Camera>(true)?.transform;

        if (characterCamera == null)
        {
            Debug.LogError("CharInputLogic: No child Camera found. Put a Camera under this character and assign it.", this);
            enabled = false;
            return;
        }

        _pitch = NormalizePitch(characterCamera.localEulerAngles.x);
        ApplyCursorState();
    }

    private void OnEnable()
    {
        ApplyCursorState();
    }

    private void OnDisable()
    {
        // When this controller is disabled, release the cursor so you don't get stuck.
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void Update()
    {
        if (escapeUnlocksCursor && Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            lockMouseCursor = false;
            ApplyCursorState();
        }

        ApplyMove();
        ApplyLook();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (!context.performed && !context.canceled)
            return;

        _moveInput = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        if (!context.performed && !context.canceled)
            return;

        _lookInput = context.ReadValue<Vector2>();
    }

    private void ApplyMove()
    {
        var dt = Time.deltaTime;

        var move = (transform.right * _moveInput.x) + (transform.forward * _moveInput.y);
        if (move.sqrMagnitude > 1f)
            move.Normalize();

        ApplyGravity(dt);

        var velocity = move * moveSpeed;
        velocity.y = _verticalVelocity;

        _characterController.Move(velocity * dt);
    }

    private void ApplyGravity(float dt)
    {
        if (!useGravity)
            return;

        if (_characterController.isGrounded)
            _verticalVelocity = -groundedStickVelocity;
        else
            _verticalVelocity -= gravity * dt;
    }

    private void ApplyLook()
    {
        if (lockMouseCursor && Cursor.lockState != CursorLockMode.Locked)
            ApplyCursorState();

        var dx = _lookInput.x * lookSensitivity;
        var dy = _lookInput.y * lookSensitivity;

        if (invertY)
            dy = -dy;

        transform.Rotate(0f, dx, 0f, Space.Self);

        _pitch = Mathf.Clamp(_pitch - dy, minPitch, maxPitch);
        characterCamera.localRotation = Quaternion.Euler(_pitch, 0f, 0f);

        _lookInput = Vector2.zero;
    }

    private void ApplyCursorState()
    {
        if (!lockMouseCursor)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            return;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private static float NormalizePitch(float eulerX)
    {
        if (eulerX > 180f)
            eulerX -= 360f;

        return eulerX;
    }
}
