using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveRandom : MonoBehaviour
{
    private Vector3 dir;
    [SerializeField] private int range = 100;
    private void Start()
    {
        dir = Random.insideUnitSphere;
    }

    private void FixedUpdate()
    {
        this.transform.position += dir * Time.deltaTime * 10;
        if (Mathf.Abs(transform.position.x) > range || Mathf.Abs(transform.position.y) > range || Mathf.Abs(transform.position.z) > range)
        {
            dir *= -1;
        }
    }
}
