using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class ShieldBossController : MonoBehaviour
{
    public float TurnSpeed = 0.1f;
    public BulletController EnemyBomb;
    public float TimeBetweenBursts = 5.0f;
    public float MinRateOfFire = 0.15f;
    public float MaxRateOfFire = 0.3f;
    public int MinBombsPerBurst = 4;
    public int MaxBombsPerBurst = 8;
    public List<Vector2> ProjectileSpawnOffsets;
    public ShieldGeneratorManager ShieldGeneratorManager;
    public float DisableShieldDuration = 5.0f;

    private GameObject player;
    private DamageableObject damageComponent;
    private Animator animator;
    private Vector3 lastSightedPlayerPosition;
    private float burstTime;
    private float fireTime;
    private int projectilesFiredInBurst = 0;
    private int projectileSpawnLocationIndex = 0;
    private bool isShieldDisabled = false;
    private float shieldDisableTime;

    void Start ()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        animator = GetComponent<Animator>();
        damageComponent = GetComponent<DamageableObject>();
        if (damageComponent != null)
        {
            // subscribe to the health changed and destroyed events
            damageComponent.HealthChanged += OnHealthChanged;
            damageComponent.Destroyed += OnDestroyed;
        }
        if (ShieldGeneratorManager != null)
        {
            ShieldGeneratorManager.ShieldDisabled += OnShieldDisabled;
        }
        SetShieldState(true);

       burstTime = Time.time;
    }

    void FixedUpdate ()
    {
        if (isShieldDisabled)
        {
            if (Time.time - shieldDisableTime > DisableShieldDuration)
            {
                isShieldDisabled = false;
                SetShieldState(true);
                ShieldGeneratorManager.ResetGeneratorSpawns();
            }
        }
        else
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
                int bombsPerBurst = (int)Mathf.Ceil(Mathf.Lerp(MaxBombsPerBurst, MinBombsPerBurst,
                    damageComponent.CurrentHealth / (float)damageComponent.MaxHealth));
                if (projectilesFiredInBurst < bombsPerBurst)
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
        if (animator != null)
        {
            // play the damaged animation
            animator.SetTrigger("OnDamaged");
        }
    }

    private void Fire()
    {
        float rateOfFire = Mathf.Lerp(MinRateOfFire, MaxRateOfFire,
            damageComponent.CurrentHealth / (float)damageComponent.MaxHealth);
        if (EnemyBomb != null && ProjectileSpawnOffsets.Count > 0
            && Time.time - fireTime > rateOfFire)
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

    private void OnDestroyed(object sender, System.EventArgs e)
    {
        SceneManager.LoadScene("VictoryScene");
    }

    private void SetShieldState(bool isShielded)
    {
        if (animator != null)
        {
            animator.SetBool("IsShielded", isShielded);
        }
        damageComponent.IsInvincible = isShielded;
        if (isShielded)
        {
            // reset the burst time once the shield re-activates so it doesn't instantly fire at the player
            burstTime = Time.time;
        }
    }

    private void OnShieldDisabled(object sender, System.EventArgs e)
    {
        SetShieldState(false);
        isShieldDisabled = true;
        shieldDisableTime = Time.time;
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
