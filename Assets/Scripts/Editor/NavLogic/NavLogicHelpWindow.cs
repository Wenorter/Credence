using UnityEditor;
using UnityEngine;

namespace Editor.NavLogic
{
    public class NavLogicHelpWindow : EditorWindow
    {
        private const string BuilderMenu = "Tools/NavLogic/Floor Builder";
        private const string BakerMenu = "Tools/NavLogic/NavMesh Baker";

        private static readonly string[] Tabs =
            { "Quick start", "Draw", "Merge", "Bake", "Agents", "Hide floor", "Problems" };

        private int _tab;
        private Vector2 _scroll;
        private GUIStyle _body, _h1, _h2, _step;

        [MenuItem("Tools/NavLogic/How To Use", false, 0)]
        private static void Open()
        {
            NavLogicHelpWindow w = GetWindow<NavLogicHelpWindow>("NavLogic Help");
            w.minSize = new Vector2(420, 400);
        }

        private void InitStyles()
        {
            if (_body != null) return;

            _body = new GUIStyle(EditorStyles.wordWrappedLabel) { richText = true, fontSize = 12 };
            _h1 = new GUIStyle(EditorStyles.boldLabel) { fontSize = 16, wordWrap = true };
            _h2 = new GUIStyle(EditorStyles.boldLabel) { fontSize = 13, wordWrap = true };
            _step = new GUIStyle(_body) { padding = new RectOffset(6, 6, 2, 2) };
        }

        private void H1(string t) { EditorGUILayout.Space(4); GUILayout.Label(t, _h1); EditorGUILayout.Space(2); }
        private void H2(string t) { EditorGUILayout.Space(8); GUILayout.Label(t, _h2); }
        private void P(string t) { GUILayout.Label(t, _body); }
        private void S(int n, string t) { GUILayout.Label(n + ".  " + t, _step); }
        private void B(string t) { GUILayout.Label("•  " + t, _step); }

        private void OnGUI()
        {
            InitStyles();

            _tab = GUILayout.SelectionGrid(_tab, Tabs, 4);
            EditorGUILayout.Space();

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Open Floor Builder")) EditorApplication.ExecuteMenuItem(BuilderMenu);
                if (GUILayout.Button("Open NavMesh Baker")) EditorApplication.ExecuteMenuItem(BakerMenu);
            }

            _scroll = EditorGUILayout.BeginScrollView(_scroll);
            switch (_tab)
            {
                case 0: QuickStart(); break;
                case 1: Draw(); break;
                case 2: Merge(); break;
                case 3: Bake(); break;
                case 4: Agents(); break;
                case 5: HideFloor(); break;
                default: Problems(); break;
            }
            EditorGUILayout.EndScrollView();
        }

        private void QuickStart()
        {
            H1("What this is");
            P("A set of tools that builds a clean, invisible floor from points you place, then bakes a Unity NavMesh " +
              "from only that floor. Props and clutter on the real ground do not affect the result.");

            H2("The whole workflow");
            S(1, "<b>Floor Builder, Draw tab:</b> place points around a walkable area and press BUILD. Repeat for each flat zone.");
            S(2, "<b>Merge tab (optional):</b> join two neighbouring shapes at the same height into one.");
            S(3, "<b>NavMesh Baker:</b> pick agent types, set wall and obstacle margins, press BAKE.");
            S(4, "<b>Agents:</b> set each NavMeshAgent's Agent Type to a baked type.");
            S(5, "<b>Hide the floor</b> from the game camera so players never see it.");
            S(6, "Save the scene.");

            H2("Key ideas");
            B("Each shape stores its points, so you can reopen it with <b>Back to Nav Editor</b> any time.");
            B("Different heights = different shapes. Do not merge zones at different heights, the merge would slope between them.");
            B("You do not need to merge shapes for the NavMesh. All shapes are baked together into one surface.");
        }

        private void Draw()
        {
            H1("Draw tab");
            S(1, "Open <b>Tools > NavLogic > Floor Builder</b> and stay on the <b>Draw</b> tab.");
            S(2, "Press <b>Add Point</b>. The first point appears in the middle of the Scene view. Every next point starts exactly where the last one is.");
            S(3, "Move each point with the Move tool, or type its position in the table.");
            S(4, "Press <b>Add Final Point (close)</b>, or <b>Close Shape</b> to connect the last point back to the first. " +
                 "The closing line is magenta, the first point is green and the last is red.");
            S(5, "Fix the order with the ▲ ▼ buttons if lines cross. Remove a point with X. The order in the table is the connection order.");
            S(6, "Check the <b>Build settings</b> (see below) and press <b>BUILD</b>. The ProBuilder plane is created and the point objects are removed.");

            H2("Build settings");
            B("<b>Face direction:</b> keep it on Up. A NavMesh only builds on upward-facing surfaces.");
            B("<b>Flatten Y:</b> None keeps each point's height. Average or Custom puts the whole shape at one height.");
            B("<b>Layer:</b> use a dedicated layer such as NavFloor, so you can hide it from the camera.");
            B("<b>Add MeshCollider / Add NavMeshModifier:</b> usually both on.");

            H2("Undo and editing");
            B("<b>Ctrl+Z</b> after BUILD removes the plane and brings the points back in the table.");
            B("Select a built shape and press <b>Back to Nav Editor</b> in its Inspector to edit its points again.");
            B("Built shapes show their point numbers as gizmos. Untick <b>Show Gizmos</b> on the Nav Floor Shape component to hide them.");
        }

        private void Merge()
        {
            H1("Merge tab");
            P("Merging joins two built shapes along a shared border. It is only needed when you want one object. " +
              "The NavMesh does not need it.");

            S(1, "Open the <b>Merge</b> tab. Set <b>Shape A</b> and <b>Shape B</b> (drag them in, or select one and press Use Selected).");
            S(2, "Read the point numbers from the gizmos on each shape.");
            S(3, "Enter <b>2 pairs per seam</b>: the two ends of the shared border. Pair row = point on A, point on B.");
            S(4, "Check the preview in the Scene view. Yellow is the outer outline, orange is a hole.");
            S(5, "Press <b>MERGE</b>. The two originals are replaced by one shape. Ctrl+Z undoes it.");

            H2("Options");
            B("<b>Weld position:</b> Midpoint, Use A or Use B for the joined points.");
            B("<b>Hole handling:</b> with 2 seams you get a ring. <b>Bridge</b> keeps the hole, <b>Fill</b> makes it solid floor.");
            B("<b>Flip kept side:</b> use it only if the wrong side of the border was removed.");

            H2("Remember");
            B("Merge shapes at the <b>same height</b>. Different heights create sloped triangles.");
            B("Shapes built before the marker existed have no stored points. Rebuild them once.");
        }

        private void Bake()
        {
            H1("NavMesh Baker");
            S(1, "Open <b>Tools > NavLogic > NavMesh Baker</b>.");
            S(2, "Tick the <b>agent types</b> to bake. One NavMesh asset is made per type.");
            S(3, "Set <b>Avoid wall radius</b> and <b>Avoid obstacle radius</b>. Use your agent's collider radius plus 0.1 to 0.2 as a start.");
            S(4, "Choose <b>Obstacle layers</b> for props that should block. Never include the floor's own layer.");
            S(5, "Keep <b>Override voxel size</b> on, with 0.1 or smaller.");
            S(6, "Press <b>BAKE NAVMESH FROM SHAPES</b>. Read the console line and check there is no EMPTY warning.");
            S(7, "Save the scene. Assets are saved in a folder next to the scene, named after the scene.");

            H2("What it creates");
            B("A <b>NavFloorNavMesh</b> object in the scene that registers the baked data when the game starts.");
            B("One NavFloorNavMesh_<type>.asset file per agent type.");

            H2("How the margins work");
            B("Unity only has one radius, so the tool uses the smaller value as the real radius.");
            B("The bigger margin is added with extra blocked strips along floor edges (walls) or bigger boxes around props (obstacles).");
            B("Use only one NavMesh source for the same floor. Remove any old NavMeshSurface that bakes it too.");
        }

        private void Agents()
        {
            H1("Setting up agents");
            S(1, "Select the agent. On <b>Nav Mesh Agent</b>, set <b>Agent Type</b> to a type you baked.");
            S(2, "Place the agent on the floor, within a small distance of the baked blue area.");
            S(3, "Make sure the <b>NavFloorNavMesh</b> object is active and its list of data is filled.");
            S(4, "Move with <b>SetDestination</b> as usual.");

            H2("Settings that help");
            B("Turn <b>Auto Braking</b> off for chasing or patrolling.");
            B("Raise <b>Angular Speed</b> and <b>Acceleration</b> so the agent turns in time at corners.");
            B("If the agent has a Rigidbody, tick <b>Is Kinematic</b>.");
            B("The <b>NavAgentUnstuck</b> script (if you added it) repaths and nudges an agent that stops against a wall.");
        }

        private void HideFloor()
        {
            H1("Hiding the floor");
            H2("Turn off renderers");
            S(1, "Use <b>Tools > NavLogic > Floor Renderers > Hide Floor Renderers</b>.");
            S(2, "Bring them back with <b>Show Floor Renderers</b>.");
        }

        private void Problems()
        {
            H1("Common problems");

            H2("The NavMesh is empty");
            B("Floor must face up.");
            B("Obstacle layers must not include the floor's layer.");
            B("Wall radius must not be bigger than the shape. Lower it, or lower the voxel size.");

            H2("Agent will not move or is not on the NavMesh");
            B("Agent Type must match a baked type.");
            B("The agent must start close to the baked area, and not in a margin-eroded spot.");
            B("The NavFloorNavMesh object must be active, and the scene saved after baking.");

            H2("Agent rubs against walls at corners");
            B("Raise Avoid wall radius to the collider radius plus a little more, then re-bake.");
            B("Check that the floor's edge is close to the real wall.");

            H2("Heights look wrong");
            B("A shape only has points on its outline. Different heights slope between them.");
            B("Use one shape per flat zone and Flatten Y: Average on each.");
            B("Connect zones with a ramp shape, within the step height and slope.");

            H2("Two zones are not connected");
            B("Overlap their edges a little, by about one voxel.");
            B("A height difference above Max step height needs a ramp between them.");
        }
    }
}