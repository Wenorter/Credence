using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(CharacterController))]
public sealed class PriestLogic : MonoBehaviour
{
    [Header("Movement")]
    [Tooltip("Represents the player's 'virtual mass' when using a CharacterController.\nUsed only for pushing logic (CharacterController itself has no mass).")]
    [SerializeField] private float characterMass = 80f;

    [Tooltip("Safety: if the object's mass is greater than (characterMass * this), we don't push it at all.\nExample: 3 means anything 3x heavier than the player is immovable by bumping.")]
    [SerializeField] private float maxPushableMassRatio = 3f;

    [Tooltip("How much the push scales with player speed.\nHigher = stronger shove when running.")]
    [SerializeField] private float pushPerSpeed = 0.12f;

    [Tooltip("Maximum velocity change applied to the object per hit.\nKeeps small props from getting launched when sprinting.")]
    [SerializeField] private float maxDeltaV = 2.5f;

    [Tooltip("Ignore tiny bumps to reduce jittery nudges on contact.")]
    [SerializeField] private float minSpeedToPush = 0.15f;

    [Tooltip("If true, only push along the ground plane (prevents launching objects upward).")]
    [SerializeField] private bool ignoreVerticalPush = true;

    [Tooltip("If true, don't push kinematic rigidbodies (usually animated / scripted).")]
    [SerializeField] private bool skipKinematic = true;

    [Header("Debug")]
    [Tooltip("Logs why something wasn't pushed (helpful while tuning / fixing setup).")]
    [SerializeField] private bool logWhyNotPushed = false;

    private CharacterController _characterController;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();

        if (_characterController == null)
        {
            Debug.LogError("PriestLogic: Missing CharacterController (RequireComponent should have prevented this).", this);
            enabled = false;
            return;
        }

        // Keep values sane even if someone types nonsense in the inspector.
        characterMass = Mathf.Max(0.01f, characterMass);
        maxPushableMassRatio = Mathf.Max(0.01f, maxPushableMassRatio);
        pushPerSpeed = Mathf.Max(0f, pushPerSpeed);
        maxDeltaV = Mathf.Max(0f, maxDeltaV);
        minSpeedToPush = Mathf.Max(0f, minSpeedToPush);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        var rb = hit.collider.attachedRigidbody;
        if (rb == null)
        {
            if (logWhyNotPushed)
                Debug.Log($"PriestLogic: Hit '{hit.collider.name}' but it has no Rigidbody.", hit.collider);
            return;
        }
        //Disabling isKinematic 
        rb.isKinematic = false;

        // Speed of the CharacterController at the moment of impact.
        // (This is why it feels 'real': running into stuff pushes more than walking.)
        var speed = _characterController.velocity.magnitude;
        if (speed < minSpeedToPush)
            return;

        // Safety rule: don't let the player shove giant props by accident.
        var maxPushableMass = characterMass * maxPushableMassRatio;
        if (rb.mass > maxPushableMass)
        {
            if (logWhyNotPushed)
                Debug.Log($"PriestLogic: Hit '{hit.collider.name}' but it's too heavy (mass {rb.mass:0.##} > {maxPushableMass:0.##}).", hit.collider);
            return;
        }

        // Direction we push in. CharacterController hit directions often have a downward component when grounded.
        var dir = ignoreVerticalPush
            ? new Vector3(hit.moveDirection.x, 0f, hit.moveDirection.z)
            : hit.moveDirection;

        if (dir.sqrMagnitude < 0.0001f)
            return;

        dir.Normalize();

        // Mass + speed logic:
        // - Faster player => larger shove.
        // - Heavier object => smaller velocity change.
        //
        // We compute a target delta-V for the object:
        // deltaV ≈ (characterMass / objectMass) * speed * pushPerSpeed
        // So:
        // - light objects get pushed noticeably
        // - heavy objects barely move even at high speed
        var massRatio = characterMass / Mathf.Max(0.01f, rb.mass);
        var deltaV = speed * pushPerSpeed * massRatio;

        if (maxDeltaV > 0f)
            deltaV = Mathf.Min(deltaV, maxDeltaV);

        if (deltaV <= 0f)
            return;

        // VelocityChange applies an immediate change in velocity (stable for quick shoves).
        rb.AddForce(dir * deltaV, ForceMode.VelocityChange);
    }
}
