using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class AngelZoneSetupWindow : EditorWindow
    {
        private const string ResetPointPropertyName = "resetPoint";

        private static readonly Color Purple =
            new Color(0.75f, 0.3f, 1f);

        private static readonly Color Green =
            new Color(0.3f, 0.9f, 0.4f);

        private static readonly Color ZoneColor =
            new Color(1f, 0.75f, 0.15f);

        private static readonly Color WarningColor =
            new Color(1f, 0.75f, 0.3f);

        private static readonly Color ErrorColor =
            new Color(1f, 0.5f, 0.5f);

        private string _pointsTag = "Untagged";
        private string _zonesTag = "Untagged";
        private float _previewRadius = 0.5f;
        private bool _showSettings = true;

        private readonly List<GameObject> _zones =
            new List<GameObject>();

        private readonly List<GameObject> _points =
            new List<GameObject>();

        private GameObject _selectedZone;
        private GameObject _previewPoint;

        private Vector2 _zonesScroll;
        private Vector2 _pointsScroll;

        private double _nextAssignmentRepaint;

        private string SettingsKey =>
            "AngelZoneSetup." + Application.dataPath + ".";

        [MenuItem("Tools/Angel Zone Setup")]
        public static void Open()
        {
            GetWindow<AngelZoneSetupWindow>("Angel Zone Setup");
        }

        private void OnEnable()
        {
            _pointsTag = EditorPrefs.GetString(
                SettingsKey + "PointsTag",
                "Untagged");

            _zonesTag = EditorPrefs.GetString(
                SettingsKey + "ZonesTag",
                "Untagged");

            _previewRadius = Mathf.Max(
                0.01f,
                EditorPrefs.GetFloat(
                    SettingsKey + "PreviewRadius",
                    0.5f));

            minSize = new Vector2(550f, 400f);

            SceneView.duringSceneGui += DrawScene;
            Undo.undoRedoPerformed += RepaintAssignmentViews;
            EditorApplication.hierarchyChanged += RepaintAssignmentViews;
            EditorApplication.update += PollAssignments;

            RefreshObjects();
        }

        private void OnDisable()
        {
            SceneView.duringSceneGui -= DrawScene;
            Undo.undoRedoPerformed -= RepaintAssignmentViews;
            EditorApplication.hierarchyChanged -= RepaintAssignmentViews;
            EditorApplication.update -= PollAssignments;

            SceneView.RepaintAll();
        }

        private void PollAssignments()
        {
            if (EditorApplication.timeSinceStartup <
                _nextAssignmentRepaint)
            {
                return;
            }

            _nextAssignmentRepaint =
                EditorApplication.timeSinceStartup + 0.25d;

            // Repaint reads current component values, including Inspector edits.
            Repaint();
        }

        private void RepaintAssignmentViews()
        {
            Repaint();
            SceneView.RepaintAll();
        }

        private void OnGUI()
        {
            DrawSettings();

            EditorGUILayout.Space();

            if (GUILayout.Button("Refresh", GUILayout.Height(26f)))
                RefreshObjects();

            EditorGUILayout.LabelField(
                $"Zones: {_zones.Count}    Points: {_points.Count}",
                EditorStyles.miniLabel);

            EditorGUILayout.Space();

            if (_zones.Count == 0)
            {
                EditorGUILayout.HelpBox(
                    "No zones found. Check Angel Zones Tag, then refresh.",
                    MessageType.Info);
                return;
            }

            DrawZonesTable();

            EditorGUILayout.Space(10f);

            if (!_selectedZone)
            {
                EditorGUILayout.HelpBox(
                    "Select a zone to continue.",
                    MessageType.Info);
                return;
            }

            if (!GetZoneComponent(_selectedZone))
            {
                EditorGUILayout.HelpBox(
                    "The selected object needs an AngelZone component.",
                    MessageType.Warning);
                return;
            }

            if (GetZoneColliders().Length == 0)
            {
                EditorGUILayout.HelpBox(
                    "This zone needs a supported 3D Collider on the same " +
                    "GameObject before you can preview or select points.\n\n" +
                    "Supported: Box, Sphere, Capsule, Mesh.",
                    MessageType.Warning);
                return;
            }

            EditorGUILayout.LabelField(
                $"Points for: {_selectedZone.name}",
                EditorStyles.boldLabel);

            DrawPointsTable();
        }

        private void DrawSettings()
        {
            _showSettings = EditorGUILayout.Foldout(
                _showSettings,
                "Settings",
                true);

            if (!_showSettings)
                return;

            EditorGUI.indentLevel++;
            EditorGUI.BeginChangeCheck();

            _pointsTag = EditorGUILayout.TagField(
                "Angel Points Tag",
                _pointsTag);

            _zonesTag = EditorGUILayout.TagField(
                "Angel Zones Tag",
                _zonesTag);

            _previewRadius = Mathf.Max(
                0.01f,
                EditorGUILayout.FloatField(
                    "Preview Sphere Radius",
                    _previewRadius));

            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefs.SetString(
                    SettingsKey + "PointsTag",
                    _pointsTag);

                EditorPrefs.SetString(
                    SettingsKey + "ZonesTag",
                    _zonesTag);

                EditorPrefs.SetFloat(
                    SettingsKey + "PreviewRadius",
                    _previewRadius);

                RefreshObjects();
            }

            EditorGUI.indentLevel--;
        }

        private void RefreshObjects()
        {
            GameObject previousZone = _selectedZone;

            _zones.Clear();
            _points.Clear();

            for (int i = 0;
                 i < UnityEngine.SceneManagement.SceneManager.sceneCount;
                 i++)
            {
                var scene =
                    UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);

                if (!scene.isLoaded)
                    continue;

                foreach (GameObject root in scene.GetRootGameObjects())
                {
                    foreach (Transform child in
                             root.GetComponentsInChildren<Transform>(true))
                    {
                        GameObject obj = child.gameObject;

                        if (obj.tag == _zonesTag)
                            _zones.Add(obj);

                        if (obj.tag == _pointsTag)
                            _points.Add(obj);
                    }
                }
            }

            _zones.Sort((a, b) =>
                string.CompareOrdinal(
                    GetObjectPath(a),
                    GetObjectPath(b)));

            _points.Sort((a, b) =>
                string.CompareOrdinal(
                    GetObjectPath(a),
                    GetObjectPath(b)));

            _selectedZone =
                previousZone &&
                _zones.Contains(previousZone) &&
                GetZoneComponent(previousZone)
                    ? previousZone
                    : _zones.FirstOrDefault(
                        zone => GetZoneComponent(zone) != null);

            if (_selectedZone != previousZone)
                _previewPoint = null;

            if (_previewPoint && !_points.Contains(_previewPoint))
                _previewPoint = null;

            RepaintAssignmentViews();
        }

        private static AngelZone GetZoneComponent(GameObject zone)
        {
            return zone ? zone.GetComponent<AngelZone>() : null;
        }

        private static Transform ReadResetPoint(AngelZone zone)
        {
            if (!zone)
                return null;

            var serializedZone = new SerializedObject(zone);
            serializedZone.Update();

            SerializedProperty property =
                serializedZone.FindProperty(ResetPointPropertyName);

            return property != null
                ? property.objectReferenceValue as Transform
                : null;
        }

        private void AssignResetPoint(GameObject point)
        {
            AngelZone zone = GetZoneComponent(_selectedZone);

            if (!zone || !point)
                return;

            var serializedZone = new SerializedObject(zone);
            serializedZone.Update();

            SerializedProperty property =
                serializedZone.FindProperty(ResetPointPropertyName);

            if (property == null)
            {
                Debug.LogError(
                    $"Cannot find serialized field " +
                    $"'{ResetPointPropertyName}' on AngelZone '{zone.name}'.",
                    zone);
                return;
            }

            property.objectReferenceValue = point.transform;
            serializedZone.ApplyModifiedProperties();

            RepaintAssignmentViews();
        }

        private void DrawZonesTable()
        {
            EditorGUILayout.LabelField(
                "Angel Zones",
                EditorStyles.boldLabel);

            using (new EditorGUILayout.HorizontalScope(
                       EditorStyles.toolbar))
            {
                GUILayout.Label("Name", GUILayout.MinWidth(100f));
                GUILayout.Label("Reset Point", GUILayout.Width(160f));
                GUILayout.Label("Select", GUILayout.Width(80f));
            }

            float tableHeight = Mathf.Clamp(
                _zones.Count * 24f + 8f,
                60f,
                200f);

            _zonesScroll = EditorGUILayout.BeginScrollView(
                _zonesScroll,
                GUILayout.Height(tableHeight));

            for (int i = 0; i < _zones.Count; i++)
            {
                GameObject zoneObject = _zones[i];

                if (!zoneObject)
                    continue;

                AngelZone zone = GetZoneComponent(zoneObject);
                Transform resetPoint = ReadResetPoint(zone);

                using (new EditorGUILayout.HorizontalScope())
                {
                    string path = GetObjectPath(zoneObject)
                        .Replace("/", " > ");

                    GUILayout.Label(
                        new GUIContent($"[{i + 1}] {path}", path),
                        GUILayout.MinWidth(100f));

                    string status;
                    string statusTooltip;

                    if (!zone)
                    {
                        status = "Missing AngelZone";
                        statusTooltip =
                            "Add AngelZone to this GameObject.";
                    }
                    else if (resetPoint)
                    {
                        status = resetPoint.name;
                        statusTooltip =
                            GetObjectPath(resetPoint.gameObject);
                    }
                    else
                    {
                        status = "Not assigned";
                        statusTooltip =
                            "This zone has no reset point assigned.";
                    }

                    Color previousContentColor = GUI.contentColor;

                    GUI.contentColor = !zone
                        ? ErrorColor
                        : resetPoint
                            ? Green
                            : WarningColor;

                    GUILayout.Label(
                        new GUIContent(status, statusTooltip),
                        GUILayout.Width(160f));

                    GUI.contentColor = previousContentColor;

                    using (new EditorGUI.DisabledScope(zone == null))
                    {
                        string tooltip = zone
                            ? "Select this zone and frame it in the Scene view."
                            : "Cannot select: add AngelZone to this GameObject.";

                        if (ColoredButton(
                                new GUIContent("Select", tooltip),
                                _selectedZone == zoneObject,
                                Green))
                        {
                            SelectZone(zoneObject);
                        }
                    }
                }
            }

            EditorGUILayout.EndScrollView();
        }

        private void SelectZone(GameObject zone)
        {
            if (!GetZoneComponent(zone))
                return;

            _selectedZone = zone;
            _previewPoint = null;

            FrameSelectedZone();
            RepaintAssignmentViews();
        }

        private void FrameSelectedZone()
        {
            if (!_selectedZone)
                return;

            Bounds bounds = new Bounds(
                _selectedZone.transform.position,
                Vector3.one * 2f);

            bool hasColliderBounds = false;

            foreach (Collider collider in GetZoneColliders())
            {
                if (!collider.enabled ||
                    !collider.gameObject.activeInHierarchy)
                {
                    continue;
                }

                if (!hasColliderBounds)
                {
                    bounds = collider.bounds;
                    hasColliderBounds = true;
                }
                else
                {
                    bounds.Encapsulate(collider.bounds);
                }
            }

            bounds.size = Vector3.Max(
                bounds.size * 1.2f,
                Vector3.one * 2f);

            SceneView view = SceneView.lastActiveSceneView;

            if (!view)
                view = GetWindow<SceneView>();

            view.Frame(bounds, false);
            view.Repaint();
        }

        private void DrawPointsTable()
        {
            AngelZone zone = GetZoneComponent(_selectedZone);

            if (!zone)
                return;

            Transform assignedPoint = ReadResetPoint(zone);

            string assignmentInfo = assignedPoint
                ? "Current reset point: " +
                  GetObjectPath(assignedPoint.gameObject)
                : "Current reset point: Not assigned";

            EditorGUILayout.HelpBox(
                assignmentInfo,
                assignedPoint ? MessageType.None : MessageType.Warning);

            if (assignedPoint &&
                !_points.Contains(assignedPoint.gameObject))
            {
                EditorGUILayout.HelpBox(
                    "The assigned reset point is not in the current points " +
                    "list. Check its tag or refresh the window.",
                    MessageType.Info);
            }

            if (_points.Count == 0)
            {
                EditorGUILayout.HelpBox(
                    "No points found. Check Angel Points Tag.",
                    MessageType.Info);
                return;
            }

            using (new EditorGUILayout.HorizontalScope(
                       EditorStyles.toolbar))
            {
                GUILayout.Label("Name", GUILayout.MinWidth(100f));
                GUILayout.Label("Preview", GUILayout.Width(80f));
                GUILayout.Label("Select", GUILayout.Width(80f));
            }

            _pointsScroll = EditorGUILayout.BeginScrollView(
                _pointsScroll);

            foreach (GameObject point in _points)
            {
                if (!point)
                    continue;

                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.Label(
                        new GUIContent(
                            point.name,
                            GetObjectPath(point)),
                        GUILayout.MinWidth(100f));

                    if (ColoredButton(
                            "Preview",
                            _previewPoint == point,
                            Purple))
                    {
                        PreviewPoint(point);
                    }

                    if (ColoredButton(
                            "Select",
                            assignedPoint == point.transform,
                            Green))
                    {
                        AssignResetPoint(point);
                        assignedPoint = ReadResetPoint(zone);
                    }
                }
            }

            EditorGUILayout.EndScrollView();
        }

        private static bool ColoredButton(
            string text,
            bool highlighted,
            Color color)
        {
            return ColoredButton(
                new GUIContent(text),
                highlighted,
                color);
        }

        private static bool ColoredButton(
            GUIContent content,
            bool highlighted,
            Color color)
        {
            Color previous = GUI.backgroundColor;

            if (highlighted)
                GUI.backgroundColor = color;

            bool clicked = GUILayout.Button(
                content,
                GUILayout.Width(80f));

            GUI.backgroundColor = previous;

            return clicked;
        }

        private void PreviewPoint(GameObject point)
        {
            _previewPoint = point;

            SceneView view = SceneView.lastActiveSceneView;

            if (!view)
                view = GetWindow<SceneView>();

            float frameSize = Mathf.Max(
                _previewRadius * 6f,
                2f);

            view.Frame(
                new Bounds(
                    point.transform.position,
                    Vector3.one * frameSize),
                false);

            view.Repaint();
            RepaintAssignmentViews();
        }

        private Collider[] GetZoneColliders()
        {
            if (!_selectedZone)
                return Array.Empty<Collider>();

            return _selectedZone.GetComponents<Collider>()
                .Where(c =>
                    c is BoxCollider ||
                    c is SphereCollider ||
                    c is CapsuleCollider ||
                    (c is MeshCollider mesh &&
                     mesh.sharedMesh))
                .ToArray();
        }

        private void DrawScene(SceneView sceneView)
        {
            if (Event.current.type != EventType.Repaint)
                return;

            AngelZone zone = GetZoneComponent(_selectedZone);

            if (!zone)
                return;

            Collider[] colliders = GetZoneColliders();

            if (colliders.Length == 0)
                return;

            using (new Handles.DrawingScope(ZoneColor))
            {
                foreach (Collider collider in colliders)
                    DrawCollider(collider);
            }

            Transform assignedPoint = ReadResetPoint(zone);

            // Assigned reset point: yellow sphere.
            if (assignedPoint)
            {
                Vector3 position = assignedPoint.position;

                using (new Handles.DrawingScope(Color.yellow))
                {
                    Handles.SphereHandleCap(
                        0,
                        position,
                        Quaternion.identity,
                        _previewRadius * 2f,
                        EventType.Repaint);

                    Handles.Label(
                        position + Vector3.up * (_previewRadius + 0.1f),
                        $"Selected: {assignedPoint.name}");
                }
            }

            if (!_previewPoint)
                return;

            Vector3 previewPosition = _previewPoint.transform.position;

            bool previewIsAssigned =
                assignedPoint == _previewPoint.transform;

            using (new Handles.DrawingScope(Purple))
            {
                if (previewIsAssigned)
                {
                    // Keep the selected sphere yellow while indicating preview.
                    DrawWireSphere(
                        previewPosition,
                        _previewRadius * 1.15f);
                }
                else
                {
                    Handles.SphereHandleCap(
                        0,
                        previewPosition,
                        Quaternion.identity,
                        _previewRadius * 2f,
                        EventType.Repaint);

                    Handles.Label(
                        previewPosition + Vector3.up * (_previewRadius + 0.1f),
                        $"Preview: {_previewPoint.name}");
                }
            }
        }

        private static void DrawCollider(Collider collider)
        {
            Transform t = collider.transform;
            Vector3 scale = Abs(t.lossyScale);

            if (collider is BoxCollider box)
            {
                using (new Handles.DrawingScope(t.localToWorldMatrix))
                {
                    Handles.DrawWireCube(
                        box.center,
                        box.size);
                }
            }
            else if (collider is SphereCollider sphere)
            {
                float radius = sphere.radius *
                    Mathf.Max(scale.x, Mathf.Max(scale.y, scale.z));

                DrawWireSphere(
                    t.TransformPoint(sphere.center),
                    radius);
            }
            else if (collider is CapsuleCollider capsule)
            {
                DrawCapsule(capsule, scale);
            }
            else if (collider is MeshCollider mesh)
            {
                Material material =
                    AssetDatabase.GetBuiltinExtraResource<Material>(
                        "Default-Line.mat");

                if (!material)
                    return;

                bool previousWireframe = GL.wireframe;

                try
                {
                    GL.wireframe = true;

                    if (material.SetPass(0))
                    {
                        Graphics.DrawMeshNow(
                            mesh.sharedMesh,
                            t.localToWorldMatrix);
                    }
                }
                finally
                {
                    GL.wireframe = previousWireframe;
                }
            }
        }

        private static void DrawWireSphere(
            Vector3 center,
            float radius)
        {
            Handles.DrawWireDisc(center, Vector3.right, radius);
            Handles.DrawWireDisc(center, Vector3.up, radius);
            Handles.DrawWireDisc(center, Vector3.forward, radius);
        }

        private static void DrawCapsule(
            CapsuleCollider capsule,
            Vector3 scale)
        {
            Transform t = capsule.transform;
            int direction = capsule.direction;

            Vector3 localAxis = direction == 0
                ? Vector3.right
                : direction == 1
                    ? Vector3.up
                    : Vector3.forward;

            Vector3 localSide = direction == 0
                ? Vector3.up
                : Vector3.right;

            Vector3 axis = t.rotation * localAxis;
            Vector3 side = t.rotation * localSide;
            Vector3 otherSide =
                Vector3.Cross(axis, side).normalized;

            float axisScale = scale[direction];

            float radiusScale = direction == 0
                ? Mathf.Max(scale.y, scale.z)
                : direction == 1
                    ? Mathf.Max(scale.x, scale.z)
                    : Mathf.Max(scale.x, scale.y);

            float radius = capsule.radius * radiusScale;

            float height = Mathf.Max(
                capsule.height * axisScale,
                radius * 2f);

            float halfSegment = height * 0.5f - radius;

            Vector3 center = t.TransformPoint(capsule.center);
            Vector3 top = center + axis * halfSegment;
            Vector3 bottom = center - axis * halfSegment;

            Handles.DrawWireDisc(top, axis, radius);
            Handles.DrawWireDisc(bottom, axis, radius);

            foreach (Vector3 radial in
                     new[] { side, -side, otherSide, -otherSide })
            {
                Handles.DrawLine(
                    bottom + radial * radius,
                    top + radial * radius);
            }

            Handles.DrawWireArc(
                top, otherSide, side, 180f, radius);

            Handles.DrawWireArc(
                top, -side, otherSide, 180f, radius);

            Handles.DrawWireArc(
                bottom, -otherSide, side, 180f, radius);

            Handles.DrawWireArc(
                bottom, side, otherSide, 180f, radius);
        }

        private static Vector3 Abs(Vector3 value)
        {
            return new Vector3(
                Mathf.Abs(value.x),
                Mathf.Abs(value.y),
                Mathf.Abs(value.z));
        }

        private static string GetObjectPath(GameObject obj)
        {
            string path = obj.name;

            for (Transform parent = obj.transform.parent;
                 parent;
                 parent = parent.parent)
            {
                path = parent.name + "/" + path;
            }

            return obj.scene.name + "/" + path;
        }
    }
}