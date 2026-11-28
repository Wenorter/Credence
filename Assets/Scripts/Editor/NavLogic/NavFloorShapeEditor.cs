using Editor.NavLogic;
using NavLogic;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(NavFloorShape))]
    public class NavFloorShapeEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            EditorGUILayout.Space();
            if (GUILayout.Button("Back to Nav Editor", GUILayout.Height(28)))
                NavFloorBuilderWindow.OpenWith((NavFloorShape)target);
        }
    }
    
    public static class NavFloorShapeGizmos
    {
        [DrawGizmo(GizmoType.Selected | GizmoType.NonSelected | GizmoType.Pickable)]
        private static void Draw(NavFloorShape shape, GizmoType type)
        {
            if (!shape.showGizmos || shape.points == null || shape.points.Count == 0) return;

            Transform t = shape.transform;
            int n = shape.points.Count;

            for (int i = 0; i < n; i++)
            {
                Vector3 p = t.TransformPoint(shape.points[i]);
                float s = UnityEditor.HandleUtility.GetHandleSize(p) * 0.12f;

                Gizmos.color = i == 0 ? Color.green : (i == n - 1 ? Color.red : new Color(1f, 0.6f, 0f));
                Gizmos.DrawSphere(p, s);

                if (i < n - 1)
                {
                    Gizmos.color = Color.cyan;
                    Gizmos.DrawLine(p, t.TransformPoint(shape.points[i + 1]));
                }

                Handles.Label(p + Vector3.up * s * 1.5f, i.ToString());
            }

            if (n > 2)
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawLine(t.TransformPoint(shape.points[n - 1]), t.TransformPoint(shape.points[0]));
            }
        }
    }
}