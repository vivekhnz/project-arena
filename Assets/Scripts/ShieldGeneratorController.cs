using UnityEngine;
using System.Collections;

public class ShieldGeneratorController : PooledObject
{
    public GameObject Core;

    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private DamageableObject damageComponent;

    void Start()
    {
        if (Core != null)
        {
            spriteRenderer = Core.GetComponent<SpriteRenderer>();
            animator = Core.GetComponent<Animator>();

            damageComponent = Core.GetComponent<DamageableObject>();
            if (damageComponent != null)
            {
                // subscribe to the damaged and destroyed events
                damageComponent.Damaged += OnDamaged;
                damageComponent.Destroyed += OnDestroyed;
            }
        }
    }

    private void OnDamaged(object sender, System.EventArgs e)
    {
        // tint sprite red based on how much damage has been taken
        spriteRenderer.color = Color.Lerp(Color.red, Color.white,
            damageComponent.CurrentHealth / (float)damageComponent.MaxHealth);

        // play the damaged animation
        animator.SetTrigger("OnDamaged");
    }

    private void OnDestroyed(object sender, System.EventArgs e)
    {
        Recycle();
    }
}
