﻿using UnityEngine;
using System.Collections;
using System;

public class BulletController : PooledObject
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
        // move the bullet in the direction it is facing
        transform.Translate(Vector3.right * Speed);

        // recycle the object if it leaves the world bounds
        if (spriteBounds.min.x + transform.position.x > worldBounds.xMax
            || spriteBounds.max.x + transform.position.x < worldBounds.xMin
            || spriteBounds.min.y + transform.position.y > worldBounds.yMax
            || spriteBounds.max.y + transform.position.y < worldBounds.yMin)
        {
            Recycle();
        }
    }

    public void Initialize(Vector3 position, float aim)
    {
        transform.position = position;
        transform.rotation = Quaternion.AngleAxis(aim, Vector3.forward);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Terrain")
        {
            Recycle();
        }
    }
}