using UnityEngine;
using System.Collections;
using System;

public class BulletController : MonoBehaviour
{
    public float Speed = 1.0f;

    void Start()
    {

    }

    void Update()
    {
        transform.Translate(Vector3.right * Speed);
    }

    public void Initialize(Vector3 position, Quaternion rotation)
    {
        transform.position = position;
        transform.rotation = rotation;
    }
}