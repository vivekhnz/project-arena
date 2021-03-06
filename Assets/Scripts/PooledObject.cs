﻿using UnityEngine;
using System.Collections;
using System;

// object pooling implementation based on http://catlikecoding.com/unity/tutorials/object-pools/
public class PooledObject : MonoBehaviour
{
    public ObjectPool Pool { get; set; }
    
    public virtual void ResetInstance() { }
    public virtual void CleanupInstance() { }

    public event EventHandler InstanceReset;
    public event EventHandler InstanceRecycled;

    public void Recycle()
    {
        if (Pool == null)
        {
            Destroy(gameObject);
        }
        else
        {
            if (InstanceRecycled != null)
            {
                InstanceRecycled(this, EventArgs.Empty);
            }
            Pool.Recycle(this);
        }
        CleanupInstance();
    }

    public T Fetch<T>() where T : PooledObject
    {
        if (Pool == null)
        {
            Pool = ObjectPool.GetPool(this);
        }
        var obj = (T)Pool.Fetch();
        if (obj.InstanceReset != null)
        {
            obj.InstanceReset(obj, EventArgs.Empty);
        }
        obj.ResetInstance();
        return obj;
    }

    void OnLevelWasLoaded()
    {
        Recycle();
    }
}