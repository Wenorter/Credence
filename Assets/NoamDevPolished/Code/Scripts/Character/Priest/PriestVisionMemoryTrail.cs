// Assets/Scripts/Vision/PriestVisionMemoryTrail.cs

using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class PriestVisionMemoryTrail : MonoBehaviour
{
    [Header("Pass Material (Required)")]
    [Tooltip("The SAME Material assigned in your URP Full Screen Pass Renderer Feature.")]
    [SerializeField] private Material passMaterial;

    [Header("World Map (Required)")]
    [Tooltip("Defines one corner of the memory rectangle in world space (XZ).")]
    [SerializeField] private Transform minPoint;

    [Tooltip("Defines the opposite corner of the memory rectangle in world space (XZ).")]
    [SerializeField] private Transform maxPoint;

    [Header("Sense Source (Optional)")]
    [Tooltip("If assigned, keeps this script logically paired with the SenseProbe (we still read values from the material).")]
    [SerializeField] private SenseProbe senseProbe;

    [Header("Memory Texture")]
    [Range(128, 2048)]
    [SerializeField] private int resolution = 512;

    [Header("Fade")]
    [Min(0.05f)]
    [Tooltip("Seconds for memory to fade from 1 -> 0 once an area is no longer inside the sense shape.")]
    [SerializeField] private float fadeSeconds = 20f;

    [Header("Strength (Shader)")]
    [Range(0f, 1f)]
    [SerializeField] private float memoryStrength = 0.75f;

    [Header("Memory Height Gate (No Roof Reveal)")]
    [Tooltip("Memory reveal only applies within this distance ABOVE/BELOW the player height (SenseCenterWS.y).\nSmaller = tighter to player height (less roof/floor bleed).")]
    [Range(0.05f, 2.0f)]
    [SerializeField] private float memoryHeightHalfRange = 0.60f;

    [Tooltip("Soft fade at the top/bottom of the height gate.\n0 = hard cutoff, small values = nicer transition.")]
    [Range(0.0f, 1.0f)]
    [SerializeField] private float memoryHeightSoftness = 0.10f;

    [Header("Sound Stamps (Memory Deposit)")]
    [Tooltip("Caps how many sound stamps we paint into the memory RT per frame.\nKeeps worst-case spam from doing too many blits.")]
    [Range(1, 16)]
    [SerializeField] private int maxSoundStampsPerFrame = 4;
    
    public float FadeSeconds => fadeSeconds;
    
    private RenderTexture _memoryRT;
    private Material _blitMat;

    private struct SoundStamp
    {
        public Vector3 posWS;
        public float radiusMeters;
        public float softnessMeters;
        public float strength01;
    }

    private readonly List<SoundStamp> _soundQueue = new(32);

    private static readonly int MemoryTexId = Shader.PropertyToID("_MemoryTex");
    private static readonly int HasMemoryId = Shader.PropertyToID("_HasMemory");
    private static readonly int MemoryStrengthId = Shader.PropertyToID("_MemoryStrength");
    private static readonly int MemoryWorldOriginXZId = Shader.PropertyToID("_MemoryWorldOriginXZ");
    private static readonly int MemoryWorldSizeXZId = Shader.PropertyToID("_MemoryWorldSizeXZ");

    private static readonly int MemoryHeightHalfRangeId = Shader.PropertyToID("_MemoryHeightHalfRange");
    private static readonly int MemoryHeightSoftnessId = Shader.PropertyToID("_MemoryHeightSoftness");

    // Sense values (read from pass material)
    private static readonly int SenseCenterWSId = Shader.PropertyToID("_SenseCenterWS");
    private static readonly int SenseLightDirWSId = Shader.PropertyToID("_SenseLightDirWS");
    private static readonly int SenseSideRadiusId = Shader.PropertyToID("_SenseSideRadius");
    private static readonly int SenseForwardRadiusId = Shader.PropertyToID("_SenseForwardRadius");
    private static readonly int SenseBackRadiusId = Shader.PropertyToID("_SenseBackRadius");
    private static readonly int SenseSoftnessId = Shader.PropertyToID("_SenseSoftness");

    // Blit shader IDs (sense pass)
    private static readonly int PrevTexId = Shader.PropertyToID("_PrevTex");
    private static readonly int PaintUvId = Shader.PropertyToID("_PaintUV");
    private static readonly int PaintDirUvId = Shader.PropertyToID("_PaintDirUV");
    private static readonly int PaintRadiiUvId = Shader.PropertyToID("_PaintRadiiUV");
    private static readonly int PaintSoftnessUvId = Shader.PropertyToID("_PaintSoftnessUV");
    private static readonly int FadeMulId = Shader.PropertyToID("_FadeMul");
    private static readonly int FadeSubId = Shader.PropertyToID("_Fade");

    // Blit shader IDs (sound stamp pass)
    private static readonly int SoundCenterUvId = Shader.PropertyToID("_SoundCenterUV");
    private static readonly int SoundRadiiUvId = Shader.PropertyToID("_SoundRadiiUV");
    private static readonly int SoundSoftnessUvId = Shader.PropertyToID("_SoundSoftnessUV");
    private static readonly int SoundStampStrengthId = Shader.PropertyToID("_SoundStampStrength");

    private void OnValidate()
    {
        resolution = Mathf.Clamp(resolution, 128, 2048);
        fadeSeconds = Mathf.Max(0.05f, fadeSeconds);
        memoryStrength = Mathf.Clamp01(memoryStrength);

        memoryHeightHalfRange = Mathf.Clamp(memoryHeightHalfRange, 0.05f, 2.0f);
        memoryHeightSoftness = Mathf.Clamp(memoryHeightSoftness, 0.0f, 1.0f);

        maxSoundStampsPerFrame = Mathf.Clamp(maxSoundStampsPerFrame, 1, 16);
    }

    private void OnEnable()
    {
        SetHasMemory(false);
        EnsureResources();
        ApplyMaterialParams(forceClear: true);
    }

    private void OnDisable()
    {
        SetHasMemory(false);

        if (_memoryRT != null)
        {
            _memoryRT.Release();
            _memoryRT = null;
        }

        if (_blitMat != null)
        {
            Destroy(_blitMat);
            _blitMat = null;
        }

        _soundQueue.Clear();
    }

    private void Update()
    {
        if (passMaterial == null || minPoint == null || maxPoint == null)
        {
            SetHasMemory(false);
            return;
        }

        EnsureResources();
        if (_memoryRT == null || _blitMat == null)
        {
            SetHasMemory(false);
            return;
        }

        ApplyMaterialParams(forceClear: false);

        // 1) Do the usual: fade + sense paint.
        PaintSenseAndFade(Time.deltaTime);

        // 2) Add any queued sound stamps WITHOUT additional fading.
        PaintSoundStamps();
    }

    /// <summary>
    /// Called by the sound system to deposit memory into the XZ memory RT.
    /// Radius/softness are in meters. Strength is 0..1.
    /// </summary>
    public void AddSoundStamp(Vector3 worldPos, float radiusMeters, float softnessMeters, float strength01)
    {
        if (passMaterial == null || minPoint == null || maxPoint == null)
            return;

        var stamp = new SoundStamp
        {
            posWS = worldPos,
            radiusMeters = Mathf.Max(0.01f, radiusMeters),
            softnessMeters = Mathf.Max(0.0f, softnessMeters),
            strength01 = Mathf.Clamp01(strength01)
        };

        _soundQueue.Add(stamp);
    }

    private void EnsureResources()
    {
        if (_blitMat == null)
        {
            var blitShader = Shader.Find("Hidden/PriestVisionMemoryTrailBlit");
            if (blitShader == null)
            {
                Debug.LogError("PriestVisionMemoryTrail: Missing shader 'Hidden/PriestVisionMemoryTrailBlit'.", this);
                return;
            }

            _blitMat = new Material(blitShader)
            {
                hideFlags = HideFlags.HideAndDontSave
            };
        }

        var res = Mathf.Clamp(resolution, 128, 2048);

        if (_memoryRT == null || _memoryRT.width != res || _memoryRT.height != res)
        {
            if (_memoryRT != null)
                _memoryRT.Release();

            _memoryRT = new RenderTexture(res, res, 0, RenderTextureFormat.R8)
            {
                name = "_PriestVisionMemoryRT",
                filterMode = FilterMode.Bilinear,
                wrapMode = TextureWrapMode.Clamp,
                useMipMap = false,
                autoGenerateMips = false
            };
            _memoryRT.Create();
            ClearToBlack(_memoryRT);
        }
    }

    private void ApplyMaterialParams(bool forceClear)
    {
        if (passMaterial == null || _memoryRT == null || minPoint == null || maxPoint == null)
        {
            SetHasMemory(false);
            return;
        }

        if (forceClear)
            ClearToBlack(_memoryRT);

        var (originXZ, sizeXZ) = GetWorldRectXZ(minPoint.position, maxPoint.position);

        passMaterial.SetTexture(MemoryTexId, _memoryRT);
        passMaterial.SetVector(MemoryWorldOriginXZId, originXZ);
        passMaterial.SetVector(MemoryWorldSizeXZId, sizeXZ);
        passMaterial.SetFloat(MemoryStrengthId, Mathf.Clamp01(memoryStrength));

        var halfRange = Mathf.Clamp(memoryHeightHalfRange, 0.05f, 2.0f);
        var soft = Mathf.Clamp(memoryHeightSoftness, 0.0f, halfRange);
        passMaterial.SetFloat(MemoryHeightHalfRangeId, halfRange);
        passMaterial.SetFloat(MemoryHeightSoftnessId, soft);

        SetHasMemory(true);
    }

    private void PaintSenseAndFade(float dt)
    {
        var (originXZ, sizeXZ) = GetWorldRectXZ(minPoint.position, maxPoint.position);

        var senseCenterWS = (Vector3)passMaterial.GetVector(SenseCenterWSId);

        var senseDirWS = (Vector3)passMaterial.GetVector(SenseLightDirWSId);
        if (senseDirWS.sqrMagnitude < 0.0001f)
            senseDirWS = Vector3.forward;

        var sideRadius = Mathf.Max(0.05f, passMaterial.GetFloat(SenseSideRadiusId));
        var forwardRadius = Mathf.Max(0.05f, passMaterial.GetFloat(SenseForwardRadiusId));
        var backRadius = Mathf.Max(0.05f, passMaterial.GetFloat(SenseBackRadiusId));
        var softnessMeters = Mathf.Max(0.0f, passMaterial.GetFloat(SenseSoftnessId));

        var fwdXZ = new Vector2(senseDirWS.x, senseDirWS.z);
        if (fwdXZ.sqrMagnitude < 0.0001f)
            fwdXZ = Vector2.up;
        fwdXZ.Normalize();

        var rightXZ = new Vector2(-fwdXZ.y, fwdXZ.x);

        var uvCenter = new Vector2(
            (senseCenterWS.x - originXZ.x) / sizeXZ.x,
            (senseCenterWS.z - originXZ.y) / sizeXZ.y
        );

        var uvVecFwd = new Vector2(fwdXZ.x / sizeXZ.x, fwdXZ.y / sizeXZ.y);
        var uvVecRight = new Vector2(rightXZ.x / sizeXZ.x, rightXZ.y / sizeXZ.y);

        var uvFwdLen = Mathf.Max(0.000001f, uvVecFwd.magnitude);
        var uvRightLen = Mathf.Max(0.000001f, uvVecRight.magnitude);

        var paintDirUV = uvVecFwd / uvFwdLen;

        var sideRadiusUv = sideRadius * uvRightLen;
        var forwardRadiusUv = forwardRadius * uvFwdLen;
        var backRadiusUv = backRadius * uvFwdLen;

        var sideSoftUv = softnessMeters * uvRightLen;
        var foreSoftUv = softnessMeters * uvFwdLen;
        var backSoftUv = softnessMeters * uvFwdLen;

        var sec = Mathf.Max(0.001f, fadeSeconds);
        var fadeSub = dt / sec;

        const float minFadeSub = 1f / 255f / 12f;
        fadeSub = Mathf.Max(fadeSub, minFadeSub);
        fadeSub = Mathf.Clamp01(fadeSub);

        var fadeMul = Mathf.Clamp01(1f - fadeSub);

        _blitMat.SetTexture(PrevTexId, _memoryRT);
        _blitMat.SetVector(PaintUvId, uvCenter);
        _blitMat.SetVector(PaintDirUvId, paintDirUV);
        _blitMat.SetVector(PaintRadiiUvId, new Vector4(sideRadiusUv, forwardRadiusUv, backRadiusUv, 0f));
        _blitMat.SetVector(PaintSoftnessUvId, new Vector4(sideSoftUv, foreSoftUv, backSoftUv, 0f));
        _blitMat.SetFloat(FadeMulId, fadeMul);
        _blitMat.SetFloat(FadeSubId, fadeSub);

        var tmp = RenderTexture.GetTemporary(_memoryRT.descriptor);
        Graphics.Blit(_memoryRT, tmp, _blitMat, 0);
        Graphics.Blit(tmp, _memoryRT);
        RenderTexture.ReleaseTemporary(tmp);
    }

    private void PaintSoundStamps()
    {
        if (_soundQueue.Count == 0)
            return;

        var (originXZ, sizeXZ) = GetWorldRectXZ(minPoint.position, maxPoint.position);

        var stampsThisFrame = Mathf.Min(maxSoundStampsPerFrame, _soundQueue.Count);

        // Paint newest first so old spam gets dropped sooner if we hit the cap.
        for (var n = 0; n < stampsThisFrame; n++)
        {
            var idx = _soundQueue.Count - 1;
            var stamp = _soundQueue[idx];
            _soundQueue.RemoveAt(idx);

            var uvCenter = new Vector2(
                (stamp.posWS.x - originXZ.x) / sizeXZ.x,
                (stamp.posWS.z - originXZ.y) / sizeXZ.y
            );

            // Convert a WORLD circle to UV ellipse radii.
            var radiusUv = new Vector2(
                stamp.radiusMeters / Mathf.Max(sizeXZ.x, 0.0001f),
                stamp.radiusMeters / Mathf.Max(sizeXZ.y, 0.0001f)
            );

            var softUv = new Vector2(
                stamp.softnessMeters / Mathf.Max(sizeXZ.x, 0.0001f),
                stamp.softnessMeters / Mathf.Max(sizeXZ.y, 0.0001f)
            );

            _blitMat.SetTexture(PrevTexId, _memoryRT);
            _blitMat.SetVector(SoundCenterUvId, uvCenter);
            _blitMat.SetVector(SoundRadiiUvId, radiusUv);
            _blitMat.SetVector(SoundSoftnessUvId, softUv);
            _blitMat.SetFloat(SoundStampStrengthId, stamp.strength01);

            var tmp = RenderTexture.GetTemporary(_memoryRT.descriptor);
            Graphics.Blit(_memoryRT, tmp, _blitMat, 1);
            Graphics.Blit(tmp, _memoryRT);
            RenderTexture.ReleaseTemporary(tmp);
        }

        // If we still have a backlog, keep it bounded.
        if (_soundQueue.Count > 256)
            _soundQueue.RemoveRange(0, _soundQueue.Count - 256);
    }

    private static (Vector2 originXZ, Vector2 sizeXZ) GetWorldRectXZ(Vector3 a, Vector3 b)
    {
        var minX = Mathf.Min(a.x, b.x);
        var maxX = Mathf.Max(a.x, b.x);
        var minZ = Mathf.Min(a.z, b.z);
        var maxZ = Mathf.Max(a.z, b.z);

        var origin = new Vector2(minX, minZ);
        var size = new Vector2(Mathf.Max(0.01f, maxX - minX), Mathf.Max(0.01f, maxZ - minZ));
        return (origin, size);
    }

    private static void ClearToBlack(RenderTexture rt)
    {
        var prev = RenderTexture.active;
        RenderTexture.active = rt;
        GL.Clear(false, true, Color.black);
        RenderTexture.active = prev;
    }

    private void SetHasMemory(bool enabled)
    {
        if (passMaterial == null)
            return;

        passMaterial.SetFloat(HasMemoryId, enabled ? 1f : 0f);
    }
}
