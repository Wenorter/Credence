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

    private Color? _color; // if null -> default white

    static void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform t in obj.transform)
            SetLayerRecursively(t.gameObject, layer);
    }

    public static GhostInstance Create(Material ghostMat, Memorable src, Color? color = null)
    {
        // If your objects are not MeshFilter-based (SkinnedMesh), consider using a proxy mesh instead.
        var mesh = src.MeshFilter ? src.MeshFilter.sharedMesh : null;

        var go = new GameObject(src.layer);
        SetLayerRecursively(go, LayerMask.NameToLayer(src.layer));
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
            _color = color
        };

        // Start hidden-ish
        gi._mr.GetPropertyBlock(gi._mpb);
        gi._mpb.SetFloat("_IsMemory", 1f);
        gi._mpb.SetFloat("_Sense", 1f);
        gi._mpb.SetFloat("_Alpha", 0f);

        // Color (defaults to white if not provided)
        gi._mpb.SetColor("_Color", gi._color ?? Color.red);

        gi._mr.SetPropertyBlock(gi._mpb);

        return gi;
    }

    public void SetColor(Color? color)
    {
        _color = color;

        if (!_mr) return;

        _mr.GetPropertyBlock(_mpb);
        _mpb.SetColor("_Color", _color ?? Color.white);
        _mr.SetPropertyBlock(_mpb);
    }

    public void SetBaseTransform(Vector3 pos, Quaternion rot, Vector3 scale)
    {
        _basePos = pos;
        _baseRot = rot;
        _baseScale = scale;
    }

    public void UpdateGhost(Vector3 drift, float confidence)
    {
        if (!_go) return;

        _go.transform.position = _basePos + drift;
        _go.transform.rotation = _baseRot;
        _go.transform.localScale = _baseScale;

        // Fade out with confidence
        float alpha = Mathf.Clamp01(confidence);

        _mr.GetPropertyBlock(_mpb);
        _mpb.SetFloat("_IsMemory", 1f);
        _mpb.SetFloat("_Alpha", alpha);
        
        _mr.SetPropertyBlock(_mpb);
    }

    public void Destroy()
    {
        if (_go) Object.Destroy(_go);
        _go = null;
    }
}
