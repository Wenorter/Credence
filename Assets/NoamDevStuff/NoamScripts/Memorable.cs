using UnityEngine;

public class Memorable : MonoBehaviour
{
    // Stable per-object ID. You can generate and serialize once in editor.
    [SerializeField] private string guid = "";

    public string Guid => guid;

    public Renderer Renderer { get; private set; }
    public MeshFilter MeshFilter { get; private set; }

    void Awake()
    {
        Renderer = GetComponentInChildren<Renderer>();
        MeshFilter = GetComponentInChildren<MeshFilter>();
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (string.IsNullOrEmpty(guid))
            guid = System.Guid.NewGuid().ToString("N");
    }
#endif
}