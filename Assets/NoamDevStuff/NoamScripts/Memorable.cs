using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Memorable : MonoBehaviour
{
    // Stable per-object ID, generated at runtime.
    // Leave empty in the inspector.
    [SerializeField] private string guid = "";

    public string layer = "";
    public Color color = Color.white;
    public string ignoredProbeLayer = ""; // this layer wont be seen if the player is close to it , only when the angly highlight it
    public string Guid => guid;

    public Renderer Renderer { get; private set; }
    public MeshFilter MeshFilter { get; private set; }
    public bool isTrigger;
    
    public bool isHighlighted;

    // Extra safety: if something accidentally duplicates a guid at runtime, we fix it.
    private static readonly HashSet<string> _usedGuidsRuntime = new HashSet<string>();

    void Awake()
    {
        Renderer = GetComponentInChildren<Renderer>();
        MeshFilter = GetComponentInChildren<MeshFilter>();

        EnsureGuidRuntime();
    }

    void Start()
    {
        if (layer == "")
        {
            layer = "Ghost";
        }
    }

    private void EnsureGuidRuntime()
    {
        // Always generate if empty (your request)
        if (string.IsNullOrEmpty(guid))
            guid = System.Guid.NewGuid().ToString("N");

        // Safety: avoid collisions in the same run
        if (_usedGuidsRuntime.Contains(guid))
            guid = System.Guid.NewGuid().ToString("N");

        _usedGuidsRuntime.Add(guid);
    }

    private void OnDestroy()
    {
        if (!string.IsNullOrEmpty(guid))
            _usedGuidsRuntime.Remove(guid);
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        // Keep it empty in editor (do not generate here).
        // If you accidentally set something, you can clear it manually.
    }
#endif
}