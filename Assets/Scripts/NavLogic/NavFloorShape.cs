using System.Collections.Generic;
using UnityEngine;

namespace NavLogic
{
    [DisallowMultipleComponent]
    public class NavFloorShape : MonoBehaviour
    {
        public List<Vector3> points = new List<Vector3>();
        public bool facingUp = true;
        public bool showGizmos = true;
        public bool customTriangulation;
    }
}