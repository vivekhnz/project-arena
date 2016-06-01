using UnityEngine;
using System.Collections;
using System;

public class DamageableObject : MonoBehaviour
{
    public int MaxHealth = 100;
    public bool DestroyObjectOnDeath = true;
    public bool IsInvincible = false;

    public class DestroyedEventArgs : EventArgs
    {
        public float DamageAngle { get; private set; }

        public DestroyedEventArgs(float damageAngle)
        {
            DamageAngle = damageAngle;
        }
    }
    public event EventHandler<DestroyedEventArgs> Destroyed;
    public event EventHandler HealthChanged;

    private bool isDestroyed = false;

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
        isDestroyed = false;
    }

    public static void DamageObject(GameObject obj, int damage)
    {
        DamageObject(obj, damage, obj.transform.rotation.eulerAngles.z);
    }
    public static void DamageObject(GameObject obj, int damage, float damageAngle)
    {
        // retrieve the damageable object component for the object if it exists
        var damageComponent = obj.GetComponent<DamageableObject>();
        if (damageComponent != null)
        {
            // apply damage
            damageComponent.Damage(damage, damageAngle);
        }
    }

    private void Damage(int damage, float damageAngle)
    {
        if (!IsInvincible)
        {
            // reduce health
            CurrentHealth -= damage;
            if (!isDestroyed && CurrentHealth <= 0)
            {
                // notify subscribers that the object has been destroyed
                if (Destroyed != null)
                {
                    Destroyed(this, new DestroyedEventArgs(damageAngle));
                }

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

                isDestroyed = true;
            }
        }
    }
}
