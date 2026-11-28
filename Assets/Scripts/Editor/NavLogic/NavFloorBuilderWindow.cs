using System;
using System.Collections.Generic;
using NavLogic;
using Unity.AI.Navigation;
using UnityEditor;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

namespace Editor.NavLogic
{
    public class NavFloorBuilderWindow : EditorWindow
    {
        private enum Tab { Draw, Merge }
        private enum FlattenMode { None, Average, Custom }
        private enum FaceDir { Up, Down }
        private enum WeldMode { Midpoint, UseA, UseB }
        private enum HoleMode { Bridge, Fill }

        [Serializable]
        private class MergePair { public int a; public int b; }

        private class Arc
        {
            public List<Vector3> pts;
            public int startW, endW;
            public bool used;
        }

        private const string DraftName = "NavFloorDraft";
        private const string ClosedName = "NavFloorDraft_Closed";

        [SerializeField] private Tab tab = Tab.Draw;
        [SerializeField] private Transform root;
        [SerializeField] private FlattenMode flatten = FlattenMode.None;
        [SerializeField] private float customY;
        [SerializeField] private FaceDir faceDir = FaceDir.Up;
        [SerializeField] private string shapeName = "NavFloor_Part";
        [SerializeField] private int layer = 0;
        [SerializeField] private bool addMeshCollider = true;
        [SerializeField] private bool addNavModifier = true;
        [SerializeField] private float gizmoSize = 0.15f;
        [SerializeField] private bool customTri;

        [SerializeField] private NavFloorShape shapeA;
        [SerializeField] private NavFloorShape shapeB;
        [SerializeField] private List<MergePair> pairs = new List<MergePair>
        {
            new MergePair { a = 0, b = 0 },
            new MergePair { a = 1, b = 1 }
        };
        [SerializeField] private WeldMode weldMode = WeldMode.Midpoint;
        [SerializeField] private HoleMode holeMode = HoleMode.Bridge;
        [SerializeField] private bool flipA;
        [SerializeField] private bool flipB;

        private readonly List<Transform> _points = new List<Transform>();
        private Vector2 _scroll;
        private static readonly Color PointCol = new Color(1f, 0.6f, 0f);
        private static readonly Color LineCol = Color.cyan;
        private static readonly Color CloseCol = Color.magenta;

        private bool closed
        {
            get => root && root.name == ClosedName;
            set
            {
                if (!root) return;
                string n = value ? ClosedName : DraftName;
                if (root.name == n) return;
                Undo.RecordObject(root.gameObject, value ? "Close shape" : "Reopen shape");
                root.name = n;
            }
        }

        [MenuItem("Tools/NavLogic/Floor Builder")]
        private static void Open() => GetWindow<NavFloorBuilderWindow>("Nav Floor Builder");

        public static void OpenWith(NavFloorShape shape)
        {
            NavFloorBuilderWindow w = GetWindow<NavFloorBuilderWindow>("Nav Floor Builder");
            w.LoadFromShape(shape);
        }

        private void OnEnable()
        {
            SceneView.duringSceneGui += OnSceneGUI;
            Undo.undoRedoPerformed += OnUndoRedo;
            SyncFromScene();
        }

        private void OnDisable()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
            Undo.undoRedoPerformed -= OnUndoRedo;
        }

        private void OnUndoRedo()
        {
            SyncFromScene();
            Repaint();
            SceneView.RepaintAll();
        }

        private void SyncFromScene()
        {
            if (!root)
            {
                GameObject g = GameObject.Find(DraftName);
                if (!g) g = GameObject.Find(ClosedName);
                root = g ? g.transform : null;
            }

            _points.Clear();
            if (!root) return;
            foreach (Transform c in root) _points.Add(c);
        }

        private void Clean() => _points.RemoveAll(p => !p);

        private void EnsureRoot()
        {
            if (root) return;
            GameObject go = new GameObject(DraftName);
            Undo.RegisterCreatedObjectUndo(go, "Create draft");
            root = go.transform;
        }

        Vector3 NextPosition()
        {
            if (_points.Count > 0)
                return _points[_points.Count - 1].position;

            SceneView sv = SceneView.lastActiveSceneView;
            return sv ? sv.pivot : Vector3.zero;
        }

        private void AddPoint()
        {
            EnsureRoot();
            GameObject go = new GameObject("P" + _points.Count);
            Undo.RegisterCreatedObjectUndo(go, "Add point");
            go.transform.SetParent(root, true);
            go.transform.position = NextPosition();
            _points.Add(go.transform);
            Selection.activeGameObject = go;
            SceneView.RepaintAll();
        }

        private void Rename()
        {
            for (int i = 0; i < _points.Count; i++) _points[i].name = "P" + i;
        }

        private void LoadFromShape(NavFloorShape shape)
        {
            SyncFromScene();
            Clean();
            if (_points.Count > 0 && !EditorUtility.DisplayDialog("Nav Floor Builder",
                    "There is already a draft in progress. Replace it?", "Replace", "Cancel"))
                return;

            Undo.IncrementCurrentGroup();
            int undoGroup = Undo.GetCurrentGroup();

            if (root) Undo.DestroyObjectImmediate(root.gameObject);
            root = null;
            _points.Clear();
            EnsureRoot();

            for (int i = 0; i < shape.points.Count; i++)
            {
                GameObject go = new GameObject("P" + i);
                Undo.RegisterCreatedObjectUndo(go, "Restore point");
                go.transform.SetParent(root, true);
                go.transform.position = shape.transform.TransformPoint(shape.points[i]);
                _points.Add(go.transform);
            }
            closed = true;

            tab = Tab.Draw;
            shapeName = shape.gameObject.name;
            layer = shape.gameObject.layer;
            addMeshCollider = shape.GetComponent<MeshCollider>() != null;
            addNavModifier = shape.GetComponent<NavMeshModifier>() != null;
            faceDir = shape.facingUp ? FaceDir.Up : FaceDir.Down;
            customTri = shape.customTriangulation;

            Undo.DestroyObjectImmediate(shape.gameObject);
            Undo.CollapseUndoOperations(undoGroup);

            Selection.activeGameObject = _points.Count > 0 ? _points[_points.Count - 1].gameObject : null;
            Repaint();
            SceneView.RepaintAll();
        }

        private void OnGUI()
        {
            tab = (Tab)GUILayout.Toolbar((int)tab, new[] { "Draw", "Merge" });
            EditorGUILayout.Space();

            if (tab == Tab.Draw) DrawTabGUI();
            else MergeTabGUI();
        }

        private void BuildSettingsGUI()
        {
            EditorGUILayout.LabelField("Build settings", EditorStyles.boldLabel);
            shapeName = EditorGUILayout.TextField("Name", shapeName);
            faceDir = (FaceDir)EditorGUILayout.EnumPopup("Face direction", faceDir);
            flatten = (FlattenMode)EditorGUILayout.EnumPopup("Flatten Y", flatten);
            if (flatten == FlattenMode.Custom) customY = EditorGUILayout.FloatField("Y value", customY);
            layer = EditorGUILayout.LayerField("Layer", layer);
            addMeshCollider = EditorGUILayout.Toggle("Add MeshCollider", addMeshCollider);
            addNavModifier = EditorGUILayout.Toggle("Add NavMeshModifier", addNavModifier);
        }

        private void DrawTabGUI()
        {
            Clean();
            EditorGUILayout.LabelField("Draw", EditorStyles.boldLabel);

            using (new EditorGUILayout.HorizontalScope())
            {
                using (new EditorGUI.DisabledScope(closed))
                {
                    if (GUILayout.Button("Add Point")) AddPoint();
                    using (new EditorGUI.DisabledScope(_points.Count < 2))
                        if (GUILayout.Button("Add Final Point (close)"))
                        {
                            AddPoint();
                            closed = true;
                        }
                }
                using (new EditorGUI.DisabledScope(_points.Count < 3))
                    if (GUILayout.Button(closed ? "Reopen" : "Close Shape"))
                    {
                        closed = !closed;
                        SceneView.RepaintAll();
                    }
            }

            gizmoSize = EditorGUILayout.Slider("Gizmo size", gizmoSize, 0.05f, 1f);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Points (order = connection order)", EditorStyles.boldLabel);

            _scroll = EditorGUILayout.BeginScrollView(_scroll, GUILayout.MinHeight(150));
            int remove = -1, swapA = -1, swapB = -1;
            for (int i = 0; i < _points.Count; i++)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField(i.ToString(), GUILayout.Width(22));
                    EditorGUI.BeginChangeCheck();
                    Vector3 np = EditorGUILayout.Vector3Field(GUIContent.none, _points[i].position);
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(_points[i], "Move point");
                        _points[i].position = np;
                        SceneView.RepaintAll();
                    }
                    if (GUILayout.Button("Sel", GUILayout.Width(34))) Selection.activeTransform = _points[i];
                    if (GUILayout.Button("▲", GUILayout.Width(24)) && i > 0) { swapA = i; swapB = i - 1; }
                    if (GUILayout.Button("▼", GUILayout.Width(24)) && i < _points.Count - 1) { swapA = i; swapB = i + 1; }
                    if (GUILayout.Button("X", GUILayout.Width(24))) remove = i;
                }
            }
            EditorGUILayout.EndScrollView();

            if (swapA >= 0)
            {
                Undo.RegisterFullObjectHierarchyUndo(root.gameObject, "Reorder points");
                (_points[swapA], _points[swapB]) = (_points[swapB], _points[swapA]);
                for (int i = 0; i < _points.Count; i++) _points[i].SetSiblingIndex(i);
                Rename();
                SceneView.RepaintAll();
            }
            if (remove >= 0)
            {
                Undo.RegisterFullObjectHierarchyUndo(root.gameObject, "Remove point");
                Undo.DestroyObjectImmediate(_points[remove].gameObject);
                _points.RemoveAt(remove);
                if (_points.Count < 3) closed = false;
                Rename();
                SceneView.RepaintAll();
            }

            EditorGUILayout.Space();
            BuildSettingsGUI();

            EditorGUILayout.Space();
            using (new EditorGUILayout.HorizontalScope())
            {
                using (new EditorGUI.DisabledScope(_points.Count < 3))
                    if (GUILayout.Button("BUILD", GUILayout.Height(30))) Build();
                if (GUILayout.Button("Clear Draft", GUILayout.Height(30))) ClearDraft();
            }
        }

        private static NavFloorShape SelectedShape()
        {
            GameObject g = Selection.activeGameObject;
            return g ? g.GetComponent<NavFloorShape>() : null;
        }

        private void MergeTabGUI()
        {
            EditorGUILayout.LabelField("Merge two built shapes", EditorStyles.boldLabel);

            using (new EditorGUILayout.HorizontalScope())
            {
                shapeA = (NavFloorShape)EditorGUILayout.ObjectField("Shape A", shapeA, typeof(NavFloorShape), true);
                if (GUILayout.Button("Use Selected", GUILayout.Width(95)))
                {
                    NavFloorShape s = SelectedShape();
                    if (s) shapeA = s;
                }
            }
            using (new EditorGUILayout.HorizontalScope())
            {
                shapeB = (NavFloorShape)EditorGUILayout.ObjectField("Shape B", shapeB, typeof(NavFloorShape), true);
                if (GUILayout.Button("Use Selected", GUILayout.Width(95)))
                {
                    NavFloorShape s = SelectedShape();
                    if (s) shapeB = s;
                }
            }

            int countA = shapeA ? shapeA.points.Count : 0;
            int countB = shapeB ? shapeB.points.Count : 0;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Weld pairs (A index = B index)", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("2 pairs per seam: pairs 1-2 are seam 1, pairs 3-4 are seam 2, and so on. " +
                                    "Each seam's two pairs are the ends of one shared border; the border points " +
                                    "between them are removed. Two seams make a ring (outer outline + hole).",
                MessageType.Info);

            int removePair = -1;
            for (int i = 0; i < pairs.Count; i++)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField("S" + (i / 2 + 1), GUILayout.Width(24));
                    EditorGUILayout.LabelField("A", GUILayout.Width(14));
                    pairs[i].a = EditorGUILayout.IntField(pairs[i].a, GUILayout.Width(50));
                    EditorGUILayout.LabelField("(0-" + Mathf.Max(0, countA - 1) + ")", GUILayout.Width(55));
                    EditorGUILayout.LabelField("B", GUILayout.Width(14));
                    pairs[i].b = EditorGUILayout.IntField(pairs[i].b, GUILayout.Width(50));
                    EditorGUILayout.LabelField("(0-" + Mathf.Max(0, countB - 1) + ")", GUILayout.Width(55));
                    if (GUILayout.Button("X", GUILayout.Width(24))) removePair = i;
                }
            }
            if (removePair >= 0) pairs.RemoveAt(removePair);

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Add Pair")) pairs.Add(new MergePair());
                if (GUILayout.Button("Add Seam (2 pairs)"))
                {
                    pairs.Add(new MergePair());
                    pairs.Add(new MergePair());
                }
            }

            EditorGUILayout.Space();
            weldMode = (WeldMode)EditorGUILayout.EnumPopup("Weld position", weldMode);
            holeMode = (HoleMode)EditorGUILayout.EnumPopup("Hole handling", holeMode);
            flipA = EditorGUILayout.Toggle("Flip kept side of A", flipA);
            flipB = EditorGUILayout.Toggle("Flip kept side of B", flipB);

            EditorGUILayout.Space();
            BuildSettingsGUI();

            EditorGUILayout.Space();
            bool ok = ComputeMerge(out List<List<Vector3>> loops, out string err);
            List<Vector3> poly = null;
            bool bridged = false;
            if (ok) ok = BuildPolygon(loops, out poly, out bridged, out err);

            if (!ok) EditorGUILayout.HelpBox(err, MessageType.Warning);
            else EditorGUILayout.HelpBox("Result: " + poly.Count + " points, " + (loops.Count - 1) + " hole(s)" +
                                         (loops.Count > 1 ? (bridged ? " (bridged)" : " (filled)") : "") +
                                         ". Yellow = outer outline, orange = hole.", MessageType.None);

            using (new EditorGUI.DisabledScope(!ok))
                if (GUILayout.Button("MERGE", GUILayout.Height(30))) Merge();

            SceneView.RepaintAll();
        }

        private static List<int> Walk(int n, int from, int to, int step)
        {
            List<int> l = new List<int> { from };
            int i = from, guard = 0;
            while (i != to && guard++ < n)
            {
                i = ((i + step) % n + n) % n;
                l.Add(i);
            }
            return l;
        }

        private static List<Vector3> Pts(List<Vector3> P, List<int> idx)
        {
            List<Vector3> l = new List<Vector3>();
            foreach (int i in idx) l.Add(P[i]);
            return l;
        }

        private static float PointSeg(Vector3 p, Vector3 a, Vector3 b)
        {
            Vector3 ab = b - a;
            float sq = ab.sqrMagnitude;
            float t = sq < 1e-9f ? 0f : Mathf.Clamp01(Vector3.Dot(p - a, ab) / sq);
            return Vector3.Distance(p, a + ab * t);
        }

        private static float PolyDist(Vector3 p, List<Vector3> poly)
        {
            if (poly.Count == 1) return Vector3.Distance(p, poly[0]);
            float best = float.MaxValue;
            for (int i = 0; i < poly.Count - 1; i++)
                best = Mathf.Min(best, PointSeg(p, poly[i], poly[i + 1]));
            return best;
        }

        private static float ChainScore(List<Vector3> a, List<Vector3> b)
        {
            float s = 0;
            foreach (Vector3 p in a) s += PolyDist(p, b);
            foreach (Vector3 q in b) s += PolyDist(q, a);
            return s / (a.Count + b.Count);
        }

        private static void MarkRemoved(bool[] rem, int n, List<int> chain)
        {
            for (int k = 0; k < chain.Count - 1; k++)
            {
                int u = chain[k], w = chain[k + 1];
                if ((u + 1) % n == w) rem[u] = true;
                else rem[w] = true;
            }
        }

        private Vector3 Weld(Vector3 a, Vector3 b)
        {
            switch (weldMode)
            {
                case WeldMode.UseA: return a;
                case WeldMode.UseB: return b;
                default: return (a + b) * 0.5f;
            }
        }

        private static List<Vector3> WorldPoints(NavFloorShape s)
        {
            List<Vector3> l = new List<Vector3>();
            foreach (Vector3 p in s.points) l.Add(s.transform.TransformPoint(p));
            return l;
        }

        private static List<Arc> BuildArcs(List<Vector3> P, bool[] rem, Dictionary<int, int> weld,
            Vector3[] weldPos, out string err)
        {
            err = null;
            List<Arc> arcs = new List<Arc>();
            int n = P.Count;

            foreach (KeyValuePair<int, int> kv in weld)
            {
                int v = kv.Key, pid = kv.Value;
                bool fwd = !rem[v];
                bool back = !rem[(v - 1 + n) % n];
                if (fwd == back)
                {
                    err = "Point " + v + " must be an end of the shared border. Check the pairs or use Flip.";
                    return null;
                }
                if (!fwd) continue;

                Arc arc = new Arc { startW = pid, pts = new List<Vector3> { weldPos[pid] } };
                int i = (v + 1) % n;
                int guard = 0;
                bool ended = false;
                while (guard++ <= n)
                {
                    if (weld.TryGetValue(i, out int pid2))
                    {
                        arc.pts.Add(weldPos[pid2]);
                        arc.endW = pid2;
                        ended = true;
                        break;
                    }
                    arc.pts.Add(P[i]);
                    i = (i + 1) % n;
                }
                if (!ended)
                {
                    err = "Could not trace the outline of a shape.";
                    return null;
                }
                arcs.Add(arc);
            }
            return arcs;
        }

        private bool ComputeMerge(out List<List<Vector3>> loops, out string err)
        {
            loops = new List<List<Vector3>>();
            err = null;

            if (!shapeA || !shapeB) { err = "Pick both shapes."; return false; }
            if (shapeA == shapeB) { err = "A and B must be different shapes."; return false; }
            if (pairs.Count < 2 || pairs.Count % 2 != 0) { err = "Use pairs in twos: 2 pairs per seam."; return false; }

            List<Vector3> A = WorldPoints(shapeA);
            List<Vector3> B = WorldPoints(shapeB);
            int nA = A.Count, nB = B.Count;
            if (nA < 3 || nB < 3) { err = "Both shapes need at least 3 points (rebuild old shapes)."; return false; }

            HashSet<int> usedA = new HashSet<int>();
            HashSet<int> usedB = new HashSet<int>();
            foreach (MergePair p in pairs)
            {
                if (p.a < 0 || p.a >= nA) { err = "A index " + p.a + " out of range."; return false; }
                if (p.b < 0 || p.b >= nB) { err = "B index " + p.b + " out of range."; return false; }
                if (!usedA.Add(p.a) || !usedB.Add(p.b)) { err = "Each point can only be in one pair."; return false; }
            }

            bool[] remA = new bool[nA];
            bool[] remB = new bool[nB];

            for (int s = 0; s < pairs.Count / 2; s++)
            {
                int a1 = pairs[2 * s].a, a2 = pairs[2 * s + 1].a;
                int b1 = pairs[2 * s].b, b2 = pairs[2 * s + 1].b;

                List<int> fA = Walk(nA, a1, a2, 1), bA = Walk(nA, a1, a2, -1);
                List<int> fB = Walk(nB, b1, b2, 1), bB = Walk(nB, b1, b2, -1);

                float best = float.MaxValue;
                List<int> ca = fA, cb = fB;
                foreach (List<int> xa in new[] { fA, bA })
                foreach (List<int> xb in new[] { fB, bB })
                {
                    float sc = ChainScore(Pts(A, xa), Pts(B, xb));
                    if (sc < best) { best = sc; ca = xa; cb = xb; }
                }

                if (flipA) ca = ReferenceEquals(ca, fA) ? bA : fA;
                if (flipB) cb = ReferenceEquals(cb, fB) ? bB : fB;

                MarkRemoved(remA, nA, ca);
                MarkRemoved(remB, nB, cb);
            }

            Dictionary<int, int> weldA = new Dictionary<int, int>();
            Dictionary<int, int> weldB = new Dictionary<int, int>();
            Vector3[] weldPos = new Vector3[pairs.Count];
            for (int p = 0; p < pairs.Count; p++)
            {
                weldA[pairs[p].a] = p;
                weldB[pairs[p].b] = p;
                weldPos[p] = Weld(A[pairs[p].a], B[pairs[p].b]);
            }

            List<Arc> arcsA = BuildArcs(A, remA, weldA, weldPos, out err);
            if (arcsA == null) return false;
            List<Arc> arcsB = BuildArcs(B, remB, weldB, weldPos, out err);
            if (arcsB == null) return false;

            Dictionary<int, Arc> aAt = new Dictionary<int, Arc>();
            Dictionary<int, Arc> bAt = new Dictionary<int, Arc>();
            foreach (Arc a in arcsA) { aAt[a.startW] = a; aAt[a.endW] = a; }
            foreach (Arc b in arcsB) { bAt[b.startW] = b; bAt[b.endW] = b; }

            foreach (Arc a0 in arcsA)
            {
                if (a0.used) continue;

                List<Vector3> loop = new List<Vector3>();
                Arc cur = a0;
                bool curIsA = true;
                int entryW = a0.startW;
                int startW = a0.startW;
                int guard = 0;

                while (true)
                {
                    if (guard++ > 1000) { err = "Outline did not close."; return false; }

                    cur.used = true;
                    bool fwd = cur.startW == entryW;
                    List<Vector3> seq = new List<Vector3>(cur.pts);
                    if (!fwd) seq.Reverse();
                    int exitW = fwd ? cur.endW : cur.startW;

                    for (int i = 0; i < seq.Count - 1; i++) loop.Add(seq[i]);

                    if (!curIsA && exitW == startW) break;

                    Dictionary<int, Arc> other = curIsA ? bAt : aAt;
                    if (!other.TryGetValue(exitW, out Arc next) || next.used)
                    {
                        err = "The seams do not form closed outlines. Check the pair order or use Flip.";
                        return false;
                    }
                    cur = next;
                    curIsA = !curIsA;
                    entryW = exitW;
                }

                if (loop.Count < 3) { err = "An outline has fewer than 3 points."; return false; }
                loops.Add(loop);
            }

            if (loops.Count == 0) { err = "No outline produced."; return false; }
            return true;
        }

        private static float AreaXZ(List<Vector3> p)
        {
            float a = 0;
            for (int i = 0; i < p.Count; i++)
            {
                Vector3 c = p[i], n = p[(i + 1) % p.Count];
                a += c.x * n.z - n.x * c.z;
            }
            return a * 0.5f;
        }

        private static Vector2 XZ(Vector3 v) => new Vector2(v.x, v.z);

        private static float Orient(Vector2 a, Vector2 b, Vector2 c)
            => (b.x - a.x) * (c.y - a.y) - (b.y - a.y) * (c.x - a.x);

        private static bool SegCross(Vector2 a, Vector2 b, Vector2 c, Vector2 d)
        {
            float o1 = Orient(a, b, c), o2 = Orient(a, b, d);
            float o3 = Orient(c, d, a), o4 = Orient(c, d, b);
            return o1 * o2 < 0f && o3 * o4 < 0f;
        }

        private static bool InsidePoly(List<Vector3> poly, Vector2 p)
        {
            bool inside = false;
            for (int i = 0, j = poly.Count - 1; i < poly.Count; j = i++)
            {
                Vector2 a = XZ(poly[i]), b = XZ(poly[j]);
                if ((a.y > p.y) != (b.y > p.y) && p.x < (b.x - a.x) * (p.y - a.y) / (b.y - a.y) + a.x)
                    inside = !inside;
            }
            return inside;
        }

        private static bool EdgesClear(List<Vector3> poly, int skipVertex, Vector2 p, Vector2 q)
        {
            int n = poly.Count;
            for (int u = 0; u < n; u++)
            {
                int w = (u + 1) % n;
                if (u == skipVertex || w == skipVertex) continue;
                if (SegCross(p, q, XZ(poly[u]), XZ(poly[w]))) return false;
            }
            return true;
        }

        private bool BuildPolygon(List<List<Vector3>> loops, out List<Vector3> poly, out bool bridged, out string err)
        {
            poly = null;
            bridged = false;
            err = null;

            if (loops.Count == 1)
            {
                poly = new List<Vector3>(loops[0]);
                return true;
            }

            int oi = 0;
            float oa = 0;
            for (int i = 0; i < loops.Count; i++)
            {
                float a = Mathf.Abs(AreaXZ(loops[i]));
                if (a > oa) { oa = a; oi = i; }
            }

            List<Vector3> outer = new List<Vector3>(loops[oi]);
            if (AreaXZ(outer) < 0) outer.Reverse();

            List<List<Vector3>> holes = new List<List<Vector3>>();
            for (int i = 0; i < loops.Count; i++)
            {
                if (i == oi) continue;
                List<Vector3> h = new List<Vector3>(loops[i]);
                if (AreaXZ(h) > 0) h.Reverse();
                holes.Add(h);
            }

            if (holeMode == HoleMode.Fill)
            {
                poly = outer;
                return true;
            }

            for (int h = 0; h < holes.Count; h++)
            {
                List<Vector3> hole = holes[h];
                List<(float d, int i, int j)> cand = new List<(float d, int i, int j)>();
                for (int i = 0; i < outer.Count; i++)
                for (int j = 0; j < hole.Count; j++)
                    cand.Add((Vector2.Distance(XZ(outer[i]), XZ(hole[j])), i, j));
                cand.Sort((x, y) => x.d.CompareTo(y.d));

                int bi = -1, bj = -1;
                foreach ((float d, int i, int j) c in cand)
                {
                    Vector2 p = XZ(outer[c.i]), q = XZ(hole[c.j]);
                    Vector2 mid = (p + q) * 0.5f;

                    if (!InsidePoly(outer, mid) || InsidePoly(hole, mid)) continue;
                    if (!EdgesClear(outer, c.i, p, q)) continue;
                    if (!EdgesClear(hole, c.j, p, q)) continue;

                    bool ok = true;
                    for (int k = 0; k < holes.Count && ok; k++)
                        if (k != h && !EdgesClear(holes[k], -1, p, q)) ok = false;
                    if (!ok) continue;

                    bi = c.i;
                    bj = c.j;
                    break;
                }

                if (bi < 0)
                {
                    err = "Could not connect a hole to the outer outline. Try Hole handling: Fill.";
                    return false;
                }

                List<Vector3> merged = new List<Vector3>();
                for (int i = 0; i <= bi; i++) merged.Add(outer[i]);
                for (int k = 0; k <= hole.Count; k++) merged.Add(hole[(bj + k) % hole.Count]);
                for (int i = bi; i < outer.Count; i++) merged.Add(outer[i]);
                outer = merged;
                bridged = true;
            }

            poly = outer;
            return true;
        }

        private void Merge()
        {
            if (!ComputeMerge(out List<List<Vector3>> loops, out string err) ||
                !BuildPolygon(loops, out List<Vector3> pos, out bool bridged, out err))
            {
                EditorUtility.DisplayDialog("Nav Floor Builder", err, "OK");
                return;
            }

            Undo.IncrementCurrentGroup();
            int undoGroup = Undo.GetCurrentGroup();

            List<Vector3> la = WorldPoints(shapeA), lb = WorldPoints(shapeB);
            float aMin = float.MaxValue, aMax = float.MinValue, bMin = float.MaxValue, bMax = float.MinValue;
            foreach (Vector3 v in la) { aMin = Mathf.Min(aMin, v.y); aMax = Mathf.Max(aMax, v.y); }
            foreach (Vector3 v in lb) { bMin = Mathf.Min(bMin, v.y); bMax = Mathf.Max(bMax, v.y); }
            Debug.Log("[NavFloor] A Y " + aMin.ToString("0.###") + ".." + aMax.ToString("0.###") +
                      " | B Y " + bMin.ToString("0.###") + ".." + bMax.ToString("0.###"));
            ApplyFlatten(pos);
            GameObject go = CreateShapeObject(pos, bridged);
            if (!go) return;

            GameObject ga = shapeA.gameObject;
            GameObject gb = shapeB.gameObject;
            shapeA = null;
            shapeB = null;
            Undo.DestroyObjectImmediate(ga);
            Undo.DestroyObjectImmediate(gb);

            Selection.activeGameObject = go;
            Undo.CollapseUndoOperations(undoGroup);
            SceneView.RepaintAll();
        }

        private void ClearDraft()
        {
            if (root) Undo.DestroyObjectImmediate(root.gameObject);
            root = null;
            _points.Clear();
            customTri = false;
            SceneView.RepaintAll();
        }

        private void ApplyFlatten(List<Vector3> pos)
        {
            if (flatten == FlattenMode.None) return;

            float y = customY;
            if (flatten == FlattenMode.Average)
            {
                y = 0;
                foreach (Vector3 v in pos) y += v.y;
                y /= pos.Count;
            }
            for (int i = 0; i < pos.Count; i++) pos[i] = new Vector3(pos[i].x, y, pos[i].z);
        }

        private static float CrossXZ(Vector3 a, Vector3 b, Vector3 p)
            => (b.x - a.x) * (p.z - a.z) - (b.z - a.z) * (p.x - a.x);

        private static bool SamePos(Vector3 a, Vector3 b)
            => Mathf.Abs(a.x - b.x) < 1e-5f && Mathf.Abs(a.z - b.z) < 1e-5f;

        private static bool EarClip(List<Vector3> v, out List<int> tris)
        {
            tris = new List<int>();
            int n = v.Count;
            List<int> idx = new List<int>();
            for (int i = 0; i < n; i++) idx.Add(i);

            bool ccw = AreaXZ(v) > 0;
            int guard = 0;

            while (idx.Count > 3 && guard++ < n * n + 10)
            {
                bool found = false;

                for (int pass = 0; pass < 2 && !found; pass++)
                {
                    for (int i = 0; i < idx.Count; i++)
                    {
                        int i0 = idx[(i + idx.Count - 1) % idx.Count];
                        int i1 = idx[i];
                        int i2 = idx[(i + 1) % idx.Count];
                        float cr = CrossXZ(v[i0], v[i1], v[i2]);

                        if (pass == 1)
                        {
                            if (Mathf.Abs(cr) > 1e-7f) continue;
                            idx.RemoveAt(i);
                            found = true;
                            break;
                        }

                        if (ccw ? cr <= 1e-7f : cr >= -1e-7f) continue;

                        bool inside = false;
                        foreach (int k in idx)
                        {
                            if (k == i0 || k == i1 || k == i2) continue;
                            if (SamePos(v[k], v[i0]) || SamePos(v[k], v[i1]) || SamePos(v[k], v[i2])) continue;

                            float s1 = CrossXZ(v[i0], v[i1], v[k]);
                            float s2 = CrossXZ(v[i1], v[i2], v[k]);
                            float s3 = CrossXZ(v[i2], v[i0], v[k]);
                            if ((s1 > 1e-7f && s2 > 1e-7f && s3 > 1e-7f) ||
                                (s1 < -1e-7f && s2 < -1e-7f && s3 < -1e-7f))
                            {
                                inside = true;
                                break;
                            }
                        }
                        if (inside) continue;

                        tris.Add(i0);
                        tris.Add(i1);
                        tris.Add(i2);
                        idx.RemoveAt(i);
                        found = true;
                        break;
                    }
                }

                if (!found) return false;
            }

            if (idx.Count == 3)
            {
                tris.Add(idx[0]);
                tris.Add(idx[1]);
                tris.Add(idx[2]);
            }
            return tris.Count > 0;
        }

        private GameObject CreateShapeObject(List<Vector3> pos, bool custom)
        {
            float minY = float.MaxValue, maxY = float.MinValue;
            foreach (Vector3 p in pos)
            {
                minY = Mathf.Min(minY, p.y);
                maxY = Mathf.Max(maxY, p.y);
            }
            if (maxY - minY > 0.001f) custom = true;
            Debug.Log("[NavFloor] input: " + pos.Count + " points, Y " + minY.ToString("0.###") + " to " +
                      maxY.ToString("0.###") + ", custom=" + custom);
            
            ProBuilderMesh pb;

            if (custom)
            {
                if (!EarClip(pos, out List<int> tris))
                {
                    Debug.LogError("Custom triangulation failed. Try Hole handling: Fill.");
                    return null;
                }

                List<Face> faces = new List<Face>();
                for (int t = 0; t < tris.Count; t += 3)
                    faces.Add(new Face(new[] { tris[t], tris[t + 1], tris[t + 2] }));

                pb = ProBuilderMesh.Create(pos, faces);
                Undo.RegisterCreatedObjectUndo(pb.gameObject, "Build nav floor");
            }
            else
            {
                pb = ProBuilderMesh.Create();
                Undo.RegisterCreatedObjectUndo(pb.gameObject, "Build nav floor");

                try
                {
                    ActionResult result = pb.CreateShapeFromPolygon(pos, 0f, false);
                    if (result.status != ActionResult.Status.Success)
                    {
                        Debug.LogError("CreateShapeFromPolygon failed: " + result.status + " " + result.notification);
                        DestroyImmediate(pb.gameObject);
                        return null;
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError("CreateShapeFromPolygon threw: " + e.Message);
                    DestroyImmediate(pb.gameObject);
                    return null;
                }
            }

            pb.ToMesh();
            pb.Refresh();

            Mesh mesh = pb.GetComponent<MeshFilter>().sharedMesh;
            
            float mn = float.MaxValue, mx = float.MinValue;
            foreach (Vector3 vv in mesh.vertices)
            {
                mn = Mathf.Min(mn, vv.y);
                mx = Mathf.Max(mx, vv.y);
            }
            Debug.Log("[NavFloor] built mesh Y " + mn.ToString("0.###") + " to " + mx.ToString("0.###"));
            
            bool facesUp = mesh.normals.Length > 0 && mesh.normals[0].y > 0;
            bool wantUp = faceDir == FaceDir.Up;
            bool needFlip = facesUp != wantUp;
            if (needFlip)
            {
                foreach (Face f in pb.faces) f.Reverse();
                pb.ToMesh();
                pb.Refresh();
            }

            pb.gameObject.name = shapeName;
            pb.gameObject.layer = layer;
            pb.GetComponent<MeshRenderer>().sharedMaterial = BuiltinMaterials.defaultMaterial;

            if (!custom)
            {
                PolyShape ps = pb.gameObject.AddComponent<PolyShape>();
                ps.SetControlPoints(pos);
                ps.extrude = 0f;
                ps.flipNormals = needFlip;
            }

            NavFloorShape marker = pb.gameObject.AddComponent<NavFloorShape>();
            marker.points = new List<Vector3>(pos);
            marker.facingUp = wantUp;
            marker.customTriangulation = custom;

            if (addMeshCollider)
            {
                MeshCollider mc = pb.GetComponent<MeshCollider>();
                if (!mc) mc = pb.gameObject.AddComponent<MeshCollider>();
                mc.sharedMesh = pb.GetComponent<MeshFilter>().sharedMesh;
            }

            if (addNavModifier && !pb.GetComponent<NavMeshModifier>())
                pb.gameObject.AddComponent<NavMeshModifier>();
            return pb.gameObject;
        }

        private void Build()
        {
            Clean();
            if (_points.Count < 3) return;

            Undo.IncrementCurrentGroup();
            int undoGroup = Undo.GetCurrentGroup();

            List<Vector3> pos = new List<Vector3>();
            foreach (Transform p in _points) pos.Add(p.position);
            ApplyFlatten(pos);

            GameObject go = CreateShapeObject(pos, customTri);
            if (!go) return;

            customTri = false;
            Selection.activeGameObject = go;

            if (root) Undo.DestroyObjectImmediate(root.gameObject);
            root = null;
            _points.Clear();
            Undo.CollapseUndoOperations(undoGroup);
            SceneView.RepaintAll();
        }

        private void DrawMergePreview()
        {
            if (!shapeA || !shapeB) return;

            List<Vector3> A = WorldPoints(shapeA);
            List<Vector3> B = WorldPoints(shapeB);

            Handles.color = Color.white;
            foreach (MergePair p in pairs)
                if (p.a >= 0 && p.a < A.Count && p.b >= 0 && p.b < B.Count)
                    Handles.DrawAAPolyLine(3f, A[p.a], B[p.b]);

            if (!ComputeMerge(out List<List<Vector3>> loops, out _)) return;

            int oi = 0;
            float oa = 0;
            for (int i = 0; i < loops.Count; i++)
            {
                float a = Mathf.Abs(AreaXZ(loops[i]));
                if (a > oa) { oa = a; oi = i; }
            }

            for (int i = 0; i < loops.Count; i++)
            {
                List<Vector3> r = loops[i];
                Vector3[] arr = new Vector3[r.Count + 1];
                r.CopyTo(arr);
                arr[r.Count] = r[0];
                Handles.color = i == oi ? Color.yellow : new Color(1f, 0.5f, 0f);
                Handles.DrawAAPolyLine(7f, arr);
            }
        }

        private void OnSceneGUI(SceneView sv)
        {
            if (tab == Tab.Merge) DrawMergePreview();

            Clean();
            if (_points.Count == 0) return;

            for (int i = 0; i < _points.Count - 1; i++)
            {
                Handles.color = LineCol;
                Handles.DrawAAPolyLine(4f, _points[i].position, _points[i + 1].position);
            }
            if (closed && _points.Count > 2)
            {
                Handles.color = CloseCol;
                Handles.DrawAAPolyLine(4f, _points[_points.Count - 1].position, _points[0].position);
            }

            for (int i = 0; i < _points.Count; i++)
            {
                Vector3 p = _points[i].position;
                float s = UnityEditor.HandleUtility.GetHandleSize(p) * gizmoSize;
                Handles.color = i == 0 ? Color.green : (closed && i == _points.Count - 1 ? Color.red : PointCol);
                if (Handles.Button(p, Quaternion.identity, s, s, Handles.SphereHandleCap))
                    Selection.activeTransform = _points[i];
                Handles.Label(p + Vector3.up * s * 1.5f, i.ToString());
            }
        }
    }
}