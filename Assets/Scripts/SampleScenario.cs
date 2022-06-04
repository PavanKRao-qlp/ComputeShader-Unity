#define Debug
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleScenario : MonoBehaviour
{
    [SerializeField] ComputeShader CollisionComputeShader;
    [SerializeField] GPU_Collision.GPUCollider ColliderObjectPrefab;
    [SerializeField] int NoOfObjects;
    [SerializeField] bool RunOnGpu;

    private int kernelIdCollisionComputeShader;
    private ComputeBuffer colliderBuffer;
    private ComputeBuffer collisionBuffer;
    private GPU_Collision.ColliderData[] colliderdata;
    private GPU_Collision.CollisionData[] collisionData;
    private GPU_Collision.GPUCollider[] colliderObjects;
    private int noOfThreadGroups = 0;
    private void Start()
    {
        kernelIdCollisionComputeShader = CollisionComputeShader.FindKernel("CheckCollision");
        colliderBuffer = new ComputeBuffer(NoOfObjects, sizeof(float) * 4);
        collisionBuffer = new ComputeBuffer(NoOfObjects, sizeof(float) * 2);
        CollisionComputeShader.SetBuffer(kernelIdCollisionComputeShader, "Colliders", colliderBuffer);
        CollisionComputeShader.SetBuffer(kernelIdCollisionComputeShader, "Collisions", collisionBuffer);
        CollisionComputeShader.SetFloat("CollidersLenght", NoOfObjects);
        noOfThreadGroups = Mathf.CeilToInt(NoOfObjects / 32f);
        colliderdata = new GPU_Collision.ColliderData[NoOfObjects];
        collisionData = new GPU_Collision.CollisionData[NoOfObjects];
        colliderObjects = new GPU_Collision.GPUCollider[NoOfObjects];

        for (int i = 0; i < NoOfObjects; i++)
        {
            var collider = GameObject.Instantiate(ColliderObjectPrefab, Random.insideUnitSphere * 100, Quaternion.identity);
            collider.gameObject.name = "Collider_" + i;
            colliderObjects[i] = collider;
            colliderdata[i] = collider.ColliderData;
        }
    }

    private void Update()
    {
        for (int i = 0; i < NoOfObjects; i++)
        {
            colliderdata[i] = colliderObjects[i].ColliderData;
        }

        if (RunOnGpu)
        {
            colliderBuffer.SetData(colliderdata);
            CollisionComputeShader.SetBuffer(kernelIdCollisionComputeShader, "Colliders", colliderBuffer);
            CollisionComputeShader.Dispatch(kernelIdCollisionComputeShader, noOfThreadGroups, 1, 1);
            collisionBuffer.GetData(collisionData);
        }
        else
        {
            for (int i = 0; i < colliderdata.Length; i++)
            {
                collisionData[i].IsCollided = 0;
                collisionData[i].CollisionIx = -1;
                GPU_Collision.ColliderData colliderA = colliderdata[i];
                for (int j = 0; j < colliderdata.Length; j++)
                {
                    if (i == j) continue;
                    GPU_Collision.ColliderData colliderb = colliderdata[j];
                    var distance = (colliderb.Position - colliderA.Position).magnitude;
                    if(distance <= colliderA.Range + colliderb.Range)
                    {
                        collisionData[i].IsCollided = 1;
                        collisionData[i].CollisionIx = j;
                    }
                }

            }
        }

        for (int i = 0; i < collisionData.Length; i++)
        {
            if (collisionData[i].IsCollided >= 1 && collisionData[i].CollisionIx > i)
            {
#if Debug
                colliderObjects[i].GetComponent<MeshRenderer>().material.color = Color.magenta;
                colliderObjects[i].GetComponent<MeshRenderer>().material.color = Color.magenta;
#endif
                colliderObjects[i].OnCollided(colliderObjects[(int)collisionData[i].CollisionIx]);
                colliderObjects[(int)collisionData[i].CollisionIx].OnCollided(colliderObjects[i]);
            }
            else colliderObjects[i].GetComponent<MeshRenderer>().material.color = Color.white;
        }
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;
        for (int i = 0; i < collisionData.Length; i++)
        {
            if (collisionData[i].IsCollided >= 1)
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawSphere(colliderdata[i].Position, colliderdata[i].Range);
                Gizmos.DrawSphere(colliderdata[(int)collisionData[i].CollisionIx].Position, colliderdata[(int)collisionData[i].CollisionIx].Range);
            }
        }
    }

    private void OnDestroy()
    {
        colliderBuffer.Dispose(); 
        colliderBuffer.Release();
        collisionBuffer.Dispose();
        collisionBuffer.Release();
    }
}
