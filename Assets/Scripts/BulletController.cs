using UnityEngine;
using System.Collections;
using System;

public class BulletController : MonoBehaviour
{
    public float Speed = 1.0f;

    private Rect worldBounds;
    private Bounds spriteBounds;

    void Start()
    {
        // retrieve world bounds from camera controller
        var cameraController = Camera.main.GetComponent<CameraController>();
        worldBounds = cameraController.Bounds;

        // retrieve sprite bounds
        var renderer = GetComponent<SpriteRenderer>();
        spriteBounds = renderer.sprite.bounds;
    }

    void Update()
    {
        transform.Translate(Vector3.right * Speed);
        if (spriteBounds.min.x + transform.position.x > worldBounds.xMax
            || spriteBounds.max.x + transform.position.x < worldBounds.xMin
            || spriteBounds.min.y + transform.position.y > worldBounds.yMax
            || spriteBounds.max.y + transform.position.y < worldBounds.yMin)
        {
            Destroy(gameObject);
        }
    }

    public void Initialize(Vector3 position, Quaternion rotation)
    {
        transform.position = position;
        transform.rotation = rotation;
    }
}