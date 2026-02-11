using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
[RequireComponent(typeof(Animator))]
public sealed class PriestAnimationDriver : MonoBehaviour
{
    [Header("Idle / Walk")]
    [SerializeField] private string isWalkingParam = "IsWalking";
    [Range(0.01f, 0.5f)]
    [SerializeField] private float walkInputThreshold = 0.1f;

    [Header("Camera Head Follow")]
    [SerializeField] private Transform cameraTransform;

    [Header("Offsets")]
    [SerializeField] private Vector3 localOffset = new Vector3(0f, 0.0f, 0.125f);

    [Header("Head Pitch Offset")]
    [Tooltip("Extra head pitch applied on top of the camera pitch.\n Negative = look down, Positive = look up.")]
    [SerializeField] private float headPitchOffsetDegrees = -50f;

    private Animator _animator;
    private int _isWalkingHash;
    private float _walkInputThresholdSqr;

    private Transform _head;
    private Quaternion _headBindLocalRotation;

    private void Reset()
    {
        _animator = GetComponent<Animator>();
    }

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _isWalkingHash = Animator.StringToHash(isWalkingParam);
        _walkInputThresholdSqr = walkInputThreshold * walkInputThreshold;

        _head = _animator.GetBoneTransform(HumanBodyBones.Head);
        if (_head == null)
        {
            Debug.LogError("PriestAnimationDriver: could not resolve Humanoid Head bone. Make sure this Animator is Humanoid.", this);
            enabled = false;
            return;
        }

        _headBindLocalRotation = _head.localRotation;

        if (cameraTransform == null)
        {
            Debug.LogError("PriestAnimationDriver: cameraTransform is not set.", this);
            enabled = false;
            return;
        }
    }

    private void OnValidate()
    {
        _walkInputThresholdSqr = walkInputThreshold * walkInputThreshold;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (!context.performed && !context.canceled)
            return;

        var moveInput = context.ReadValue<Vector2>();
        var isWalking = moveInput.sqrMagnitude >= _walkInputThresholdSqr;
        _animator.SetBool(_isWalkingHash, isWalking);
    }

    private void LateUpdate()
    {
        ApplyHeadPitchFromCamera();
        FollowHeadWithCamera();
    }

    private void ApplyHeadPitchFromCamera()
    {
        if (_head == null || cameraTransform == null)
            return;

        var pitch = cameraTransform.localEulerAngles.x;
        if (pitch > 180f) pitch -= 360f;

        // Keep the original correct mapping.
        // Only invert how the OFFSET is applied: positive offset => look down.
        pitch -= headPitchOffsetDegrees;

        var pitchOffset = Quaternion.AngleAxis(pitch, Vector3.right);
        _head.localRotation = _headBindLocalRotation * pitchOffset;
    }

    private void FollowHeadWithCamera()
    {
        var headPos = _head.position;
        var headRot = _head.rotation;

        cameraTransform.position = headPos + headRot * localOffset;
    }
}
