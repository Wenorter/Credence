// Assets/Editor/ChurchColliderGeneratorWindow.cs
//
// One MeshCollider per object on target layer.
// FIX: Adaptive simplification by WORLD size (renderer bounds diagonal):
// - Big meshes (buildings/walls/floors): much larger cell size + much lower triangle target
// - Small props (candles etc): smaller cell + higher triangle target
//
// Also: disconnected island splitting to preserve thin parts, but auto-disabled for huge meshes
// (big building meshes) to avoid massive editor time/memory.
//
// MeshCollider only (no BoxCollider fallback).

#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class ChurchColliderGeneratorWindow : EditorWindow
{
    private Vector2 _scroll;

    [Header("Targeting")]
    [SerializeField] private string targetLayerName = "Church";
    [SerializeField] private bool includeInactive = true;
    [SerializeField] private bool skipAlreadyMarkedObjects = true;
    [SerializeField] private bool requireMeshRenderer = true;

    [Header("Read/Write Auto-Fix")]
    [SerializeField] private bool autoEnableReadWriteOnModelAssets = true;
    [SerializeField] private bool logReadWriteChanges = true;

    [Header("Cleanup")]
    [SerializeField] private bool removeEmptyTransformOnlyLeafNodes = true;

    [Tooltip("Remove existing colliders from objects that do NOT have GeneratedColliderMarker.\n" +
             "Default is safe: only affects objects on the target layer.\n" +
             "Use with caution if you disable the target-layer-only toggle.")]
    [SerializeField] private bool removeCollidersIfNoMarker = true;

    [Tooltip("If true, only remove colliders (no marker) on the target layer. Recommended.")]
    [SerializeField] private bool removeCollidersNoMarker_TargetLayerOnly = true;

    [Header("Replace Behavior")]
    [SerializeField] private bool alwaysReplaceCollidersOnUnmarkedObjects = true;

    // -------------------------
    // Adaptive settings (STATIC)
    // -------------------------
    [Serializable]
    private struct StaticAdaptiveTier
    {
        [Tooltip("If world bounds diagonal >= this, tier applies. Sorted by minDiag ascending.")]
        public float minDiag;

        [Tooltip("Starting base cell size (meters/units). Larger = fewer polygons.")]
        public float baseCellSize;

        [Tooltip("Hard cap for cell size during passes.")]
        public float maxCellSize;

        [Tooltip("Stop when triangle count <= this.")]
        public int targetMaxTriangles;

        [Tooltip("Max simplification passes.")]
        public int maxPasses;

        [Tooltip("Multiply cell size by this each pass when too detailed / cooking fails.")]
        public float cellGrowFactor;

        [Tooltip("Min cells across each axis for anisotropic cells (keeps thin parts alive).")]
        public int minCellsAcrossAxis;

        [Tooltip("If planar handling enabled, min cells across the two large axes when planar.\n" +
                 "Smaller = coarser big flat walls; larger = more detail.")]
        public int planarMinCellsAcrossLargeAxes;
    }

    [Header("Adaptive STATIC tiers (by world size)")]
    [Tooltip("Tiers are chosen by world bounds diagonal (renderer.bounds).\n" +
             "Make big building meshes coarse and small props detailed.")]
    [SerializeField] private StaticAdaptiveTier[] staticTiers = new StaticAdaptiveTier[]
    {
        // Small props (candles, small furniture)
        new StaticAdaptiveTier
        {
            minDiag = 0f,
            baseCellSize = 0.05f,
            maxCellSize = 0.40f,
            targetMaxTriangles = 6000,
            maxPasses = 10,
            cellGrowFactor = 1.25f,
            minCellsAcrossAxis = 10,
            planarMinCellsAcrossLargeAxes = 20
        },
        // Medium props (tables, chairs)
        new StaticAdaptiveTier
        {
            minDiag = 2.5f,
            baseCellSize = 0.10f,
            maxCellSize = 0.80f,
            targetMaxTriangles = 3500,
            maxPasses = 10,
            cellGrowFactor = 1.28f,
            minCellsAcrossAxis = 8,
            planarMinCellsAcrossLargeAxes = 16
        },
        // Large props / sections
        new StaticAdaptiveTier
        {
            minDiag = 8f,
            baseCellSize = 0.25f,
            maxCellSize = 2.0f,
            targetMaxTriangles = 1800,
            maxPasses = 10,
            cellGrowFactor = 1.30f,
            minCellsAcrossAxis = 6,
            planarMinCellsAcrossLargeAxes = 12
        },
        // Buildings / huge architecture
        new StaticAdaptiveTier
        {
            minDiag = 20f,
            baseCellSize = 0.60f,
            maxCellSize = 6.0f,
            targetMaxTriangles = 800,
            maxPasses = 10,
            cellGrowFactor = 1.35f,
            minCellsAcrossAxis = 4,
            planarMinCellsAcrossLargeAxes = 8
        }
    };

    // -------------------------
    // Dynamic (Rigidbody)
    // -------------------------
    [Header("Dynamic (has Rigidbody) Convex MeshCollider (adaptive by size too)")]
    [SerializeField] private float dynamicSmallDiag = 3.0f;

    [Tooltip("Convex colliders must be simpler; small dynamic objects can still keep some detail.")]
    [SerializeField] private float dynamicSmallBaseCell = 0.10f;
    [SerializeField] private int dynamicSmallTargetTris = 800;

    [Tooltip("Large dynamic objects: very coarse convex.")]
    [SerializeField] private float dynamicLargeBaseCell = 0.30f;
    [SerializeField] private int dynamicLargeTargetTris = 300;

    [SerializeField] private int dynamicMaxPasses = 10;
    [SerializeField] private float dynamicGrowFactor = 1.35f;
    [SerializeField] private float dynamicMaxCell = 3.0f;

    // -------------------------
    // Planar + degenerates
    // -------------------------
    [Header("Planar Mesh Handling")]
    [SerializeField] private bool enablePlanarMeshHandling = true;

    [Tooltip("If minAxis/maxAxis below this, treat as planar.")]
    [Range(0.001f, 0.15f)]
    [SerializeField] private float planarRatioThreshold = 0.03f;

    [Header("Degenerate Triangle Cleanup (bounds-relative)")]
    [Tooltip("Relative epsilon used to remove ONLY truly-degenerate triangles.\n" +
             "If thin parts vanish, decrease (1e-10). If you see lots of sliver garbage, increase slightly (1e-8).")]
    [Range(1e-12f, 1e-6f)]
    [SerializeField] private float degenerateAreaEpsRelative = 1e-9f;

    [Header("Preserve thin disconnected parts (island splitting)")]
    [SerializeField] private bool splitDisconnectedIslands = true;

    [Tooltip("If the mesh is huge, island splitting is auto-disabled (performance).")]
    [SerializeField] private float autoDisableIslandsIfWorldDiagAbove = 18f;

    [Tooltip("If triangle count is above this, island splitting is auto-disabled (performance).")]
    [SerializeField] private int autoDisableIslandsIfTrisAbove = 200000;

    [Tooltip("Safety cap: if more islands than this, do not split.")]
    [Range(1, 256)]
    [SerializeField] private int maxIslandsToSplit = 64;

    [Tooltip("Use average-of-vertices per cell rather than snapping to cell centers. Better quality.")]
    [SerializeField] private bool useCellCentroidAveraging = true;

    [Header("Output")]
    [SerializeField] private string outputFolder = "Assets/GeneratedColliders";

    [Header("Logging")]
    [SerializeField] private bool verbosePerObjectLogging = false;

    [MenuItem("Tools/Church Colliders/Open Generator Window")]
    public static void OpenWindow()
    {
        var window = GetWindow<ChurchColliderGeneratorWindow>("Church Colliders");
        window.minSize = new Vector2(800, 740);
        window.Show();
    }

    [MenuItem("Tools/Church Colliders/Generate (Active Scene)")]
    public static void GenerateFromMenu()
    {
        var temp = CreateInstance<ChurchColliderGeneratorWindow>();
        temp.GenerateForActiveScene();
        DestroyImmediate(temp);
    }

    [MenuItem("Tools/Church Colliders/Remove Generated (Active Scene)")]
    public static void RemoveFromMenu()
    {
        var temp = CreateInstance<ChurchColliderGeneratorWindow>();
        temp.RemoveGeneratedForActiveScene();
        DestroyImmediate(temp);
    }

    private void OnGUI()
    {
        _scroll = EditorGUILayout.BeginScrollView(_scroll);

        EditorGUILayout.LabelField("Church Collider Generator", EditorStyles.boldLabel);
        EditorGUILayout.Space(6);

        EditorGUILayout.HelpBox(
            "One MeshCollider per object.\n\n" +
            "Adaptive by WORLD size:\n" +
            "- Big building meshes become coarse (few triangles)\n" +
            "- Small props keep more detail\n\n" +
            "Island splitting preserves thin parts, but auto-disables for huge meshes for performance.",
            MessageType.Info);

        EditorGUILayout.Space(10);

        EditorGUILayout.LabelField("Targeting", EditorStyles.boldLabel);
        targetLayerName = EditorGUILayout.TextField("Target Layer Name", targetLayerName);
        includeInactive = EditorGUILayout.ToggleLeft("Include inactive objects", includeInactive);
        requireMeshRenderer = EditorGUILayout.ToggleLeft("Only process objects with MeshRenderer", requireMeshRenderer);
        skipAlreadyMarkedObjects = EditorGUILayout.ToggleLeft("Skip objects already marked as success", skipAlreadyMarkedObjects);

        EditorGUILayout.Space(10);

        EditorGUILayout.LabelField("Read/Write Auto-Fix", EditorStyles.boldLabel);
        autoEnableReadWriteOnModelAssets = EditorGUILayout.ToggleLeft("Auto-enable Read/Write on model assets", autoEnableReadWriteOnModelAssets);
        logReadWriteChanges = EditorGUILayout.ToggleLeft("Log Read/Write changes", logReadWriteChanges);

        EditorGUILayout.Space(10);

        EditorGUILayout.LabelField("Cleanup", EditorStyles.boldLabel);
        removeEmptyTransformOnlyLeafNodes = EditorGUILayout.ToggleLeft("Remove empty Transform-only leaf nodes", removeEmptyTransformOnlyLeafNodes);
        removeCollidersIfNoMarker = EditorGUILayout.ToggleLeft("Remove colliders on objects WITHOUT marker", removeCollidersIfNoMarker);
        removeCollidersNoMarker_TargetLayerOnly = EditorGUILayout.ToggleLeft("Only remove colliders on target layer (recommended)", removeCollidersNoMarker_TargetLayerOnly);

        EditorGUILayout.Space(10);

        EditorGUILayout.LabelField("Replace Behavior", EditorStyles.boldLabel);
        alwaysReplaceCollidersOnUnmarkedObjects = EditorGUILayout.ToggleLeft("Always replace colliders on unmarked objects", alwaysReplaceCollidersOnUnmarkedObjects);

        EditorGUILayout.Space(10);

        EditorGUILayout.LabelField("Adaptive STATIC tiers", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(
            "Tiers are chosen by world bounds diagonal.\n" +
            "If buildings are still too dense, lower targetMaxTriangles for the large tiers, or increase baseCellSize.",
            MessageType.None);

        if (staticTiers == null || staticTiers.Length == 0)
        {
            EditorGUILayout.HelpBox("staticTiers is empty. Add at least 1 tier.", MessageType.Warning);
        }
        else
        {
            for (int i = 0; i < staticTiers.Length; i++)
            {
                EditorGUILayout.Space(6);
                EditorGUILayout.LabelField($"Tier {i}", EditorStyles.miniBoldLabel);
                staticTiers[i].minDiag = EditorGUILayout.FloatField("Min Diagonal", staticTiers[i].minDiag);
                staticTiers[i].baseCellSize = EditorGUILayout.FloatField("Base Cell Size", staticTiers[i].baseCellSize);
                staticTiers[i].maxCellSize = EditorGUILayout.FloatField("Max Cell Size", staticTiers[i].maxCellSize);
                staticTiers[i].targetMaxTriangles = EditorGUILayout.IntField("Target Max Tris", staticTiers[i].targetMaxTriangles);
                staticTiers[i].maxPasses = EditorGUILayout.IntField("Max Passes", staticTiers[i].maxPasses);
                staticTiers[i].cellGrowFactor = EditorGUILayout.FloatField("Grow Factor", staticTiers[i].cellGrowFactor);
                staticTiers[i].minCellsAcrossAxis = EditorGUILayout.IntSlider("Min Cells Across Axis", staticTiers[i].minCellsAcrossAxis, 2, 32);
                staticTiers[i].planarMinCellsAcrossLargeAxes = EditorGUILayout.IntSlider("Planar Min Cells (Large Axes)", staticTiers[i].planarMinCellsAcrossLargeAxes, 4, 40);
            }
        }

        EditorGUILayout.Space(10);

        EditorGUILayout.LabelField("Dynamic Convex (Rigidbody)", EditorStyles.boldLabel);
        dynamicSmallDiag = EditorGUILayout.FloatField("Small Diag Threshold", dynamicSmallDiag);
        dynamicSmallBaseCell = EditorGUILayout.FloatField("Small Base Cell", dynamicSmallBaseCell);
        dynamicSmallTargetTris = EditorGUILayout.IntField("Small Target Tris", dynamicSmallTargetTris);
        dynamicLargeBaseCell = EditorGUILayout.FloatField("Large Base Cell", dynamicLargeBaseCell);
        dynamicLargeTargetTris = EditorGUILayout.IntField("Large Target Tris", dynamicLargeTargetTris);
        dynamicMaxPasses = EditorGUILayout.IntField("Max Passes", dynamicMaxPasses);
        dynamicGrowFactor = EditorGUILayout.FloatField("Grow Factor", dynamicGrowFactor);
        dynamicMaxCell = EditorGUILayout.FloatField("Max Cell", dynamicMaxCell);

        EditorGUILayout.Space(10);

        EditorGUILayout.LabelField("Planar Handling", EditorStyles.boldLabel);
        enablePlanarMeshHandling = EditorGUILayout.ToggleLeft("Enable planar handling", enablePlanarMeshHandling);
        planarRatioThreshold = EditorGUILayout.Slider("Planar ratio threshold", planarRatioThreshold, 0.001f, 0.15f);

        EditorGUILayout.Space(10);

        EditorGUILayout.LabelField("Degenerate Triangle Cleanup", EditorStyles.boldLabel);
        degenerateAreaEpsRelative = EditorGUILayout.Slider("Degenerate area eps (relative)", degenerateAreaEpsRelative, 1e-12f, 1e-6f);

        EditorGUILayout.Space(10);

        EditorGUILayout.LabelField("Island splitting (thin parts)", EditorStyles.boldLabel);
        splitDisconnectedIslands = EditorGUILayout.ToggleLeft("Split disconnected islands", splitDisconnectedIslands);
        maxIslandsToSplit = EditorGUILayout.IntSlider("Max islands to split", maxIslandsToSplit, 1, 256);
        autoDisableIslandsIfWorldDiagAbove = EditorGUILayout.FloatField("Auto-disable if world diag >", autoDisableIslandsIfWorldDiagAbove);
        autoDisableIslandsIfTrisAbove = EditorGUILayout.IntField("Auto-disable if tris >", autoDisableIslandsIfTrisAbove);
        useCellCentroidAveraging = EditorGUILayout.ToggleLeft("Use centroid averaging", useCellCentroidAveraging);

        EditorGUILayout.Space(10);

        EditorGUILayout.LabelField("Output", EditorStyles.boldLabel);
        outputFolder = EditorGUILayout.TextField("Output folder", outputFolder);

        EditorGUILayout.Space(10);

        EditorGUILayout.LabelField("Logging", EditorStyles.boldLabel);
        verbosePerObjectLogging = EditorGUILayout.ToggleLeft("Verbose per-object logging", verbosePerObjectLogging);

        EditorGUILayout.Space(14);

        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Generate (Active Scene)", GUILayout.Height(36)))
                GenerateForActiveScene();

            if (GUILayout.Button("Remove Generated (Active Scene)", GUILayout.Height(36)))
                RemoveGeneratedForActiveScene();
        }

        EditorGUILayout.Space(14);
        EditorGUILayout.EndScrollView();
    }

    private void GenerateForActiveScene()
    {
        var targetLayer = LayerMask.NameToLayer(targetLayerName);
        if (targetLayer < 0)
        {
            Debug.LogError($"Layer '{targetLayerName}' not found. Create it in Project Settings -> Tags and Layers.");
            return;
        }

        var scene = SceneManager.GetActiveScene();
        if (!scene.IsValid() || !scene.isLoaded)
        {
            Debug.LogError("No valid active scene loaded.");
            return;
        }

        EnsureUnityFolderExists(outputFolder);

        var transforms = GetAllTransformsInScene(scene, includeInactive);

        if (removeEmptyTransformOnlyLeafNodes)
        {
            RemoveTransformOnlyLeafNodes(transforms);
            transforms = GetAllTransformsInScene(scene, includeInactive);
        }

        int removedBadArtistColliders = 0;
        if (removeCollidersIfNoMarker)
        {
            removedBadArtistColliders = RemoveCollidersOnObjectsWithoutMarker(transforms, targetLayer);
            transforms = GetAllTransformsInScene(scene, includeInactive);
        }

        if (autoEnableReadWriteOnModelAssets)
        {
            var changedCount = EnableReadWriteForCandidateMeshes(transforms, targetLayer, out var scannedNonReadable);
            if (logReadWriteChanges)
                Debug.Log($"Read/Write pre-pass: non-readable meshes found: {scannedNonReadable}, assets changed: {changedCount}");

            transforms = GetAllTransformsInScene(scene, includeInactive);
        }

        var candidates = 0;
        var skippedMarked = 0;
        var replacedColliders = 0;

        var addedStaticMesh = 0;
        var addedDynamicConvex = 0;

        foreach (var tr in transforms)
        {
            if (tr == null) continue;
            var go = tr.gameObject;

            if (go.layer != targetLayer) continue;
            if (requireMeshRenderer && go.GetComponent<MeshRenderer>() == null) continue;

            candidates++;

            var marker = go.GetComponent<GeneratedColliderMarker>();
            if (skipAlreadyMarkedObjects && marker != null && marker.generatedSuccessfully)
            {
                skippedMarked++;
                continue;
            }

            if (marker == null)
                marker = Undo.AddComponent<GeneratedColliderMarker>(go);

            ResetMarkerForNewAttempt(marker);

            if (alwaysReplaceCollidersOnUnmarkedObjects)
                replacedColliders += RemoveAllCollidersOnObject(go);

            var rb = go.GetComponent<Rigidbody>();
            var isDynamic = rb != null;

            var mf = go.GetComponent<MeshFilter>();
            var sourceMesh = mf != null ? mf.sharedMesh : null;

            if (sourceMesh == null)
            {
                Fail(marker, "No MeshFilter/sharedMesh found on this object.");
                LogPerObjectIfEnabled(go, marker, false);
                continue;
            }

            if (!sourceMesh.isReadable)
            {
                var assetPath = AssetDatabase.GetAssetPath(sourceMesh);
                Fail(marker, string.IsNullOrEmpty(assetPath)
                    ? "Mesh is not readable and has no importable asset path."
                    : $"Mesh is not readable even after Read/Write pass. Asset: {assetPath}");
                LogPerObjectIfEnabled(go, marker, false);
                continue;
            }

            var worldDiag = GetWorldBoundsDiagonal(go, sourceMesh);
            var trisSrc = sourceMesh.triangles != null ? sourceMesh.triangles.Length / 3 : 0;

            if (!isDynamic)
            {
                // Choose tier based on world size
                var tier = PickStaticTier(worldDiag);

                // Auto-disable island splitting for huge building meshes
                bool allowIslandsForThis = splitDisconnectedIslands
                                          && worldDiag <= autoDisableIslandsIfWorldDiagAbove
                                          && trisSrc <= autoDisableIslandsIfTrisAbove;

                var simplified = TryBuildCookableSimplifiedMesh(
                    sourceMesh,
                    worldDiag,
                    tier.baseCellSize,
                    tier.maxCellSize,
                    tier.maxPasses,
                    tier.cellGrowFactor,
                    tier.targetMaxTriangles,
                    tier.minCellsAcrossAxis,
                    tier.planarMinCellsAcrossLargeAxes,
                    convex: false,
                    allowIslands: allowIslandsForThis,
                    out var usedCell,
                    out var triCount,
                    out var failReason);

                if (simplified == null)
                {
                    Fail(marker, $"Static simplification/cooking failed: {failReason}");
                    LogPerObjectIfEnabled(go, marker, false);
                    continue;
                }

                var assetPath = SaveMeshAsAsset(simplified, outputFolder, go, usedCell, out var saveReason);
                if (string.IsNullOrEmpty(assetPath))
                {
                    Fail(marker, $"Failed to save generated mesh asset: {saveReason}");
                    LogPerObjectIfEnabled(go, marker, false);
                    continue;
                }

                var savedMesh = AssetDatabase.LoadAssetAtPath<Mesh>(assetPath);
                if (savedMesh == null)
                {
                    AssetDatabase.DeleteAsset(assetPath);
                    Fail(marker, "Generated mesh asset created but could not be loaded.");
                    LogPerObjectIfEnabled(go, marker, false);
                    continue;
                }

                var mc = Undo.AddComponent<MeshCollider>(go);
                mc.sharedMesh = null;
                mc.convex = false;
                mc.cookingOptions =
                    MeshColliderCookingOptions.CookForFasterSimulation |
                    MeshColliderCookingOptions.UseFastMidphase |
                    MeshColliderCookingOptions.WeldColocatedVertices;
                mc.sharedMesh = savedMesh;

                marker.generatedMeshAssetPath = assetPath;
                Succeed(marker, GeneratedColliderMarker.ColliderKind.StaticMeshCollider, "", usedCell, triCount);

                addedStaticMesh++;
                LogPerObjectIfEnabled(go, marker, true);
            }
            else
            {
                // Dynamic convex: adaptive based on size
                float baseCell = worldDiag <= dynamicSmallDiag ? dynamicSmallBaseCell : dynamicLargeBaseCell;
                int targetTris = worldDiag <= dynamicSmallDiag ? dynamicSmallTargetTris : dynamicLargeTargetTris;

                var simplified = TryBuildCookableSimplifiedMesh(
                    sourceMesh,
                    worldDiag,
                    baseCell,
                    dynamicMaxCell,
                    dynamicMaxPasses,
                    dynamicGrowFactor,
                    targetTris,
                    minCellsAcrossAxis: 4,
                    planarMinCellsAcrossLargeAxes: 8,
                    convex: true,
                    allowIslands: false, // convex + islands often expensive; keep simple and stable
                    out var usedCell,
                    out var triCount,
                    out var failReason);

                if (simplified == null)
                {
                    Fail(marker, $"Dynamic convex simplification/cooking failed: {failReason}");
                    LogPerObjectIfEnabled(go, marker, false);
                    continue;
                }

                var assetPath = SaveMeshAsAsset(simplified, outputFolder, go, usedCell, out var saveReason);
                if (string.IsNullOrEmpty(assetPath))
                {
                    Fail(marker, $"Failed to save convex generated mesh asset: {saveReason}");
                    LogPerObjectIfEnabled(go, marker, false);
                    continue;
                }

                var savedMesh = AssetDatabase.LoadAssetAtPath<Mesh>(assetPath);
                if (savedMesh == null)
                {
                    AssetDatabase.DeleteAsset(assetPath);
                    Fail(marker, "Convex mesh asset created but could not be loaded.");
                    LogPerObjectIfEnabled(go, marker, false);
                    continue;
                }

                var mc = Undo.AddComponent<MeshCollider>(go);
                mc.sharedMesh = null;
                mc.convex = true;
                mc.cookingOptions =
                    MeshColliderCookingOptions.CookForFasterSimulation |
                    MeshColliderCookingOptions.UseFastMidphase |
                    MeshColliderCookingOptions.WeldColocatedVertices;
                mc.sharedMesh = savedMesh;

                marker.generatedMeshAssetPath = assetPath;
                Succeed(marker, GeneratedColliderMarker.ColliderKind.DynamicConvexMeshCollider, "", usedCell, triCount);

                addedDynamicConvex++;
                LogPerObjectIfEnabled(go, marker, true);
            }
        }

        AssetDatabase.SaveAssets();
        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());

        Debug.Log(
            "Church collider generation finished.\n" +
            $"Bad artist colliders removed (no marker): {removedBadArtistColliders}\n" +
            $"Candidates: {candidates}\n" +
            $"Skipped already marked: {skippedMarked}\n" +
            $"Existing colliders removed/replaced (unmarked targets): {replacedColliders}\n" +
            $"Generated static MeshColliders: {addedStaticMesh}\n" +
            $"Generated dynamic convex MeshColliders: {addedDynamicConvex}\n" +
            $"Output folder: {outputFolder}"
        );
    }

    private void RemoveGeneratedForActiveScene()
    {
        var scene = SceneManager.GetActiveScene();
        if (!scene.IsValid() || !scene.isLoaded)
        {
            Debug.LogError("No valid active scene loaded.");
            return;
        }

        var transforms = GetAllTransformsInScene(scene, includeInactive);

        var removedColliders = 0;
        var removedMarkers = 0;
        var deletedAssets = 0;

        foreach (var tr in transforms)
        {
            if (tr == null) continue;

            var go = tr.gameObject;
            var marker = go.GetComponent<GeneratedColliderMarker>();
            if (marker == null)
                continue;

            removedColliders += RemoveAllCollidersOnObject(go);

            if (!string.IsNullOrEmpty(marker.generatedMeshAssetPath))
            {
                if (AssetDatabase.DeleteAsset(marker.generatedMeshAssetPath))
                    deletedAssets++;
            }

            Undo.DestroyObjectImmediate(marker);
            removedMarkers++;
        }

        AssetDatabase.SaveAssets();
        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());

        Debug.Log(
            "Removed generated colliders.\n" +
            $"Removed colliders: {removedColliders}\n" +
            $"Removed markers: {removedMarkers}\n" +
            $"Deleted mesh assets: {deletedAssets}"
        );
    }

    // ----------------------------
    // Adaptive tier selection
    // ----------------------------

    private StaticAdaptiveTier PickStaticTier(float worldDiag)
    {
        // Ensure tiers sorted by minDiag
        if (staticTiers == null || staticTiers.Length == 0)
        {
            // Fallback safe tier
            return new StaticAdaptiveTier
            {
                minDiag = 0f,
                baseCellSize = 0.15f,
                maxCellSize = 2f,
                targetMaxTriangles = 2500,
                maxPasses = 10,
                cellGrowFactor = 1.30f,
                minCellsAcrossAxis = 6,
                planarMinCellsAcrossLargeAxes = 12
            };
        }

        // pick last tier whose minDiag <= worldDiag
        StaticAdaptiveTier best = staticTiers[0];
        for (int i = 0; i < staticTiers.Length; i++)
        {
            if (worldDiag >= staticTiers[i].minDiag)
                best = staticTiers[i];
        }

        // sanitize values
        best.baseCellSize = Mathf.Max(0.0001f, best.baseCellSize);
        best.maxCellSize = Mathf.Max(best.baseCellSize, best.maxCellSize);
        best.targetMaxTriangles = Mathf.Max(50, best.targetMaxTriangles);
        best.maxPasses = Mathf.Max(1, best.maxPasses);
        best.cellGrowFactor = Mathf.Max(1.01f, best.cellGrowFactor);
        best.minCellsAcrossAxis = Mathf.Clamp(best.minCellsAcrossAxis, 2, 64);
        best.planarMinCellsAcrossLargeAxes = Mathf.Clamp(best.planarMinCellsAcrossLargeAxes, 4, 96);

        return best;
    }

    private float GetWorldBoundsDiagonal(GameObject go, Mesh mesh)
    {
        // Prefer renderer bounds (world-space), because transforms/scales matter.
        var r = go.GetComponent<Renderer>();
        if (r != null)
            return r.bounds.size.magnitude;

        // Fallback: approximate world size using mesh bounds + lossy scale.
        var s = mesh.bounds.size;
        var lossy = go.transform.lossyScale;
        var ws = new Vector3(Mathf.Abs(s.x * lossy.x), Mathf.Abs(s.y * lossy.y), Mathf.Abs(s.z * lossy.z));
        return ws.magnitude;
    }

    // ----------------------------
    // Mesh generation
    // ----------------------------

    private Mesh TryBuildCookableSimplifiedMesh(
        Mesh sourceMesh,
        float worldDiag,
        float startCellSize,
        float maxAllowedCellSize,
        int passes,
        float growFactor,
        int targetMaxTris,
        int minCellsAcrossAxis,
        int planarMinCellsAcrossLargeAxes,
        bool convex,
        bool allowIslands,
        out float usedCellSize,
        out int finalTriCount,
        out string failReason)
    {
        usedCellSize = startCellSize;
        finalTriCount = 0;
        failReason = "";

        if (sourceMesh == null)
        {
            failReason = "Source mesh is null.";
            return null;
        }

        passes = Mathf.Max(1, passes);
        growFactor = Mathf.Max(1.01f, growFactor);

        float cell = Mathf.Max(0.0001f, startCellSize);

        Mesh lastValid = null;
        float lastValidCell = cell;
        int lastValidTris = 0;

        for (int pass = 1; pass <= passes; pass++)
        {
            cell = Mathf.Min(cell, maxAllowedCellSize);

            Mesh simplified;

            if (allowIslands)
            {
                simplified = SimplifyMeshSmart_Islands(
                    sourceMesh,
                    baseCell: cell,
                    minCellsAcrossAxis: minCellsAcrossAxis,
                    planarMinCellsAcrossLargeAxes: planarMinCellsAcrossLargeAxes,
                    out var simpReason);
                if (simplified == null)
                {
                    failReason = $"Pass {pass}/{passes}: island simplify failed: {simpReason}";
                    cell = Mathf.Min(maxAllowedCellSize, cell * growFactor);
                    continue;
                }
            }
            else
            {
                var cellVec = ComputeCellVectorForBounds(
                    sourceMesh.bounds,
                    baseCell: cell,
                    minCellsAcrossAxis: minCellsAcrossAxis,
                    planarMinCellsAcrossLargeAxes: planarMinCellsAcrossLargeAxes);

                simplified = SimplifyMeshByVertexClustering(sourceMesh, cellVec, out var simpReason);
                if (simplified == null)
                {
                    failReason = $"Pass {pass}/{passes}: simplify failed: {simpReason}";
                    cell = Mathf.Min(maxAllowedCellSize, cell * growFactor);
                    continue;
                }
            }

            int tris = simplified.triangles.Length / 3;

            lastValid = simplified;
            lastValidCell = cell;
            lastValidTris = tris;

            if (tris > targetMaxTris)
            {
                failReason = $"Pass {pass}/{passes}: too detailed ({tris} > target {targetMaxTris}). Increasing cell.";
                cell = Mathf.Min(maxAllowedCellSize, cell * growFactor);
                continue;
            }

            if (!CanPhysXCookMesh(simplified, convex, out var cookReason))
            {
                failReason = $"Pass {pass}/{passes}: PhysX cooking failed: {cookReason}. Increasing cell.";
                cell = Mathf.Min(maxAllowedCellSize, cell * growFactor);
                continue;
            }

            usedCellSize = cell;
            finalTriCount = tris;
            failReason = "";
            return simplified;
        }

        // Final fallback: if last valid is cookable, return it even if above target
        if (lastValid != null && CanPhysXCookMesh(lastValid, convex, out _))
        {
            usedCellSize = lastValidCell;
            finalTriCount = lastValidTris;
            failReason = $"Cookable fallback above target ({finalTriCount} > {targetMaxTris}).";
            return lastValid;
        }

        failReason = $"After {passes} passes, no cookable simplified mesh produced.";
        return null;
    }

    private Vector3 ComputeCellVectorForBounds(
        Bounds bounds,
        float baseCell,
        int minCellsAcrossAxis,
        int planarMinCellsAcrossLargeAxes)
    {
        var b = bounds.size;

        float maxAxis = Mathf.Max(b.x, Mathf.Max(b.y, b.z));
        float minAxis = Mathf.Min(b.x, Mathf.Min(b.y, b.z));
        bool isPlanar = enablePlanarMeshHandling && maxAxis > 1e-6f && (minAxis / maxAxis) < planarRatioThreshold;

        int cellsX = minCellsAcrossAxis;
        int cellsY = minCellsAcrossAxis;
        int cellsZ = minCellsAcrossAxis;

        if (isPlanar)
        {
            // Boost only the TWO large axes to keep planar surfaces stable.
            if (b.x <= b.y && b.x <= b.z) { cellsY = planarMinCellsAcrossLargeAxes; cellsZ = planarMinCellsAcrossLargeAxes; }
            else if (b.y <= b.x && b.y <= b.z) { cellsX = planarMinCellsAcrossLargeAxes; cellsZ = planarMinCellsAcrossLargeAxes; }
            else { cellsX = planarMinCellsAcrossLargeAxes; cellsY = planarMinCellsAcrossLargeAxes; }
        }

        float ClampAxis(float axisSize, int cells)
        {
            if (axisSize <= 1e-6f) return baseCell;
            float byCells = axisSize / Mathf.Max(2, cells);
            // Key behavior:
            // - Never use a cell larger than baseCell (keep detail when needed)
            // - But enforce enough cells across axis (thin parts survive)
            return Mathf.Min(baseCell, Mathf.Max(0.000001f, byCells));
        }

        return new Vector3(
            ClampAxis(b.x, cellsX),
            ClampAxis(b.y, cellsY),
            ClampAxis(b.z, cellsZ)
        );
    }

    // ----------------------------
    // Island splitting (thin parts)
    // ----------------------------

    private sealed class UF
    {
        private readonly int[] parent;
        private readonly byte[] rank;

        public UF(int n)
        {
            parent = new int[n];
            rank = new byte[n];
            for (int i = 0; i < n; i++) parent[i] = i;
        }

        public int Find(int x)
        {
            while (parent[x] != x)
            {
                parent[x] = parent[parent[x]];
                x = parent[x];
            }
            return x;
        }

        public void Union(int a, int b)
        {
            int ra = Find(a), rb = Find(b);
            if (ra == rb) return;

            if (rank[ra] < rank[rb]) parent[ra] = rb;
            else if (rank[ra] > rank[rb]) parent[rb] = ra;
            else { parent[rb] = ra; rank[ra]++; }
        }
    }

    private Mesh SimplifyMeshSmart_Islands(
        Mesh sourceMesh,
        float baseCell,
        int minCellsAcrossAxis,
        int planarMinCellsAcrossLargeAxes,
        out string reason)
    {
        reason = "";

        var islands = ExtractTriangleIslands(sourceMesh, maxIslandsToSplit, out var islandReason);
        if (islands == null)
        {
            reason = islandReason;
            return null;
        }

        if (islands.Count <= 1)
        {
            var cellVec = ComputeCellVectorForBounds(sourceMesh.bounds, baseCell, minCellsAcrossAxis, planarMinCellsAcrossLargeAxes);
            return SimplifyMeshByVertexClustering(sourceMesh, cellVec, out reason);
        }

        var mergedVerts = new List<Vector3>(Mathf.Min(sourceMesh.vertexCount, 200000));
        var mergedTris = new List<int>(Mathf.Min(sourceMesh.triangles.Length, 400000));

        int vertexOffset = 0;

        for (int i = 0; i < islands.Count; i++)
        {
            var islandMesh = islands[i];
            if (islandMesh == null || islandMesh.vertexCount < 3 || islandMesh.triangles.Length < 3)
                continue;

            var cellVec = ComputeCellVectorForBounds(islandMesh.bounds, baseCell, minCellsAcrossAxis, planarMinCellsAcrossLargeAxes);

            var simp = SimplifyMeshByVertexClustering(islandMesh, cellVec, out var simpReason);
            if (simp == null)
            {
                // Fallback: keep original island rather than losing it
                simp = islandMesh;
            }

            var v = simp.vertices;
            var t = simp.triangles;

            mergedVerts.AddRange(v);
            for (int ti = 0; ti < t.Length; ti++)
                mergedTris.Add(t[ti] + vertexOffset);

            vertexOffset += v.Length;
        }

        if (mergedVerts.Count < 3 || mergedTris.Count < 3)
        {
            reason = "All islands collapsed during merge (no triangles).";
            return null;
        }

        var merged = new Mesh
        {
            name = sourceMesh.name + "_ColliderMergedIslands",
            indexFormat = mergedVerts.Count > 65535
                ? UnityEngine.Rendering.IndexFormat.UInt32
                : UnityEngine.Rendering.IndexFormat.UInt16
        };
        merged.SetVertices(mergedVerts);
        merged.SetTriangles(mergedTris, 0);
        merged.RecalculateBounds();
        merged.RecalculateNormals();

        if (!IsValidForMeshCollider(merged))
        {
            reason = "Merged island mesh failed validity checks.";
            return null;
        }

        return merged;
    }

    private List<Mesh> ExtractTriangleIslands(Mesh sourceMesh, int maxIslands, out string reason)
    {
        reason = "";

        var verts = sourceMesh.vertices;
        var tris = sourceMesh.triangles;

        if (verts == null || verts.Length < 3 || tris == null || tris.Length < 3)
        {
            reason = "Mesh has no valid geometry to split.";
            return null;
        }

        var uf = new UF(verts.Length);

        for (int i = 0; i < tris.Length; i += 3)
        {
            int a = tris[i];
            int b = tris[i + 1];
            int c = tris[i + 2];

            if ((uint)a >= (uint)verts.Length || (uint)b >= (uint)verts.Length || (uint)c >= (uint)verts.Length)
                continue;

            uf.Union(a, b);
            uf.Union(b, c);
            uf.Union(c, a);
        }

        var compToTriStarts = new Dictionary<int, List<int>>(64);

        for (int i = 0; i < tris.Length; i += 3)
        {
            int a = tris[i];
            if ((uint)a >= (uint)verts.Length) continue;

            int root = uf.Find(a);

            if (!compToTriStarts.TryGetValue(root, out var list))
            {
                list = new List<int>();
                compToTriStarts[root] = list;

                if (compToTriStarts.Count > maxIslands)
                {
                    // Too many islands; return a single clone (no split)
                    return new List<Mesh>(1) { CloneMesh(sourceMesh, sourceMesh.name + "_IslandSingle") };
                }
            }

            list.Add(i); // store tri start index
        }

        var islands = new List<Mesh>(compToTriStarts.Count);

        foreach (var kvp in compToTriStarts)
        {
            var triStarts = kvp.Value;
            if (triStarts.Count == 0) continue;

            var map = new Dictionary<int, int>(256);
            var newVerts = new List<Vector3>(256);
            var newTris = new List<int>(triStarts.Count * 3);

            for (int ti = 0; ti < triStarts.Count; ti++)
            {
                int t0 = triStarts[ti];

                int a = tris[t0];
                int b = tris[t0 + 1];
                int c = tris[t0 + 2];

                int na = RemapVertex(a, verts, map, newVerts);
                int nb = RemapVertex(b, verts, map, newVerts);
                int nc = RemapVertex(c, verts, map, newVerts);

                newTris.Add(na);
                newTris.Add(nb);
                newTris.Add(nc);
            }

            if (newVerts.Count < 3 || newTris.Count < 3)
                continue;

            var m = new Mesh
            {
                name = sourceMesh.name + "_Island",
                indexFormat = newVerts.Count > 65535
                    ? UnityEngine.Rendering.IndexFormat.UInt32
                    : UnityEngine.Rendering.IndexFormat.UInt16
            };
            m.SetVertices(newVerts);
            m.SetTriangles(newTris, 0);
            m.RecalculateBounds();
            m.RecalculateNormals();

            islands.Add(m);
        }

        if (islands.Count == 0)
        {
            reason = "No islands produced.";
            return null;
        }

        return islands;
    }

    private static int RemapVertex(int oldIndex, Vector3[] verts, Dictionary<int, int> map, List<Vector3> newVerts)
    {
        if (map.TryGetValue(oldIndex, out int ni))
            return ni;

        ni = newVerts.Count;
        map[oldIndex] = ni;

        if ((uint)oldIndex < (uint)verts.Length)
            newVerts.Add(verts[oldIndex]);
        else
            newVerts.Add(Vector3.zero);

        return ni;
    }

    private static Mesh CloneMesh(Mesh src, string name)
    {
        var m = new Mesh
        {
            name = name,
            indexFormat = src.vertexCount > 65535
                ? UnityEngine.Rendering.IndexFormat.UInt32
                : UnityEngine.Rendering.IndexFormat.UInt16
        };
        m.SetVertices(src.vertices);
        m.SetTriangles(src.triangles, 0);
        m.RecalculateBounds();
        m.RecalculateNormals();
        return m;
    }

    // ----------------------------
    // Vertex clustering simplifier
    // ----------------------------

    private Mesh SimplifyMeshByVertexClustering(Mesh sourceMesh, Vector3 cellSize, out string reason)
    {
        reason = "";

        if (cellSize.x <= 0f || cellSize.y <= 0f || cellSize.z <= 0f)
        {
            reason = "Cell size components must all be > 0.";
            return null;
        }

        var sourceVertices = sourceMesh.vertices;
        var sourceTriangles = sourceMesh.triangles;

        if (sourceVertices == null || sourceVertices.Length < 3)
        {
            reason = "Source mesh has too few vertices.";
            return null;
        }

        if (sourceTriangles == null || sourceTriangles.Length < 3)
        {
            reason = "Source mesh has no triangles.";
            return null;
        }

        // Bounds-relative epsilon for removing truly-degenerate triangles
        float diag = sourceMesh.bounds.size.magnitude;
        if (diag < 1e-6f) diag = 1e-6f;
        float epsLen = diag * Mathf.Max(1e-12f, degenerateAreaEpsRelative);
        float epsArea = epsLen * epsLen;
        float areaEpsSqr = epsArea * epsArea; // compare against cross.sqrMagnitude

        var inv = new Vector3(1f / cellSize.x, 1f / cellSize.y, 1f / cellSize.z);

        var cellToIndex = new Dictionary<Vector3Int, int>(sourceVertices.Length);
        var sum = new List<Vector3>(sourceVertices.Length);
        var count = new List<int>(sourceVertices.Length);
        var oldToNew = new int[sourceVertices.Length];

        for (int i = 0; i < sourceVertices.Length; i++)
        {
            var p = sourceVertices[i];

            var key = new Vector3Int(
                Mathf.FloorToInt(p.x * inv.x),
                Mathf.FloorToInt(p.y * inv.y),
                Mathf.FloorToInt(p.z * inv.z)
            );

            if (!cellToIndex.TryGetValue(key, out int newIndex))
            {
                newIndex = sum.Count;
                cellToIndex[key] = newIndex;
                sum.Add(Vector3.zero);
                count.Add(0);
            }

            sum[newIndex] += p;
            count[newIndex] += 1;
            oldToNew[i] = newIndex;
        }

        var newVertices = new List<Vector3>(sum.Count);

        if (useCellCentroidAveraging)
        {
            for (int i = 0; i < sum.Count; i++)
                newVertices.Add(count[i] > 0 ? (sum[i] / count[i]) : Vector3.zero);
        }
        else
        {
            newVertices.Capacity = sum.Count;
            for (int i = 0; i < sum.Count; i++)
                newVertices.Add(Vector3.zero);

            foreach (var kvp in cellToIndex)
            {
                var key = kvp.Key;
                int idx = kvp.Value;

                newVertices[idx] = new Vector3(
                    (key.x + 0.5f) * cellSize.x,
                    (key.y + 0.5f) * cellSize.y,
                    (key.z + 0.5f) * cellSize.z
                );
            }
        }

        var newTriangles = new List<int>(sourceTriangles.Length);

        for (int i = 0; i < sourceTriangles.Length; i += 3)
        {
            int a = oldToNew[sourceTriangles[i]];
            int b = oldToNew[sourceTriangles[i + 1]];
            int c = oldToNew[sourceTriangles[i + 2]];

            if (a == b || b == c || a == c)
                continue;

            var va = newVertices[a];
            var vb = newVertices[b];
            var vc = newVertices[c];

            var ab = vb - va;
            var ac = vc - va;
            var cross = Vector3.Cross(ab, ac);

            if (cross.sqrMagnitude <= areaEpsSqr)
                continue;

            newTriangles.Add(a);
            newTriangles.Add(b);
            newTriangles.Add(c);
        }

        if (newVertices.Count < 3)
        {
            reason = "Simplified mesh ended with <3 vertices.";
            return null;
        }

        if (newTriangles.Count < 3)
        {
            reason = "Simplified mesh has no valid triangles after degenerate removal.";
            return null;
        }

        var simplified = new Mesh
        {
            name = sourceMesh.name + "_ColliderSimplified",
            indexFormat = newVertices.Count > 65535
                ? UnityEngine.Rendering.IndexFormat.UInt32
                : UnityEngine.Rendering.IndexFormat.UInt16
        };

        simplified.SetVertices(newVertices);
        simplified.SetTriangles(newTriangles, 0);
        simplified.RecalculateBounds();
        simplified.RecalculateNormals();

        if (!IsValidForMeshCollider(simplified))
        {
            reason = "Simplified mesh failed validity checks.";
            return null;
        }

        return simplified;
    }

    // ----------------------------
    // PhysX + validity
    // ----------------------------

    private static bool CanPhysXCookMesh(Mesh mesh, bool convex, out string reason)
    {
        reason = "";
        try
        {
            Physics.BakeMesh(mesh.GetInstanceID(), convex);
            return true;
        }
        catch (Exception ex)
        {
            reason = ex.Message;
            return false;
        }
    }

    private static bool IsValidForMeshCollider(Mesh mesh)
    {
        if (mesh == null) return false;
        if (mesh.vertexCount < 3) return false;

        var tris = mesh.triangles;
        if (tris == null || tris.Length < 3) return false;

        var size = mesh.bounds.size;
        const float tiny = 0.00001f;
        if (size.x < tiny && size.y < tiny && size.z < tiny) return false;

        return true;
    }

    // ----------------------------
    // Scene traversal & cleanup
    // ----------------------------

    private static Transform[] GetAllTransformsInScene(Scene scene, bool includeInactiveObjects)
    {
        var roots = scene.GetRootGameObjects();
        var list = new List<Transform>(2048);
        foreach (var root in roots)
            list.AddRange(root.GetComponentsInChildren<Transform>(includeInactiveObjects));
        return list.ToArray();
    }

    private static int RemoveTransformOnlyLeafNodes(Transform[] allTransforms)
    {
        Array.Sort(allTransforms, (a, b) => GetDepth(b).CompareTo(GetDepth(a)));

        int removed = 0;

        foreach (var tr in allTransforms)
        {
            if (tr == null) continue;
            if (tr.childCount != 0) continue;
            if (tr.parent == null) continue;

            var go = tr.gameObject;
            var comps = go.GetComponents<Component>();
            if (comps.Length != 1) continue;

            Undo.DestroyObjectImmediate(go);
            removed++;
        }

        return removed;
    }

    private int RemoveCollidersOnObjectsWithoutMarker(Transform[] transforms, int targetLayer)
    {
        int removed = 0;

        foreach (var tr in transforms)
        {
            if (tr == null) continue;
            var go = tr.gameObject;

            if (removeCollidersNoMarker_TargetLayerOnly && go.layer != targetLayer)
                continue;

            if (go.GetComponent<GeneratedColliderMarker>() != null)
                continue;

            var cols = go.GetComponents<Collider>();
            for (int i = 0; i < cols.Length; i++)
            {
                Undo.DestroyObjectImmediate(cols[i]);
                removed++;
            }
        }

        return removed;
    }

    private static int RemoveAllCollidersOnObject(GameObject go)
    {
        int removed = 0;
        var cols = go.GetComponents<Collider>();
        for (int i = 0; i < cols.Length; i++)
        {
            Undo.DestroyObjectImmediate(cols[i]);
            removed++;
        }
        return removed;
    }

    private static int GetDepth(Transform tr)
    {
        int depth = 0;
        while (tr.parent != null) { depth++; tr = tr.parent; }
        return depth;
    }

    // ----------------------------
    // Read/Write prepass
    // ----------------------------

    private int EnableReadWriteForCandidateMeshes(Transform[] transforms, int targetLayer, out int nonReadableMeshesFound)
    {
        nonReadableMeshesFound = 0;
        var pathsToFix = new HashSet<string>();

        foreach (var tr in transforms)
        {
            if (tr == null) continue;
            var go = tr.gameObject;

            if (go.layer != targetLayer) continue;
            if (requireMeshRenderer && go.GetComponent<MeshRenderer>() == null) continue;

            var marker = go.GetComponent<GeneratedColliderMarker>();
            if (skipAlreadyMarkedObjects && marker != null && marker.generatedSuccessfully) continue;

            var mf = go.GetComponent<MeshFilter>();
            var mesh = mf != null ? mf.sharedMesh : null;
            if (mesh == null) continue;
            if (mesh.isReadable) continue;

            nonReadableMeshesFound++;

            var assetPath = AssetDatabase.GetAssetPath(mesh);
            if (!string.IsNullOrEmpty(assetPath))
                pathsToFix.Add(assetPath);
        }

        int changed = 0;

        foreach (var assetPath in pathsToFix)
        {
            var importer = AssetImporter.GetAtPath(assetPath) as ModelImporter;
            if (importer == null) continue;
            if (importer.isReadable) continue;

            importer.isReadable = true;
            importer.SaveAndReimport();
            changed++;

            if (logReadWriteChanges)
                Debug.Log($"Enabled Read/Write on: {assetPath}");
        }

        if (changed > 0)
            AssetDatabase.Refresh();

        return changed;
    }

    // ----------------------------
    // Marker helpers
    // ----------------------------

    private static void ResetMarkerForNewAttempt(GeneratedColliderMarker marker)
    {
        marker.generatedSuccessfully = false;
        marker.generatedColliderKind = GeneratedColliderMarker.ColliderKind.None;
        marker.lastFailureReason = "";
        marker.cellSizeUsed = 0f;
        marker.triangleCount = 0;
        marker.generatedMeshAssetPath = "";
        EditorUtility.SetDirty(marker);
    }

    private static void Fail(GeneratedColliderMarker marker, string reason)
    {
        marker.generatedSuccessfully = false;
        marker.generatedColliderKind = GeneratedColliderMarker.ColliderKind.None;
        marker.lastFailureReason = reason;
        EditorUtility.SetDirty(marker);
    }

    private static void Succeed(GeneratedColliderMarker marker, GeneratedColliderMarker.ColliderKind kind, string reasonIfAny, float cellSizeUsedValue, int triCountValue)
    {
        marker.generatedSuccessfully = true;
        marker.generatedColliderKind = kind;
        marker.lastFailureReason = reasonIfAny ?? "";
        marker.cellSizeUsed = cellSizeUsedValue;
        marker.triangleCount = triCountValue;
        EditorUtility.SetDirty(marker);
    }

    private void LogPerObjectIfEnabled(GameObject go, GeneratedColliderMarker marker, bool isSuccess)
    {
        if (!verbosePerObjectLogging) return;

        var path = GetHierarchyPath(go);

        if (isSuccess)
            Debug.Log($"[ColliderGen OK] {path} -> {marker.generatedColliderKind} (cell={marker.cellSizeUsed:0.###}, tris={marker.triangleCount})");
        else
            Debug.LogWarning($"[ColliderGen FAIL] {path} -> {marker.lastFailureReason}");
    }

    // ----------------------------
    // Asset saving
    // ----------------------------

    private static string SaveMeshAsAsset(Mesh mesh, string folder, GameObject owner, float usedCellSize, out string reason)
    {
        reason = "";

        if (mesh == null)
        {
            reason = "Mesh is null.";
            return "";
        }

        try
        {
            EnsureUnityFolderExists(folder);

            var safeName = SanitizeFileName(owner.name);
            var id = owner.GetInstanceID();
            var cellTag = usedCellSize.ToString("0.###").Replace('.', '_');

            var basePath = $"{folder}/{safeName}_{id}_cell{cellTag}_Collider.asset";
            var uniquePath = AssetDatabase.GenerateUniqueAssetPath(basePath);

            AssetDatabase.CreateAsset(mesh, uniquePath);
            AssetDatabase.ImportAsset(uniquePath);

            return uniquePath;
        }
        catch (Exception ex)
        {
            reason = ex.Message;
            return "";
        }
    }

    private static void EnsureUnityFolderExists(string folderPath)
    {
        if (AssetDatabase.IsValidFolder(folderPath))
            return;

        var parts = folderPath.Split('/');
        if (parts.Length == 0 || parts[0] != "Assets")
            throw new Exception("Output folder must be under 'Assets/...'. Example: Assets/GeneratedColliders");

        var current = "Assets";
        for (int i = 1; i < parts.Length; i++)
        {
            var next = current + "/" + parts[i];
            if (!AssetDatabase.IsValidFolder(next))
                AssetDatabase.CreateFolder(current, parts[i]);

            current = next;
        }
    }

    private static string SanitizeFileName(string name)
    {
        foreach (var c in Path.GetInvalidFileNameChars())
            name = name.Replace(c, '_');
        return name.Replace(' ', '_');
    }

    private static string GetHierarchyPath(GameObject go)
    {
        var tr = go.transform;
        var path = tr.name;
        while (tr.parent != null)
        {
            tr = tr.parent;
            path = tr.name + "/" + path;
        }
        return path;
    }
}
#endif
