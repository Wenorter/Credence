using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class PriestPlayerScript : MonoBehaviour
    {
        [Header("Input")]
        [SerializeField] private InputActionReference moveAction;
        [SerializeField] private InputActionReference lookAction;
        [SerializeField] private InputActionReference sprintAction;

        [Header("Movement")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float sprintSpeed = 7f;
        
        [Header("Rotation")]
        [SerializeField] private float mouseSensitivity = 0.15f;
        [SerializeField] private float minPitch = -85f;
        [SerializeField] private float maxPitch = 85f;

        [Header("Camera")]
        [SerializeField] private CameraScript playerCamera;
        
        private float _pitch;
        private Rigidbody _rb;
    
        private float _pendingYaw;
        private bool _isSprinting;
        
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
            sprintAction.action.Enable();
        }

        private void OnDisable()
        {
            moveAction.action.Disable();
            lookAction.action.Disable();
            sprintAction.action.Disable();

            MoveInput = Vector2.zero;
            _pendingYaw = 0f;
        
            if (_rb) _rb.linearVelocity = Vector2.zero;
        }
    
        private void Update()
        {
            MoveInput = moveAction.action.ReadValue<Vector2>();
            _isSprinting = sprintAction.action.ReadValue<float>() > 0.5f;
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

            float speed = _isSprinting ? sprintSpeed : moveSpeed;
            Vector3 movement = rotation * localDirection * speed;
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

            Gizmos.DrawLine(origin, tip);

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
}
