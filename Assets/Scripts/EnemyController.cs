using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System;

[RequireComponent(typeof(DamageableObject))]
public class EnemyController : PooledObject
{
    public float TurnSpeed = 0.1f;
    public float MovementSpeed = 0.15f;
    public int ScoreValue = 100;
    public int ShardsToDrop = 1;
    public ShardController Shard;

    public event EventHandler EnemyDestroyed;
    public event EventHandler EnemyEscaped;
    
    private ArenaManager arena;
    private GameStateManager gameStateManager;
    private ExplosionManager explosionManager;
    private WorldUIController worldUIController;

    private DamageableObject damageComponent;
    private Animator animator;
    private Vector2 velocity;

    public bool IsEscaping { get; private set; }
    private float escapeAngle;
    private bool escaped;

    private bool canBeShockwaved;
    private float shockwavedTime;
    private float ShockwaveImmunityTime = 1.0f;

    public void Initialize(Vector3 position)
    {
        transform.position = position;
    }

    public override void ResetInstance()
    {
        IsEscaping = false;
        escaped = false;

        canBeShockwaved = true;
        shockwavedTime = Time.time;

        damageComponent = GetComponent<DamageableObject>();
        if (damageComponent != null)
        {
            damageComponent.ResetHealth();
            damageComponent.HealthChanged += OnHealthChanged;
            damageComponent.Destroyed += OnDestroyed;
        }
        animator = GetComponent<Animator>();

        var arenaManagerObj = GameObject.FindGameObjectWithTag("ArenaManager");
        if (arenaManagerObj != null)
        {
            arena = arenaManagerObj.GetComponent<ArenaManager>();
        }

        var gameStateManagerObj = GameObject.FindGameObjectWithTag("GameStateManager");
        if (gameStateManagerObj != null)
        {
            gameStateManager = gameStateManagerObj.GetComponent<GameStateManager>();
        }

        var explosionManagerObj = GameObject.FindGameObjectWithTag("ExplosionManager");
        if (explosionManagerObj != null)
        {
            explosionManager = explosionManagerObj.GetComponent<ExplosionManager>();
        }

        var worldUIControllerObj = GameObject.FindGameObjectWithTag("WorldUIController");
        if (worldUIControllerObj != null)
        {
            worldUIController = worldUIControllerObj.GetComponent<WorldUIController>();
        }

        base.ResetInstance();
    }

    public override void CleanupInstance()
    {
        // unhook from events so we don't get duplicate notifications when this object is pooled
        if (damageComponent != null)
        {
            damageComponent.HealthChanged -= OnHealthChanged;
            damageComponent.Destroyed -= OnDestroyed;
        }

        base.CleanupInstance();
    }

    void FixedUpdate()
    {
        if (!canBeShockwaved && Time.time - shockwavedTime > ShockwaveImmunityTime)
        {
            canBeShockwaved = true;
        }

        if (IsEscaping)
        {
            if (!escaped && transform.position.magnitude > arena.ArenaRadius)
            {
                if (EnemyEscaped != null)
                {
                    EnemyEscaped(this, EventArgs.Empty);
                }
                Recycle();
                escaped = true;
            }
            else
            {
                // rotate the enemy towards the nearest point of escape
                RotateTo(escapeAngle, TurnSpeed);
                // move the enemy in the direction it is facing
                transform.Translate(Vector3.right * MovementSpeed);
            }
        }

        // apply velocity
        transform.position = new Vector3(
            transform.position.x + velocity.x,
            transform.position.y + velocity.y,
            transform.position.z);
        velocity *= 0.9f;
    }

    void RotateTo(float targetRotation, float turnSpeed)
    {
        // interpolate towards the target rotation
        Quaternion target = Quaternion.Euler(0.0f, 0.0f, targetRotation);
        transform.rotation = Quaternion.Lerp(transform.rotation, target, turnSpeed);
    }

    public void AddForce(Vector2 direction)
    {
        velocity += direction * 0.01f;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        ManageCollisions(collision);
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        ManageCollisions(collision);
    }
    
    private void ManageCollisions(Collision2D collision)
    {
        string[] tags = collision.gameObject.tag.Split('|');
        foreach (string tag in tags)
        {
            switch (tag)
            {
                case "Player":
                    // kill the player
                    var player = collision.gameObject.GetComponent<PlayerController>();
                    if (player != null)
                    {
                        player.Kill();
                    }

                    // destroy this enemy
                    Vector2 damageDir = (transform.position - player.transform.position).normalized;
                    DamageableObject.DamageObject(gameObject, damageComponent.MaxHealth,
                        Mathf.Atan2(damageDir.y, damageDir.x) * Mathf.Rad2Deg);
                    break;
                case "Shockwave":
                    if (canBeShockwaved)
                    {
                        Vector2 direction = ((Vector2)transform.position
                            - (Vector2)collision.transform.position).normalized;
                        AddForce(direction * 100.0f);
                        DamageableObject.DamageObject(gameObject, 2,
                            Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);

                        canBeShockwaved = false;
                        shockwavedTime = Time.time;
                    }
                    break;
            }
        }
    }

    public void Escape()
    {
        if (!IsEscaping)
        {
            // attempt to escape the arena
            IsEscaping = true;

            // calculate the angle to the nearest point on the edge of the arena
            Vector2 direction = ((Vector2)transform.position).normalized;
            escapeAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        }
    }

    private void OnHealthChanged(object sender, EventArgs e)
    {
        if (animator != null)
        {
            // play the damaged animation
            animator.SetTrigger("OnDamaged");
        }
    }

    private void OnDestroyed(object sender, DamageableObject.DestroyedEventArgs e)
    {
        if (EnemyDestroyed != null)
        {
            EnemyDestroyed(this, EventArgs.Empty);
        }
        gameStateManager.AddScore(ScoreValue);
        
        Vector2 calloutDirection = new Vector2(
            Mathf.Cos(e.DamageAngle * Mathf.Deg2Rad),
            Mathf.Sin(e.DamageAngle * Mathf.Deg2Rad));
        worldUIController.CreateScoreCallout(ScoreValue, transform.position,
            calloutDirection * 0.1f);

        explosionManager.CreateEnemyExplosion(transform.position, e.DamageAngle);
        if (Shard != null)
        {
            for (int i = 0; i < ShardsToDrop; i++)
            {
                var shard = Shard.Fetch<ShardController>();
                shard.Initialize(transform.position,
                    e.DamageAngle + UnityEngine.Random.Range(-90.0f, 90.0f));
            }
        }
    }
}
