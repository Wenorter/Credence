using System.Collections.Generic;
using System.IO;
using NavLogic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;

namespace Editor
{
    public class NavFloorNavMeshWindow : EditorWindow
    {
        [SerializeField] private List<int> selectedTypes = new List<int> { 0 };
        [SerializeField] private bool useTypeSettings = true;
        [SerializeField] private float avoidWallRadius = 0.3f;
        [SerializeField] private float avoidObstacleRadius = 0.5f;
        [SerializeField] private bool inflateObstaclesAsBoxes = true;
        [SerializeField] private float agentHeight = 1.8f;
        [SerializeField] private float agentClimb = 0.3f;
        [SerializeField] private float agentSlope = 45f;
        [SerializeField] private bool overrideVoxel = true;
        [SerializeField] private float voxelSize = 0.1f;
        [SerializeField] private float minRegionArea = 0f;
        [SerializeField] private LayerMask obstacleLayers;

        private Vector2 _scroll;

        [MenuItem("Tools/NavLogic/NavMesh Baker")]
        private static void Open() => GetWindow<NavFloorNavMeshWindow>("Nav Floor NavMesh");

        private void OnGUI()
        {
            NavFloorShape[] shapes = FindObjectsByType<NavFloorShape>(FindObjectsSortMode.None);
            _scroll = EditorGUILayout.BeginScrollView(_scroll);

            EditorGUILayout.LabelField("NavMesh from built shapes", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Found " + shapes.Length + " shape(s). Shapes are the walkable " +
                                    "sources. Objects on the obstacle layers are added as Not Walkable. " +
                                    "Layers used by the floor shapes are ignored automatically.",
                MessageType.Info);

            EditorGUILayout.LabelField("Agent types to bake", EditorStyles.boldLabel);
            int count = NavMesh.GetSettingsCount();
            for (int i = 0; i < count; i++)
            {
                NavMeshBuildSettings s = NavMesh.GetSettingsByIndex(i);
                string n = NavMesh.GetSettingsNameFromID(s.agentTypeID);
                bool on = selectedTypes.Contains(s.agentTypeID);
                bool now = EditorGUILayout.ToggleLeft(n + "   (ID " + s.agentTypeID + ")", on);
                if (now && !on) selectedTypes.Add(s.agentTypeID);
                else if (!now && on) selectedTypes.Remove(s.agentTypeID);
            }

            useTypeSettings = EditorGUILayout.Toggle("Use agent type's own height/climb/slope", useTypeSettings);
            if (!useTypeSettings)
            {
                agentHeight = Mathf.Max(0.1f, EditorGUILayout.FloatField("Agent height", agentHeight));
                agentClimb = Mathf.Max(0f, EditorGUILayout.FloatField("Max step height", agentClimb));
                agentSlope = EditorGUILayout.Slider("Max slope", agentSlope, 0f, 60f);
            }

            EditorGUILayout.Space();
            avoidWallRadius = Mathf.Max(0f, EditorGUILayout.FloatField("Avoid wall radius", avoidWallRadius));
            avoidObstacleRadius = Mathf.Max(0f, EditorGUILayout.FloatField("Avoid obstacle radius", avoidObstacleRadius));
            inflateObstaclesAsBoxes = EditorGUILayout.Toggle("Inflate obstacles as boxes", inflateObstaclesAsBoxes);

            float baseR = Mathf.Min(avoidWallRadius, avoidObstacleRadius);
            EditorGUILayout.HelpBox("Unity radius used: " + baseR.ToString("0.00") +
                                    "  |  extra wall margin: " + (avoidWallRadius - baseR).ToString("0.00") +
                                    "  |  extra obstacle margin: " + (avoidObstacleRadius - baseR).ToString("0.00"),
                MessageType.None);

            overrideVoxel = EditorGUILayout.Toggle("Override voxel size", overrideVoxel);
            if (overrideVoxel) voxelSize = Mathf.Max(0.01f, EditorGUILayout.FloatField("Voxel size", voxelSize));
            minRegionArea = Mathf.Max(0f, EditorGUILayout.FloatField("Min region area", minRegionArea));

            SerializedObject so = new SerializedObject(this);
            so.Update();
            EditorGUILayout.PropertyField(so.FindProperty("obstacleLayers"), new GUIContent("Obstacle layers"));
            so.ApplyModifiedProperties();

            EditorGUILayout.Space();
            using (new EditorGUI.DisabledScope(shapes.Length == 0 || selectedTypes.Count == 0))
                if (GUILayout.Button("BAKE NAVMESH FROM SHAPES", GUILayout.Height(30))) Bake(shapes);

            EditorGUILayout.EndScrollView();
        }

        private static string GetFolder()
        {
            string scenePath = EditorSceneManager.GetActiveScene().path;
            if (string.IsNullOrEmpty(scenePath))
            {
                Debug.LogWarning("Scene is not saved yet, NavMesh assets go to Assets/");
                return "Assets";
            }

            string dir = scenePath.Substring(0, scenePath.LastIndexOf('/'));
            string name = Path.GetFileNameWithoutExtension(scenePath);
            string folder = dir + "/" + name;
            if (!AssetDatabase.IsValidFolder(folder)) AssetDatabase.CreateFolder(dir, name);
            return folder;
        }

        private static bool InsidePoly(List<Vector2> poly, Vector2 p)
        {
            bool inside = false;
            for (int i = 0, j = poly.Count - 1; i < poly.Count; j = i++)
            {
                Vector2 a = poly[i], b = poly[j];
                if ((a.y > p.y) != (b.y > p.y) &&
                    p.x < (b.x - a.x) * (p.y - a.y) / (b.y - a.y) + a.x)
                    inside = !inside;
            }
            return inside;
        }

        private static int AddWallStrips(NavFloorShape[] shapes, float extra, List<NavMeshBuildSource> sources)
        {
            List<List<Vector3>> world = new List<List<Vector3>>();
            List<List<Vector2>> flat = new List<List<Vector2>>();

            foreach (NavFloorShape s in shapes)
            {
                List<Vector3> w = new List<Vector3>();
                List<Vector2> f = new List<Vector2>();
                foreach (Vector3 p in s.points)
                {
                    Vector3 wp = s.transform.TransformPoint(p);
                    w.Add(wp);
                    f.Add(new Vector2(wp.x, wp.z));
                }
                world.Add(w);
                flat.Add(f);
            }

            int count = 0;
            for (int i = 0; i < world.Count; i++)
            {
                List<Vector3> pts = world[i];
                if (pts.Count < 3) continue;

                for (int k = 0; k < pts.Count; k++)
                {
                    Vector3 a = pts[k];
                    Vector3 b = pts[(k + 1) % pts.Count];
                    float len = new Vector2(b.x - a.x, b.z - a.z).magnitude;
                    if (len < 0.01f) continue;

                    int segs = Mathf.Max(1, Mathf.CeilToInt(len / 1f));
                    for (int sIdx = 0; sIdx < segs; sIdx++)
                    {
                        Vector3 p0 = Vector3.Lerp(a, b, sIdx / (float)segs);
                        Vector3 p1 = Vector3.Lerp(a, b, (sIdx + 1) / (float)segs);
                        Vector3 mid = (p0 + p1) * 0.5f;

                        Vector2 d = new Vector2(p1.x - p0.x, p1.z - p0.z);
                        float segLen = d.magnitude;
                        if (segLen < 0.001f) continue;
                        d /= segLen;

                        Vector2 perp = new Vector2(-d.y, d.x);
                        Vector2 m2 = new Vector2(mid.x, mid.z);
                        Vector2 n2;
                        if (InsidePoly(flat[i], m2 + perp * 0.03f)) n2 = perp;
                        else if (InsidePoly(flat[i], m2 - perp * 0.03f)) n2 = -perp;
                        else continue;

                        Vector2 outP = m2 - n2 * 0.05f;
                        bool shared = false;
                        for (int j = 0; j < flat.Count && !shared; j++)
                            if (j != i && flat[j].Count >= 3 && InsidePoly(flat[j], outP)) shared = true;
                        if (shared) continue;

                        Vector3 center = new Vector3(m2.x + n2.x * extra * 0.5f, mid.y + 0.3f,
                            m2.y + n2.y * extra * 0.5f);
                        Quaternion rot = Quaternion.LookRotation(new Vector3(d.x, 0f, d.y), Vector3.up);

                        sources.Add(new NavMeshBuildSource
                        {
                            shape = NavMeshBuildSourceShape.Box,
                            transform = Matrix4x4.TRS(center, rot, Vector3.one),
                            size = new Vector3(extra, 1f, segLen + 0.1f),
                            area = 1
                        });
                        count++;
                    }
                }
            }
            return count;
        }

        private static NavMeshBuildSource InflateObstacle(NavMeshBuildSource src, float extra, out bool inflated)
        {
            inflated = false;
            Mesh m = src.sourceObject as Mesh;
            if (src.shape != NavMeshBuildSourceShape.Mesh || !m) return src;

            Bounds lb = m.bounds;
            Matrix4x4 t = src.transform;
            float sx = Mathf.Max(0.0001f, t.GetColumn(0).magnitude);
            float sz = Mathf.Max(0.0001f, t.GetColumn(2).magnitude);

            NavMeshBuildSource box = new NavMeshBuildSource
            {
                shape = NavMeshBuildSourceShape.Box,
                transform = t * Matrix4x4.Translate(lb.center),
                size = new Vector3(lb.size.x + 2f * extra / sx, lb.size.y, lb.size.z + 2f * extra / sz),
                area = 1
            };
            inflated = true;
            return box;
        }

        private void Bake(NavFloorShape[] shapes)
        {
            float baseR = Mathf.Min(avoidWallRadius, avoidObstacleRadius);
            float wallExtra = avoidWallRadius - baseR;
            float obstacleExtra = avoidObstacleRadius - baseR;

            List<NavMeshBuildSource> sources = new List<NavMeshBuildSource>();
            Bounds bounds = new Bounds();
            bool first = true;
            int floorMask = 0;

            foreach (NavFloorShape s in shapes)
            {
                floorMask |= 1 << s.gameObject.layer;

                MeshFilter mf = s.GetComponent<MeshFilter>();
                if (!mf || !mf.sharedMesh) continue;

                sources.Add(new NavMeshBuildSource
                {
                    shape = NavMeshBuildSourceShape.Mesh,
                    sourceObject = mf.sharedMesh,
                    transform = mf.transform.localToWorldMatrix,
                    area = 0
                });

                Renderer r = s.GetComponent<Renderer>();
                Bounds b = r ? r.bounds : new Bounds(s.transform.position, Vector3.one);
                if (first) { bounds = b; first = false; }
                else bounds.Encapsulate(b);
            }

            if (sources.Count == 0)
            {
                Debug.LogError("No mesh sources found.");
                return;
            }

            bounds.Expand(new Vector3(2f, 4f, 2f));
            int floorCount = sources.Count;

            int wallStrips = 0;
            if (wallExtra > 0.001f)
                wallStrips = AddWallStrips(shapes, wallExtra, sources);

            int requested = obstacleLayers.value;
            int obstacleMask = requested & ~floorMask;
            if (requested != 0 && obstacleMask != requested)
                Debug.LogWarning("Obstacle layers overlap the floor shapes' layer(s). Those layers were ignored.");
            if (requested != 0 && obstacleMask == 0)
                Debug.LogWarning("Obstacle layers only contained floor layers, so no obstacles were added.");

            int obstacleCount = 0, obstacleBoxes = 0;
            if (obstacleMask != 0)
            {
                List<NavMeshBuildSource> tmp = new List<NavMeshBuildSource>();
                NavMeshBuilder.CollectSources(bounds, obstacleMask, NavMeshCollectGeometry.RenderMeshes,
                    1, new List<NavMeshBuildMarkup>(), tmp);

                foreach (NavMeshBuildSource src in tmp)
                {
                    if (src.component && src.component.GetComponentInParent<NavFloorShape>()) continue;

                    NavMeshBuildSource final = src;
                    if (inflateObstaclesAsBoxes && obstacleExtra > 0.001f)
                    {
                        final = InflateObstacle(src, obstacleExtra, out bool inflated);
                        if (inflated) obstacleBoxes++;
                    }
                    sources.Add(final);
                    obstacleCount++;
                }
            }

            Debug.Log("Unity radius " + baseR.ToString("0.00") + " | sources: " + floorCount + " floor, " +
                      wallStrips + " wall strips, " + obstacleCount + " obstacle (" + obstacleBoxes + " inflated).");

            string folder = GetFolder();
            List<NavMeshData> results = new List<NavMeshData>();

            foreach (int typeId in selectedTypes)
            {
                NavMeshBuildSettings settings = NavMesh.GetSettingsByID(typeId);
                settings.agentRadius = baseR;
                if (!useTypeSettings)
                {
                    settings.agentHeight = agentHeight;
                    settings.agentClimb = agentClimb;
                    settings.agentSlope = agentSlope;
                }
                settings.overrideVoxelSize = overrideVoxel;
                if (overrideVoxel) settings.voxelSize = voxelSize;
                settings.minRegionArea = minRegionArea;

                NavMeshData data = NavMeshBuilder.BuildNavMeshData(settings, sources, bounds,
                    Vector3.zero, Quaternion.identity);
                if (!data)
                {
                    Debug.LogError("BuildNavMeshData returned null for agent type " + typeId);
                    continue;
                }

                string typeName = NavMesh.GetSettingsNameFromID(typeId).Replace(" ", "_");
                string path = folder + "/NavFloorNavMesh_" + typeName + ".asset";
                if (AssetDatabase.LoadAssetAtPath<NavMeshData>(path)) AssetDatabase.DeleteAsset(path);
                AssetDatabase.CreateAsset(data, path);
                results.Add(data);
                Debug.Log("Baked agent type '" + typeName + "' to " + path);
            }

            AssetDatabase.SaveAssets();
            if (results.Count == 0) return;

            NavFloorNavMesh[] found = FindObjectsByType<NavFloorNavMesh>(FindObjectsSortMode.None);
            NavFloorNavMesh target;
            if (found.Length > 0) target = found[0];
            else
            {
                GameObject go = new GameObject("NavFloorNavMesh");
                Undo.RegisterCreatedObjectUndo(go, "Create NavFloorNavMesh");
                target = go.AddComponent<NavFloorNavMesh>();
            }

            Undo.RecordObject(target, "Assign NavMesh data");
            target.data = null;
            target.datas = results;
            target.Reload();
            EditorUtility.SetDirty(target);
            EditorSceneManager.MarkSceneDirty(target.gameObject.scene);

            NavMeshTriangulation tri = NavMesh.CalculateTriangulation();
            int triCount = tri.indices.Length / 3;
            if (triCount == 0)
                Debug.LogWarning("NavMesh is EMPTY (0 triangles). Check: floor faces up, obstacle not covering " +
                                 "the floor, wall radius not larger than the shape, voxel size not too large.");
            else
                Debug.Log("NavMesh registered for " + results.Count + " agent type(s), " + triCount + " triangles total.");
        }
    }
}