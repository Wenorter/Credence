using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(CharacterSwitchManager))]
    public class CharacterSwitchManagerEditor : UnityEditor.Editor
    {
        private const float ButtonHeight = 32f;
        private const float MinButtonWidth = 110f;

        private static readonly Color ActiveCharacterColor = new Color(0.35f, 0.85f, 0.45f);
        private static readonly Color ActiveOffColor = new Color(0.95f, 0.42f, 0.42f);
        private static readonly Color PanelAccent = new Color(0.30f, 0.60f, 1.00f);

        private SerializedProperty _switchAction;
        private SerializedProperty _characters;
        private SerializedProperty _startIndex;

        private GUIStyle _buttonStyle;
        private GUIStyle _activeButtonStyle;
        private GUIStyle _headerStyle;
        private GUIStyle _modeStyle;

        private void OnEnable()
        {
            _switchAction = serializedObject.FindProperty("switchAction");
            _characters = serializedObject.FindProperty("characters");
            _startIndex = serializedObject.FindProperty("startIndex");
        }

        // Keep highlights in sync when the input action switches characters during play.
        public override bool RequiresConstantRepaint() => Application.isPlaying;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_switchAction);
            EditorGUILayout.PropertyField(_characters, true);
            EditorGUILayout.PropertyField(_startIndex);

            int maxIndex = Mathf.Max(-1, _characters.arraySize - 1);
            _startIndex.intValue = Mathf.Clamp(_startIndex.intValue, -1, maxIndex);

            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.Space(10);
            DrawSwitchPanel();
        }

        private void DrawSwitchPanel()
        {
            EnsureStyles();

            if (targets.Length > 1)
            {
                EditorGUILayout.HelpBox("Switch buttons are available when a single object is selected.", MessageType.Info);
                return;
            }

            CharacterSwitchManager manager = (CharacterSwitchManager)target;

            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                // Accent line
                Rect accent = GUILayoutUtility.GetRect(1f, 2f, GUILayout.ExpandWidth(true));
                EditorGUI.DrawRect(accent, PanelAccent);
                EditorGUILayout.Space(4);

                // Header
                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.Label("Character Switch", _headerStyle);
                    GUILayout.FlexibleSpace();
                    GUILayout.Label(Application.isPlaying ? "● PLAY MODE" : "EDIT MODE", _modeStyle);
                }

                int count = manager.Count;
                if (count == 0)
                {
                    EditorGUILayout.HelpBox("Add characters to the list to get switch buttons.", MessageType.None);
                    return;
                }

                List<int> enabled = GetEnabledIndices(manager);
                int active = enabled.Count > 0 ? enabled[0] : CharacterSwitchManager.NoneIndex;

                string status = active >= 0 ? $"Active: {GetLabel(manager, active)}" : "All characters off";
                EditorGUILayout.LabelField(status, EditorStyles.miniLabel);

                if (enabled.Count > 1)
                {
                    EditorGUILayout.HelpBox("Several characters are enabled at once. Click a button to clean up.", MessageType.Warning);
                }

                EditorGUILayout.Space(4);
                DrawButtonGrid(manager, count, active);
                EditorGUILayout.Space(2);
            }
        }

        private void DrawButtonGrid(CharacterSwitchManager manager, int count, int active)
        {
            int total = count + 1;
            float available = EditorGUIUtility.currentViewWidth - 40f;
            int columns = Mathf.Clamp(Mathf.FloorToInt(available / MinButtonWidth), 1, total);

            for (int rowStart = 0; rowStart < total; rowStart += columns)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    for (int c = 0; c < columns; c++)
                    {
                        int cell = rowStart + c;

                        if (cell >= total)
                        {
                            GUILayout.Label(GUIContent.none, GUILayout.Height(ButtonHeight), GUILayout.ExpandWidth(true));
                            continue;
                        }

                        DrawButton(manager, cell - 1, active);
                    }
                }
            }
        }

        private void DrawButton(CharacterSwitchManager manager, int index, int active)
        {
            bool isActive = index == active;
            bool isOff = index < 0;

            GUIContent content = isOff ? new GUIContent("Off", "Turn off every character") : new GUIContent(GetLabel(manager, index), $"Switch to character #{index}");

            Color previous = GUI.backgroundColor;
            if (isActive) GUI.backgroundColor = isOff ? ActiveOffColor : ActiveCharacterColor;

            bool clicked = GUILayout.Button(
                content,
                isActive ? _activeButtonStyle : _buttonStyle,
                GUILayout.Height(ButtonHeight),
                GUILayout.MinWidth(60f),
                GUILayout.ExpandWidth(true)
            );

            GUI.backgroundColor = previous;

            if (clicked) Apply(manager, index);
        }

        private void Apply(CharacterSwitchManager manager, int index)
        {
            if (Application.isPlaying)
            {
                manager.SwitchTo(index);
                return;
            }
            
            manager.SwitchTo(index);

            serializedObject.Update();
            _startIndex.intValue = index;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();

            foreach (Object obj in CollectTouchedObjects(manager))
            {
                EditorUtility.SetDirty(obj);

                if (PrefabUtility.IsPartOfPrefabInstance(obj)) PrefabUtility.RecordPrefabInstancePropertyModifications(obj);
            }

            EditorUtility.SetDirty(manager);

            if (manager.gameObject.scene.IsValid()) EditorSceneManager.MarkSceneDirty(manager.gameObject.scene);
        }

        private static List<int> GetEnabledIndices(CharacterSwitchManager manager)
        {
            List<int> result = new List<int>();

            if (Application.isPlaying)
            {
                if (manager.CurrentIndex >= 0) result.Add(manager.CurrentIndex);
                return result;
            }

            for (int i = 0; i < manager.Count; i++)
            {
                if (manager.IsCharacterEnabled(i)) result.Add(i);
            }

            return result;
        }

        private static Object[] CollectTouchedObjects(CharacterSwitchManager manager)
        {
            HashSet<Object> set = new HashSet<Object>();

            foreach (CharacterSwitchManager.CharacterEntry entry in manager.Characters)
            {
                if (entry.gameObjects != null)
                {
                    foreach (GameObject go in entry.gameObjects) if (go) set.Add(go);
                }
                    

                if (entry.behaviours != null)
                {
                    foreach (MonoBehaviour b in entry.behaviours) if (b) set.Add(b);
                }
                    
            }

            Object[] array = new Object[set.Count];
            set.CopyTo(array);
            return array;
        }

        private static string GetLabel(CharacterSwitchManager manager, int index)
        {
            string name = manager.Characters[index].name;
            return string.IsNullOrWhiteSpace(name) ? $"Character {index + 1}" : name;
        }

        private void EnsureStyles()
        {
            if (_buttonStyle != null) return;

            _buttonStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 12,
                alignment = TextAnchor.MiddleCenter,
                clipping = TextClipping.Clip,
                wordWrap = false
            };

            _activeButtonStyle = new GUIStyle(_buttonStyle)
            {
                fontStyle = FontStyle.Bold
            };

            _headerStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 13
            };

            _modeStyle = new GUIStyle(EditorStyles.miniBoldLabel)
            {
                alignment = TextAnchor.MiddleRight,
                normal = { textColor = Application.isPlaying ? ActiveCharacterColor : Color.gray }
            };
        }
    }
}