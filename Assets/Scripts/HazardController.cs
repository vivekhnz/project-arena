using UnityEngine;
using System.Collections;

public class HazardController : PooledObject
{
    public void Initialize(Vector3 position)
    {
        transform.position = position;
    }
}
