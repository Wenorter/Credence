using UnityEngine;
using UnityEngine.InputSystem;

public class PriestPlayerScript : MonoBehaviour
{
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference lookAction;
    
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float mouseSensitivity = 0.15f;
    [SerializeField] private PriestCameraScript playerCamera;
    [SerializeField] private float minPitch = -85f;
    [SerializeField] private float maxPitch = 85f;

    private float _pitch;
    private Rigidbody _rb;
    
    private float _pendingYaw;
    
    public Vector2 MoveInput { get; private set; }
    
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.freezeRotation = true;
    }
    
    private void OnEnable()
    {
        moveAction.action.Enable();
        lookAction.action.Enable();
    }

    private void OnDisable()
    {
        moveAction.action.Disable();
        lookAction.action.Disable();

        MoveInput = Vector2.zero;
        _pendingYaw = 0f;
        
        if (_rb) _rb.linearVelocity = Vector2.zero;
    }
    
    private void Update()
    {
        MoveInput = moveAction.action.ReadValue<Vector2>();
        Vector2 mouseDelta = lookAction.action.ReadValue<Vector2>();

        _pendingYaw += mouseDelta.x * mouseSensitivity;
        _pitch -= mouseDelta.y * mouseSensitivity;
        _pitch = Mathf.Clamp(_pitch, minPitch, maxPitch);
    }
    
    private void FixedUpdate()
    {
        Quaternion rotation = _rb.rotation * Quaternion.Euler(0f, _pendingYaw, 0f);
        if (_pendingYaw != 0f)
        {
            _rb.MoveRotation(rotation);
            _pendingYaw = 0f;
        }
        
        playerCamera.SetPitch(_pitch);

        Vector2 input = Vector2.ClampMagnitude(MoveInput, 1f);
        Vector3 localDirection = new Vector3(
            input.x,
            0f,
            input.y
        );

        Vector3 movement = rotation * localDirection * moveSpeed;
        _rb.linearVelocity = new Vector3(
            movement.x,
            _rb.linearVelocity.y,
            movement.z
        );
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Vector3 origin = transform.position + Vector3.up * 1f;
        Vector3 forward = transform.forward;
        Vector3 tip = origin + forward * 2f;

        // Arrow shaft
        Gizmos.DrawLine(origin, tip);

        // Arrow head
        Vector3 headBase = tip - forward * 0.3f;

        Gizmos.DrawLine(
            tip,
            headBase + transform.right * 0.3f * 0.5f
        );

        Gizmos.DrawLine(
            tip,
            headBase - transform.right * 0.3f * 0.5f
        );
    }
}
