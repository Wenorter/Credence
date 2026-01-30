using UnityEngine;
using UnityEngine.Serialization;

public class Memorable : MonoBehaviour
{
    // Stable per-object ID. You can generate and serialize once in editor.
    [SerializeField] private string guid = "";
    public string layer = "";
    public Color color = Color.white;
    public string ignoredProbeLayer = ""; // this layer wont be seen if the player is close to it , only when the angly highlight it
    public string Guid => guid;
    

    public Renderer Renderer { get; private set; }
    public MeshFilter MeshFilter { get; private set; }

    void Awake()
    {
        Renderer = GetComponentInChildren<Renderer>();
        MeshFilter = GetComponentInChildren<MeshFilter>();
    }
    void Start()
    {
        if (layer == "")
        {
            layer = "Ghost";
            color.a = 0.5f;
        }
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (string.IsNullOrEmpty(guid))
            guid = System.Guid.NewGuid().ToString("N");
    }
#endif
}