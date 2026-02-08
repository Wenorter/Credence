using UnityEngine;

[DisallowMultipleComponent]
public sealed class SenseProbe : MonoBehaviour
{
    [Header("Pass Material (Required)")]
    [SerializeField] private Material passMaterial;

    [Header("Sense Settings")]
    [Min(0.01f)]
    [SerializeField] private float senseRadius = 2.5f;

    [Min(0.0f)]
    [SerializeField] private float senseSoftness = 0.75f;

    [Header("Driver")]
    [Tooltip("If set, uses this transform for position/forward (recommended: the Priest camera). If null, uses this object.")]
    [SerializeField] private Transform driver;

    private static readonly int SenseCenterWSId = Shader.PropertyToID("_SenseCenterWS");
    private static readonly int SenseLightDirWSId = Shader.PropertyToID("_SenseLightDirWS");
    private static readonly int SenseRadiusId = Shader.PropertyToID("_SenseRadius");
    private static readonly int SenseSoftnessId = Shader.PropertyToID("_SenseSoftness");

    private void LateUpdate()
    {
        if (passMaterial == null)
            return;

        var t = driver != null ? driver : transform;

        // Write to the SAME material used by the Full Screen Pass.
        passMaterial.SetVector(SenseCenterWSId, t.position);
        passMaterial.SetVector(SenseLightDirWSId, t.forward);
        passMaterial.SetFloat(SenseRadiusId, senseRadius);
        passMaterial.SetFloat(SenseSoftnessId, senseSoftness);
    }
}