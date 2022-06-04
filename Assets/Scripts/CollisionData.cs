using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GPU_Collision
{
    [System.Serializable]
    public struct ColliderData
    {
        public Vector3 Position;
        public float Range;
    }

    public struct CollisionData
    {
        public float IsCollided;
        public float CollisionIx;
    }
}