using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Rendering;

namespace MeshProcess
{
    public class VhacdRuntime : MonoBehaviour
    {
        [Serializable]
        public unsafe struct Parameters
        {
            public void Init()
            {
                m_resolution = 100000;
                m_concavity = 0.001;
                m_planeDownsampling = 4;
                m_convexhullDownsampling = 4;
                m_alpha = 0.05;
                m_beta = 0.05;
                m_pca = 0;
                m_mode = 0; // 0: voxel-based (recommended), 1: tetrahedron-based
                m_maxNumVerticesPerCH = 64;
                m_minVolumePerCH = 0.0001;
                m_callback = null;
                m_logger = null;
                m_convexhullApproximation = 1;
                m_oclAcceleration = 0;
                m_maxConvexHulls = 1024;
                m_projectHullVertices = true;
            }

            [Tooltip("maximum concavity")]
            [Range(0, 1)]
            public double m_concavity;

            [Tooltip("controls the bias toward clipping along symmetry planes")]
            [Range(0, 1)]
            public double m_alpha;

            [Tooltip("controls the bias toward clipping along revolution axes")]
            [Range(0, 1)]
            public double m_beta;

            [Tooltip("controls the adaptive sampling of the generated convex-hulls")]
            [Range(0, 0.01f)]
            public double m_minVolumePerCH;

            public void* m_callback;
            public void* m_logger;

            [Tooltip("maximum number of voxels generated during the voxelization stage")]
            [Range(10000, 64000000)]
            public uint m_resolution;

            [Tooltip("controls the maximum number of vertices per convex-hull")]
            [Range(4, 1024)]
            public uint m_maxNumVerticesPerCH;

            [Tooltip("controls the granularity of the search for the \"best\" clipping plane")]
            [Range(1, 16)]
            public uint m_planeDownsampling;

            [Tooltip("controls the precision of the convex-hull generation process during the clipping plane selection stage")]
            [Range(1, 16)]
            public uint m_convexhullDownsampling;

            [Tooltip("enable/disable normalizing the mesh before applying the convex decomposition")]
            [Range(0, 1)]
            public uint m_pca;

            [Tooltip("0: voxel-based (recommended), 1: tetrahedron-based")]
            [Range(0, 1)]
            public uint m_mode;

            [Range(0, 1)]
            public uint m_convexhullApproximation;

            [Range(0, 1)]
            public uint m_oclAcceleration;

            [Tooltip("maximum number of convex hulls to generate")]
            public uint m_maxConvexHulls;

            [Tooltip("Project output convex hull vertices onto the original source mesh for improved accuracy")]
            public bool m_projectHullVertices;
        }

        private unsafe struct ConvexHull
        {
            public double* m_points;
            public uint* m_triangles;
            public uint m_nPoints;
            public uint m_nTriangles;
            public double m_volume;
            public fixed double m_center[3];
        }

        [DllImport("libvhacd")] private static extern unsafe void* CreateVHACD();
        [DllImport("libvhacd")] private static extern unsafe void DestroyVHACD(void* pVHACD);

        [DllImport("libvhacd")]
        private static extern unsafe bool ComputeFloat(
            void* pVHACD,
            float* points,
            uint countPoints,
            uint* triangles,
            uint countTriangles,
            Parameters* parameters);

        [DllImport("libvhacd")] private static extern unsafe uint GetNConvexHulls(void* pVHACD);

        [DllImport("libvhacd")]
        private static extern unsafe void GetConvexHull(
            void* pVHACD,
            uint index,
            ConvexHull* ch);

        [Header("VHACD Parameters")]
        public Parameters m_parameters;

        [Header("Generated Output")]
        [Tooltip("Name of the child container that will hold generated hull colliders.")]
        [SerializeField] private string containerName = "VHACD_Hulls";

        [Tooltip("If true, deletes the old generated container before regenerating.")]
        [SerializeField] private bool clearPrevious = true;

        [Tooltip("If true, adds MeshCollider components to generated hull objects.")]
        [SerializeField] private bool addColliders = true;

        [Tooltip("If true, marks generated container as Static.")]
        [SerializeField] private bool markGeneratedStatic = false;

        public VhacdRuntime() => m_parameters.Init();

        /// <summary>
        /// Generate convex hull meshes from the MeshFilter on this GameObject (or a provided mesh).
        /// </summary>
        public unsafe List<Mesh> GenerateConvexMeshes(Mesh mesh = null)
        {
            if (mesh == null)
            {
                var mf = GetComponent<MeshFilter>();
                if (mf == null || mf.sharedMesh == null)
                    throw new InvalidOperationException("VHACD needs a MeshFilter with a valid sharedMesh.");

                mesh = mf.sharedMesh;
            }

            void* vhacd = null;
            try
            {
                vhacd = CreateVHACD();
                var parameters = m_parameters;

                var verts = mesh.vertices;
                var tris = mesh.triangles;

                fixed (Vector3* pVerts = verts)
                fixed (int* pTris = tris)
                {
                    ComputeFloat(
                        vhacd,
                        (float*)pVerts, (uint)verts.Length,
                        (uint*)pTris, (uint)tris.Length / 3,
                        &parameters);
                }

                var numHulls = (int)GetNConvexHulls(vhacd);
                var convexMeshes = new List<Mesh>(numHulls);

                for (var i = 0; i < numHulls; i++)
                {
                    ConvexHull hull;
                    GetConvexHull(vhacd, (uint)i, &hull);

                    var hullVerts = new Vector3[hull.m_nPoints];
                    fixed (Vector3* pHullVerts = hullVerts)
                    {
                        var pComponents = hull.m_points;
                        var pOut = pHullVerts;

                        for (var pointCount = hull.m_nPoints; pointCount != 0; --pointCount)
                        {
                            pOut->x = (float)pComponents[0];
                            pOut->y = (float)pComponents[1];
                            pOut->z = (float)pComponents[2];

                            pOut += 1;
                            pComponents += 3;
                        }
                    }

                    var indices = new int[hull.m_nTriangles * 3];
                    Marshal.Copy((IntPtr)hull.m_triangles, indices, 0, indices.Length);

                    var hullMesh = new Mesh
                    {
                        name = $"VHACD_Hull_{i}"
                    };

                    // Safety for large hull meshes.
                    if (hullVerts.Length > 65535)
                        hullMesh.indexFormat = IndexFormat.UInt32;

                    hullMesh.vertices = hullVerts;
                    hullMesh.triangles = indices;
                    hullMesh.RecalculateBounds();

                    convexMeshes.Add(hullMesh);
                }

                return convexMeshes;
            }
            finally
            {
                if (vhacd != null)
                    DestroyVHACD(vhacd);
            }
        }

        /// <summary>
        /// Generates convex hull meshes and creates child objects with convex MeshColliders (compound collider).
        /// </summary>
        public void GenerateAndApplyConvexColliders()
        {
            var meshes = GenerateConvexMeshes();

            // Container
            var existing = transform.Find(containerName);
            if (clearPrevious && existing != null)
            {
#if UNITY_EDITOR
                if (!Application.isPlaying)
                    DestroyImmediate(existing.gameObject);
                else
                    Destroy(existing.gameObject);
#else
                Destroy(existing.gameObject);
#endif
                existing = null;
            }

            var container = existing != null
                ? existing
                : new GameObject(containerName).transform;

            container.SetParent(transform, false);
            container.localPosition = Vector3.zero;
            container.localRotation = Quaternion.identity;
            container.localScale = Vector3.one;
            container.gameObject.isStatic = markGeneratedStatic;

            // Clear any old children if we're reusing the container.
            if (!clearPrevious && existing != null)
            {
                for (var i = container.childCount - 1; i >= 0; i--)
                {
#if UNITY_EDITOR
                    if (!Application.isPlaying)
                        DestroyImmediate(container.GetChild(i).gameObject);
                    else
                        Destroy(container.GetChild(i).gameObject);
#else
                    Destroy(container.GetChild(i).gameObject);
#endif
                }
            }

            // Create hull children
            for (var i = 0; i < meshes.Count; i++)
            {
                var child = new GameObject($"Hull_{i}");
                child.transform.SetParent(container, false);

                if (addColliders)
                {
                    var mc = child.AddComponent<MeshCollider>();
                    mc.sharedMesh = meshes[i];
                    mc.convex = true;
                }
            }
        }

        // Keeping the old context menu too (optional). You can remove this if you want.
        [ContextMenu("Generate & Apply Convex Colliders")]
        private void ContextGenerateAndApply() => GenerateAndApplyConvexColliders();
    }
}
