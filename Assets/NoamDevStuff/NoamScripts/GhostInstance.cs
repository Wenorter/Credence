using UnityEngine;
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

    static void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform t in obj.transform)
            SetLayerRecursively(t.gameObject, layer);
    }

    public static GhostInstance Create(Material ghostMat, Memorable src, Color color, string layer, float fadeInSeconds = 8f)
    {
        var mesh = src.MeshFilter ? src.MeshFilter.sharedMesh : null;

        var go = new GameObject(layer);
        SetLayerRecursively(go, LayerMask.NameToLayer(layer));

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
        gi._mpb.SetColor("_Color", color);
        gi._mr.SetPropertyBlock(gi._mpb);

        return gi;
    }

    /// <summary>
    /// If you ever want to re-trigger the fade-in (e.g., player leaves and comes back),
    /// call this when you "activate" the ghost again.
    /// </summary>
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

    /// <param name="desiredFade">Your normal fade/brightness target (0..1).</param>
    /// <param name="dt">Pass Time.deltaTime from your manager.</param>
    public void UpdateGhost(Vector3 drift, float desiredFade, float dt)
    {
        if (!_go) return;

        _go.transform.position = _basePos + drift;
        _go.transform.rotation = _baseRot;
        _go.transform.localScale = _baseScale;

        desiredFade = Mathf.Clamp01(desiredFade);

        // Fade-in ramp multiplier (0->1 over _fadeInDuration)
        float ramp = 1f;
        if (_fadeInActive)
        {
            _fadeInElapsed += Mathf.Max(0f, dt);
            ramp = Mathf.Clamp01(_fadeInElapsed / _fadeInDuration);

            if (ramp >= 1f)
                _fadeInActive = false;
        }

        float alpha = desiredFade * ramp;

        _mr.GetPropertyBlock(_mpb);
        _mpb.SetFloat("_IsMemory", 1f);

        // Main fade
        _mpb.SetFloat("_Alpha", alpha);

        // Optional: emission/intensity fade (keep if your shader uses _Sense)
        _mpb.SetFloat("_Sense", alpha);

        _mr.SetPropertyBlock(_mpb);
    }

    public void Destroy()
    {
        if (_go) Object.Destroy(_go);
        _go = null;
    }
}
