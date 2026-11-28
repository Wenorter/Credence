using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

namespace Editor
{
    public class NavMeshAgentTrackerWindow : EditorWindow
    {
        private NavMeshAgent[] _agents;
        private NavMeshAgent _selectedAgent;
        private GameObject _followTarget;
        private bool _isFollowing = false;
        private Vector2 _scrollPos;

        [MenuItem("Tools/NavMesh Agent Tracker")]
        public static void ShowWindow()
        {
            GetWindow<NavMeshAgentTrackerWindow>("Agent Tracker");
        }

        private void OnEnable()
        {
            RefreshAgents();
            // Subscribe to Scene view rendering to draw custom Gizmos
            SceneView.duringSceneGui += OnSceneGUI;
        }

        private void OnDisable()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
        }

        private void RefreshAgents()
        {
            // Finds all NavMeshAgents in the active scene
            _agents = FindObjectsByType<NavMeshAgent>(FindObjectsSortMode.None);
        }

        private void OnGUI()
        {
            EditorGUILayout.Space();
            if (GUILayout.Button("Refresh Agents in Scene", GUILayout.Height(25)))
            {
                RefreshAgents();
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Scene NavMesh Agents", EditorStyles.boldLabel);

            // --- Table Header ---
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Agent Name", EditorStyles.boldLabel, GUILayout.Width(140));
            EditorGUILayout.LabelField("Position", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Action", EditorStyles.boldLabel, GUILayout.Width(65));
            EditorGUILayout.EndHorizontal();

            // --- Table Body ---
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, GUILayout.Height(180));
            if (_agents != null && _agents.Length > 0)
            {
                foreach (NavMeshAgent agent in _agents)
                {
                    if (!agent) continue;

                    // Highlight the row if this agent is currently selected
                    GUI.backgroundColor = (agent == _selectedAgent) ? Color.cyan : Color.white;
                    EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                    GUI.backgroundColor = Color.white;

                    EditorGUILayout.LabelField(agent.gameObject.name, GUILayout.Width(140));
                    EditorGUILayout.LabelField(agent.transform.position.ToString("F1"));

                    if (GUILayout.Button("Select", GUILayout.Width(60)))
                    {
                        SelectAndFocusAgent(agent);
                    }

                    EditorGUILayout.EndHorizontal();
                }
            }
            else
            {
                EditorGUILayout.HelpBox("No NavMeshAgents found in the scene.", MessageType.Info);
            }
            EditorGUILayout.EndScrollView();

            // --- Selected Agent Controls ---
            if (_selectedAgent)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField($"Selected: {_selectedAgent.gameObject.name}", EditorStyles.boldLabel);

                _followTarget = (GameObject)EditorGUILayout.ObjectField(
                    "Target to Follow",
                    _followTarget,
                    typeof(GameObject),
                    true // Allow scene objects
                );

                // Disable follow controls if not in Play Mode or if no target is assigned
                GUI.enabled = Application.isPlaying && _followTarget;

                if (!_isFollowing)
                {
                    if (GUILayout.Button("Force Follow Target (Play Mode)", GUILayout.Height(30)))
                    {
                        _isFollowing = true;
                    }
                }
                else
                {
                    GUI.backgroundColor = new Color(1f, 0.5f, 0.5f);
                    if (GUILayout.Button("Stop Following", GUILayout.Height(30)))
                    {
                        _isFollowing = false;
                        if (_selectedAgent.isOnNavMesh)
                            _selectedAgent.ResetPath();
                    }
                    GUI.backgroundColor = Color.white;
                }

                GUI.enabled = true;

                if (!Application.isPlaying)
                {
                    EditorGUILayout.HelpBox("Enter Play Mode to force the agent to follow a target.", MessageType.Warning);
                }
            }
        }

        private void SelectAndFocusAgent(NavMeshAgent agent)
        {
            _selectedAgent = agent;
            _isFollowing = false; // Reset follow state when switching agents

            // Highlight in Hierarchy and frame camera on the agent in Scene view
            Selection.activeGameObject = agent.gameObject;
            SceneView.lastActiveSceneView?.FrameSelected();
            SceneView.RepaintAll();
        }

        private void Update()
        {
            // Continuously update destination in real time during Play Mode
            if (Application.isPlaying && _isFollowing && _selectedAgent && _followTarget)
            {
                if (_selectedAgent.isOnNavMesh)
                {
                    _selectedAgent.SetDestination(_followTarget.transform.position);
                }
            }

            // Keep positions live-updating in the window while playing
            if (Application.isPlaying)
            {
                Repaint();
            }
        }

        private void OnSceneGUI(SceneView sceneView)
        {
            if (!_selectedAgent) return;

            // Draw a green wire disc & cylinder around the selected agent
            Handles.color = Color.green;
            Vector3 pos = _selectedAgent.transform.position;
            float radius = Mathf.Max(_selectedAgent.radius, 0.5f);

            Handles.DrawWireDisc(pos, Vector3.up, radius + 0.3f);
            Handles.DrawWireDisc(pos + Vector3.up * _selectedAgent.height, Vector3.up, radius + 0.3f);
            Handles.DrawLine(pos, pos + Vector3.up * _selectedAgent.height);

            // Draw a line to the follow target and the agent's current NavMesh path
            if (_followTarget && _isFollowing)
            {
                Handles.color = Color.yellow;
                Handles.DrawDottedLine(pos, _followTarget.transform.position, 4f);
            }

            if (_selectedAgent.hasPath)
            {
                Handles.color = Color.cyan;
                Handles.DrawPolyLine(_selectedAgent.path.corners);
            }
        }
    }
}
