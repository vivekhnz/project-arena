using UnityEngine;
using System.Collections;
using System;

public class DamageableObject : MonoBehaviour
{
    public int MaxHealth = 100;
    public event EventHandler Destroyed;
    public event EventHandler Damaged;

    public int CurrentHealth
    {
        get;
        private set;
    }

    void Start()
    {
        // reset the health to the initial maximum value
        CurrentHealth = MaxHealth;
    }

    public static void DamageObject(GameObject obj, int damage)
    {
        // retrieve the damageable object component for the object if it exists
        var damageComponent = obj.GetComponent<DamageableObject>();
        if (damageComponent != null)
        {
            // apply damage
            damageComponent.Damage(damage);
        }
    }

    private void Damage(int damage)
    {
        // reduce health
        CurrentHealth--;
        if (CurrentHealth <= 0)
        {
            // is this object poolable?
            var poolable = gameObject.GetComponent<PooledObject>();
            if (poolable == null)
            {
                // if not, destroy the object
                Destroy(gameObject);
            }
            else
            {
                // recycle a poolable instance
                poolable.Recycle();
            }

            // notify subscribers that the object has been destroyed
            if (Destroyed != null)
            {
                Destroyed(this, EventArgs.Empty);
            }
        }
        else
        {
            // notify subscribers that the object has been damaged
            if (Damaged != null)
            {
                Damaged(this, EventArgs.Empty);
            }
        }
    }
}
