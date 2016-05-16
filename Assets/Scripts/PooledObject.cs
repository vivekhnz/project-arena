using UnityEngine;
using System.Collections;
using System;

// object pooling implementation based on http://catlikecoding.com/unity/tutorials/object-pools/
public class PooledObject : MonoBehaviour
{
    public ObjectPool Pool { get; set; }

    public void Recycle()
    {
        if (Pool == null)
        {
            Destroy(gameObject);
        }
        else
        {
            Pool.Recycle(this);
        }
    }

    public T Fetch<T>() where T : PooledObject
    {
        if (Pool == null)
        {
            Pool = ObjectPool.GetPool(this);
        }
        return (T)Pool.Fetch();
    }
}