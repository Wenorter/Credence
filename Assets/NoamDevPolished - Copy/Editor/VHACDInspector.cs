#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace MeshProcess
{
    [CustomEditor(typeof(VhacdRuntime))]
    public class VHACDInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.Space(8);
            EditorGUILayout.LabelField("Actions", EditorStyles.boldLabel);

            using (new EditorGUI.DisabledScope(Application.isPlaying))
            {
                if (GUILayout.Button("Generate & Apply Convex Colliders"))
                {
                    var comp = (VhacdRuntime)target;

                    Undo.RegisterFullObjectHierarchyUndo(comp.gameObject, "VHACD Generate Colliders");

                    try
                    {
                        comp.GenerateAndApplyConvexColliders();
                        EditorUtility.SetDirty(comp.gameObject);
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError($"VHACD failed: {e.Message}\n{e.StackTrace}", comp);
                    }
                }
            }

            EditorGUILayout.HelpBox(
                "Runs in Edit Mode. Requires a MeshFilter with a sharedMesh on this GameObject.\n" +
                "Also make sure Player Settings has 'Allow unsafe code' enabled.",
                MessageType.Info
            );
        }
    }
}
#endif