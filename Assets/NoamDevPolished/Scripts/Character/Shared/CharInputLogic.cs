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

    [Header("Step Offset Gate (Option B)")]
    [Tooltip("Only allow stepOffset to work when the obstacle in front is tagged with this.")]
    [SerializeField] private string buildingTag = "Building";

    [Tooltip("How far ahead to look for a step candidate (meters).")]
    [Range(0.05f, 1.0f)]
    [SerializeField] private float stepCheckDistance = 0.25f;

    [Tooltip("Height above the controller bottom where we check for a 'step obstacle'.")]
    [Range(0.0f, 0.3f)]
    [SerializeField] private float footCheckHeight = -1.025f;

    [Tooltip("If true, triggers are ignored for step checks.")]
    [SerializeField] private QueryTriggerInteraction stepTriggerInteraction = QueryTriggerInteraction.Ignore;

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

    // StepOffset gating state
    private float _defaultStepOffset;

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

        if (stepCheckDistance < 0.05f) stepCheckDistance = 0.05f;
        if (footCheckHeight < 0f) footCheckHeight = 0f;
    }

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _defaultStepOffset = _characterController.stepOffset;

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

        // Safety: restore default step offset if script gets disabled mid-game.
        if (_characterController != null)
            _characterController.stepOffset = _defaultStepOffset;
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

        // Desired horizontal move direction (world)
        var move = (transform.right * _moveInput.x) + (transform.forward * _moveInput.y);
        if (move.sqrMagnitude > 1f)
            move.Normalize();

        // Gate stepOffset BEFORE calling Move() so the built-in solver uses the right value this frame.
        UpdateStepOffsetGate(move);

        ApplyGravity(dt);

        var velocity = move * moveSpeed;
        velocity.y = _verticalVelocity;

        _characterController.Move(velocity * dt);
    }

    private void UpdateStepOffsetGate(Vector3 desiredHorizontalMoveWorld)
    {
        // If the designer set stepOffset to 0, don't fight it.
        if (_defaultStepOffset <= 0.0001f)
        {
            _characterController.stepOffset = 0f;
            return;
        }

        desiredHorizontalMoveWorld.y = 0f;

        // If no input, keep stepping enabled (standing still).
        if (desiredHorizontalMoveWorld.sqrMagnitude < 0.0001f)
        {
            _characterController.stepOffset = _defaultStepOffset;
            return;
        }

        var dir = desiredHorizontalMoveWorld.normalized;

        var canStep = IsStepCandidateBuilding(dir);

        _characterController.stepOffset = canStep ? _defaultStepOffset : 0f;
    }

    private bool IsStepCandidateBuilding(Vector3 forwardDir)
    {
        var radius = _characterController.radius;
        var height = Mathf.Max(_characterController.height, radius * 2f);

        var centerWorld = transform.TransformPoint(_characterController.center);

        var bottom = centerWorld + Vector3.up * (radius + footCheckHeight);
        var top = centerWorld + Vector3.up * (height - radius);

        if (Physics.CapsuleCast(bottom, top, radius, forwardDir, out var hitInfo, stepCheckDistance, ~0, stepTriggerInteraction))
            return hitInfo.collider != null && hitInfo.collider.CompareTag(buildingTag);

        // Nothing in front -> allow stepping (ramps / terrain).
        return true;
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

    private void OnDrawGizmos()
    {
        var controller = _characterController != null ? _characterController : GetComponent<CharacterController>();
        if (controller == null)
            return;

        // Direction: prefer current input direction; fallback to forward.
        var dir = (transform.right * _moveInput.x) + (transform.forward * _moveInput.y);

        // Project onto ground plane so it never points up/down if the transform has tilt.
        dir = Vector3.ProjectOnPlane(dir, Vector3.up);

        if (dir.sqrMagnitude < 0.0001f)
            dir = Vector3.ProjectOnPlane(transform.forward, Vector3.up);

        if (dir.sqrMagnitude < 0.0001f)
            return;

        dir.Normalize();

        var radius = controller.radius;
        var height = Mathf.Max(controller.height, radius * 2f);

        // Controller center in world space.
        var centerWorld = transform.TransformPoint(controller.center);

        // Foot-level sphere center (bottom hemisphere center of the capsule), then lift slightly by footCheckHeight.
        // Capsule bottom sphere center = center - up*(height/2 - radius)
        var footSphereCenter = centerWorld - Vector3.up * (height * 0.5f - radius);
        footSphereCenter += Vector3.up * footCheckHeight;

        // End position of the cast.
        var end = footSphereCenter + dir * stepCheckDistance;

        // Color indicates what WOULD happen right now.
        bool allowed;
        if (Physics.SphereCast(footSphereCenter, radius, dir, out var hitInfo, stepCheckDistance, ~0, stepTriggerInteraction))
            allowed = hitInfo.collider != null && hitInfo.collider.CompareTag(buildingTag);
        else
            allowed = true; // nothing in front -> allow stepping

        Gizmos.color = allowed ? Color.green : Color.red;

        // Draw the "sphere cast" at feet: start sphere, end sphere, and a line between them.
        Gizmos.DrawWireSphere(footSphereCenter, radius);
        Gizmos.DrawWireSphere(end, radius);
        Gizmos.DrawLine(footSphereCenter, end);

        // Optional: mark the hit point if we hit something.
        if (Physics.SphereCast(footSphereCenter, radius, dir, out hitInfo, stepCheckDistance, ~0, stepTriggerInteraction))
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(hitInfo.point, Mathf.Max(0.03f, radius * 0.2f));
            Gizmos.DrawLine(hitInfo.point, hitInfo.point + hitInfo.normal * 0.25f);
        }
    }

}
