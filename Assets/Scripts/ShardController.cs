using UnityEngine;
using System.Collections;

public class ShardController : PooledObject
{
    public float MaxSpeed = 0.3f;
    public float MinSpeed = 0.1f;
    public float FrictionCoefficient = 0.95f;
    public double SuperEnergy = 0.1;
    public float MagnetismDistance = 20.0f;
    public int ScoreValue = 5;

    private Rect worldBounds;
    private Bounds spriteBounds;
    private PlayerController player;
    private GameStateManager gameStateManager;

    private Vector2 direction;
    private float speed;

    void Start()
    {
        // retrieve world bounds from camera controller
        var cameraController = Camera.main.GetComponent<CameraController>();
        worldBounds = cameraController.Bounds;

        // retrieve sprite bounds
        var renderer = GetComponent<SpriteRenderer>();
        spriteBounds = renderer.sprite.bounds;
    }

    public void Initialize(Vector3 position)
    {
        transform.position = position;

        float angle = Random.Range(0.0f, 360.0f) * Mathf.Deg2Rad;
        direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        speed = MaxSpeed;
    }

    public override void ResetInstance()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.GetComponent<PlayerController>();
        }

        var gameStateManagerObj = GameObject.FindGameObjectWithTag("GameStateManager");
        if (gameStateManagerObj != null)
        {
            gameStateManager = gameStateManagerObj.GetComponent<GameStateManager>();
        }

        base.ResetInstance();
    }

    void FixedUpdate()
    {
        // recycle the object if it leaves the world bounds
        if (spriteBounds.min.x + transform.position.x > worldBounds.xMax
            || spriteBounds.max.x + transform.position.x < worldBounds.xMin
            || spriteBounds.min.y + transform.position.y > worldBounds.yMax
            || spriteBounds.max.y + transform.position.y < worldBounds.yMin)
        {
            Recycle();
        }
        
        // move towards the player if they are close enough
        // otherwise, drift
        Vector2 velocity = direction * speed;
        if (player != null && !player.IsSuperActive && player.SuperEnergy < 1.0f)
        {
            Vector2 directionTowardsPlayer = (Vector2)player.transform.position - (Vector2)transform.position;
            if (directionTowardsPlayer.magnitude < MagnetismDistance)
            {
                float proximity = (MagnetismDistance - directionTowardsPlayer.magnitude)
                    / MagnetismDistance;
                velocity = directionTowardsPlayer.normalized * (MaxSpeed * proximity);
            }
        }

        transform.position = new Vector3(
            transform.position.x + velocity.x,
            transform.position.y + velocity.y,
            transform.position.z);

        transform.Rotate(0.0f, 0.0f, speed * 10.0f);

        // apply friction
        speed = Mathf.Clamp(speed * FrictionCoefficient, MinSpeed, MaxSpeed);

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
                    if (!player.IsSuperActive && player.SuperEnergy < 1.0f)
                    {
                        player.AddSuperEnergy(SuperEnergy);
                        gameStateManager.AddScore(ScoreValue);
                        Recycle();
                    }
                    break;
            }
        }
    }
}
