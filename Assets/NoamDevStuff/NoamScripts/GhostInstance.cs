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

    static void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform t in obj.transform)
            SetLayerRecursively(t.gameObject, layer);
    }

    
    public static GhostInstance Create(Material ghostMat, Memorable src , Color color , string layer)
    {
        // If your objects are not MeshFilter-based (SkinnedMesh), consider using a proxy mesh instead.
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
            _mpb = new MaterialPropertyBlock()
        };

        // Start hidden-ish
        gi._mr.GetPropertyBlock(gi._mpb);
        gi._mpb.SetFloat("_IsMemory", 1f);
        gi._mpb.SetFloat("_Sense", 1f);
        gi._mpb.SetFloat("_Alpha", 0f);
        gi._mpb.SetColor("_Color", color);
        gi._mr.SetPropertyBlock(gi._mpb);

        return gi;
    }

    public void SetBaseTransform(Vector3 pos, Quaternion rot, Vector3 scale)
    {
        _basePos = pos;
        _baseRot = rot;
        _baseScale = scale;
    }

    public void UpdateGhost(Vector3 drift, float fade)
    {
        if (!_go) return;

        _go.transform.position = _basePos + drift;
        _go.transform.rotation = _baseRot;
        _go.transform.localScale = _baseScale;

        float alpha = Mathf.Clamp01(fade);

        _mr.GetPropertyBlock(_mpb);
        _mpb.SetFloat("_IsMemory", 1f);

        // Main continuous fade
        _mpb.SetFloat("_Alpha", alpha);

        // Optional: also reduce intensity so bloom fades smoothly
        // (Only keep this if your ghost shader uses _Sense to drive emission)
        _mpb.SetFloat("_Sense", alpha);

        _mr.SetPropertyBlock(_mpb);
    }


    public void Destroy()
    {
        if (_go) Object.Destroy(_go);
        _go = null;
    }
}