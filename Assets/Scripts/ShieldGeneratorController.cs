using UnityEngine;
using System.Collections;

public class ShieldGeneratorController : PooledObject
{
    public int MaxHealth = 100;

    private int currentHealth = 100;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentHealth = MaxHealth;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Bullet")
        {
            currentHealth--;
            if (currentHealth <= 0)
            {
                Recycle();
            }
            else
            {
                spriteRenderer.color = Color.Lerp(Color.red, Color.white, currentHealth / (float)MaxHealth);
            }
        }
    }
}
