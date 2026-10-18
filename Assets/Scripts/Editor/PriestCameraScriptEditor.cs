using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(CameraScript))]
    public class PriestCameraScriptEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.Space();

            if (GUILayout.Button("Snap Camera To Priest"))
            {
                CameraScript cameraScript =
                    (CameraScript)target;

                Undo.RecordObject(
                    cameraScript.transform,
                    "Snap Camera To Priest"
                );

                cameraScript.SnapToTarget();

                PrefabUtility.RecordPrefabInstancePropertyModifications(
                    cameraScript.transform
                );

                if (!Application.isPlaying)
                {
                    EditorSceneManager.MarkSceneDirty(
                        cameraScript.gameObject.scene
                    );
                }

                SceneView.RepaintAll();
            }
        }
    }
}