using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class PriestVisionMaterialMechanics : MonoBehaviour
    {
        private static readonly int FadeEndId = Shader.PropertyToID("_Fade_End");

        [Header("References")]
        [SerializeField] private Rigidbody target;
        [SerializeField] private Material material;

        [Header("Speed range (units/sec)")]
        [SerializeField] private float minSpeed = 0f;
        [SerializeField] private float maxSpeed = 7f; 

        [Header("_Fade_End range")]
        [SerializeField] private float valueAtMinSpeed = 4.5f;
        [SerializeField] private float valueAtMaxSpeed = 10f;

        [Header("Smoothing")]
        [SerializeField] private float smoothTime = 0.15f;

        private float _current;
        private float _velocity;
        private float _originalValue;

        private void Reset()
        {
            target = GetComponent<Rigidbody>();
        }

        private void Awake()
        {
            if (!target) target = GetComponent<Rigidbody>();
        }

        private void OnEnable()
        {
            if (!material) return;

            _originalValue = material.GetFloat(FadeEndId);
            _current = valueAtMinSpeed;
            material.SetFloat(FadeEndId, _current);
        }

        private void OnDisable()
        {
            if (material) material.SetFloat(FadeEndId, _originalValue);
        }

        private void Update()
        {
            if (!target || !material) return;

            Vector3 v = target.linearVelocity;
            float speed = new Vector2(v.x, v.z).magnitude;

            float t = Mathf.InverseLerp(minSpeed, maxSpeed, speed);
            float targetValue = Mathf.Lerp(valueAtMinSpeed, valueAtMaxSpeed, t);

            _current = Mathf.SmoothDamp(_current, targetValue, ref _velocity, smoothTime);
            material.SetFloat(FadeEndId, _current);
        }
    }
}
