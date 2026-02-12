// Assets/Editor/ChurchColliderGeneratorWindow.cs
//
// Church Collider Generator (Editor-only)
//
// STATIC: one non-convex MeshCollider per object (simplified mesh asset saved under Assets/).
// DYNAMIC (throwables): VHACD compound convex hull MeshColliders under a child container + Rigidbody defaults,
//                       sets Rigidbody mass from hull volume, and optionally sets Rigidbody.isKinematic = true.
//
// Update in this version:
// - Big rooms/buildings are forced MUCH simpler (roughly 2x+ less detailed) by:
//   1) More aggressive cell scaling for big meshes.
//   2) More aggressive triangle budget reduction for big meshes.
//   3) Lower max triangle cap.
//   4) Earlier disabling of expensive island splitting on large meshes.
// - "Remove Generated" now also removes Rigidbody on dynamic/throwable objects.
//
// Unity: 6000.3.0f1

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

    [Tooltip("If true, requires a MeshRenderer on the object to be considered.")]
    [SerializeField] private bool requireMeshRenderer = true;

    [Tooltip("If true, objects with a successful GeneratedColliderMarker are skipped.")]
    [SerializeField] private bool skipAlreadyMarkedObjects = true;

    [Header("Static/Dynamic Tag Filters")]
    [Tooltip("If not empty: only objects with this tag will be treated as STATIC candidates (still must be on target layer).")]
    [SerializeField] private string staticTag = "";

    [Tooltip("If not empty: only objects with this tag will be treated as DYNAMIC (VHACD/Rigidbody) candidates (still must be on target layer).")]
    [SerializeField] private string dynamicTag = "Throwable";

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
    // Output + logging
    // -------------------------
    [Header("Output")]
    [Tooltip("Folder to store generated static collider mesh assets (must be under Assets/).")]
    [SerializeField] private string outputFolder = "Assets/GeneratedColliders";

    [Header("Logging")]
    [Tooltip("If enabled, prints per-object success/failure logs.")]
    [SerializeField] private bool verbosePerObjectLogging;

    // -------------------------
    // Static baseline + scaling
    // -------------------------
    [Serializable]
    private struct StaticBaseline
    {
        [Tooltip("Starting base cell size (world units). Larger = fewer polygons.")]
        public float baseCellSize;

        [Tooltip("Hard cap for cell size during passes.")]
        public float maxCellSize;

        [Tooltip("Stop trying to refine when triangle count <= this.")]
        public int targetMaxTriangles;

        [Tooltip("Max simplification passes (each pass may increase cell size).")]
        public int maxPasses;

        [Tooltip("Multiply cell size by this each pass when too detailed / cooking fails.")]
        public float cellGrowFactor;

        [Tooltip("Min cells across each axis (helps keep thin parts alive).")]
        public int minCellsAcrossAxis;

        [Tooltip("Planar meshes: min cells across the two large axes (walls/floors). Higher = more detail.")]
        public int planarMinCellsAcrossLargeAxes;
    }

    [Header("Static Baseline (reference 1m cube)")]
    [SerializeField] private StaticBaseline staticBaseline = new StaticBaseline
    {
        baseCellSize = 0.10f,
        maxCellSize = 0.80f,
        targetMaxTriangles = 3000,
        maxPasses = 10,
        cellGrowFactor = 1.28f,
        minCellsAcrossAxis = 8,
        planarMinCellsAcrossLargeAxes = 14
    };

    [Header("Static Scaling")]
    [Tooltip("Reference diagonal for scaling. 1m cube diagonal is about 1.732.")]
    [SerializeField] private float staticReferenceDiagonal = 1.732051f;

    [Tooltip(
        "How strongly BIG objects get coarser.\n" +
        "Bigger value => bigger cell size for big meshes => fewer triangles.\n" +
        "Small meshes keep baseline quality (no upscaling for ratio < 1).")]
    [Range(0f, 2f)]
    [SerializeField] private float staticCellInverseScalePower = 0.6f; // was ~0.55, now much coarser for rooms

    [Tooltip("Clamp the size ratio used for scaling (prevents extreme tiny/huge objects from exploding settings).")]
    [SerializeField] private float staticScaleClampMin = 0.30f;

    [SerializeField] private float staticScaleClampMax = 12.0f; // allow large rooms to scale more

    [Header("Static Cell Clamp (world units)")]
    [SerializeField] private float staticBaseCellClampMin = 0.03f;
    [SerializeField] private float staticBaseCellClampMax = 4.00f; // was 2.50, now allows bigger/coarser

    [Header("Static Max Cell Clamp (world units)")]
    [SerializeField] private float staticMaxCellClampMin = 0.05f;
    [SerializeField] private float staticMaxCellClampMax = 12.0f; // was 8, now allows bigger/coarser

    [Header("Static Triangle Budget Scaling")]
    [Tooltip(
        "How strongly BIG objects get a smaller triangle budget.\n" +
        "Bigger value => fewer target triangles for big meshes => coarser results.\n" +
        "Small meshes keep baseline quality.")]
    [Range(0f, 2f)]
    [SerializeField] private float staticTargetTrisScalePower = 1.05f; // was ~0.55, now much fewer tris for rooms

    [SerializeField] private int staticTargetTrisMin = 350;   // slightly lower
    [SerializeField] private int staticTargetTrisMax = 4500;  // was 9000, cap down hard (big rooms stop being detailed)

    [Header("Static Min-Cell Scaling (optional)")]
    [Tooltip("If enabled, min cell counts scale with object size (can make walls/rooms much more detailed).")]
    [SerializeField] private bool scaleStaticMinCellsBySize = false;

    [Tooltip("How strongly min-cell counts grow with size (only if enabled).")]
    [Range(0f, 2f)]
    [SerializeField] private float staticMinCellsScalePower = 0.15f;

    [SerializeField] private int staticMinCellsClampMin = 3;
    [SerializeField] private int staticMinCellsClampMax = 24;

    [SerializeField] private int staticPlanarCellsClampMin = 6;
    [SerializeField] private int staticPlanarCellsClampMax = 22;

    // -------------------------
    // Planar + degenerates + islands
    // -------------------------
    [Header("Planar Mesh Handling")]
    [Tooltip("If enabled, very thin planar meshes get a different cell layout (better walls/floors).")]
    [SerializeField] private bool enablePlanarMeshHandling = true;

    [Tooltip(
        "If minAxis/maxAxis is below this, treat mesh as planar.\n" +
        "Lower => fewer meshes treated as planar.\n" +
        "For big rooms, too much planar detection can add detail everywhere.")]
    [Range(0.001f, 0.15f)]
    [SerializeField] private float planarRatioThreshold = 0.012f; // was ~0.015, slightly stricter planar detection

    [Header("Degenerate Triangle Cleanup (bounds-relative)")]
    [Tooltip("Relative epsilon used to remove only truly degenerate triangles.")]
    [Range(1e-12f, 1e-6f)]
    [SerializeField] private float degenerateAreaEpsRelative = 1e-9f;

    [Header("Preserve thin disconnected parts (island splitting)")]
    [Tooltip("If true, splits disconnected triangle islands before simplification (helps thin parts).")]
    [SerializeField] private bool splitDisconnectedIslands = true;

    [Tooltip("Auto-disables island splitting if world diagonal is above this (performance safety).")]
    [SerializeField] private float autoDisableIslandsIfWorldDiagAbove = 10f; // was ~14, disable islands sooner

    [Tooltip("Auto-disables island splitting if triangle count is above this (performance safety).")]
    [SerializeField] private int autoDisableIslandsIfTrisAbove = 80000; // was ~120k, disable islands sooner

    [Tooltip("Safety cap: if more islands than this, do not split.")]
    [Range(1, 256)]
    [SerializeField] private int maxIslandsToSplit = 64;

    [Tooltip("Use centroid-of-cell averaging rather than snapping to cell center (better quality).")]
    [SerializeField] private bool useCellCentroidAveraging = true;

    // -------------------------
    // Dynamic (VHACD Runtime) + scaling
    // -------------------------
    [Header("Dynamic (VHACD)")]
    [Tooltip("Type name of your runtime VHACD component. Example: 'VhacdRuntime'.")]
    [SerializeField] private string vhacdRuntimeTypeName = "VhacdRuntime";

    [Tooltip("Child container name to delete before regenerating dynamic hulls (if present).")]
    [SerializeField] private string vhacdDynamicContainerName = "VHACD_Hulls";

    [Header("Dynamic Rigidbody Defaults")]
    [Tooltip("Rigidbody mass for a 1m cube reference volume (1.0 m^3). Final mass scales by hull volume.")]
    [SerializeField] private float dynamicMassPerCubicMeter = 1.0f;

    [Tooltip("If true, Rigidbody.isKinematic will be set true after generation (no physics simulation).")]
    [SerializeField] private bool dynamicSetKinematicTrue = true;

    [Header("Dynamic VHACD Adaptive Tuning")]
    [Tooltip("If enabled, some VHACD parameters will scale with object size.")]
    [SerializeField] private bool enableAdaptiveVhacdParams = true;

    [Tooltip("Reference diagonal used for dynamic scaling. 1m cube diagonal is about 1.732.")]
    [SerializeField] private float dynamicReferenceDiagonal = 1.732051f;

    [Tooltip("Base VHACD resolution at reference size. Bigger = slower but more accurate decomposition.")]
    [SerializeField] private int vhacdResolutionBase = 100000;

    [Tooltip("Resolution scales down for big objects: res = base / (ratio^power). Higher power = more aggressive downscale.")]
    [Range(0f, 2f)]
    [SerializeField] private float vhacdResolutionInversePower = 0.9f;

    [SerializeField] private int vhacdResolutionMin = 8000;
    [SerializeField] private int vhacdResolutionMax = 160000;

    [Tooltip("Base max hulls at reference size. Bigger objects can allow more hulls, but don’t go crazy.")]
    [SerializeField] private int vhacdMaxConvexHullsBase = 20;

    [Tooltip("Max hulls scale up with size: hulls = base * (ratio^power).")]
    [Range(0f, 2f)]
    [SerializeField] private float vhacdMaxHullsScalePower = 0.35f;

    [SerializeField] private int vhacdMaxHullsMin = 8;
    [SerializeField] private int vhacdMaxHullsMax = 80;

    [Tooltip("Base max vertices per hull at reference size. Larger = more accurate hulls, slower.")]
    [SerializeField] private int vhacdMaxVertsPerHullBase = 64;

    [Tooltip("For big objects, reduce verts/hull a bit: verts = base / (ratio^power).")]
    [Range(0f, 2f)]
    [SerializeField] private float vhacdMaxVertsInversePower = 0.25f;

    [SerializeField] private int vhacdMaxVertsMin = 24;
    [SerializeField] private int vhacdMaxVertsMax = 96;

    [Tooltip("Base min volume per hull at reference size. Increasing this merges tiny hulls away.")]
    [SerializeField] private float vhacdMinVolumePerHullBase = 0.0001f;

    [Tooltip("Min volume per hull grows with size: minVol = base * (ratio^power).")]
    [Range(0f, 3f)]
    [SerializeField] private float vhacdMinVolumeScalePower = 1.2f;

    [SerializeField] private float vhacdMinVolumeMin = 1e-06f;
    [SerializeField] private float vhacdMinVolumeMax = 0.25f;

    private static Type _cachedVhacdRuntimeType;
    private static string _cachedVhacdRuntimeTypeName;

    // -------------------------
    // Menu
    // -------------------------
    [MenuItem(MenuRoot + "Open Generator Window")]
    public static void OpenWindow()
    {
        var window = GetWindow<ChurchColliderGeneratorWindow>(WindowTitle);
        window.minSize = new Vector2(820, 760);
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
            "STATIC: one non-convex MeshCollider per object (simplified mesh asset saved under Assets/).\n" +
            "DYNAMIC: VHACD convex hulls + Rigidbody defaults + mass from hull volume (and optionally kinematic).\n\n" +
            "This version is tuned to make BIG rooms/buildings MUCH simpler.",
            MessageType.Info);

        EditorGUILayout.Space(10);

        DrawTargetingSection();
        DrawReadWriteSection();
        DrawCleanupSection();
        DrawReplaceSection();

        DrawDynamicSection();
        DrawDynamicAdaptiveSection();

        DrawStaticBaselineSection();
        DrawStaticScalingSection();
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

    private static void SectionHeader(string title, string explanation)
    {
        EditorGUILayout.Space(6);
        EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(explanation, MessageType.None);
    }

    private void DrawTargetingSection()
    {
        SectionHeader(
            "Targeting",
            "Decides which scene objects get processed at all. Layer is the hard gate. Tags decide static vs dynamic.");

        targetLayerName = EditorGUILayout.TextField("Target Layer Name", targetLayerName);
        includeInactive = EditorGUILayout.ToggleLeft("Include inactive objects", includeInactive);
        requireMeshRenderer = EditorGUILayout.ToggleLeft("Only process objects with MeshRenderer", requireMeshRenderer);
        skipAlreadyMarkedObjects = EditorGUILayout.ToggleLeft("Skip objects already marked as success", skipAlreadyMarkedObjects);

        staticTag = EditorGUILayout.TextField("Static Tag Filter", staticTag);
        dynamicTag = EditorGUILayout.TextField("Dynamic Tag Filter", dynamicTag);

        EditorGUILayout.Space(10);
    }

    private void DrawReadWriteSection()
    {
        SectionHeader(
            "Read/Write Auto-Fix",
            "If a mesh isn’t readable, Unity can’t access vertices/triangles for simplification. This can auto-enable Read/Write on model importers.");

        autoEnableReadWriteOnModelAssets = EditorGUILayout.ToggleLeft("Auto-enable Read/Write on model assets", autoEnableReadWriteOnModelAssets);
        logReadWriteChanges = EditorGUILayout.ToggleLeft("Log Read/Write changes", logReadWriteChanges);
        EditorGUILayout.Space(10);
    }

    private void DrawCleanupSection()
    {
        SectionHeader(
            "Cleanup",
            "Optional pre-pass to remove junk transforms and wipe bad colliders before generating clean ones.");

        removeEmptyTransformOnlyLeafNodes = EditorGUILayout.ToggleLeft("Remove empty Transform-only leaf nodes", removeEmptyTransformOnlyLeafNodes);
        removeCollidersIfNoMarker = EditorGUILayout.ToggleLeft("Remove colliders on objects WITHOUT marker", removeCollidersIfNoMarker);
        removeCollidersNoMarker_TargetLayerOnly = EditorGUILayout.ToggleLeft("Only remove colliders on target layer (recommended)", removeCollidersNoMarker_TargetLayerOnly);
        EditorGUILayout.Space(10);
    }

    private void DrawReplaceSection()
    {
        SectionHeader(
            "Replace Behavior",
            "Controls whether we wipe existing colliders on objects we’re about to process.");

        alwaysReplaceCollidersOnUnmarkedObjects = EditorGUILayout.ToggleLeft("Always replace colliders on unmarked objects", alwaysReplaceCollidersOnUnmarkedObjects);
        EditorGUILayout.Space(10);
    }

    private void DrawDynamicSection()
    {
        SectionHeader(
            "Dynamic (VHACD)",
            "Settings for finding the VHACD runtime component and where generated hulls are stored.");

        vhacdRuntimeTypeName = EditorGUILayout.TextField("VHACD Runtime Type Name", vhacdRuntimeTypeName);
        vhacdDynamicContainerName = EditorGUILayout.TextField("Dynamic Hull Container Name", vhacdDynamicContainerName);

        EditorGUILayout.Space(6);
        EditorGUILayout.LabelField("Dynamic Rigidbody Defaults", EditorStyles.miniBoldLabel);
        dynamicMassPerCubicMeter = EditorGUILayout.FloatField("Mass per m^3 (reference)", dynamicMassPerCubicMeter);
        dynamicSetKinematicTrue = EditorGUILayout.ToggleLeft("Set Rigidbody Is Kinematic = true", dynamicSetKinematicTrue);

        EditorGUILayout.Space(10);
    }

    private void DrawDynamicAdaptiveSection()
    {
        SectionHeader(
            "Dynamic VHACD Adaptive Tuning",
            "Lets big objects use cheaper VHACD settings (fewer verts/hull, lower resolution), while small props can stay cleaner.");

        enableAdaptiveVhacdParams = EditorGUILayout.ToggleLeft("Enable adaptive VHACD params", enableAdaptiveVhacdParams);

        dynamicReferenceDiagonal = EditorGUILayout.FloatField("Reference Diagonal", dynamicReferenceDiagonal);

        vhacdResolutionBase = EditorGUILayout.IntField("Resolution Base", vhacdResolutionBase);
        vhacdResolutionInversePower = EditorGUILayout.Slider("Resolution Inverse Power", vhacdResolutionInversePower, 0f, 2f);
        vhacdResolutionMin = EditorGUILayout.IntField("Resolution Min", vhacdResolutionMin);
        vhacdResolutionMax = EditorGUILayout.IntField("Resolution Max", vhacdResolutionMax);

        EditorGUILayout.Space(4);
        vhacdMaxConvexHullsBase = EditorGUILayout.IntField("Max Convex Hulls Base", vhacdMaxConvexHullsBase);
        vhacdMaxHullsScalePower = EditorGUILayout.Slider("Max Hulls Scale Power", vhacdMaxHullsScalePower, 0f, 2f);
        vhacdMaxHullsMin = EditorGUILayout.IntField("Max Hulls Min", vhacdMaxHullsMin);
        vhacdMaxHullsMax = EditorGUILayout.IntField("Max Hulls Max", vhacdMaxHullsMax);

        EditorGUILayout.Space(4);
        vhacdMaxVertsPerHullBase = EditorGUILayout.IntField("Max Verts/Hull Base", vhacdMaxVertsPerHullBase);
        vhacdMaxVertsInversePower = EditorGUILayout.Slider("Max Verts Inverse Power", vhacdMaxVertsInversePower, 0f, 2f);
        vhacdMaxVertsMin = EditorGUILayout.IntField("Max Verts Min", vhacdMaxVertsMin);
        vhacdMaxVertsMax = EditorGUILayout.IntField("Max Verts Max", vhacdMaxVertsMax);

        EditorGUILayout.Space(4);
        vhacdMinVolumePerHullBase = EditorGUILayout.FloatField("Min Volume/Hull Base", vhacdMinVolumePerHullBase);
        vhacdMinVolumeScalePower = EditorGUILayout.Slider("Min Volume Scale Power", vhacdMinVolumeScalePower, 0f, 3f);
        vhacdMinVolumeMin = EditorGUILayout.FloatField("Min Volume Min", vhacdMinVolumeMin);
        vhacdMinVolumeMax = EditorGUILayout.FloatField("Min Volume Max", vhacdMinVolumeMax);

        EditorGUILayout.Space(10);
    }

    private void DrawStaticBaselineSection()
    {
        SectionHeader(
            "Static Baseline (reference 1m cube)",
            "These are the core simplification settings at the reference size. Small meshes keep these. Big rooms get scaled coarser.");

        staticBaseline.baseCellSize = EditorGUILayout.FloatField("Base Cell Size", staticBaseline.baseCellSize);
        staticBaseline.maxCellSize = EditorGUILayout.FloatField("Max Cell Size", staticBaseline.maxCellSize);
        staticBaseline.targetMaxTriangles = EditorGUILayout.IntField("Target Max Tris", staticBaseline.targetMaxTriangles);
        staticBaseline.maxPasses = EditorGUILayout.IntField("Max Passes", staticBaseline.maxPasses);
        staticBaseline.cellGrowFactor = EditorGUILayout.FloatField("Grow Factor", staticBaseline.cellGrowFactor);
        staticBaseline.minCellsAcrossAxis = EditorGUILayout.IntSlider("Min Cells Across Axis", staticBaseline.minCellsAcrossAxis, 2, 32);
        staticBaseline.planarMinCellsAcrossLargeAxes = EditorGUILayout.IntSlider("Planar Min Cells (Large Axes)", staticBaseline.planarMinCellsAcrossLargeAxes, 4, 40);

        EditorGUILayout.Space(10);
    }

    private void DrawStaticScalingSection()
    {
        SectionHeader(
            "Static Scaling",
            "Main quality knobs for rooms/buildings. Small meshes keep baseline quality. Big meshes get coarser automatically.");

        staticReferenceDiagonal = EditorGUILayout.FloatField("Reference Diagonal", staticReferenceDiagonal);

        staticCellInverseScalePower = EditorGUILayout.Slider("Big Mesh Cell Scale Power", staticCellInverseScalePower, 0f, 2f);

        EditorGUILayout.LabelField("Scale Clamp (min / max)", EditorStyles.miniBoldLabel);
        staticScaleClampMin = EditorGUILayout.FloatField("Scale Min", staticScaleClampMin);
        staticScaleClampMax = EditorGUILayout.FloatField("Scale Max", staticScaleClampMax);

        EditorGUILayout.Space(4);
        EditorGUILayout.LabelField("Base Cell Clamp (world units)", EditorStyles.miniBoldLabel);
        staticBaseCellClampMin = EditorGUILayout.FloatField("Base Cell Min", staticBaseCellClampMin);
        staticBaseCellClampMax = EditorGUILayout.FloatField("Base Cell Max", staticBaseCellClampMax);

        EditorGUILayout.Space(4);
        EditorGUILayout.LabelField("Max Cell Clamp (world units)", EditorStyles.miniBoldLabel);
        staticMaxCellClampMin = EditorGUILayout.FloatField("Max Cell Min", staticMaxCellClampMin);
        staticMaxCellClampMax = EditorGUILayout.FloatField("Max Cell Max", staticMaxCellClampMax);

        EditorGUILayout.Space(6);
        staticTargetTrisScalePower = EditorGUILayout.Slider("Big Mesh Tris Inverse Power", staticTargetTrisScalePower, 0f, 2f);
        staticTargetTrisMin = EditorGUILayout.IntField("Target Tris Min", staticTargetTrisMin);
        staticTargetTrisMax = EditorGUILayout.IntField("Target Tris Max", staticTargetTrisMax);

        EditorGUILayout.Space(6);
        scaleStaticMinCellsBySize = EditorGUILayout.ToggleLeft("Scale min cell counts by size", scaleStaticMinCellsBySize);
        using (new EditorGUI.DisabledScope(!scaleStaticMinCellsBySize))
        {
            staticMinCellsScalePower = EditorGUILayout.Slider("Min Cells Scale Power", staticMinCellsScalePower, 0f, 2f);
            staticMinCellsClampMin = EditorGUILayout.IntField("Min Cells Clamp Min", staticMinCellsClampMin);
            staticMinCellsClampMax = EditorGUILayout.IntField("Min Cells Clamp Max", staticMinCellsClampMax);
            staticPlanarCellsClampMin = EditorGUILayout.IntField("Planar Cells Clamp Min", staticPlanarCellsClampMin);
            staticPlanarCellsClampMax = EditorGUILayout.IntField("Planar Cells Clamp Max", staticPlanarCellsClampMax);
        }

        EditorGUILayout.Space(10);
    }

    private void DrawPlanarSection()
    {
        SectionHeader(
            "Planar Handling",
            "Walls/floors are usually very thin in one axis. Planar handling gives them a better cell layout without needing insane triangles.");

        enablePlanarMeshHandling = EditorGUILayout.ToggleLeft("Enable planar handling", enablePlanarMeshHandling);
        planarRatioThreshold = EditorGUILayout.Slider("Planar ratio threshold", planarRatioThreshold, 0.001f, 0.15f);
        EditorGUILayout.Space(10);
    }

    private void DrawDegenerateSection()
    {
        SectionHeader(
            "Degenerate Triangle Cleanup",
            "After clustering, some triangles collapse into slivers/zero-area. This filters the junk triangles out.");

        degenerateAreaEpsRelative = EditorGUILayout.Slider("Degenerate area eps (relative)", degenerateAreaEpsRelative, 1e-12f, 1e-6f);
        EditorGUILayout.Space(10);
    }

    private void DrawIslandSection()
    {
        SectionHeader(
            "Island Splitting (thin parts)",
            "Splits disconnected triangle islands before simplification. Helps thin props, but is expensive and adds detail on large meshes.");

        splitDisconnectedIslands = EditorGUILayout.ToggleLeft("Split disconnected islands", splitDisconnectedIslands);
        maxIslandsToSplit = EditorGUILayout.IntSlider("Max islands to split", maxIslandsToSplit, 1, 256);
        autoDisableIslandsIfWorldDiagAbove = EditorGUILayout.FloatField("Auto-disable if world diag >", autoDisableIslandsIfWorldDiagAbove);
        autoDisableIslandsIfTrisAbove = EditorGUILayout.IntField("Auto-disable if tris >", autoDisableIslandsIfTrisAbove);
        useCellCentroidAveraging = EditorGUILayout.ToggleLeft("Use centroid averaging", useCellCentroidAveraging);
        EditorGUILayout.Space(10);
    }

    private void DrawOutputSection()
    {
        SectionHeader(
            "Output",
            "Static collider meshes are saved as assets so they’re stable, reload fast, and don’t regenerate every time.");

        outputFolder = EditorGUILayout.TextField("Output folder", outputFolder);
        EditorGUILayout.Space(10);
    }

    private void DrawLoggingSection()
    {
        SectionHeader(
            "Logging",
            "Verbose logging is helpful when a specific mesh fails cooking or VHACD can’t run.");

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

        if (!IsTagFilterValid(staticTag))
            return;

        if (!IsTagFilterValid(dynamicTag))
            return;

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

            var isDynamic = MatchesTagFilter(go, dynamicTag);
            var isStatic = !isDynamic && MatchesTagFilter(go, staticTag);

            if (!isDynamic && !isStatic)
                continue;

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
                if (isDynamic)
                {
                    EnsureFreshRigidbodyDefaults(go);

                    if (TryGenerateDynamicConvexWithVhacdRuntime(go, out var dynFail))
                    {
                        if (TryComputeContainerColliderVolume(go, vhacdDynamicContainerName, out var volumeM3))
                        {
                            var mass = Mathf.Max(0.0001f, dynamicMassPerCubicMeter * volumeM3);
                            var rb = go.GetComponent<Rigidbody>();
                            if (rb != null)
                            {
                                Undo.RecordObject(rb, "Set Rigidbody Mass");
                                rb.mass = mass;

                                if (dynamicSetKinematicTrue)
                                    rb.isKinematic = true;
                            }
                        }
                        else
                        {
                            Debug.LogWarning($"{GetHierarchyPath(go)}: dynamic hulls generated, but volume could not be computed (mass left at default).");
                        }

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

                if (!TryGenerateStaticMeshCollider(go, marker, out var usedCell, out var triCount, out var staticFail))
                {
                    Fail(marker, staticFail);
                    LogPerObjectIfEnabled(go, marker, false);
                    continue;
                }

                Succeed(marker, GeneratedColliderMarker.ColliderKind.StaticMeshCollider, "", usedCell, triCount);
                addedStaticMesh++;
                LogPerObjectIfEnabled(go, marker, true);
            }
            catch (Exception ex)
            {
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
        var removedRigidbodies = 0;

        foreach (var tr in transforms)
        {
            if (tr == null) continue;

            var go = tr.gameObject;
            if (go.layer != targetLayer) continue;

            var marker = go.GetComponent<GeneratedColliderMarker>();
            if (marker == null) continue;

            // Remove VHACD hull container if present.
            var container = go.transform.Find(vhacdDynamicContainerName);
            if (container != null)
            {
                Undo.DestroyObjectImmediate(container.gameObject);
                removedDynamicContainers++;
            }

            // Remove colliders on the root (and any remaining on hulls if they exist).
            removedColliders += RemoveAllCollidersOnObject(go);

            // NEW: If this object is dynamic/throwable (tag), remove Rigidbody as well.
            if (MatchesTagFilter(go, dynamicTag))
            {
                var rb = go.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    Undo.DestroyObjectImmediate(rb);
                    removedRigidbodies++;
                }
            }

            // Delete generated static mesh asset if it exists.
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
            $"Removed dynamic VHACD containers: {removedDynamicContainers}\n" +
            $"Removed rigidbodies (dynamic tag): {removedRigidbodies}"
        );
    }

    // -------------------------
    // Tag filter helpers
    // -------------------------
    private static bool IsTagFilterValid(string tag)
    {
        if (string.IsNullOrWhiteSpace(tag))
            return true;

        try
        {
            GameObject.FindWithTag(tag);
            return true;
        }
        catch
        {
            Debug.LogError($"Tag '{tag}' does not exist. Create it in Project Settings -> Tags and Layers, or clear the filter.");
            return false;
        }
    }

    private static bool MatchesTagFilter(GameObject go, string tagFilter)
    {
        if (string.IsNullOrWhiteSpace(tagFilter))
            return true;

        return go.CompareTag(tagFilter);
    }

    // -------------------------
    // Static generation
    // -------------------------
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
        var sourceTris = sourceMesh.triangles != null ? sourceMesh.triangles.Length / 3 : 0;

        var (baseCell, maxCell, targetTris, maxPasses, growFactor, minCells, planarMinCells) =
            GetScaledStaticSettings(worldDiag);

        var allowIslandsForThis =
            splitDisconnectedIslands &&
            worldDiag <= autoDisableIslandsIfWorldDiagAbove &&
            sourceTris <= autoDisableIslandsIfTrisAbove;

        var simplified = TryBuildCookableSimplifiedMesh(
            sourceMesh,
            baseCell,
            maxCell,
            maxPasses,
            growFactor,
            targetTris,
            minCells,
            planarMinCells,
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

    private (float baseCell, float maxCell, int targetTris, int maxPasses, float growFactor, int minCells, int planarMinCells)
        GetScaledStaticSettings(float worldDiag)
    {
        var refDiag = Mathf.Max(0.0001f, staticReferenceDiagonal);

        var ratio = Mathf.Max(0.0001f, worldDiag / refDiag);
        ratio = Mathf.Clamp(ratio, Mathf.Max(0.01f, staticScaleClampMin), Mathf.Max(staticScaleClampMin, staticScaleClampMax));

        // Small meshes keep baseline quality.
        var ratioBig = Mathf.Max(1f, ratio);

        // BIG meshes: increase cell size aggressively (coarser).
        var cellScale = Mathf.Pow(ratioBig, staticCellInverseScalePower);
        var scaledBaseCell = staticBaseline.baseCellSize * cellScale;
        scaledBaseCell = Mathf.Clamp(scaledBaseCell, staticBaseCellClampMin, staticBaseCellClampMax);

        var scaledMaxCell = staticBaseline.maxCellSize * cellScale;
        scaledMaxCell = Mathf.Clamp(scaledMaxCell, staticMaxCellClampMin, staticMaxCellClampMax);
        scaledMaxCell = Mathf.Max(scaledBaseCell, scaledMaxCell);

        // BIG meshes: reduce triangle budget aggressively (coarser).
        var trisDiv = Mathf.Max(1f, Mathf.Pow(ratioBig, staticTargetTrisScalePower));
        var scaledTargetTrisF = staticBaseline.targetMaxTriangles / trisDiv;
        var scaledTargetTris = Mathf.Clamp(Mathf.RoundToInt(scaledTargetTrisF), staticTargetTrisMin, staticTargetTrisMax);

        // Keep min cell counts stable (scaling these up makes rooms more detailed).
        var minCells = Mathf.Clamp(staticBaseline.minCellsAcrossAxis, staticMinCellsClampMin, staticMinCellsClampMax);
        var planarMinCells = Mathf.Clamp(staticBaseline.planarMinCellsAcrossLargeAxes, staticPlanarCellsClampMin, staticPlanarCellsClampMax);

        if (scaleStaticMinCellsBySize)
        {
            var cellsScale = Mathf.Pow(ratioBig, staticMinCellsScalePower);
            minCells = Mathf.Clamp(Mathf.RoundToInt(minCells * cellsScale), staticMinCellsClampMin, staticMinCellsClampMax);
            planarMinCells = Mathf.Clamp(Mathf.RoundToInt(planarMinCells * cellsScale), staticPlanarCellsClampMin, staticPlanarCellsClampMax);
        }

        var maxPasses = Mathf.Max(1, staticBaseline.maxPasses);
        var growFactor = Mathf.Max(1.01f, staticBaseline.cellGrowFactor);

        return (scaledBaseCell, scaledMaxCell, scaledTargetTris, maxPasses, growFactor, minCells, planarMinCells);
    }

    private float GetWorldBoundsDiagonal(GameObject go, Mesh mesh)
    {
        var r = go.GetComponent<Renderer>();
        if (r != null)
            return r.bounds.size.magnitude;

        var s = mesh.bounds.size;
        var lossy = go.transform.lossyScale;
        var ws = new Vector3(Mathf.Abs(s.x * lossy.x), Mathf.Abs(s.y * lossy.y), Mathf.Abs(s.z * lossy.z));
        return ws.magnitude;
    }

    // -------------------------
    // Dynamic generation (VHACD)
    // -------------------------
    private bool TryGenerateDynamicConvexWithVhacdRuntime(GameObject go, out string failReason)
    {
        failReason = "";

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

            if (enableAdaptiveVhacdParams)
                ApplyAdaptiveVhacdParams(go, runtime);

            if (InvokeIfExists(runtime, "GenerateAndApplyConvexColliders"))
            {
                Undo.DestroyObjectImmediate(runtime);
                return true;
            }

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

    private void ApplyAdaptiveVhacdParams(GameObject go, Component runtime)
    {
        var worldDiag = GetWorldBoundsDiagonal(go, GetMeshForBounds(go));
        var refDiag = Mathf.Max(0.0001f, dynamicReferenceDiagonal);

        var ratio = Mathf.Max(0.0001f, worldDiag / refDiag);
        ratio = Mathf.Clamp(ratio, 0.25f, 8f);

        var res = Mathf.RoundToInt(vhacdResolutionBase / Mathf.Max(0.0001f, Mathf.Pow(ratio, vhacdResolutionInversePower)));
        res = Mathf.Clamp(res, vhacdResolutionMin, vhacdResolutionMax);

        var maxHulls = Mathf.RoundToInt(vhacdMaxConvexHullsBase * Mathf.Pow(ratio, vhacdMaxHullsScalePower));
        maxHulls = Mathf.Clamp(maxHulls, vhacdMaxHullsMin, vhacdMaxHullsMax);

        var maxVerts = Mathf.RoundToInt(vhacdMaxVertsPerHullBase / Mathf.Max(0.0001f, Mathf.Pow(ratio, vhacdMaxVertsInversePower)));
        maxVerts = Mathf.Clamp(maxVerts, vhacdMaxVertsMin, vhacdMaxVertsMax);

        var minVol = vhacdMinVolumePerHullBase * Mathf.Pow(ratio, vhacdMinVolumeScalePower);
        minVol = Mathf.Clamp(minVol, vhacdMinVolumeMin, vhacdMinVolumeMax);

        var setCount = 0;
        setCount += SetIntMemberIfExists(runtime, "Resolution", res) ? 1 : 0;
        setCount += SetIntMemberIfExists(runtime, "MaxConvexHulls", maxHulls) ? 1 : 0;
        setCount += SetIntMemberIfExists(runtime, "MaxNumVerticesPerCH", maxVerts) ? 1 : 0;
        setCount += SetFloatMemberIfExists(runtime, "MinVolumePerCH", minVol) ? 1 : 0;

        if (setCount == 0 && verbosePerObjectLogging)
        {
            Debug.LogWarning(
                $"{GetHierarchyPath(go)}: Adaptive VHACD params enabled, but runtime exposes none of the expected fields " +
                "(Resolution, MaxConvexHulls, MaxNumVerticesPerCH, MinVolumePerCH). If your runtime uses different names, add them here.");
        }
    }

    private static Mesh GetMeshForBounds(GameObject go)
    {
        var mf = go.GetComponent<MeshFilter>();
        if (mf != null && mf.sharedMesh != null)
            return mf.sharedMesh;

        return null;
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

    private void EnsureFreshRigidbodyDefaults(GameObject go)
    {
        var existing = go.GetComponent<Rigidbody>();
        if (existing != null)
            Undo.DestroyObjectImmediate(existing);

        var rb = Undo.AddComponent<Rigidbody>(go);

        rb.useGravity = true;
        rb.isKinematic = false;
        rb.interpolation = RigidbodyInterpolation.None;
        rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
        rb.constraints = RigidbodyConstraints.None;
        rb.mass = 1f;
    }

    private static bool TryComputeContainerColliderVolume(GameObject root, string containerName, out float volumeM3)
    {
        volumeM3 = 0f;

        var container = root.transform.Find(containerName);
        if (container == null)
            return false;

        var colliders = container.GetComponentsInChildren<MeshCollider>(true);
        if (colliders == null || colliders.Length == 0)
            return false;

        var total = 0.0;
        for (var i = 0; i < colliders.Length; i++)
        {
            var mc = colliders[i];
            if (mc == null || mc.sharedMesh == null)
                continue;

            total += ComputeMeshVolumeWorld(mc.sharedMesh, mc.transform);
        }

        volumeM3 = (float)Math.Max(0.0, total);
        return volumeM3 > 0f;
    }

    private static double ComputeMeshVolumeWorld(Mesh mesh, Transform tr)
    {
        var verts = mesh.vertices;
        var tris = mesh.triangles;

        if (verts == null || verts.Length < 3 || tris == null || tris.Length < 3)
            return 0.0;

        double volume = 0.0;

        for (var i = 0; i < tris.Length; i += 3)
        {
            var a = tr.TransformPoint(verts[tris[i]]);
            var b = tr.TransformPoint(verts[tris[i + 1]]);
            var c = tr.TransformPoint(verts[tris[i + 2]]);

            volume += SignedTetraVolume(a, b, c);
        }

        return Math.Abs(volume);
    }

    private static double SignedTetraVolume(Vector3 a, Vector3 b, Vector3 c)
    {
        return Vector3.Dot(a, Vector3.Cross(b, c)) / 6.0;
    }

    // -------------------------
    // VHACD runtime reflection helpers
    // -------------------------
    private Type GetVhacdRuntimeType()
    {
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

        var direct = Type.GetType(typeName, false);
        if (direct != null) return direct;

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
            result = mi.Invoke(target, null);
        else if (ps.Length == 1)
            result = mi.Invoke(target, new object[] { null });
        else
            return null;

        return result as List<Mesh>;
    }

    private static bool SetIntMemberIfExists(Component target, string name, int value)
    {
        var type = target.GetType();

        var field = type.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (field != null && field.FieldType == typeof(int))
        {
            field.SetValue(target, value);
            return true;
        }

        var prop = type.GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (prop != null && prop.PropertyType == typeof(int) && prop.CanWrite)
        {
            prop.SetValue(target, value);
            return true;
        }

        return false;
    }

    private static bool SetFloatMemberIfExists(Component target, string name, float value)
    {
        var type = target.GetType();

        var field = type.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (field != null && field.FieldType == typeof(float))
        {
            field.SetValue(target, value);
            return true;
        }

        var prop = type.GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (prop != null && prop.PropertyType == typeof(float) && prop.CanWrite)
        {
            prop.SetValue(target, value);
            return true;
        }

        return false;
    }

    // -------------------------
    // Mesh generation core (STATIC)
    // -------------------------
    private Mesh TryBuildCookableSimplifiedMesh(
        Mesh sourceMesh,
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
            if (b.x <= b.y && b.x <= b.z) { cellsY = planarMinCellsAcrossLargeAxes; cellsZ = planarMinCellsAcrossLargeAxes; }
            else if (b.y <= b.x && b.y <= b.z) { cellsX = planarMinCellsAcrossLargeAxes; cellsZ = planarMinCellsAcrossLargeAxes; }
            else { cellsX = planarMinCellsAcrossLargeAxes; cellsY = planarMinCellsAcrossLargeAxes; }
        }

        float ClampAxis(float axisSize, int cells)
        {
            if (axisSize <= 1e-6f) return baseCell;

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
        private readonly int[] _parent;
        private readonly byte[] _rank;

        public UF(int n)
        {
            _parent = new int[n];
            _rank = new byte[n];
            for (var i = 0; i < n; i++) _parent[i] = i;
        }

        public int Find(int x)
        {
            while (_parent[x] != x)
            {
                _parent[x] = _parent[_parent[x]];
                x = _parent[x];
            }
            return x;
        }

        public void Union(int a, int b)
        {
            var ra = Find(a);
            var rb = Find(b);
            if (ra == rb) return;

            if (_rank[ra] < _rank[rb]) _parent[ra] = rb;
            else if (_rank[ra] > _rank[rb]) _parent[rb] = ra;
            else { _parent[rb] = ra; _rank[ra]++; }
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
                continue;

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
