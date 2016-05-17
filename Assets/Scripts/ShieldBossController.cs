using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShieldBossController : MonoBehaviour
{
    public float TurnSpeed = 0.1f;
    public BulletController EnemyBomb;
    public float TimeBetweenBursts = 5.0f;
    public float RateOfFire = 0.5f;
    public int BombsPerBurst = 4;
    public List<Vector2> ProjectileSpawnOffsets;

    private GameObject player;
    private DamageableObject damageComponent;
    private Animator animator;
    private Vector3 lastSightedPlayerPosition;
    private float burstTime;
    private float fireTime;
    private int projectilesFiredInBurst = 0;
    private int projectileSpawnLocationIndex = 0;

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
                lastSightedPlayerPosition = player.transform.position;
            }

            direction = lastSightedPlayerPosition - transform.position;
            RotateTo(Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
        }

        if (Time.time - burstTime > TimeBetweenBursts)
        {
            if (projectilesFiredInBurst < BombsPerBurst)
            {
                Fire();
            }
            else
            {
                burstTime = Time.time;
                projectilesFiredInBurst = 0;
                projectileSpawnLocationIndex = 0;
            }
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

    private void Fire()
    {
        if (EnemyBomb != null && ProjectileSpawnOffsets.Count > 0
            && Time.time - fireTime > RateOfFire)
        {
            // fetch a bullet instance from the object pool
            var bullet = EnemyBomb.Fetch<BulletController>();

            Vector2 offset = ProjectileSpawnOffsets[projectileSpawnLocationIndex];
            Vector3 spawnPos = this.transform.position + (transform.rotation * offset);
            Vector3 direction = lastSightedPlayerPosition - spawnPos;

            bullet.Initialize(spawnPos, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);

            projectilesFiredInBurst++;
            projectileSpawnLocationIndex = (projectileSpawnLocationIndex + 1) % ProjectileSpawnOffsets.Count;
            fireTime = Time.time;
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

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        foreach (var offset in ProjectileSpawnOffsets)
        {
            Vector3 rotatedOffset = transform.rotation * offset;
            Gizmos.DrawSphere(transform.position + rotatedOffset, 0.5f);
        }
    }
}
