using UnityEngine;

public class GhostInstance
{
    private GameObject _go;
    private MeshRenderer _mr;
    private MeshFilter _mf;
    private MaterialPropertyBlock _mpb;

    private Vector3 _basePos;
    private Quaternion _baseRot;
    private Vector3 _baseScale;

    // Fade-in state
    private float _fadeInDuration = 2f;
    private float _fadeInElapsed = 0f;
    private bool _fadeInActive = true;

    private static readonly int ColorId = Shader.PropertyToID("_Color");
    private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");

    static void SetLayerRecursively(GameObject obj, int layer)
    {
        if (!obj) return;
        obj.layer = layer;
        foreach (Transform t in obj.transform)
            SetLayerRecursively(t.gameObject, layer);
    }

    public static GhostInstance Create(
        Material ghostMat,
        Memorable src,
        Color color,
        string layer,
        bool isFading = true,
        float fadeInSeconds = 8f)
    {
        if (!isFading)
            fadeInSeconds = 0.001f;

        var mesh = (src != null && src.MeshFilter) ? src.MeshFilter.sharedMesh : null;

        var go = new GameObject(layer);

        int layerId = LayerMask.NameToLayer(layer);
        if (layerId < 0) layerId = 0; // fallback to Default if missing
        SetLayerRecursively(go, layerId);

        var mf = go.AddComponent<MeshFilter>();
        var mr = go.AddComponent<MeshRenderer>();

        mf.sharedMesh = mesh;
        mr.sharedMaterial = ghostMat;

        var gi = new GhostInstance
        {
            _go = go,
            _mr = mr,
            _mf = mf,
            _mpb = new MaterialPropertyBlock(),
            _fadeInDuration = Mathf.Max(0.0001f, fadeInSeconds),
            _fadeInElapsed = 0f,
            _fadeInActive = true
        };

        // Start fully invisible
        gi._mr.GetPropertyBlock(gi._mpb);
        gi._mpb.SetFloat("_IsMemory", 1f);
        gi._mpb.SetFloat("_Sense", 0f);
        gi._mpb.SetFloat("_Alpha", 0f);
        gi.SetGhostColor(color);
        gi._mr.SetPropertyBlock(gi._mpb);

        // If we have src, sync transform immediately
        if (src != null)
        {
            gi.SetBaseTransform(src.transform.position, src.transform.rotation, src.transform.lossyScale);
        }

        return gi;
    }

    public void RestartFadeIn(float fadeInSeconds = 2f)
    {
        _fadeInDuration = Mathf.Max(0.0001f, fadeInSeconds);
        _fadeInElapsed = 0f;
        _fadeInActive = true;
    }

    public void SetBaseTransform(Vector3 pos, Quaternion rot, Vector3 scale)
    {
        _basePos = pos;
        _baseRot = rot;
        _baseScale = scale;
    }

    /// <summary>
    /// Updates all ghost-relevant settings from the given Memorable:
    /// mesh, color, layer, and base transform.
    /// NOTE: ignoredProbeLayer is not ghost data, so it's intentionally ignored here.
    /// </summary>
    public void ApplyFromMemorable(Memorable src, bool restartFadeIn = false, float fadeInSeconds = 2f)
    {
        if (src == null) return;
        if (_go == null) return;

        // 1) Mesh
        if (_mf != null)
        {
            var mesh = src.MeshFilter ? src.MeshFilter.sharedMesh : null;
            _mf.sharedMesh = mesh;
        }

        // 2) Layer (by name)
        if (!string.IsNullOrEmpty(src.layer))
        {
            int layerId = LayerMask.NameToLayer(src.layer);
            if (layerId >= 0)
                SetLayerRecursively(_go, layerId);
        }

        // 3) Color
        SetGhostColor(src.color);

        // 4) Base transform snapshot (prevents “teleporting” when the object moved)
        SetBaseTransform(src.transform.position, src.transform.rotation, src.transform.lossyScale);

        // 5) Optional: restart fade-in if you want a clean re-appear effect
        if (restartFadeIn)
            RestartFadeIn(fadeInSeconds);
    }

    /// <summary>
    /// Sets the ghost color in a shader-safe way (supports _BaseColor or _Color).
    /// </summary>
    public void SetGhostColor(Color c)
    {
        if (_mr == null) return;
        if (_mpb == null) _mpb = new MaterialPropertyBlock();

        _mr.GetPropertyBlock(_mpb);

        // Prefer BaseColor if present, else Color
        if (_mr.sharedMaterial != null && _mr.sharedMaterial.HasProperty(BaseColorId))
            _mpb.SetColor(BaseColorId, c);
        else
            _mpb.SetColor(ColorId, c);

        _mr.SetPropertyBlock(_mpb);
    }

    public void UpdateGhost(Vector3 drift, float desiredFade, float dt)
    {
        if (!_go) return;

        _go.transform.position = _basePos + drift;
        _go.transform.rotation = _baseRot;
        _go.transform.localScale = _baseScale;

        desiredFade = Mathf.Clamp01(desiredFade);

        float ramp = 1f;
        if (_fadeInActive)
        {
            _fadeInElapsed += Mathf.Max(0f, dt);
            ramp = Mathf.Clamp01(_fadeInElapsed / _fadeInDuration);
            if (ramp >= 1f) _fadeInActive = false;
        }

        float alpha = desiredFade * ramp;

        _mr.GetPropertyBlock(_mpb);
        _mpb.SetFloat("_IsMemory", 1f);
        _mpb.SetFloat("_Alpha", alpha);
        _mpb.SetFloat("_Sense", alpha);
        _mr.SetPropertyBlock(_mpb);
    }

    public void Destroy()
    {
        if (_go) Object.Destroy(_go);
        _go = null;
    }
}
