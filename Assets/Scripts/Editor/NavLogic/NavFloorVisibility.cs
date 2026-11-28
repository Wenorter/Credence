using NavLogic;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public static class NavFloorVisibility
    {
        [MenuItem("Tools/NavLogic/Floor Renderers/Hide Floor Renderers")]
        private static void Hide() => Set(false);

        [MenuItem("Tools/NavLogic/Floor Renderers/Show Floor Renderers")]
        private static void Show() => Set(true);

        private static void Set(bool visible)
        {
            foreach (NavFloorShape s in Object.FindObjectsByType<NavFloorShape>(FindObjectsSortMode.None))
            {
                MeshRenderer r = s.GetComponent<MeshRenderer>();
                if (!r) continue;
                Undo.RecordObject(r, visible ? "Show floor" : "Hide floor");
                r.enabled = visible;
            }
        }
    }
}