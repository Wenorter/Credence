using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class AngelPlayerScript : MonoBehaviour
    {
        [Header("Input")]
        [SerializeField] private InputActionReference moveAction;
        [SerializeField] private InputActionReference lookAction;
        [SerializeField] private InputActionReference ascendAction;
        [SerializeField] private InputActionReference descendAction;

        [Header("Movement")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float verticalSpeed = 5f;

        [Header("Rotation")]
        [SerializeField] private float mouseSensitivity = 0.15f;
        [SerializeField] private float minPitch = -85f;
        [SerializeField] private float maxPitch = 85f;

        private Rigidbody _rb;

        private float _yaw;
        private float _pitch;
        private float _verticalInput;

        public Vector2 MoveInput { get; private set; }

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _rb.freezeRotation = true;
            _rb.useGravity = false;

            // Preserve the initial orientation, without roll.
            Vector3 angles = _rb.rotation.eulerAngles;

            _yaw = angles.y;
            _pitch = Mathf.DeltaAngle(0f, angles.x);
            _pitch = Mathf.Clamp(_pitch, minPitch, maxPitch);
        }

        private void OnEnable()
        {
            _rb.useGravity = false;
            _rb.linearVelocity = Vector3.zero;

            moveAction.action.Enable();
            lookAction.action.Enable();
            ascendAction.action.Enable();
            descendAction.action.Enable();
        }

        private void OnDisable()
        {
            moveAction.action.Disable();
            lookAction.action.Disable();
            ascendAction.action.Disable();
            descendAction.action.Disable();

            MoveInput = Vector2.zero;
            _verticalInput = 0f;

            if (_rb)
            {
                _rb.linearVelocity = Vector3.zero;
            }
        }

        private void Update()
        {
            MoveInput = moveAction.action.ReadValue<Vector2>();

            float up = ascendAction.action.ReadValue<float>();
            float down = descendAction.action.ReadValue<float>();

            _verticalInput = up - down;

            Vector2 mouseDelta = lookAction.action.ReadValue<Vector2>();

            _yaw += mouseDelta.x * mouseSensitivity;

            _pitch -= mouseDelta.y * mouseSensitivity;
            _pitch = Mathf.Clamp(_pitch, minPitch, maxPitch);
        }

        private void FixedUpdate()
        {
            Quaternion rotation = Quaternion.Euler(_pitch, _yaw, 0f);

            _rb.MoveRotation(rotation);

            Vector2 input = Vector2.ClampMagnitude(MoveInput, 1f);

            Vector3 localDirection = new Vector3(
                input.x,
                0f,
                input.y
            );

            Vector3 flightVelocity = rotation * localDirection * moveSpeed;
            Vector3 worldVerticalVelocity = Vector3.up * (_verticalInput * verticalSpeed);
            _rb.linearVelocity = flightVelocity + worldVerticalVelocity;
        }
    }
}