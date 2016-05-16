using UnityEngine;
using System.Collections;

public class ShieldGeneratorController : PooledObject
{
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private DamageableObject damageComponent;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        damageComponent = GetComponent<DamageableObject>();
        if (damageComponent != null)
        {
            // subscribe to the damaged event
            damageComponent.Damaged += OnDamaged;
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
}
