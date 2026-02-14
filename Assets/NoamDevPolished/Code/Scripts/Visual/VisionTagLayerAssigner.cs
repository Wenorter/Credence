// Assets/Scripts/Vision/VisionTagLayerAssigner.cs
using System;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class VisionTagLayerAssigner : MonoBehaviour
{
    [Serializable]
    private struct TagToLayer
    {
        [Tooltip("Objects with this tag will be moved to the layer below.")]
        public string tag;

        [Tooltip("Layer name to assign (must exist in Project Settings > Tags and Layers).")]
        public string layerName;
    }

    [Header("Tag → Layer Rules")]
    [Tooltip("Rendering systems filter by Layer, not Tag. This maps gameplay tags into render categories.")]
    [SerializeField] private TagToLayer[] rules =
    {
        new TagToLayer { tag = "Collectible", layerName = "Vision_Collectible" },
    };

    [Header("Scope")]
    [Tooltip("If true, applies the layer to this object and all children recursively.")]
    [SerializeField] private bool includeChildren = true;

    [Header("Editor Behavior")]
    [Tooltip("If enabled, the script will also apply in Edit Mode (changes will persist in the scene).")]
    [SerializeField] private bool applyInEditMode = false;

    private void Awake()
    {
        // Runtime only by default.
        Apply();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        // Only touch layers in Edit Mode if you explicitly want that workflow.
        if (Application.isPlaying)
            return;

        if (!applyInEditMode)
            return;

        Apply();
    }
#endif

    [ContextMenu("Apply Tag → Layer Rules Now")]
    private void Apply()
    {
        if (rules == null || rules.Length == 0)
            return;

        if (includeChildren)
            ApplyRecursive(transform);

        ApplyToGameObject(gameObject);
    }

    private void ApplyRecursive(Transform t)
    {
        ApplyToGameObject(t.gameObject);

        for (var i = 0; i < t.childCount; i++)
            ApplyRecursive(t.GetChild(i));
    }

    private void ApplyToGameObject(GameObject go)
    {
        for (var i = 0; i < rules.Length; i++)
        {
            var r = rules[i];

            if (string.IsNullOrWhiteSpace(r.tag) || string.IsNullOrWhiteSpace(r.layerName))
                continue;

            if (!go.CompareTag(r.tag))
                continue;

            var layer = LayerMask.NameToLayer(r.layerName);
            if (layer < 0)
            {
#if UNITY_EDITOR
                Debug.LogWarning($"Layer '{r.layerName}' does not exist. Add it in Project Settings > Tags and Layers.", go);
#endif
                return;
            }

            go.layer = layer;
            return;
        }
    }
}
