using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleScenario : MonoBehaviour
{
    [SerializeField] ComputeShader CollisionComputeShader;
    [SerializeField] MeshRenderer GameObjectA, GameObjectB;
    private int kernelIdCollisionComputeShader;
    private ComputeBuffer colliderBuffer;
    private ComputeBuffer collisionBuffer;
    private GPU_Collision.ColliderData[] data;
    private float[] collisionData;
    private void Start()
    {
        kernelIdCollisionComputeShader = CollisionComputeShader.FindKernel("CheckCollision");
        colliderBuffer = new ComputeBuffer(2, sizeof(float)*4);
        collisionBuffer = new ComputeBuffer(1, 4);
        CollisionComputeShader.SetBuffer(kernelIdCollisionComputeShader, "Colliders", colliderBuffer);
        CollisionComputeShader.SetBuffer(kernelIdCollisionComputeShader, "Collided", collisionBuffer);
        data = new GPU_Collision.ColliderData[2];
        collisionData = new float[1];
}

    private void Update()
    {
        data[0].Position = GameObjectA.transform.position;
        data[0].Range = 0.5f;
        data[1].Position = GameObjectB.transform.position;
        data[1].Range = 0.5f;

        colliderBuffer.SetData(data);
        CollisionComputeShader.SetBuffer(kernelIdCollisionComputeShader, "Colliders", colliderBuffer);
        CollisionComputeShader.Dispatch(kernelIdCollisionComputeShader, 1, 1, 1);
        collisionBuffer.GetData(collisionData);
        GameObjectA.material.color = collisionData[0] == 1 ? Color.red : Color.white;
        GameObjectB.material.color = collisionData[0] == 1 ? Color.red : Color.white;

    }

    private void OnDestroy()
    {
        colliderBuffer.Dispose(); 
        colliderBuffer.Release();
        collisionBuffer.Dispose();
        collisionBuffer.Release();
    }
}
