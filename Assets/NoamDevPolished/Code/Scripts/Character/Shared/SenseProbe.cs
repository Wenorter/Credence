using UnityEngine;

[DisallowMultipleComponent]
public sealed class SenseProbe : MonoBehaviour
{
    [Header("Pass Material (Required)")]
    [SerializeField] private Material passMaterial;

    [Header("Shape (meters)")]
    [Min(0.01f)]
    [SerializeField] private float sideRadius = 2.0f;

    [Min(0.01f)]
    [SerializeField] private float forwardRadius = 3.5f;

    [Min(0.01f)]
    [SerializeField] private float backRadius = 0.8f;

    [Min(0.01f)]
    [SerializeField] private float upRadius = 0.6f;

    [Min(0.01f)]
    [SerializeField] private float downRadius = 2.0f;

    [Header("Soft Edge (meters)")]
    [Min(0.0f)]
    [SerializeField] private float softness = 0.75f;

    [Header("Driver")]
    [Tooltip("If set, uses this transform for position/forward (recommended: the Priest camera). If null, uses this object.")]
    [SerializeField] private Transform driver;

    private static readonly int SenseCenterWSId = Shader.PropertyToID("_SenseCenterWS");
    private static readonly int SenseLightDirWSId = Shader.PropertyToID("_SenseLightDirWS");

    private static readonly int SenseSideRadiusId = Shader.PropertyToID("_SenseSideRadius");
    private static readonly int SenseForwardRadiusId = Shader.PropertyToID("_SenseForwardRadius");
    private static readonly int SenseBackRadiusId = Shader.PropertyToID("_SenseBackRadius");
    private static readonly int SenseUpRadiusId = Shader.PropertyToID("_SenseUpRadius");
    private static readonly int SenseDownRadiusId = Shader.PropertyToID("_SenseDownRadius");
    private static readonly int SenseSoftnessId = Shader.PropertyToID("_SenseSoftness");

    private void LateUpdate()
    {
        if (passMaterial == null)
            return;

        var t = driver != null ? driver : transform;

        var fwd = t.forward;
        if (fwd.sqrMagnitude < 0.0001f)
            fwd = Vector3.forward;

        passMaterial.SetVector(SenseCenterWSId, t.position);
        passMaterial.SetVector(SenseLightDirWSId, fwd.normalized);

        passMaterial.SetFloat(SenseSideRadiusId, Mathf.Max(0.01f, sideRadius));
        passMaterial.SetFloat(SenseForwardRadiusId, Mathf.Max(0.01f, forwardRadius));
        passMaterial.SetFloat(SenseBackRadiusId, Mathf.Max(0.01f, backRadius));
        passMaterial.SetFloat(SenseUpRadiusId, Mathf.Max(0.01f, upRadius));
        passMaterial.SetFloat(SenseDownRadiusId, Mathf.Max(0.01f, downRadius));

        passMaterial.SetFloat(SenseSoftnessId, Mathf.Max(0.0f, softness));
    }
}
