// Assets/Scripts/GeneratedColliderMarker.cs
//
// Runtime-safe marker component used by the editor tool.
//
// IMPORTANT:
// - This file must NOT be inside an "Editor" folder.
// - It contains NO UnityEditor references, so it is safe in builds.

using UnityEngine;

public sealed class GeneratedColliderMarker : MonoBehaviour
{
    public enum ColliderKind
    {
        None = 0,
        StaticMeshCollider = 1,         // non-convex MeshCollider (static objects)
        DynamicConvexMeshCollider = 2,  // convex MeshCollider (Rigidbody objects)
        BoxColliderFallback = 3
    }

    [Header("Generation Status")]
    [Tooltip("True if the last generation attempt succeeded.")]
    public bool generatedSuccessfully = false;

    [Tooltip("What collider type was generated last time.")]
    public ColliderKind generatedColliderKind = ColliderKind.None;

    [Header("Debug / Diagnostics")]
    [Tooltip("Short human-readable reason if generation failed.")]
    [TextArea(2, 6)]
    public string lastFailureReason = "";

    [Tooltip("Cell size used when generating (for mesh-based colliders).")]
    public float cellSizeUsed = 0f;

    [Tooltip("Triangle count of the generated mesh (for mesh-based colliders).")]
    public int triangleCount = 0;

    [Header("Asset Tracking")]
    [Tooltip("If a mesh asset was created for the MeshCollider, this is its asset path.")]
    public string generatedMeshAssetPath = "";
}