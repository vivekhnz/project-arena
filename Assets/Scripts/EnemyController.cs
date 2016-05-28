using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System;

[RequireComponent(typeof(DamageableObject))]
public class EnemyController : PooledObject
{
    public float TurnSpeed = 0.1f;
    public float MovementSpeed = 0.15f;

    public event EventHandler EnemyDestroyed;
    public event EventHandler EnemyEscaped;
    
    private ArenaManager arena;
    private GameStateManager gameStateManager;

    private DamageableObject damageComponent;
    private Vector2 velocity;

    public bool IsEscaping { get; private set; }
    private float escapeAngle;
    private bool escaped;

    public void Initialize(Vector3 position)
    {
        transform.position = position;
    }

    public override void ResetInstance()
    {
        IsEscaping = false;
        escaped = false;

        damageComponent = GetComponent<DamageableObject>();
        if (damageComponent != null)
        {
            damageComponent.ResetHealth();
            damageComponent.Destroyed += OnDestroyed;
        }

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

        base.ResetInstance();
    }

    public override void CleanupInstance()
    {
        // unhook from events so we don't get duplicate notifications when this object is pooled
        if (damageComponent != null)
        {
            damageComponent.Destroyed -= OnDestroyed;
        }

        base.CleanupInstance();
    }

    void FixedUpdate()
    {
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
                    // navigate to the Defeat scene if an enemy collides with the player
                    SceneManager.LoadScene("DefeatScene");
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

    private void OnDestroyed(object sender, System.EventArgs e)
    {
        if (EnemyDestroyed != null)
        {
            EnemyDestroyed(this, EventArgs.Empty);
        }
        gameStateManager.AddScore(100);
    }
}
