using UnityEngine;
using System.Collections;

public class ShardController : PooledObject
{
    public float MaxSpeed = 0.3f;
    public float MinSpeed = 0.1f;
    public float FrictionCoefficient = 0.95f;

    private Rect worldBounds;
    private Bounds spriteBounds;
    private ExplosionManager explosionManager;

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

        // apply velocity
        Vector2 velocity = direction * speed;
        transform.position = new Vector3(
            transform.position.x + velocity.x,
            transform.position.y + velocity.y,
            transform.position.z);

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
                    Recycle();
                    break;
            }
        }
    }
}
