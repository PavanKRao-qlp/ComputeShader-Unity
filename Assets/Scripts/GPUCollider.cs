using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GPU_Collision {
    public class GPUCollider : MonoBehaviour
    {
        [SerializeField]  
        public ColliderData ColliderData;

        public void Update()
        {
            ColliderData.Position = transform.position;
        }

        public void OnCollided(GPUCollider other) {

        }

      

    }
}