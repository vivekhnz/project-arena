using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SeekerEnemyController : PooledObject
{
    public float TurnSpeed = 0.05f;
    public float MovementSpeed = 0.5f;
    public GameObject FollowTarget;

    private GameObject player;
    private float startTime;
    private DamageableObject damageComponent;
    private Vector2 velocity;

    private WaveManager waveManager;
    private int wave;

    public void Initialize(Vector3 position, int wave)
    {
        transform.position = position;
        this.wave = wave;
    }

    public override void ResetInstance()
    {
        startTime = Time.time;
        player = GameObject.FindGameObjectWithTag("Player");

        if (damageComponent == null)
        {
            damageComponent = GetComponent<DamageableObject>();
        }
        if (damageComponent != null)
        {
            damageComponent.ResetHealth();
        }

        // subscribe to wave changed event so we are notified when this enemy's wave has ended
        var waveManagerObj = GameObject.FindGameObjectWithTag("WaveManager");
        if (waveManagerObj != null)
        {
            waveManager = waveManagerObj.GetComponent<WaveManager>();
            if (waveManager != null)
            {
                waveManager.WaveChanged += OnWaveChanged;
            }
        }

        RotateToTarget(1.0f);

        base.ResetInstance();
    }

    public override void CleanupInstance()
    {
        // unhook from wave changed events so we don't get duplicate notifications when this object is pooled
        if (waveManager != null)
        {
            waveManager.WaveChanged -= OnWaveChanged;
        }

        base.CleanupInstance();
    }

    void FixedUpdate()
    {
        RotateToTarget(TurnSpeed);

        // move the enemy in the direction it is facing
        transform.Translate(Vector3.right * MovementSpeed);

        // apply velocity
        transform.position = new Vector3(
            transform.position.x + velocity.x,
            transform.position.y + velocity.y,
            transform.position.z);
        velocity *= 0.9f;
    }

    private void RotateToTarget(float turnSpeed)
    {
        // target the player if no target has been explicitly specified
        if (FollowTarget == null)
        {
            FollowTarget = GameObject.FindGameObjectWithTag("Player");
        }
        GameObject target = FollowTarget ?? player;

        if (target != null)
        {
            // rotate to face the target
            Vector2 direction = target.transform.position - transform.position;
            RotateTo(Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg, turnSpeed);
        }
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

    private void OnWaveChanged(object sender, System.EventArgs e)
    {
        if (waveManager.CurrentWave > wave)
        {
            Recycle();
        }
    }
}
