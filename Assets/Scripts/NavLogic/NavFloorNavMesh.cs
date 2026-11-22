using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace NavLogic
{
    [ExecuteAlways]
    [DefaultExecutionOrder(-102)]
    public class NavFloorNavMesh : MonoBehaviour
    {
        public NavMeshData data;
        public List<NavMeshData> datas = new List<NavMeshData>();

        private readonly List<NavMeshDataInstance> _instances = new List<NavMeshDataInstance>();

        private void OnEnable() => AddAll();

        private void OnDisable() => RemoveAll();

        public void Reload()
        {
            RemoveAll();
            AddAll();
        }

        private void AddAll()
        {
            if (data) _instances.Add(NavMesh.AddNavMeshData(data));
            foreach (NavMeshData d in datas)
                if (d) _instances.Add(NavMesh.AddNavMeshData(d));
        }

        private void RemoveAll()
        {
            foreach (NavMeshDataInstance i in _instances)
                if (i.valid) i.Remove();
            _instances.Clear();
        }
    }
}