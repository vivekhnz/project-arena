﻿using UnityEngine;
using System.Collections;

public class SeekerEnemyController : MonoBehaviour
{
    public float TurnSpeed = 0.05f;
    public float MovementSpeed = 0.5f;
    public GameObject FollowTarget;

    private GameObject player;
    private float startTime;

    private Vector2 velocity;

    void Start()
    {
        startTime = Time.time;
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void FixedUpdate()
    {
        if (FollowTarget == null)
        {
            FollowTarget = GameObject.FindGameObjectWithTag("Player");
        }
        GameObject target = FollowTarget ?? player;
        if (target != null)
        {
            // rotate to face the player
            Vector2 direction = target.transform.position - transform.position;
            RotateTo(Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg, TurnSpeed);
        }

        // move the enemy in the direction it is facing
        transform.Translate(Vector3.right * MovementSpeed);

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
}
