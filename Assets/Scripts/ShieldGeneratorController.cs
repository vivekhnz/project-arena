using UnityEngine;
using System.Collections;
using System;

public class ShieldGeneratorController : PooledObject
{
    public GameObject Core;

    public event EventHandler Destroyed;

    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private DamageableObject damageComponent;

    void Start()
    {
        animator = GetComponent<Animator>();
        if (Core != null)
        {
            spriteRenderer = Core.GetComponent<SpriteRenderer>();

            damageComponent = Core.GetComponent<DamageableObject>();
            if (damageComponent != null)
            {
                // subscribe to the health changed and destroyed events
                damageComponent.HealthChanged += OnHealthChanged;
                damageComponent.Destroyed += OnDestroyed;
            }
        }
    }

    public override void ResetInstance()
    {
        if (damageComponent == null)
        {
            damageComponent = Core.GetComponent<DamageableObject>();
        }
        if (damageComponent != null)
        {
            damageComponent.ResetHealth();
        }

        base.ResetInstance();
    }

    private void OnHealthChanged(object sender, System.EventArgs e)
    {
        // tint sprite red based on how much damage has been taken
        spriteRenderer.color = Color.Lerp(Color.red, Color.white,
            damageComponent.CurrentHealth / (float)damageComponent.MaxHealth);

        if (animator != null)
        {
            // play the damaged animation
            animator.SetTrigger("OnDamaged");
        }
    }

    private void OnDestroyed(object sender, System.EventArgs e)
    {
        if (Destroyed != null)
        {
            Destroyed(this, EventArgs.Empty);
        }
        Recycle();
    }
}
