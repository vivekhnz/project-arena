using UnityEngine;
using System.Collections;
using System;

public class DamageableObject : MonoBehaviour
{
    public int MaxHealth = 100;
    public bool DestroyObjectOnDeath = true;
    public event EventHandler Destroyed;
    public event EventHandler HealthChanged;

    private int currentHealth;
    public int CurrentHealth
    {
        get { return currentHealth; }
        private set
        {
            currentHealth = value;

            // notify subscribers that the object's health has changed
            if (HealthChanged != null)
            {
                HealthChanged(this, EventArgs.Empty);
            }
        }
    }

    void Start()
    {
        ResetHealth();
    }

    // reset the health to the initial maximum value
    public void ResetHealth()
    {
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
        CurrentHealth -= damage;
        if (CurrentHealth <= 0)
        {
            if (DestroyObjectOnDeath)
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
            }

            // notify subscribers that the object has been destroyed
            if (Destroyed != null)
            {
                Destroyed(this, EventArgs.Empty);
            }
        }
    }
}
