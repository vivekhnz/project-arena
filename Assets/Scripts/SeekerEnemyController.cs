using UnityEngine;
using System.Collections;

public class SeekerEnemyController : MonoBehaviour
{
    public GameObject FollowTarget;

    private GameObject player;
    private float turnSpeed;
    private float movementSpeed;

    public void Initialize(float turnSpeed, float movementSpeed)
    {
        this.turnSpeed = turnSpeed;
        this.movementSpeed = movementSpeed;
        
        player = GameObject.FindGameObjectWithTag("Player");
        RotateToTarget(1.0f);
    }

    public void UpdateAI()
    {
        RotateToTarget(turnSpeed);

        // move the enemy in the direction it is facing
        transform.Translate(Vector3.right * movementSpeed);
    }

    public void RotateToTarget(float turnSpeed)
    {
        // target the player if no target has been explicitly specified
        GameObject target = FollowTarget;
        if (target == null)
        {
            target = player;
        }

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
}
