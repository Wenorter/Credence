using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
[RequireComponent(typeof(Animator))]
public class PriestAnimationDriver : MonoBehaviour
{
    [Header("Idle / Walk")]
    [Tooltip("Animator bool parameter that switches between idle and walk.")]
    [SerializeField] private string isWalkingParam = "IsWalking";

    [Tooltip("How much input counts as 'walking'. Small dead-zone prevents flicker.")]
    [Range(0.01f, 0.5f)]
    [SerializeField] private float walkInputThreshold = 0.1f;

    private Animator _animator;
    private int _isWalkingHash;
    private float _walkInputThresholdSqr;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _isWalkingHash = Animator.StringToHash(isWalkingParam);
        _walkInputThresholdSqr = walkInputThreshold * walkInputThreshold;
    }

    private void OnValidate()
    {
        _walkInputThresholdSqr = walkInputThreshold * walkInputThreshold;
    }

    // Use this if you wire the method directly from PlayerInput ("Send Messages" / callback context).
    public void OnMove(InputAction.CallbackContext context)
    {
        if (!context.performed && !context.canceled)
            return;

        var moveInput = context.ReadValue<Vector2>();
        var isWalking = moveInput.sqrMagnitude >= _walkInputThresholdSqr;

        _animator.SetBool(_isWalkingHash, isWalking);
    }
}