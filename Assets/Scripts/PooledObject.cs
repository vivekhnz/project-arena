using UnityEngine;
using System.Collections;
using System;

// object pooling implementation based on http://catlikecoding.com/unity/tutorials/object-pools/
public class PooledObject : MonoBehaviour
{
    public ObjectPool Pool { get; set; }
    
    public virtual void ResetInstance() { }

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
        var obj = (T)Pool.Fetch();
        obj.ResetInstance();
        return obj;
    }
}