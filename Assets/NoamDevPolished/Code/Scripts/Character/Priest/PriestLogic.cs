// Assets/Scripts/CharInputLogic.cs
//
// CharInputLogic
//
// What this script does:
// - First-person CharacterController movement (WASD / stick) + gravity.
// - FPS look: yaw on the character, pitch on the camera.
// - Optional smoothing (lerp) for movement + look.
// - Anti-exploit surface rules:
//     - You can ONLY step/climb/ride ramps on colliders tagged with "Building".
//     - Non-Building "stepable" obstacles: stepOffset is forced to 0 and movement glides along the obstacle (FPS wall-slide).
//     - Non-Building ramps/slopes: prevents climbing by removing ONLY the upslope component, but still allows strafing/backing out.
//     - When already standing on a non-Building slope, upslope is continuously removed so holding W can't keep creeping upward.
// - Pushing:
//     - No OnControllerColliderHit (CharacterController contact is unreliable when we intentionally block stepping).
//     - Re-uses the SAME anti-exploit probe hits (no extra casts).
//     - If a probe hits a Rigidbody, it always forces rb.isKinematic = false (wake-up behavior).
//     - Applies a small impulse (velocity change) with cooldown to avoid machine-gun pushing.
// - Debug & tuning:
//     - Gizmos show the exact probes and whether they hit Building vs blocked.
//     - A few safety logs exist for missing references / weird grounded states.
//
// Unity: 6000.3.0f1

using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
[RequireComponent(typeof(CharacterController))]
public sealed class PriestLogic : MonoBehaviour
{
    // -------------------------
    // Inspector: References
    // -------------------------
    [Header("References")]
    [Tooltip("Camera (or camera pivot) under this character. Pitch is applied here (local X).")]
    [SerializeField] private Transform characterCamera;

    // -------------------------
    // Inspector: Movement
    // -------------------------
    [Header("Movement")]
    [Tooltip("Meters per second on flat ground.")]
    [SerializeField] private float moveSpeed = 5.5f;

    [Header("Movement Smoothing")]
    [InspectorName("IsLerpMove")]
    [Tooltip("If true, horizontal movement smoothly blends toward the desired direction/speed.")]
    [SerializeField] private bool isLerpMove = true;

    [Tooltip("How quickly movement catches up to the target.\nHigher = snappier, lower = floatier.")]
    [Range(0.1f, 40f)]
    [SerializeField] private float moveLerpSpeed = 12f;

    [Header("Gravity")]
    [Tooltip("If false, no gravity is applied (useful for debug).")]
    [SerializeField] private bool useGravity = true;

    [Tooltip("Downward acceleration (m/s^2).")]
    [SerializeField] private float gravity = 18f;

    [Tooltip("Small constant downward velocity while grounded to prevent 'hovering' on slopes.")]
    [SerializeField] private float groundedStickVelocity = 2f;

    // -------------------------
    // Inspector: Look
    // -------------------------
    [Header("Look")]
    [Tooltip("Multiplier for look input (mouse delta / stick).")]
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

    // -------------------------
    // Inspector: Anti-Exploit / Surface Rules
    // -------------------------
    [Header("Surface Restriction (Anti-Exploit)")]
    [Tooltip("If enabled, the player can only step/climb/ride slopes on colliders tagged with 'Building'.")]
    [SerializeField] private bool restrictToBuildingTag = true;

    [Tooltip("Tag name that is allowed to be stepped/climbed on.")]
    [SerializeField] private string buildingTag = "Building";

    [Tooltip("How far ahead (meters) we check for the next ground surface.")]
    [Range(0.05f, 1.5f)]
    [SerializeField] private float nextSurfaceProbeDistance = 0.55f;

    [Tooltip("How far down we probe for ground from the predicted position (meters).")]
    [Range(0.2f, 3f)]
    [SerializeField] private float groundProbeDownDistance = 1.6f;

    [Tooltip("How wide the ground probe is. 1.0 means near the controller radius.")]
    [Range(0.3f, 1.2f)]
    [SerializeField] private float groundProbeRadiusScale = 0.9f;

    [Tooltip("If the predicted ground is non-Building and rises by more than this (meters), block movement.")]
    [Range(0.0f, 0.4f)]
    [SerializeField] private float nonBuildingHeightRiseBlock = 0.03f;

    [Tooltip("If the predicted ground normal Y is below this and it's non-Building, treat it as a ramp/slope and block climbing.")]
    [Range(0.5f, 0.999f)]
    [SerializeField] private float nonBuildingSlopeNormalYBlock = 0.985f;

    [Tooltip("How far ahead to check for a 'stepable' obstacle (low obstacle you'd normally step up onto).")]
    [Range(0.05f, 0.8f)]
    [SerializeField] private float stepObstacleCheckDistance = 0.18f;

    [Tooltip("Which layers count as 'surfaces' for the anti-exploit probes.")]
    [SerializeField] private LayerMask surfaceProbeMask = ~0;

    [Header("Next-Ground Probe Offsets (Yellow Sphere)")]
    [Tooltip("Moves the predicted-feet point up/down. This is the 'yellow sphere height' control.")]
    [Range(-0.35f, 0.35f)]
    [SerializeField] private float predictedFeetYOffset = 0.0f;

    [Tooltip("Extra lift applied to the start of the next-ground SphereCast.\nHelps avoid missing hits when the start sphere overlaps geometry.")]
    [Range(0.0f, 0.8f)]
    [SerializeField] private float nextGroundCastExtraStartLift = 0.18f;

    [Tooltip("If cast start is overlapping, lift it in steps and retry (SphereCast won't report initial overlaps).")]
    [Range(0, 6)]
    [SerializeField] private int nextGroundCastLiftSteps = 3;

    [Tooltip("Lift per step when escaping overlap.")]
    [Range(0.02f, 0.35f)]
    [SerializeField] private float nextGroundCastLiftStepSize = 0.10f;

    [Header("Blocked Movement Behavior")]
    [Tooltip("Keeps the same collider blocked for a short time even if probes flicker for a frame.\nFixes edge jitter when moving along corners.")]
    [Range(0.0f, 0.35f)]
    [SerializeField] private float blockedStickyTime = 0.12f;

    // -------------------------
    // Inspector: Pushing (re-uses probe hits)
    // -------------------------
    [Header("Pushing (re-uses Surface Restriction hits)")]
    [Tooltip("Represents the player's 'virtual mass' for push impulse calculation (CharacterController has no real mass).")]
    [SerializeField] private float characterMass = 80f;

    [Tooltip("Safety: if object's mass is greater than (characterMass * this), don't push it.")]
    [SerializeField] private float maxPushableMassRatio = 3f;

    [Tooltip("How much the push scales with player speed.\nHigher = stronger shove when running.")]
    [SerializeField] private float pushPerSpeed = 0.12f;

    [Tooltip("Maximum velocity change applied to the object per hit.")]
    [SerializeField] private float maxDeltaV = 2.5f;

    [Tooltip("Ignore tiny bumps to reduce jittery nudges on contact.")]
    [SerializeField] private float minSpeedToPush = 0.15f;

    [Tooltip("If true, only push along the ground plane (prevents launching objects upward).")]
    [SerializeField] private bool ignoreVerticalPush = true;

    [Tooltip("Cooldown between push impulses on the same collider (prevents rapid-fire pushing).")]
    [Range(0.0f, 0.25f)]
    [SerializeField] private float pushCooldown = 0.08f;

    // -------------------------
    // Inspector: Debug / Gizmos
    // -------------------------
    [Header("Debug Gizmos")]
    [Tooltip("If true, draws the probes used for step/ramp blocking.")]
    [SerializeField] private bool drawSurfaceRestrictionGizmos = true;

    [Tooltip("Gizmo color when the probe is NOT hitting anything.")]
    [SerializeField] private Color gizmoProbeIdleColor = new Color(0.15f, 0.9f, 0.15f, 1f);

    [Tooltip("Gizmo color when the probe hits something tagged Building.")]
    [SerializeField] private Color gizmoProbeBuildingHitColor = new Color(0.15f, 0.55f, 1f, 1f);

    [Tooltip("Gizmo color when the probe hits something that would get blocked (non-Building).")]
    [SerializeField] private Color gizmoProbeBlockedHitColor = new Color(1f, 0.25f, 0.25f, 1f);

    [Header("Mouse Lock")]
    [Tooltip("If true, locks the mouse cursor to the center and hides it (standard FPS).")]
    [SerializeField] private bool lockMouseCursor = true;

    [Tooltip("If true, pressing Escape releases the cursor (useful while testing).")]
    [SerializeField] private bool escapeUnlocksCursor = true;

    [Header("Debug Logs")]
    [Tooltip("If true, logs a warning when required references are missing or setup looks wrong.")]
    [SerializeField] private bool logSetupWarnings = true;

    // -------------------------
    // Private state
    // -------------------------
    private CharacterController _characterController;

    private Vector2 _moveInput;
    private Vector2 _lookInput;

    private float _pitch;
    private float _targetPitch;

    private float _yaw;
    private float _targetYaw;

    private float _verticalVelocity;
    private Vector3 _horizontalVelocity;

    private float _originalStepOffset;

    // Ground tracking: used for ramp detection (rise vs last ground).
    private float _lastGroundY;
    private bool _hasLastGroundY;

    // Current ground: used to keep removing upslope when already standing on a non-building ramp.
    private Collider _currentGroundCollider;
    private Vector3 _currentGroundNormal;
    private bool _currentGroundIsBuilding;

    // Sticky blocking: reduces probe flicker jitter at edges/corners.
    private int _stickyBlockedColliderId;
    private float _stickyBlockedUntilTime;

    // Push cooldown: avoids spamming forces every frame on the same collider.
    private int _lastPushedColliderId;
    private float _lastPushTime;

    // Gizmo cache (updated every frame so the gizmos match the latest logic inputs/starts).
    private bool _gizmoHasMoveDir;
    private Vector3 _gizmoMoveDir;

    private bool _gizmoObstacleHit;
    private RaycastHit _gizmoObstacleHitInfo;

    private bool _gizmoNextGroundHit;
    private RaycastHit _gizmoNextGroundHitInfo;

    private Vector3 _gizmoPredictedFeetPoint;
    private Vector3 _gizmoNextGroundCastStart;
    private float _gizmoNextGroundCastRadius;
    private float _gizmoNextGroundCastDistance;

    private enum SurfaceBlockReason
    {
        None = 0,
        NonBuildingStepableObstacle = 1,
        NonBuildingRampOrSlope = 2
    }

    // -------------------------
    // Unity events
    // -------------------------

    public void Asd(int asd)
    {
        
    }
    private void Reset()
    {
        if (characterCamera == null)
            characterCamera = GetComponentInChildren<Camera>(true)?.transform;
    }

    private void OnValidate()
    {
        moveSpeed = Mathf.Max(0f, moveSpeed);
        moveLerpSpeed = Mathf.Max(0.1f, moveLerpSpeed);

        lookSensitivity = Mathf.Max(0f, lookSensitivity);
        lookLerpSpeed = Mathf.Max(0.1f, lookLerpSpeed);

        gravity = Mathf.Max(0f, gravity);
        groundedStickVelocity = Mathf.Max(0f, groundedStickVelocity);

        nextSurfaceProbeDistance = Mathf.Max(0.05f, nextSurfaceProbeDistance);
        groundProbeDownDistance = Mathf.Max(0.2f, groundProbeDownDistance);
        stepObstacleCheckDistance = Mathf.Max(0.05f, stepObstacleCheckDistance);

        nextGroundCastExtraStartLift = Mathf.Max(0f, nextGroundCastExtraStartLift);
        nextGroundCastLiftSteps = Mathf.Max(0, nextGroundCastLiftSteps);
        nextGroundCastLiftStepSize = Mathf.Max(0.02f, nextGroundCastLiftStepSize);
        blockedStickyTime = Mathf.Max(0f, blockedStickyTime);

        characterMass = Mathf.Max(0.01f, characterMass);
        maxPushableMassRatio = Mathf.Max(0.01f, maxPushableMassRatio);
        pushPerSpeed = Mathf.Max(0f, pushPerSpeed);
        maxDeltaV = Mathf.Max(0f, maxDeltaV);
        minSpeedToPush = Mathf.Max(0f, minSpeedToPush);
        pushCooldown = Mathf.Max(0f, pushCooldown);

        if (string.IsNullOrWhiteSpace(buildingTag))
            buildingTag = "Building";
    }

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _originalStepOffset = _characterController.stepOffset;

        if (characterCamera == null)
            characterCamera = GetComponentInChildren<Camera>(true)?.transform;

        if (characterCamera == null)
        {
            Debug.LogError("CharInputLogic: No Camera found under this character. Assign 'Character Camera' or put a Camera as a child.", this);
            enabled = false;
            return;
        }

        // Start from current transforms so there's no snap on first frame.
        _yaw = transform.eulerAngles.y;
        _targetYaw = _yaw;

        _pitch = NormalizePitch(characterCamera.localEulerAngles.x);
        _targetPitch = _pitch;

        ApplyCursorState();
    }

    private void OnEnable()
    {
        ApplyCursorState();
    }

    private void OnDisable()
    {
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

    // -------------------------
    // Input System callbacks
    // -------------------------
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

    // -------------------------
    // Movement
    // -------------------------
    private void ApplyMove()
    {
        var dt = Time.deltaTime;

        // Convert input to a world-space move direction on the ground plane.
        var moveDir = (transform.right * _moveInput.x) + (transform.forward * _moveInput.y);
        moveDir.y = 0f;

        if (moveDir.sqrMagnitude > 1f)
            moveDir.Normalize();

        _gizmoHasMoveDir = moveDir.sqrMagnitude > 0.0001f;
        _gizmoMoveDir = _gizmoHasMoveDir ? moveDir : transform.forward;

        // Keep probe gizmos "alive" even if we early-out later.
        UpdateProbePreview(_gizmoMoveDir);

        // Desired velocity BEFORE restrictions.
        var targetHorizontalVelocity = moveDir * moveSpeed;

        // We only force stepOffset to 0 temporarily (when blocking). Otherwise keep original.
        if (_characterController.stepOffset != _originalStepOffset)
            _characterController.stepOffset = _originalStepOffset;

        UpdateCurrentGroundInfo();

        // Reset probe hit flags each frame (they will be repopulated by checks).
        _gizmoObstacleHit = false;
        _gizmoNextGroundHit = false;

        // Sticky block: keeps stepOffset disabled for a moment even if a probe misses for 1 frame.
        if (_stickyBlockedColliderId != 0 && Time.time < _stickyBlockedUntilTime)
            _characterController.stepOffset = 0f;

        if (restrictToBuildingTag && _gizmoHasMoveDir && _characterController.isGrounded)
        {
            // If we're already standing on a non-building ramp, prevent any further upslope movement.
            if (ShouldTreatCurrentGroundAsNonBuildingRamp())
            {
                _characterController.stepOffset = 0f;

                targetHorizontalVelocity = RemoveUpslopeComponentXZ(targetHorizontalVelocity, _currentGroundNormal);
                _horizontalVelocity = RemoveUpslopeComponentXZ(_horizontalVelocity, _currentGroundNormal);

                if (_currentGroundCollider != null)
                {
                    // This also forces rb.isKinematic = false when applicable.
                    TryPushFromHit(_currentGroundCollider, default, moveDir);
                    ApplyStickyBlock(_currentGroundCollider.GetInstanceID());
                }
            }
            else
            {
                var reason = EvaluateSurfaceRestriction(moveDir, out var hitCollider, out var hitInfo);

                if (reason != SurfaceBlockReason.None && hitCollider != null)
                {
                    _characterController.stepOffset = 0f;
                    ApplyStickyBlock(hitCollider.GetInstanceID());

                    // Wake + push using the SAME hit that caused the block (no extra casts).
                    TryPushFromHit(hitCollider, hitInfo, moveDir);

                    if (reason == SurfaceBlockReason.NonBuildingStepableObstacle)
                    {
                        // FPS wall glide: remove the "into the wall" component so we slide along it.
                        targetHorizontalVelocity = SlideAlongPlaneXZ(targetHorizontalVelocity, hitInfo.normal);
                        _horizontalVelocity = SlideAlongPlaneXZ(_horizontalVelocity, hitInfo.normal);
                    }
                    else if (reason == SurfaceBlockReason.NonBuildingRampOrSlope)
                    {
                        // Ramp block: remove ONLY the upslope component so we can't gain height.
                        targetHorizontalVelocity = RemoveUpslopeComponentXZ(targetHorizontalVelocity, hitInfo.normal);
                        _horizontalVelocity = RemoveUpslopeComponentXZ(_horizontalVelocity, hitInfo.normal);
                    }
                }
            }
        }

        // Smooth horizontal velocity AFTER restrictions are applied.
        if (isLerpMove)
        {
            var t = ExpLerpFactor(moveLerpSpeed, dt);
            _horizontalVelocity = Vector3.Lerp(_horizontalVelocity, targetHorizontalVelocity, t);
        }
        else
        {
            _horizontalVelocity = targetHorizontalVelocity;
        }

        ApplyGravity(dt);

        var velocity = _horizontalVelocity;
        velocity.y = _verticalVelocity;

        _characterController.Move(velocity * dt);
    }

    private void ApplyGravity(float dt)
    {
        if (!useGravity)
        {
            _verticalVelocity = 0f;
            return;
        }

        if (_characterController.isGrounded)
            _verticalVelocity = -groundedStickVelocity;
        else
            _verticalVelocity -= gravity * dt;
    }

    // -------------------------
    // Look
    // -------------------------
    private void ApplyLook()
    {
        if (lockMouseCursor && Cursor.lockState != CursorLockMode.Locked)
            ApplyCursorState();

        var dx = _lookInput.x * lookSensitivity;
        var dy = _lookInput.y * lookSensitivity;

        if (invertY)
            dy = -dy;

        _targetYaw += dx;
        _targetPitch = Mathf.Clamp(_targetPitch - dy, minPitch, maxPitch);

        if (isLerpLook)
        {
            var t = ExpLerpFactor(lookLerpSpeed, Time.deltaTime);
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

        // Mouse delta should not "accumulate" if input isn't refreshed this frame.
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

    // -------------------------
    // Surface restriction logic
    // -------------------------
    private SurfaceBlockReason EvaluateSurfaceRestriction(
        Vector3 moveDir,
        out Collider hitCollider,
        out RaycastHit hitInfo)
    {
        hitCollider = null;
        hitInfo = default;

        // 1) Forward obstacle check (stepable bump / low obstacle).
        if (IsFacingNonBuildingStepableObstacle(moveDir, out var obstacleCol, out var obstacleHit))
        {
            hitCollider = obstacleCol;
            hitInfo = obstacleHit;
            return SurfaceBlockReason.NonBuildingStepableObstacle;
        }

        // 2) Next ground check (ramp/slope/height change).
        if (IsNextGroundNonBuildingAndRising(moveDir, out var groundCol, out var groundHit))
        {
            hitCollider = groundCol;
            hitInfo = groundHit;
            return SurfaceBlockReason.NonBuildingRampOrSlope;
        }

        return SurfaceBlockReason.None;
    }

    private bool IsFacingNonBuildingStepableObstacle(Vector3 moveDir, out Collider hitCollider, out RaycastHit hit)
    {
        hitCollider = null;
        hit = default;

        if (_originalStepOffset <= 0.0001f)
            return false;

        var up = transform.up;

        var bottom = GetBottomSphereCenter();
        var castOrigin = bottom + (up * Mathf.Max(0.02f, _characterController.skinWidth));

        var castRadius = Mathf.Max(0.02f, _characterController.radius * 0.95f);
        var castDistance = stepObstacleCheckDistance + _characterController.skinWidth;

        if (!Physics.SphereCast(
                castOrigin,
                castRadius,
                moveDir,
                out hit,
                castDistance,
                surfaceProbeMask,
                QueryTriggerInteraction.Ignore))
        {
            return false;
        }

        _gizmoObstacleHit = true;
        _gizmoObstacleHitInfo = hit;

        var col = hit.collider;
        if (col == null)
            return false;

        if (col.transform.IsChildOf(transform))
            return false;

        // Wake any kinematic Rigidbody we touched (your "throwables wake up when touched" behavior).
        EnsureRigidbodyAwake(col);

        // Building is always allowed.
        if (col.CompareTag(buildingTag))
            return false;

        hitCollider = col;
        return true;
    }

    private bool IsNextGroundNonBuildingAndRising(Vector3 moveDir, out Collider hitCollider, out RaycastHit hit)
    {
        hitCollider = null;
        hit = default;

        var up = transform.up;

        // Predicted feet point is what the yellow gizmo represents.
        var feetPoint = GetFeetPoint() + (up * predictedFeetYOffset);

        // Make sure "ahead" isn't shorter than the controller radius (stops weird misses at some angles).
        var minAhead = Mathf.Max(0.05f, _characterController.radius * 0.65f);
        var probeAhead = Mathf.Max(minAhead, nextSurfaceProbeDistance);

        var predictedFeetPoint = feetPoint + (moveDir * probeAhead);
        _gizmoPredictedFeetPoint = predictedFeetPoint;

        var castRadius = Mathf.Max(0.02f, _characterController.radius * Mathf.Clamp(groundProbeRadiusScale, 0.3f, 1.2f));

        // Start above predicted feet so the sphere "sees" the ground without starting inside geometry.
        var startPaddingUp = castRadius + Mathf.Max(0.08f, _characterController.skinWidth * 2f) + nextGroundCastExtraStartLift;
        var castStart = predictedFeetPoint + (up * startPaddingUp);

        // SphereCast does not reliably report initial overlaps. If start is intersecting, lift and retry.
        if (nextGroundCastLiftSteps > 0)
        {
            var tries = 0;
            while (tries < nextGroundCastLiftSteps &&
                   Physics.CheckSphere(castStart, castRadius, surfaceProbeMask, QueryTriggerInteraction.Ignore))
            {
                castStart += up * nextGroundCastLiftStepSize;
                tries++;
            }
        }

        var castDistance = groundProbeDownDistance + startPaddingUp;

        // Cache these for gizmos.
        _gizmoNextGroundCastStart = castStart;
        _gizmoNextGroundCastRadius = castRadius;
        _gizmoNextGroundCastDistance = castDistance;

        if (!Physics.SphereCast(
                castStart,
                castRadius,
                -up,
                out hit,
                castDistance,
                surfaceProbeMask,
                QueryTriggerInteraction.Ignore))
        {
            return false;
        }

        _gizmoNextGroundHit = true;
        _gizmoNextGroundHitInfo = hit;

        var col = hit.collider;
        if (col == null)
            return false;

        if (col.transform.IsChildOf(transform))
            return false;

        // Wake any kinematic Rigidbody we touched.
        EnsureRigidbodyAwake(col);

        // Building is always allowed.
        if (col.CompareTag(buildingTag))
            return false;

        var risingEnough = false;
        if (_hasLastGroundY)
        {
            var predictedGroundY = hit.point.y;
            var rise = predictedGroundY - _lastGroundY;
            risingEnough = rise > nonBuildingHeightRiseBlock;
        }

        var slopeEnough = hit.normal.y < nonBuildingSlopeNormalYBlock;

        if (!risingEnough && !slopeEnough)
            return false;

        hitCollider = col;
        return true;
    }

    private void UpdateCurrentGroundInfo()
    {
        _currentGroundCollider = null;
        _currentGroundNormal = Vector3.up;
        _currentGroundIsBuilding = false;

        var up = transform.up;

        var feetPoint = GetFeetPoint();
        var start = feetPoint + (up * 0.25f);

        if (Physics.Raycast(
                start,
                -up,
                out var hit,
                0.9f,
                surfaceProbeMask,
                QueryTriggerInteraction.Ignore))
        {
            _lastGroundY = hit.point.y;
            _hasLastGroundY = true;

            _currentGroundCollider = hit.collider;
            _currentGroundNormal = hit.normal;
            _currentGroundIsBuilding = hit.collider != null && hit.collider.CompareTag(buildingTag);

            EnsureRigidbodyAwake(hit.collider);
            return;
        }

        if (!_characterController.isGrounded)
        {
            _hasLastGroundY = false;
            return;
        }

        // If Unity says grounded but our ground ray misses, something is off (mask/colliders).
        if (logSetupWarnings)
            Debug.LogWarning("CharInputLogic: Controller says grounded, but ground raycast missed. Check 'Surface Probe Mask' and collider setup.", this);
    }

    private bool ShouldTreatCurrentGroundAsNonBuildingRamp()
    {
        if (_currentGroundCollider == null)
            return false;

        if (_currentGroundIsBuilding)
            return false;

        // If we're standing on a steep enough slope and it's non-building, keep blocking upslope.
        return _currentGroundNormal.y < nonBuildingSlopeNormalYBlock;
    }

    private void ApplyStickyBlock(int colliderInstanceId)
    {
        _stickyBlockedColliderId = colliderInstanceId;
        _stickyBlockedUntilTime = Time.time + blockedStickyTime;
    }

    // -------------------------
    // Pushing (re-uses hits)
    // -------------------------
    private static void EnsureRigidbodyAwake(Collider col)
    {
        if (col == null)
            return;

        var rb = col.attachedRigidbody;
        if (rb == null)
            return;

        if (rb.isKinematic)
            rb.isKinematic = false;
    }

    private void TryPushFromHit(Collider col, RaycastHit hit, Vector3 moveDir)
    {
        if (col == null)
            return;

        var rb = col.attachedRigidbody;
        if (rb == null)
            return;

        // Wake on touch (always).
        if (rb.isKinematic)
            rb.isKinematic = false;

        var colId = col.GetInstanceID();
        var now = Time.time;

        if (colId == _lastPushedColliderId && (now - _lastPushTime) < pushCooldown)
            return;

        _lastPushedColliderId = colId;
        _lastPushTime = now;

        var speed = _characterController.velocity.magnitude;
        if (speed < minSpeedToPush)
            return;

        var maxPushableMass = characterMass * maxPushableMassRatio;
        if (rb.mass > maxPushableMass)
            return;

        var dir = ignoreVerticalPush
            ? new Vector3(moveDir.x, 0f, moveDir.z)
            : moveDir;

        if (dir.sqrMagnitude < 0.0001f)
            return;

        dir.Normalize();

        var massRatio = characterMass / Mathf.Max(0.01f, rb.mass);
        var deltaV = speed * pushPerSpeed * massRatio;

        if (maxDeltaV > 0f)
            deltaV = Mathf.Min(deltaV, maxDeltaV);

        if (deltaV <= 0f)
            return;

        // If we have a real hit point, push there. If not (grounded case), push around center of mass.
        var point = hit.collider != null ? hit.point : rb.worldCenterOfMass;

        rb.AddForceAtPosition(dir * deltaV, point, ForceMode.VelocityChange);
    }

    // -------------------------
    // Movement math helpers
    // -------------------------
    private static Vector3 SlideAlongPlaneXZ(Vector3 velocity, Vector3 planeNormal)
    {
        // Remove the component into the surface -> classic FPS "glide along wall".
        var v = Vector3.ProjectOnPlane(velocity, planeNormal);
        v.y = 0f;
        return v;
    }

    private static Vector3 RemoveUpslopeComponentXZ(Vector3 desiredVelocity, Vector3 surfaceNormal)
    {
        // "Upslope direction" is the direction you'd move to go up the surface.
        var upslope = Vector3.ProjectOnPlane(Vector3.up, surfaceNormal);
        if (upslope.sqrMagnitude < 0.000001f)
            return desiredVelocity;

        var upslopeXZ = new Vector3(upslope.x, 0f, upslope.z);
        if (upslopeXZ.sqrMagnitude < 0.000001f)
            return desiredVelocity;

        upslopeXZ.Normalize();

        var v = desiredVelocity;
        v.y = 0f;

        // Only remove if we're trying to go UP the slope (dot > 0).
        var dot = Vector3.Dot(v, upslopeXZ);
        if (dot <= 0f)
            return desiredVelocity;

        var result = v - (upslopeXZ * dot);
        return new Vector3(result.x, desiredVelocity.y, result.z);
    }

    // -------------------------
    // Probe gizmo preview helpers
    // -------------------------
    private void UpdateProbePreview(Vector3 moveDirForPreview)
    {
        if (_characterController == null)
            return;

        var up = transform.up;

        var feetPoint = GetFeetPoint() + (up * predictedFeetYOffset);

        var minAhead = Mathf.Max(0.05f, _characterController.radius * 0.65f);
        var probeAhead = Mathf.Max(minAhead, nextSurfaceProbeDistance);

        _gizmoPredictedFeetPoint = feetPoint + (moveDirForPreview * probeAhead);

        var castRadius = Mathf.Max(0.02f, _characterController.radius * Mathf.Clamp(groundProbeRadiusScale, 0.3f, 1.2f));
        var startPaddingUp = castRadius + Mathf.Max(0.08f, _characterController.skinWidth * 2f) + nextGroundCastExtraStartLift;

        _gizmoNextGroundCastStart = _gizmoPredictedFeetPoint + (up * startPaddingUp);
        _gizmoNextGroundCastRadius = castRadius;
        _gizmoNextGroundCastDistance = groundProbeDownDistance + startPaddingUp;
    }

    // -------------------------
    // Geometry helpers
    // -------------------------
    private Vector3 GetBottomSphereCenter()
    {
        var up = transform.up;
        var centerWorld = transform.TransformPoint(_characterController.center);
        var halfHeight = Mathf.Max(0f, (_characterController.height * 0.5f) - _characterController.radius);

        return centerWorld - (up * halfHeight);
    }

    private Vector3 GetFeetPoint()
    {
        // Bottom sphere center minus radius is roughly where the controller touches ground when grounded.
        var up = transform.up;
        return GetBottomSphereCenter() - (up * _characterController.radius);
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
        if (!drawSurfaceRestrictionGizmos || !restrictToBuildingTag)
            return;

        var controller = _characterController != null ? _characterController : GetComponent<CharacterController>();
        if (controller == null)
            return;

        var up = transform.up;
        var moveDir = _gizmoHasMoveDir ? _gizmoMoveDir : transform.forward;

        // 1) Forward obstacle probe.
        var bottomSphere = GetBottomSphereCenterGizmo(controller);
        var obstacleOrigin = bottomSphere + (up * Mathf.Max(0.02f, controller.skinWidth));
        var obstacleRadius = Mathf.Max(0.02f, controller.radius * 0.95f);
        var obstacleDistance = stepObstacleCheckDistance + controller.skinWidth;

        if (_gizmoObstacleHit && _gizmoObstacleHitInfo.collider != null)
        {
            var col = _gizmoObstacleHitInfo.collider;
            Gizmos.color = col.CompareTag(buildingTag) ? gizmoProbeBuildingHitColor : gizmoProbeBlockedHitColor;

            Gizmos.DrawWireSphere(obstacleOrigin, obstacleRadius);
            Gizmos.DrawLine(obstacleOrigin, obstacleOrigin + (moveDir * obstacleDistance));
            Gizmos.DrawWireSphere(_gizmoObstacleHitInfo.point, obstacleRadius * 0.35f);
        }
        else
        {
            Gizmos.color = gizmoProbeIdleColor;
            Gizmos.DrawWireSphere(obstacleOrigin, obstacleRadius);
            Gizmos.DrawLine(obstacleOrigin, obstacleOrigin + (moveDir * obstacleDistance));
        }

        // 2) Next-ground probe.
        Gizmos.color = new Color(1f, 1f, 0.2f, 1f);
        Gizmos.DrawWireSphere(_gizmoPredictedFeetPoint, 0.06f);

        var start = _gizmoNextGroundCastStart;
        var radius = _gizmoNextGroundCastRadius;
        var dist = _gizmoNextGroundCastDistance;

        if (_gizmoNextGroundHit && _gizmoNextGroundHitInfo.collider != null)
        {
            var col = _gizmoNextGroundHitInfo.collider;
            Gizmos.color = col.CompareTag(buildingTag) ? gizmoProbeBuildingHitColor : gizmoProbeBlockedHitColor;

            Gizmos.DrawWireSphere(start, radius);
            Gizmos.DrawLine(start, start - (up * dist));
            Gizmos.DrawWireSphere(_gizmoNextGroundHitInfo.point, radius * 0.35f);

            var nFrom = _gizmoNextGroundHitInfo.point;
            var nTo = nFrom + (_gizmoNextGroundHitInfo.normal.normalized * 0.6f);
            Gizmos.DrawLine(nFrom, nTo);
        }
        else
        {
            Gizmos.color = gizmoProbeIdleColor;
            Gizmos.DrawWireSphere(start, radius);
            Gizmos.DrawLine(start, start - (up * dist));
        }
    }

    private Vector3 GetBottomSphereCenterGizmo(CharacterController controller)
    {
        var up = transform.up;
        var centerWorld = transform.TransformPoint(controller.center);
        var halfHeight = Mathf.Max(0f, (controller.height * 0.5f) - controller.radius);

        return centerWorld - (up * halfHeight);
    }
}
