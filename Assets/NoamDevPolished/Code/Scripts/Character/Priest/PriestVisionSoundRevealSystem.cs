// Assets/Scripts/Vision/PriestVisionSoundRevealSystem.cs
//
// Sound blobs:
// - Strength controls radius only
// - Brightness is NOT tied to strength; shader multiplies sound by _MemoryStrength
// - Long sounds: re-triggering refreshes lastHeardTime (no re-expand flicker)
// - Optional: deposits into memory RT (throttled)

using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class PriestVisionSoundRevealSystem : MonoBehaviour
{
    [Header("Pass Material (Required)")]
    [Tooltip("The SAME Material used by your URP Full Screen Pass.")]
    [SerializeField] private Material passMaterial;

    [Header("Memory Deposit (Recommended)")]
    [Tooltip("If assigned, sounds also deposit into the memory RT (so they linger and fade naturally).")]
    [SerializeField] private PriestVisionMemoryTrail memoryTrail;

    [Header("Blob Pool")]
    [Range(1, 16)]
    [SerializeField] private int maxBlobs = 10;

    [Tooltip("If a new sound happens close to an existing blob, we refresh that blob instead of creating a new one.")]
    [Min(0.01f)]
    [SerializeField] private float refreshDistance = 0.75f;

    [Header("Radius (Strength Mapping)")]
    [Min(0.01f)]
    [SerializeField] private float minRadius = 2.0f;

    [Min(0.01f)]
    [SerializeField] private float maxRadius = 12.0f;

    [Header("Visual Shape")]
    [Tooltip("How soft the blob edge looks (meters). Bigger = softer edge.")]
    [Range(0.01f, 3.0f)]
    [SerializeField] private float blobEdgeSoftness = 0.65f;

    [Tooltip("How quickly the blob expands to the target radius (seconds).")]
    [Range(0.01f, 0.5f)]
    [SerializeField] private float blobExpandSeconds = 0.10f;

    [Header("Fade Time")]
    [Tooltip("If enabled, blob fade time matches PriestVisionMemoryTrail.fadeSeconds.")]
    [SerializeField] private bool matchMemoryFadeTime = true;

    [Tooltip("Fallback fade seconds if match is off or memoryTrail is missing.")]
    [Min(0.05f)]
    [SerializeField] private float soundFadeSeconds = 20f;

    [Header("Memory Deposit")]
    [Tooltip("How strong the memory stamp is (0..1). Strength only controls radius.")]
    [Range(0f, 1f)]
    [SerializeField] private float memoryDepositStrength = 0.35f;

    [Tooltip("Softness of the memory stamp edge (meters).")]
    [Range(0.0f, 5.0f)]
    [SerializeField] private float memoryStampSoftness = 1.25f;

    [Tooltip("Minimum time (seconds) between memory deposits.\nPrevents long sounds from spamming the RT every tick.")]
    [Range(0.01f, 1.0f)]
    [SerializeField] private float memoryDepositMinInterval = 0.20f;

    [Header("Master")]
    [Tooltip("Global multiplier for sound blobs (0..1). Use this only for overall tuning.")]
    [Range(0f, 1f)]
    [SerializeField] private float soundBlobGlobal = 1.0f;

    private struct Blob
    {
        public Vector3 pos;
        public float startTime;     // used ONLY for expand (never reset on refresh)
        public float lastHeardTime; // refreshed while sound continues (drives fade)
        public float radius;
        public float intensity;     // keep as 1.0; brightness matching is handled in shader via _MemoryStrength
        public float edgeSoftness;
    }

    private readonly List<Blob> _blobs = new(16);
    private float _nextAllowedMemoryDepositTime;

    private static readonly int SoundPulseCountId = Shader.PropertyToID("_SoundPulseCount");
    private static readonly int SoundPulseGlobalId = Shader.PropertyToID("_SoundPulseGlobal");
    private static readonly int SoundBlobExpandSecondsId = Shader.PropertyToID("_SoundBlobExpandSeconds");
    private static readonly int SoundFadeSecondsId = Shader.PropertyToID("_SoundFadeSeconds");
    private static readonly int SoundPulseDataId = Shader.PropertyToID("_SoundPulseData");
    private static readonly int SoundPulseParamsId = Shader.PropertyToID("_SoundPulseParams");

    private readonly Vector4[] _data = new Vector4[16];
    private readonly Vector4[] _pars = new Vector4[16];

    private void OnValidate()
    {
        maxBlobs = Mathf.Clamp(maxBlobs, 1, 16);
        refreshDistance = Mathf.Max(0.01f, refreshDistance);

        minRadius = Mathf.Max(0.01f, minRadius);
        maxRadius = Mathf.Max(minRadius, maxRadius);

        blobEdgeSoftness = Mathf.Clamp(blobEdgeSoftness, 0.01f, 3.0f);
        blobExpandSeconds = Mathf.Clamp(blobExpandSeconds, 0.01f, 0.5f);

        soundFadeSeconds = Mathf.Max(0.05f, soundFadeSeconds);

        memoryDepositStrength = Mathf.Clamp01(memoryDepositStrength);
        memoryStampSoftness = Mathf.Clamp(memoryStampSoftness, 0.0f, 5.0f);
        memoryDepositMinInterval = Mathf.Clamp(memoryDepositMinInterval, 0.01f, 1.0f);

        soundBlobGlobal = Mathf.Clamp01(soundBlobGlobal);
    }

    private void OnDisable()
    {
        if (passMaterial == null)
            return;

        passMaterial.SetFloat(SoundPulseCountId, 0f);
        passMaterial.SetFloat(SoundPulseGlobalId, soundBlobGlobal);
    }

    private void Update()
    {
        if (passMaterial == null)
            return;

        var now = Time.time;
        var fadeSec = GetFadeSeconds();

        // Remove blobs that haven't been refreshed for fadeSec seconds.
        for (var i = _blobs.Count - 1; i >= 0; i--)
        {
            var b = _blobs[i];
            if (now - b.lastHeardTime > fadeSec)
                _blobs.RemoveAt(i);
        }

        passMaterial.SetFloat(SoundPulseGlobalId, soundBlobGlobal);
        passMaterial.SetFloat(SoundBlobExpandSecondsId, blobExpandSeconds);
        passMaterial.SetFloat(SoundFadeSecondsId, fadeSec);

        var count = Mathf.Min(_blobs.Count, maxBlobs);
        passMaterial.SetFloat(SoundPulseCountId, count);

        // Newest first.
        for (var i = 0; i < count; i++)
        {
            var b = _blobs[_blobs.Count - 1 - i];

            // data.w = startTime
            _data[i] = new Vector4(b.pos.x, b.pos.y, b.pos.z, b.startTime);

            // params.z = lastHeardTime (drives fade)
            _pars[i] = new Vector4(b.radius, b.intensity, b.lastHeardTime, b.edgeSoftness);
        }

        for (var i = count; i < 16; i++)
        {
            _data[i] = Vector4.zero;
            _pars[i] = Vector4.zero;
        }

        passMaterial.SetVectorArray(SoundPulseDataId, _data);
        passMaterial.SetVectorArray(SoundPulseParamsId, _pars);
    }

    /// <summary>
    /// Call this repeatedly while a sound is "playing".
    /// Strength controls radius only.
    /// </summary>
    public void TriggerSound(Vector3 worldPos, float strength01)
    {
        if (passMaterial == null)
            return;

        strength01 = Mathf.Clamp01(strength01);
        var radius = Mathf.Lerp(minRadius, maxRadius, strength01);

        if (!TryRefreshNearby(worldPos, radius))
        {
            var now = Time.time;

            var b = new Blob
            {
                pos = worldPos,
                startTime = now,
                lastHeardTime = now,
                radius = radius,
                intensity = 1.0f, // brightness match is handled in shader via _MemoryStrength
                edgeSoftness = blobEdgeSoftness
            };

            _blobs.Add(b);

            while (_blobs.Count > maxBlobs * 2)
                _blobs.RemoveAt(0);
        }

        if (memoryTrail != null && memoryDepositStrength > 0.0001f)
            TryDepositMemory(worldPos, radius);
    }

    private bool TryRefreshNearby(Vector3 pos, float radius)
    {
        var r2 = refreshDistance * refreshDistance;
        var now = Time.time;

        for (var i = _blobs.Count - 1; i >= 0; i--)
        {
            var b = _blobs[i];

            if ((b.pos - pos).sqrMagnitude > r2)
                continue;

            // Keep original startTime so expand happens only once.
            b.lastHeardTime = now;

            // Loudness only increases radius (never changes brightness).
            b.radius = Mathf.Max(b.radius, radius);

            b.edgeSoftness = blobEdgeSoftness;
            b.intensity = 1.0f;

            // Follow moving emitters.
            b.pos = pos;

            _blobs[i] = b;
            return true;
        }

        return false;
    }

    private float GetFadeSeconds()
    {
        if (matchMemoryFadeTime && memoryTrail != null)
            return Mathf.Max(0.05f, memoryTrail.FadeSeconds);

        return Mathf.Max(0.05f, soundFadeSeconds);
    }

    private void TryDepositMemory(Vector3 pos, float radius)
    {
        if (Time.time < _nextAllowedMemoryDepositTime)
            return;

        _nextAllowedMemoryDepositTime = Time.time + memoryDepositMinInterval;

        // Deposit strength is constant; radius comes from sound strength.
        memoryTrail.AddSoundStamp(pos, radius, memoryStampSoftness, memoryDepositStrength);
    }
}
