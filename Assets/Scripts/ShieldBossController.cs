using UnityEngine;
using System.Collections;

public class ShieldBossController : MonoBehaviour
{
    public float TurnSpeed = 0.1f;

    private GameObject player;
    private DamageableObject damageComponent;
    private Animator animator;

    void Start ()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        animator = GetComponent<Animator>();
        damageComponent = GetComponent<DamageableObject>();
        if (damageComponent != null)
        {
            // subscribe to the health changed event
            damageComponent.HealthChanged += OnHealthChanged;
        }
    }

    void FixedUpdate ()
    {
        if (player != null)
        {
            Vector2 direction;
            var hit = RaycastLineOfSight(out direction);
            
            // if we have line of sight with the player, rotate to face the player
            if (hit.collider.gameObject.tag == "Player")
            {
                RotateTo(Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
            }
        }
    }

    void OnDrawGizmos()
    {
        // draw line of sight in the Unity editor
        if (player != null)
        {
            Vector2 direction;
            var hit = RaycastLineOfSight(out direction);

            if (hit.collider.gameObject.tag == "Player")
            {
                Gizmos.color = Color.red;
            }
            else
            {
                Gizmos.color = Color.white;
            }
            
            Gizmos.DrawRay(transform.position, direction.normalized * hit.distance);
        }
    }

    void RotateTo(float targetRotation)
    {
        // interpolate towards the target rotation
        Quaternion target = Quaternion.Euler(0.0f, 0.0f, targetRotation);
        transform.rotation = Quaternion.Lerp(transform.rotation, target, TurnSpeed);
    }

    private RaycastHit2D RaycastLineOfSight(out Vector2 direction)
    {
        direction = player.transform.position - transform.position;
        // only raycast against objects on the Player (9) and Terrain (10) layers
        int layerMask = (1 << 9) | (1 << 10);
        return Physics2D.Raycast(transform.position, direction, float.MaxValue, layerMask);
    }

    private void OnHealthChanged(object sender, System.EventArgs e)
    {
        // play the damaged animation
        animator.SetTrigger("OnDamaged");
    }
}
