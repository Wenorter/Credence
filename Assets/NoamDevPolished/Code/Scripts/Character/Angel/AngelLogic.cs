// Assets/Scripts/AngelLogic.cs
//
// AngelLogic
//
// What this script does:
// - Uses Unity's Input System callbacks (InputAction.CallbackContext) for Fly + Look.
// - Fly (WASD Vector2): FPS-style movement by default (yaw only), with optional "true fly" (camera pitch affects movement).
// - Look (mouse delta Vector2): yaw on the root, pitch on the camera (same structure as PriestLogic).
// - Optional smoothing (exp-lerp) for fly velocity and for look rotation.
// - Draw gizmos: SphereCollider wire sphere + a look ray (length configurable).
//
// Notes / expectations:
// - No CharacterController, no Rigidbody. This script moves transform.position directly.
// - If you want physics collisions to push things / be blocked, you *normally* use a Rigidbody (often kinematic).
// - To use CallbackContext with PlayerInput, set Behavior to "Invoke Unity Events" (not "Send Messages").

using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
[RequireComponent(typeof(SphereCollider))]
public sealed class AngelLogic : MonoBehaviour
{
    // -------------------------
    // Inspector: References
    // -------------------------
    [Header("References")]
    [Tooltip("Camera (or a pivot) under this object. Pitch is applied here (local X).")]
    [SerializeField] private Transform characterCamera;

    // -------------------------
    // Inspector: Fly
    // -------------------------
    [Header("Fly")]
    [Tooltip("Units per second at full input (WASD held).")]
    [Min(0f)]
    [SerializeField] private float flySpeed = 7.5f;

    [Tooltip("If true: movement uses camera forward/right including pitch (look up + W = go up).\nIf false: FPS-style (ignores pitch, stays on horizontal plane).")]
    [SerializeField] private bool useCameraPitchForMovement = false;

    [Header("Fly Smoothing")]
    [InspectorName("IsLerpFly")]
    [Tooltip("If true, fly velocity smoothly blends toward the desired velocity (like the Priest movement smoothing).")]
    [SerializeField] private bool isLerpFly = true;

    [Tooltip("How quickly fly velocity catches up to the target.\nHigher = snappier, lower = floatier.")]
    [Range(0.1f, 40f)]
    [SerializeField] private float flyLerpSpeed = 14f;

    [Tooltip("When smoothed speed drops below this, we snap to zero to prevent tiny drifting says 'still moving'.")]
    [Min(0f)]
    [SerializeField] private float flyStopSpeedEpsilon = 0.03f;

    // -------------------------
    // Inspector: Look
    // -------------------------
    [Header("Look")]
    [Tooltip("Multiplier for look input (mouse delta / stick).")]
    [Min(0f)]
    [SerializeField] private float lookSensitivity = 0.12f;

    [Tooltip("Lowest pitch allowed (looking down).")]
    [Range(-90f, -5f)]
    [SerializeField] private float minPitch = -80f;

    [Tooltip("Highest pitch allowed (looking up).")]
    [Range(5f, 90f)]
    [SerializeField] private float maxPitch = 80f;

    [Tooltip("Invert Y look if it feels more natural.")]
    [SerializeField] private bool invertY;

    [Header("Look Smoothing")]
    [InspectorName("IsLerpLook")]
    [Tooltip("If true, yaw/pitch smoothly blends toward the target rotation.")]
    [SerializeField] private bool isLerpLook = true;

    [Tooltip("How quickly look rotation catches up to the target.\nHigher = snappier, lower = more 'weight'.")]
    [Range(0.1f, 80f)]
    [SerializeField] private float lookLerpSpeed = 18f;

    [Tooltip("Mouse delta is usually already per-frame.\nLeave OFF for mouse. Turn ON if your look input is 'per second' (gamepad-like) and you want dt scaling.")]
    [SerializeField] private bool scaleLookByDeltaTime = false;

    // -------------------------
    // Inspector: Gizmos
    // -------------------------
    [Header("Gizmos")]
    [Tooltip("If true, draw the SphereCollider volume and the look ray in the Scene view.")]
    [SerializeField] private bool drawGizmos = true;

    [Tooltip("Length of the look ray gizmo.")]
    [Min(0.05f)]
    [SerializeField] private float lookRayLength = 5f;

    [Tooltip("Extra radius to show around the SphereCollider gizmo (visual only).")]
    [Min(0f)]
    [SerializeField] private float gizmoRadiusPadding = 0f;

    // -------------------------
    // Private state (no attributes -> underscore)
    // -------------------------
    private SphereCollider _sphereCollider;

    private Vector2 _flyInput;
    private Vector2 _lookInput;

    private float _yaw;
    private float _targetYaw;

    private float _pitch;
    private float _targetPitch;

    private Vector3 _flyVelocity;

    private void Reset()
    {
        if (characterCamera == null)
            characterCamera = GetComponentInChildren<Camera>(true)?.transform;
    }

    private void OnValidate()
    {
        flySpeed = Mathf.Max(0f, flySpeed);
        flyLerpSpeed = Mathf.Max(0.1f, flyLerpSpeed);
        flyStopSpeedEpsilon = Mathf.Max(0f, flyStopSpeedEpsilon);

        lookSensitivity = Mathf.Max(0f, lookSensitivity);
        lookLerpSpeed = Mathf.Max(0.1f, lookLerpSpeed);

        if (maxPitch < minPitch)
            maxPitch = minPitch;
    }

    private void Awake()
    {
        _sphereCollider = GetComponent<SphereCollider>();

        if (characterCamera == null)
            characterCamera = GetComponentInChildren<Camera>(true)?.transform;

        if (characterCamera == null)
        {
            Debug.LogError($"{nameof(AngelLogic)}: No Camera found under this object. Assign 'Character Camera' or put a Camera as a child.", this);
            enabled = false;
            return;
        }

        // Start from current transforms so there's no snap on first frame.
        _yaw = transform.eulerAngles.y;
        _targetYaw = _yaw;

        _pitch = NormalizePitch(characterCamera.localEulerAngles.x);
        _pitch = Mathf.Clamp(_pitch, minPitch, maxPitch);
        _targetPitch = _pitch;
    }

    private void Update()
    {
        if (characterCamera == null)
            return;

        var dt = Time.deltaTime;

        ApplyFly(dt);
        ApplyLook(dt);
    }

    // -------------------------
    // Input System callbacks
    // -------------------------
    public void OnFly(InputAction.CallbackContext context)
    {
        // Same pattern as PriestLogic: only respond to performed/canceled.
        if (!context.performed && !context.canceled)
            return;

        _flyInput = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        if (!context.performed && !context.canceled)
            return;

        _lookInput = context.ReadValue<Vector2>();
    }

    // Optional hook you already had in your Angel script.
    public void OnPriestChangedRooms(Transform newPos)
    {
        if (newPos == null)
        {
            Debug.LogWarning($"{nameof(AngelLogic)}.{nameof(OnPriestChangedRooms)} got a null Transform.", this);
            return;
        }

        transform.position = newPos.position;
    }

    // -------------------------
    // Fly
    // -------------------------
    private void ApplyFly(float dt)
    {
        // Build desired move direction from input.
        var moveDir = (transform.right * _flyInput.x) + (transform.forward * _flyInput.y);

        if (useCameraPitchForMovement && characterCamera != null)
        {
            // True-fly: movement basis comes from the camera orientation.
            var camForward = characterCamera.forward;
            var camRight = characterCamera.right;

            moveDir = (camRight * _flyInput.x) + (camForward * _flyInput.y);
        }
        else
        {
            // FPS-style: stay on horizontal plane.
            moveDir.y = 0f;
        }

        if (moveDir.sqrMagnitude > 1f)
            moveDir.Normalize();

        var targetVelocity = moveDir * flySpeed;

        if (isLerpFly)
        {
            var t = ExpLerpFactor(flyLerpSpeed, dt);
            _flyVelocity = Vector3.Lerp(_flyVelocity, targetVelocity, t);

            // Prevent endless micro-drifting when input is released.
            if (_flyVelocity.magnitude < flyStopSpeedEpsilon && targetVelocity.sqrMagnitude < 0.0001f)
                _flyVelocity = Vector3.zero;
        }
        else
        {
            _flyVelocity = targetVelocity;
        }

        if (_flyVelocity.sqrMagnitude > 0.000001f)
            transform.position += _flyVelocity * dt; // movement must be dt-scaled
    }

    // -------------------------
    // Look
    // -------------------------
    private void ApplyLook(float dt)
    {
        // Same structure as PriestLogic: accumulate into target yaw/pitch, then optionally lerp.
        var dx = _lookInput.x * lookSensitivity;
        var dy = _lookInput.y * lookSensitivity;

        if (scaleLookByDeltaTime)
        {
            dx *= dt;
            dy *= dt;
        }

        if (invertY)
            dy = -dy;

        _targetYaw += dx;
        _targetPitch = Mathf.Clamp(_targetPitch - dy, minPitch, maxPitch);

        if (isLerpLook)
        {
            var t = ExpLerpFactor(lookLerpSpeed, dt);
            _yaw = Mathf.LerpAngle(_yaw, _targetYaw, t);
            _pitch = Mathf.Lerp(_pitch, _targetPitch, t);
        }
        else
        {
            _yaw = _targetYaw;
            _pitch = _targetPitch;
        }

        transform.rotation = Quaternion.Euler(0f, _yaw, 0f);
        characterCamera.localRotation = Quaternion.Euler(_pitch, 0f, 0f);

        // Mouse delta should not "stick" if there isn't a new event this frame.
        _lookInput = Vector2.zero;
    }

    private static float NormalizePitch(float eulerX)
    {
        if (eulerX > 180f)
            eulerX -= 360f;

        return eulerX;
    }

    private static float ExpLerpFactor(float speed, float dt)
    {
        // Exponential lerp: stable across framerates.
        return 1f - Mathf.Exp(-speed * dt);
    }

    // -------------------------
    // Gizmos
    // -------------------------
    private void OnDrawGizmos()
    {
        if (!drawGizmos)
            return;

        var sphere = _sphereCollider != null ? _sphereCollider : GetComponent<SphereCollider>();
        if (sphere != null)
        {
            var worldCenter = transform.TransformPoint(sphere.center);

            // Best-effort sphere radius under non-uniform scaling (use max axis).
            var scale = transform.lossyScale;
            var maxAxis = Mathf.Max(Mathf.Abs(scale.x), Mathf.Abs(scale.y), Mathf.Abs(scale.z));
            var worldRadius = (sphere.radius * maxAxis) + gizmoRadiusPadding;

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(worldCenter, worldRadius);
        }
        else
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, 0.25f);
        }

        var origin = characterCamera != null ? characterCamera.position : transform.position;
        var dir = characterCamera != null ? characterCamera.forward : transform.forward;

        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(origin, dir * lookRayLength);
    }
}
