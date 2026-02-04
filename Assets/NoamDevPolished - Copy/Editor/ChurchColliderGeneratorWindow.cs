// Assets/Editor/ChurchColliderGeneratorWindow.cs
//
// Church Collider Generator (Editor-only)
//
// Behavior:
// - STATIC objects (no Rigidbody):
//   Generates ONE non-convex MeshCollider per object using a simplified mesh asset saved to disk.
//   Simplification is adaptive by world size (renderer bounds diagonal). Optional island-splitting for thin parts.
//
// - DYNAMIC objects (has Rigidbody):
//   Temporarily adds a VHACD runtime component, generates compound convex MeshColliders,
//   removes the runtime component, then marks the object as DynamicConvexMeshCollider.
//   (For now the dynamic marker debug fields are left empty/zero.)

#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public sealed class ChurchColliderGeneratorWindow : EditorWindow
{
    private const string MenuRoot = "Tools/Church Colliders/";
    private const string WindowTitle = "Church Colliders";

    private Vector2 _scroll;

    // -------------------------
    // Targeting
    // -------------------------
    [Header("Targeting")]
    [Tooltip("Only GameObjects on this layer will be processed.")]
    [SerializeField] private string targetLayerName = "Church";

    [Tooltip("If true, also processes inactive objects.")]
    [SerializeField] private bool includeInactive = true;

    [Tooltip("If true, objects with a successful GeneratedColliderMarker are skipped.")]
    [SerializeField] private bool skipAlreadyMarkedObjects = true;

    [Tooltip("If true, requires a MeshRenderer on the object to be considered.")]
    [SerializeField] private bool requireMeshRenderer = true;

    // -------------------------
    // Read/Write Auto-Fix
    // -------------------------
    [Header("Read/Write Auto-Fix")]
    [Tooltip("Attempts to enable Read/Write on model importers for meshes that need it.")]
    [SerializeField] private bool autoEnableReadWriteOnModelAssets = true;

    [Tooltip("Prints which assets were changed to Read/Write enabled.")]
    [SerializeField] private bool logReadWriteChanges = true;

    // -------------------------
    // Cleanup
    // -------------------------
    [Header("Cleanup")]
    [Tooltip("Deletes empty leaf nodes (Transform-only objects) on the target layer.")]
    [SerializeField] private bool removeEmptyTransformOnlyLeafNodes = true;

    [Tooltip(
        "Removes colliders from objects on the target layer that do NOT have GeneratedColliderMarker.\n" +
        "Useful for wiping bad artist colliders before generation.")]
    [SerializeField] private bool removeCollidersIfNoMarker = true;

    [Tooltip("If true, only removes colliders-without-marker on the target layer (recommended).")]
    [SerializeField] private bool removeCollidersNoMarker_TargetLayerOnly = true;

    // -------------------------
    // Replace behavior
    // -------------------------
    [Header("Replace Behavior")]
    [Tooltip("If true, removes ALL colliders on processed objects before generating new ones.")]
    [SerializeField] private bool alwaysReplaceCollidersOnUnmarkedObjects = true;

    // -------------------------
    // Adaptive settings (STATIC)
    // -------------------------
    [Serializable]
    private struct StaticAdaptiveTier
    {
        [Tooltip("If world bounds diagonal >= this, tier applies. Sorted by minDiag ascending.")]
        public float minDiag;

        [Tooltip("Starting base cell size (world units). Larger = fewer polygons.")]
        public float baseCellSize;

        [Tooltip("Hard cap for cell size during passes.")]
        public float maxCellSize;

        [Tooltip("Stop when triangle count <= this.")]
        public int targetMaxTriangles;

        [Tooltip("Max simplification passes.")]
        public int maxPasses;

        [Tooltip("Multiply cell size by this each pass when too detailed / cooking fails.")]
        public float cellGrowFactor;

        [Tooltip("Min cells across each axis (helps keep thin parts alive).")]
        public int minCellsAcrossAxis;

        [Tooltip(
            "If planar handling is enabled and the mesh is planar, this controls the min cell count " +
            "across the two large axes (walls/floors). Higher = more detail.")]
        public int planarMinCellsAcrossLargeAxes;
    }

    [Header("Adaptive STATIC tiers (by world size)")]
    [Tooltip(
        "Tiers are chosen by world bounds diagonal.\n" +
        "Make big building meshes coarse and small props more detailed.")]
    [SerializeField] private StaticAdaptiveTier[] staticTiers =
    {
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
    // Planar + degenerates
    // -------------------------
    [Header("Planar Mesh Handling")]
    [Tooltip("If enabled, very thin planar meshes get a different cell layout (better walls/floors).")]
    [SerializeField] private bool enablePlanarMeshHandling = true;

    [Tooltip("If minAxis/maxAxis is below this, treat mesh as planar.")]
    [Range(0.001f, 0.15f)]
    [SerializeField] private float planarRatioThreshold = 0.03f;

    [Header("Degenerate Triangle Cleanup (bounds-relative)")]
    [Tooltip(
        "Relative epsilon used to remove only truly degenerate triangles.\n" +
        "If thin parts vanish, decrease slightly. If you see lots of sliver garbage, increase slightly.")]
    [Range(1e-12f, 1e-6f)]
    [SerializeField] private float degenerateAreaEpsRelative = 1e-9f;

    [Header("Preserve thin disconnected parts (island splitting)")]
    [Tooltip("If true, splits disconnected triangle islands before simplification (helps thin parts).")]
    [SerializeField] private bool splitDisconnectedIslands = true;

    [Tooltip("Auto-disables island splitting if world diagonal is above this (performance safety).")]
    [SerializeField] private float autoDisableIslandsIfWorldDiagAbove = 18f;

    [Tooltip("Auto-disables island splitting if triangle count is above this (performance safety).")]
    [SerializeField] private int autoDisableIslandsIfTrisAbove = 200000;

    [Tooltip("Safety cap: if more islands than this, do not split.")]
    [Range(1, 256)]
    [SerializeField] private int maxIslandsToSplit = 64;

    [Tooltip("Use centroid-of-cell averaging rather than snapping to cell center (better quality).")]
    [SerializeField] private bool useCellCentroidAveraging = true;

    // -------------------------
    // Output + logging
    // -------------------------
    [Header("Output")]
    [Tooltip("Folder to store generated static collider mesh assets (must be under Assets/).")]
    [SerializeField] private string outputFolder = "Assets/GeneratedColliders";

    [Header("Logging")]
    [Tooltip("If enabled, prints per-object success/failure logs.")]
    [SerializeField] private bool verbosePerObjectLogging;

    // -------------------------
    // DYNAMIC (VHACD Runtime)
    // -------------------------
    [Header("Dynamic (Rigidbody) - VHACD Runtime")]
    [Tooltip("Type name of your runtime VHACD component. Example: 'VhacdRuntime'.")]
    [SerializeField] private string vhacdRuntimeTypeName = "VhacdRuntime";

    [Tooltip("Child container name to delete before regenerating dynamic hulls (if present).")]
    [SerializeField] private string vhacdDynamicContainerName = "VHACD_Hulls";

    private static Type _cachedVhacdRuntimeType;
    private static string _cachedVhacdRuntimeTypeName;

    // -------------------------
    // Menu
    // -------------------------
    [MenuItem(MenuRoot + "Open Generator Window")]
    public static void OpenWindow()
    {
        var window = GetWindow<ChurchColliderGeneratorWindow>(WindowTitle);
        window.minSize = new Vector2(800, 700);
        window.Show();
    }

    [MenuItem(MenuRoot + "Generate (Active Scene)")]
    public static void GenerateFromMenu()
    {
        var temp = CreateInstance<ChurchColliderGeneratorWindow>();
        temp.GenerateForActiveScene();
        DestroyImmediate(temp);
    }

    [MenuItem(MenuRoot + "Remove Generated (Active Scene)")]
    public static void RemoveFromMenu()
    {
        var temp = CreateInstance<ChurchColliderGeneratorWindow>();
        temp.RemoveGeneratedForActiveScene();
        DestroyImmediate(temp);
    }

    // -------------------------
    // UI
    // -------------------------
    private void OnGUI()
    {
        _scroll = EditorGUILayout.BeginScrollView(_scroll);

        EditorGUILayout.LabelField("Church Collider Generator (Static + Dynamic)", EditorStyles.boldLabel);
        EditorGUILayout.Space(6);

        EditorGUILayout.HelpBox(
            "STATIC (no Rigidbody): One NON-convex MeshCollider per object.\n" +
            "DYNAMIC (has Rigidbody): Runs VHACD at edit-time to generate compound convex colliders, then removes the VHACD runtime component.\n\n" +
            "Static generation is adaptive by WORLD size. Island splitting helps thin parts but auto-disables for huge meshes.",
            MessageType.Info);

        EditorGUILayout.Space(10);

        DrawTargetingSection();
        DrawReadWriteSection();
        DrawCleanupSection();
        DrawReplaceSection();
        DrawDynamicSection();
        DrawStaticAdaptiveSection();
        DrawPlanarSection();
        DrawDegenerateSection();
        DrawIslandSection();
        DrawOutputSection();
        DrawLoggingSection();

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

    private void DrawTargetingSection()
    {
        EditorGUILayout.LabelField("Targeting", EditorStyles.boldLabel);
        targetLayerName = EditorGUILayout.TextField("Target Layer Name", targetLayerName);
        includeInactive = EditorGUILayout.ToggleLeft("Include inactive objects", includeInactive);
        requireMeshRenderer = EditorGUILayout.ToggleLeft("Only process objects with MeshRenderer", requireMeshRenderer);
        skipAlreadyMarkedObjects = EditorGUILayout.ToggleLeft("Skip objects already marked as success", skipAlreadyMarkedObjects);
        EditorGUILayout.Space(10);
    }

    private void DrawReadWriteSection()
    {
        EditorGUILayout.LabelField("Read/Write Auto-Fix", EditorStyles.boldLabel);
        autoEnableReadWriteOnModelAssets = EditorGUILayout.ToggleLeft("Auto-enable Read/Write on model assets", autoEnableReadWriteOnModelAssets);
        logReadWriteChanges = EditorGUILayout.ToggleLeft("Log Read/Write changes", logReadWriteChanges);
        EditorGUILayout.Space(10);
    }

    private void DrawCleanupSection()
    {
        EditorGUILayout.LabelField("Cleanup", EditorStyles.boldLabel);
        removeEmptyTransformOnlyLeafNodes = EditorGUILayout.ToggleLeft("Remove empty Transform-only leaf nodes", removeEmptyTransformOnlyLeafNodes);
        removeCollidersIfNoMarker = EditorGUILayout.ToggleLeft("Remove colliders on objects WITHOUT marker", removeCollidersIfNoMarker);
        removeCollidersNoMarker_TargetLayerOnly = EditorGUILayout.ToggleLeft("Only remove colliders on target layer (recommended)", removeCollidersNoMarker_TargetLayerOnly);
        EditorGUILayout.Space(10);
    }

    private void DrawReplaceSection()
    {
        EditorGUILayout.LabelField("Replace Behavior", EditorStyles.boldLabel);
        alwaysReplaceCollidersOnUnmarkedObjects = EditorGUILayout.ToggleLeft("Always replace colliders on unmarked objects", alwaysReplaceCollidersOnUnmarkedObjects);
        EditorGUILayout.Space(10);
    }

    private void DrawDynamicSection()
    {
        EditorGUILayout.LabelField("Dynamic (Rigidbody) - VHACD Runtime", EditorStyles.boldLabel);
        vhacdRuntimeTypeName = EditorGUILayout.TextField("VHACD Runtime Type Name", vhacdRuntimeTypeName);
        vhacdDynamicContainerName = EditorGUILayout.TextField("Dynamic Hull Container Name", vhacdDynamicContainerName);
        EditorGUILayout.Space(10);
    }

    private void DrawStaticAdaptiveSection()
    {
        EditorGUILayout.LabelField("Adaptive STATIC tiers", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(
            "Tiers are chosen by world bounds diagonal.\n" +
            "If buildings are still too dense, lower targetMaxTriangles for large tiers, or increase baseCellSize.",
            MessageType.None);

        if (staticTiers == null || staticTiers.Length == 0)
        {
            EditorGUILayout.HelpBox("staticTiers is empty. Add at least 1 tier.", MessageType.Warning);
            EditorGUILayout.Space(10);
            return;
        }

        for (var i = 0; i < staticTiers.Length; i++)
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

        EditorGUILayout.Space(10);
    }

    private void DrawPlanarSection()
    {
        EditorGUILayout.LabelField("Planar Handling", EditorStyles.boldLabel);
        enablePlanarMeshHandling = EditorGUILayout.ToggleLeft("Enable planar handling", enablePlanarMeshHandling);
        planarRatioThreshold = EditorGUILayout.Slider("Planar ratio threshold", planarRatioThreshold, 0.001f, 0.15f);
        EditorGUILayout.Space(10);
    }

    private void DrawDegenerateSection()
    {
        EditorGUILayout.LabelField("Degenerate Triangle Cleanup", EditorStyles.boldLabel);
        degenerateAreaEpsRelative = EditorGUILayout.Slider("Degenerate area eps (relative)", degenerateAreaEpsRelative, 1e-12f, 1e-6f);
        EditorGUILayout.Space(10);
    }

    private void DrawIslandSection()
    {
        EditorGUILayout.LabelField("Island splitting (thin parts)", EditorStyles.boldLabel);
        splitDisconnectedIslands = EditorGUILayout.ToggleLeft("Split disconnected islands", splitDisconnectedIslands);
        maxIslandsToSplit = EditorGUILayout.IntSlider("Max islands to split", maxIslandsToSplit, 1, 256);
        autoDisableIslandsIfWorldDiagAbove = EditorGUILayout.FloatField("Auto-disable if world diag >", autoDisableIslandsIfWorldDiagAbove);
        autoDisableIslandsIfTrisAbove = EditorGUILayout.IntField("Auto-disable if tris >", autoDisableIslandsIfTrisAbove);
        useCellCentroidAveraging = EditorGUILayout.ToggleLeft("Use centroid averaging", useCellCentroidAveraging);
        EditorGUILayout.Space(10);
    }

    private void DrawOutputSection()
    {
        EditorGUILayout.LabelField("Output", EditorStyles.boldLabel);
        outputFolder = EditorGUILayout.TextField("Output folder", outputFolder);
        EditorGUILayout.Space(10);
    }

    private void DrawLoggingSection()
    {
        EditorGUILayout.LabelField("Logging", EditorStyles.boldLabel);
        verbosePerObjectLogging = EditorGUILayout.ToggleLeft("Verbose per-object logging", verbosePerObjectLogging);
        EditorGUILayout.Space(10);
    }

    // -------------------------
    // Generate / Remove
    // -------------------------
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

        try
        {
            EnsureUnityFolderExists(outputFolder);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Output folder is invalid: {ex.Message}");
            return;
        }

        var transforms = GetAllTransformsInScene(scene, includeInactive);

        if (removeEmptyTransformOnlyLeafNodes)
        {
            var removedEmpty = RemoveTransformOnlyLeafNodes(transforms, targetLayer);
            if (removedEmpty > 0 && verbosePerObjectLogging)
                Debug.Log($"Removed Transform-only leaf nodes: {removedEmpty}");

            transforms = GetAllTransformsInScene(scene, includeInactive);
        }

        var removedBadArtistColliders = 0;
        if (removeCollidersIfNoMarker)
        {
            removedBadArtistColliders = RemoveCollidersOnObjectsWithoutMarker(transforms, targetLayer);
            transforms = GetAllTransformsInScene(scene, includeInactive);
        }

        if (autoEnableReadWriteOnModelAssets)
        {
            var changedCount = EnableReadWriteForCandidateMeshes(transforms, targetLayer, out var nonReadableFound);
            if (logReadWriteChanges)
                Debug.Log($"Read/Write pre-pass: non-readable meshes found: {nonReadableFound}, assets changed: {changedCount}");

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

            try
            {
                // DYNAMIC (Rigidbody) -> VHACD runtime.
                if (go.GetComponent<Rigidbody>() != null)
                {
                    if (TryGenerateDynamicConvexWithVhacdRuntime(go, out var dynFail))
                    {
                        // Dynamic debug fields intentionally left empty/zero.
                        marker.generatedMeshAssetPath = "";
                        Succeed(marker, GeneratedColliderMarker.ColliderKind.DynamicConvexMeshCollider, "", 0f, 0);

                        addedDynamicConvex++;
                        LogPerObjectIfEnabled(go, marker, true);
                    }
                    else
                    {
                        Fail(marker, $"Dynamic VHACD failed: {dynFail}");
                        LogPerObjectIfEnabled(go, marker, false);
                    }

                    continue;
                }

                // STATIC (no Rigidbody) -> simplified mesh asset + single MeshCollider.
                if (!TryGenerateStaticMeshCollider(go, marker, out var staticUsedCell, out var staticTris, out var staticFail))
                {
                    Fail(marker, staticFail);
                    LogPerObjectIfEnabled(go, marker, false);
                    continue;
                }

                Succeed(marker, GeneratedColliderMarker.ColliderKind.StaticMeshCollider, "", staticUsedCell, staticTris);
                addedStaticMesh++;
                LogPerObjectIfEnabled(go, marker, true);
            }
            catch (Exception ex)
            {
                // Catching here keeps the batch running even if one mesh explodes.
                Fail(marker, $"Exception while generating colliders: {ex.Message}");
                Debug.LogError($"Collider generation exception on '{GetHierarchyPath(go)}':\n{ex}");
                LogPerObjectIfEnabled(go, marker, false);
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
            $"Generated dynamic convex (VHACD) colliders: {addedDynamicConvex}\n" +
            $"Output folder: {outputFolder}"
        );
    }

    private bool TryGenerateStaticMeshCollider(
        GameObject go,
        GeneratedColliderMarker marker,
        out float usedCell,
        out int triCount,
        out string failReason)
    {
        usedCell = 0f;
        triCount = 0;
        failReason = "";

        var mf = go.GetComponent<MeshFilter>();
        var sourceMesh = mf != null ? mf.sharedMesh : null;

        if (sourceMesh == null)
        {
            failReason = "No MeshFilter/sharedMesh found on this object.";
            return false;
        }

        if (!sourceMesh.isReadable)
        {
            var assetPath = AssetDatabase.GetAssetPath(sourceMesh);
            failReason = string.IsNullOrEmpty(assetPath)
                ? "Mesh is not readable and has no importable asset path."
                : $"Mesh is not readable even after Read/Write pass. Asset: {assetPath}";
            return false;
        }

        var worldDiag = GetWorldBoundsDiagonal(go, sourceMesh);
        var trisSrc = sourceMesh.triangles != null ? sourceMesh.triangles.Length / 3 : 0;

        var tier = PickStaticTier(worldDiag);

        // Island splitting keeps thin disconnected parts, but it can be expensive on huge meshes.
        var allowIslandsForThis =
            splitDisconnectedIslands &&
            worldDiag <= autoDisableIslandsIfWorldDiagAbove &&
            trisSrc <= autoDisableIslandsIfTrisAbove;

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
            out usedCell,
            out triCount,
            out var simpFailReason);

        if (simplified == null)
        {
            failReason = $"Static simplification/cooking failed: {simpFailReason}";
            return false;
        }

        var assetPathOut = SaveMeshAsAsset(simplified, outputFolder, go, usedCell, out var saveReason);
        if (string.IsNullOrEmpty(assetPathOut))
        {
            failReason = $"Failed to save generated mesh asset: {saveReason}";
            return false;
        }

        var savedMesh = AssetDatabase.LoadAssetAtPath<Mesh>(assetPathOut);
        if (savedMesh == null)
        {
            AssetDatabase.DeleteAsset(assetPathOut);
            failReason = "Generated mesh asset created but could not be loaded.";
            return false;
        }

        var mc = Undo.AddComponent<MeshCollider>(go);
        mc.sharedMesh = null;
        mc.convex = false;
        mc.cookingOptions =
            MeshColliderCookingOptions.CookForFasterSimulation |
            MeshColliderCookingOptions.UseFastMidphase |
            MeshColliderCookingOptions.WeldColocatedVertices;
        mc.sharedMesh = savedMesh;

        marker.generatedMeshAssetPath = assetPathOut;
        return true;
    }

    private void RemoveGeneratedForActiveScene()
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

        var transforms = GetAllTransformsInScene(scene, includeInactive);

        var removedColliders = 0;
        var removedMarkers = 0;
        var deletedAssets = 0;
        var removedDynamicContainers = 0;

        foreach (var tr in transforms)
        {
            if (tr == null) continue;

            var go = tr.gameObject;
            if (go.layer != targetLayer) continue;

            var marker = go.GetComponent<GeneratedColliderMarker>();
            if (marker == null) continue;

            // Dynamic path creates a hull container. Delete it so regen doesn't stack.
            var container = go.transform.Find(vhacdDynamicContainerName);
            if (container != null)
            {
                Undo.DestroyObjectImmediate(container.gameObject);
                removedDynamicContainers++;
            }

            removedColliders += RemoveAllCollidersOnObject(go);

            if (!string.IsNullOrEmpty(marker.generatedMeshAssetPath))
            {
                if (AssetDatabase.DeleteAsset(marker.generatedMeshAssetPath))
                    deletedAssets++;
                else
                    Debug.LogWarning($"Failed to delete generated mesh asset: {marker.generatedMeshAssetPath}");
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
            $"Deleted mesh assets: {deletedAssets}\n" +
            $"Removed dynamic VHACD containers: {removedDynamicContainers}"
        );
    }

    // -------------------------
    // Dynamic VHACD
    // -------------------------
    private bool TryGenerateDynamicConvexWithVhacdRuntime(GameObject go, out string failReason)
    {
        failReason = "";

        // Clean old hull container so we don't stack duplicates.
        var existingContainer = go.transform.Find(vhacdDynamicContainerName);
        if (existingContainer != null)
            Undo.DestroyObjectImmediate(existingContainer.gameObject);

        var runtimeType = GetVhacdRuntimeType();
        if (runtimeType == null)
        {
            failReason =
                $"Could not find type '{vhacdRuntimeTypeName}'. " +
                "Make sure the runtime script exists and the name matches (case-sensitive).";
            Debug.LogError($"{GetHierarchyPath(go)}: {failReason}");
            return false;
        }

        Component runtime = null;

        try
        {
            runtime = Undo.AddComponent(go, runtimeType);

            // Preferred: runtime handles generation + application internally.
            if (InvokeIfExists(runtime, "GenerateAndApplyConvexColliders"))
            {
                Undo.DestroyObjectImmediate(runtime);
                return true;
            }

            // Fallback: get convex meshes, then we apply compound MeshColliders here.
            var meshes = InvokeGenerateConvexMeshes(runtime);
            if (meshes == null || meshes.Count == 0)
            {
                failReason =
                    "No convex meshes produced. Missing GenerateAndApplyConvexColliders and GenerateConvexMeshes returned nothing.";
                Debug.LogError($"{GetHierarchyPath(go)}: {failReason}");
                Undo.DestroyObjectImmediate(runtime);
                return false;
            }

            ApplyConvexMeshesAsCompoundColliders(go, meshes, vhacdDynamicContainerName);

            Undo.DestroyObjectImmediate(runtime);
            return true;
        }
        catch (Exception ex)
        {
            failReason = ex.Message;
            Debug.LogError($"VHACD runtime exception on '{GetHierarchyPath(go)}':\n{ex}");

            if (runtime != null)
                Undo.DestroyObjectImmediate(runtime);

            return false;
        }
    }

    private static void ApplyConvexMeshesAsCompoundColliders(GameObject go, List<Mesh> meshes, string containerName)
    {
        var container = new GameObject(containerName);
        Undo.RegisterCreatedObjectUndo(container, "Create VHACD Hull Container");
        container.transform.SetParent(go.transform, false);

        for (var i = 0; i < meshes.Count; i++)
        {
            var hullMesh = meshes[i];
            if (hullMesh == null) continue;

            var child = new GameObject($"Hull_{i}");
            Undo.RegisterCreatedObjectUndo(child, "Create VHACD Hull");
            child.transform.SetParent(container.transform, false);

            var mc = Undo.AddComponent<MeshCollider>(child);
            mc.sharedMesh = hullMesh;
            mc.convex = true;
        }
    }

    private Type GetVhacdRuntimeType()
    {
        // Cache is keyed by the current type name so changing the field in the UI works instantly.
        if (_cachedVhacdRuntimeType != null && _cachedVhacdRuntimeTypeName == vhacdRuntimeTypeName)
            return _cachedVhacdRuntimeType;

        _cachedVhacdRuntimeTypeName = vhacdRuntimeTypeName;
        _cachedVhacdRuntimeType = FindTypeByName(vhacdRuntimeTypeName);
        return _cachedVhacdRuntimeType;
    }

    private static Type FindTypeByName(string typeName)
    {
        if (string.IsNullOrWhiteSpace(typeName))
            return null;

        // Works if typeName is assembly-qualified (rare, but useful).
        var direct = Type.GetType(typeName, false);
        if (direct != null) return direct;

        // Otherwise scan loaded assemblies.
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        for (var i = 0; i < assemblies.Length; i++)
        {
            Type[] types;

            try { types = assemblies[i].GetTypes(); }
            catch { continue; }

            for (var t = 0; t < types.Length; t++)
            {
                var candidate = types[t];
                if (candidate == null) continue;

                if (candidate.Name == typeName || candidate.FullName == typeName)
                    return candidate;
            }
        }

        return null;
    }

    private static bool InvokeIfExists(Component target, string methodName)
    {
        if (target == null) return false;

        var mi = target.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (mi == null) return false;

        if (mi.GetParameters().Length != 0)
            return false;

        mi.Invoke(target, null);
        return true;
    }

    private static List<Mesh> InvokeGenerateConvexMeshes(Component target)
    {
        if (target == null) return null;

        var mi = target.GetType().GetMethod("GenerateConvexMeshes", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (mi == null) return null;

        var ps = mi.GetParameters();
        object result;

        if (ps.Length == 0)
        {
            result = mi.Invoke(target, null);
        }
        else if (ps.Length == 1)
        {
            // Pass null so the runtime can pull MeshFilter.sharedMesh internally if it wants.
            result = mi.Invoke(target, new object[] { null });
        }
        else
        {
            return null;
        }

        return result as List<Mesh>;
    }

    // -------------------------
    // Adaptive tier selection
    // -------------------------
    private StaticAdaptiveTier PickStaticTier(float worldDiag)
    {
        if (staticTiers == null || staticTiers.Length == 0)
        {
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

        var best = staticTiers[0];
        for (var i = 0; i < staticTiers.Length; i++)
        {
            if (worldDiag >= staticTiers[i].minDiag)
                best = staticTiers[i];
        }

        // Safety clamps so weird values don't break the generator.
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
        var r = go.GetComponent<Renderer>();
        if (r != null)
            return r.bounds.size.magnitude;

        // Fallback if object has no renderer.
        var s = mesh.bounds.size;
        var lossy = go.transform.lossyScale;
        var ws = new Vector3(Mathf.Abs(s.x * lossy.x), Mathf.Abs(s.y * lossy.y), Mathf.Abs(s.z * lossy.z));
        return ws.magnitude;
    }

    // -------------------------
    // Mesh generation (STATIC)
    // -------------------------
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

        var cell = Mathf.Max(0.0001f, startCellSize);

        Mesh lastValid = null;
        var lastValidCell = cell;
        var lastValidTris = 0;

        for (var pass = 1; pass <= passes; pass++)
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

            var tris = simplified.triangles.Length / 3;

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

        // If we never hit the target, return the last cookable mesh we had.
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

        var maxAxis = Mathf.Max(b.x, Mathf.Max(b.y, b.z));
        var minAxis = Mathf.Min(b.x, Mathf.Min(b.y, b.z));
        var isPlanar = enablePlanarMeshHandling && maxAxis > 1e-6f && (minAxis / maxAxis) < planarRatioThreshold;

        var cellsX = minCellsAcrossAxis;
        var cellsY = minCellsAcrossAxis;
        var cellsZ = minCellsAcrossAxis;

        if (isPlanar)
        {
            // Thin axis keeps default, big axes get higher cell count for detail.
            if (b.x <= b.y && b.x <= b.z) { cellsY = planarMinCellsAcrossLargeAxes; cellsZ = planarMinCellsAcrossLargeAxes; }
            else if (b.y <= b.x && b.y <= b.z) { cellsX = planarMinCellsAcrossLargeAxes; cellsZ = planarMinCellsAcrossLargeAxes; }
            else { cellsX = planarMinCellsAcrossLargeAxes; cellsY = planarMinCellsAcrossLargeAxes; }
        }

        float ClampAxis(float axisSize, int cells)
        {
            if (axisSize <= 1e-6f) return baseCell;

            // Keep at least a few cells across the axis so thin features don't disappear.
            var byCells = axisSize / Mathf.Max(2, cells);
            return Mathf.Min(baseCell, Mathf.Max(0.000001f, byCells));
        }

        return new Vector3(
            ClampAxis(b.x, cellsX),
            ClampAxis(b.y, cellsY),
            ClampAxis(b.z, cellsZ)
        );
    }

    // -------------------------
    // Island splitting (thin parts)
    // -------------------------
    private sealed class UF
    {
        private readonly int[] parent;
        private readonly byte[] rank;

        public UF(int n)
        {
            parent = new int[n];
            rank = new byte[n];
            for (var i = 0; i < n; i++) parent[i] = i;
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
            var ra = Find(a);
            var rb = Find(b);
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

        var vertexOffset = 0;

        for (var i = 0; i < islands.Count; i++)
        {
            var islandMesh = islands[i];
            if (islandMesh == null || islandMesh.vertexCount < 3 || islandMesh.triangles.Length < 3)
                continue;

            var cellVec = ComputeCellVectorForBounds(islandMesh.bounds, baseCell, minCellsAcrossAxis, planarMinCellsAcrossLargeAxes);

            var simp = SimplifyMeshByVertexClustering(islandMesh, cellVec, out _);
            if (simp == null)
                simp = islandMesh;

            var v = simp.vertices;
            var t = simp.triangles;

            mergedVerts.AddRange(v);
            for (var ti = 0; ti < t.Length; ti++)
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
            indexFormat = mergedVerts.Count > 65535 ? IndexFormat.UInt32 : IndexFormat.UInt16
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

        // Union vertices that are connected by triangles.
        for (var i = 0; i < tris.Length; i += 3)
        {
            var a = tris[i];
            var b = tris[i + 1];
            var c = tris[i + 2];

            if ((uint)a >= (uint)verts.Length || (uint)b >= (uint)verts.Length || (uint)c >= (uint)verts.Length)
                continue;

            uf.Union(a, b);
            uf.Union(b, c);
            uf.Union(c, a);
        }

        var compToTriStarts = new Dictionary<int, List<int>>(64);

        for (var i = 0; i < tris.Length; i += 3)
        {
            var a = tris[i];
            if ((uint)a >= (uint)verts.Length) continue;

            var root = uf.Find(a);

            if (!compToTriStarts.TryGetValue(root, out var list))
            {
                list = new List<int>();
                compToTriStarts[root] = list;

                // Too many islands -> just treat as one mesh (avoid explosion).
                if (compToTriStarts.Count > maxIslands)
                    return new List<Mesh>(1) { CloneMesh(sourceMesh, sourceMesh.name + "_IslandSingle") };
            }

            list.Add(i);
        }

        var islands = new List<Mesh>(compToTriStarts.Count);

        foreach (var kvp in compToTriStarts)
        {
            var triStarts = kvp.Value;
            if (triStarts.Count == 0) continue;

            var map = new Dictionary<int, int>(256);
            var newVerts = new List<Vector3>(256);
            var newTris = new List<int>(triStarts.Count * 3);

            for (var ti = 0; ti < triStarts.Count; ti++)
            {
                var t0 = triStarts[ti];

                var a = tris[t0];
                var b = tris[t0 + 1];
                var c = tris[t0 + 2];

                var na = RemapVertex(a, verts, map, newVerts);
                var nb = RemapVertex(b, verts, map, newVerts);
                var nc = RemapVertex(c, verts, map, newVerts);

                newTris.Add(na);
                newTris.Add(nb);
                newTris.Add(nc);
            }

            if (newVerts.Count < 3 || newTris.Count < 3)
                continue;

            var m = new Mesh
            {
                name = sourceMesh.name + "_Island",
                indexFormat = newVerts.Count > 65535 ? IndexFormat.UInt32 : IndexFormat.UInt16
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
        if (map.TryGetValue(oldIndex, out var ni))
            return ni;

        ni = newVerts.Count;
        map[oldIndex] = ni;

        newVerts.Add((uint)oldIndex < (uint)verts.Length ? verts[oldIndex] : Vector3.zero);
        return ni;
    }

    private static Mesh CloneMesh(Mesh src, string name)
    {
        var m = new Mesh
        {
            name = name,
            indexFormat = src.vertexCount > 65535 ? IndexFormat.UInt32 : IndexFormat.UInt16
        };
        m.SetVertices(src.vertices);
        m.SetTriangles(src.triangles, 0);
        m.RecalculateBounds();
        m.RecalculateNormals();
        return m;
    }

    // -------------------------
    // Vertex clustering simplifier
    // -------------------------
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

        // Degenerate filtering scale based on mesh bounds.
        var diag = sourceMesh.bounds.size.magnitude;
        if (diag < 1e-6f) diag = 1e-6f;

        var epsLen = diag * Mathf.Max(1e-12f, degenerateAreaEpsRelative);
        var epsArea = epsLen * epsLen;
        var areaEpsSqr = epsArea * epsArea;

        var inv = new Vector3(1f / cellSize.x, 1f / cellSize.y, 1f / cellSize.z);

        var cellToIndex = new Dictionary<Vector3Int, int>(sourceVertices.Length);
        var sum = new List<Vector3>(sourceVertices.Length);
        var count = new List<int>(sourceVertices.Length);
        var oldToNew = new int[sourceVertices.Length];

        for (var i = 0; i < sourceVertices.Length; i++)
        {
            var p = sourceVertices[i];

            var key = new Vector3Int(
                Mathf.FloorToInt(p.x * inv.x),
                Mathf.FloorToInt(p.y * inv.y),
                Mathf.FloorToInt(p.z * inv.z)
            );

            if (!cellToIndex.TryGetValue(key, out var newIndex))
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
            for (var i = 0; i < sum.Count; i++)
                newVertices.Add(count[i] > 0 ? (sum[i] / count[i]) : Vector3.zero);
        }
        else
        {
            // Keep indices aligned with sum/count by filling zeros first.
            for (var i = 0; i < sum.Count; i++)
                newVertices.Add(Vector3.zero);

            foreach (var kvp in cellToIndex)
            {
                var key = kvp.Key;
                var idx = kvp.Value;

                newVertices[idx] = new Vector3(
                    (key.x + 0.5f) * cellSize.x,
                    (key.y + 0.5f) * cellSize.y,
                    (key.z + 0.5f) * cellSize.z
                );
            }
        }

        var newTriangles = new List<int>(sourceTriangles.Length);

        for (var i = 0; i < sourceTriangles.Length; i += 3)
        {
            var a = oldToNew[sourceTriangles[i]];
            var b = oldToNew[sourceTriangles[i + 1]];
            var c = oldToNew[sourceTriangles[i + 2]];

            // Collapse triangles that became degenerate due to clustering.
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
            indexFormat = newVertices.Count > 65535 ? IndexFormat.UInt32 : IndexFormat.UInt16
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

    // -------------------------
    // PhysX + validity
    // -------------------------
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

    // -------------------------
    // Scene traversal & cleanup
    // -------------------------
    private static Transform[] GetAllTransformsInScene(Scene scene, bool includeInactiveObjects)
    {
        var roots = scene.GetRootGameObjects();
        var list = new List<Transform>(2048);

        for (var i = 0; i < roots.Length; i++)
            list.AddRange(roots[i].GetComponentsInChildren<Transform>(includeInactiveObjects));

        return list.ToArray();
    }

    private static int RemoveTransformOnlyLeafNodes(Transform[] allTransforms, int targetLayer)
    {
        Array.Sort(allTransforms, (a, b) => GetDepth(b).CompareTo(GetDepth(a)));

        var removed = 0;

        for (var i = 0; i < allTransforms.Length; i++)
        {
            var tr = allTransforms[i];
            if (tr == null) continue;
            if (tr.childCount != 0) continue;
            if (tr.parent == null) continue;

            var go = tr.gameObject;
            if (go.layer != targetLayer) continue;

            // Only Transform means exactly one component: Transform.
            var comps = go.GetComponents<Component>();
            if (comps.Length != 1) continue;

            Undo.DestroyObjectImmediate(go);
            removed++;
        }

        return removed;
    }

    private int RemoveCollidersOnObjectsWithoutMarker(Transform[] transforms, int targetLayer)
    {
        var removed = 0;

        for (var i = 0; i < transforms.Length; i++)
        {
            var tr = transforms[i];
            if (tr == null) continue;

            var go = tr.gameObject;

            if (removeCollidersNoMarker_TargetLayerOnly && go.layer != targetLayer)
                continue;

            if (!removeCollidersNoMarker_TargetLayerOnly && go.layer != targetLayer)
                continue; // safety: original behavior was effectively target-layer only anyway

            if (go.GetComponent<GeneratedColliderMarker>() != null)
                continue;

            var cols = go.GetComponents<Collider>();
            for (var c = 0; c < cols.Length; c++)
            {
                Undo.DestroyObjectImmediate(cols[c]);
                removed++;
            }
        }

        return removed;
    }

    private static int RemoveAllCollidersOnObject(GameObject go)
    {
        var removed = 0;
        var cols = go.GetComponents<Collider>();

        for (var i = 0; i < cols.Length; i++)
        {
            Undo.DestroyObjectImmediate(cols[i]);
            removed++;
        }

        return removed;
    }

    private static int GetDepth(Transform tr)
    {
        var depth = 0;
        while (tr.parent != null)
        {
            depth++;
            tr = tr.parent;
        }
        return depth;
    }

    // -------------------------
    // Read/Write prepass
    // -------------------------
    private int EnableReadWriteForCandidateMeshes(Transform[] transforms, int targetLayer, out int nonReadableMeshesFound)
    {
        nonReadableMeshesFound = 0;
        var pathsToFix = new HashSet<string>();

        for (var i = 0; i < transforms.Length; i++)
        {
            var tr = transforms[i];
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

        var changed = 0;

        foreach (var assetPath in pathsToFix)
        {
            if (AssetImporter.GetAtPath(assetPath) is not ModelImporter importer)
                continue;

            if (importer.isReadable)
                continue;

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

    // -------------------------
    // Marker helpers
    // -------------------------
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
        marker.lastFailureReason = reason ?? "";
        EditorUtility.SetDirty(marker);
    }

    private static void Succeed(
        GeneratedColliderMarker marker,
        GeneratedColliderMarker.ColliderKind kind,
        string reasonIfAny,
        float cellSizeUsedValue,
        int triCountValue)
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

    // -------------------------
    // Asset saving (STATIC)
    // -------------------------
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
        for (var i = 1; i < parts.Length; i++)
        {
            var next = current + "/" + parts[i];
            if (!AssetDatabase.IsValidFolder(next))
                AssetDatabase.CreateFolder(current, parts[i]);

            current = next;
        }
    }

    private static string SanitizeFileName(string name)
    {
        var invalid = Path.GetInvalidFileNameChars();
        for (var i = 0; i < invalid.Length; i++)
            name = name.Replace(invalid[i], '_');

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
