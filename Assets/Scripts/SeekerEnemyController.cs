using UnityEngine;
using System.Collections;

public class SeekerEnemyController : MonoBehaviour
{
    public float InitialTurnSpeed = 0.05f;
    public float FinalTurnSpeed = 0.1f;
    public float TurnSpeedTransitionDurationSeconds = 5.0f;
    public float MovementSpeed = 0.5f;

    private GameObject player;
    private float startTime;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        startTime = Time.time;
    }

    void FixedUpdate()
    {
        if (player != null)
        {
            // rotate to face the player
            Vector2 direction = player.transform.position - transform.position;
            RotateTo(Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
        }

        // move the enemy in the direction it is facing
        transform.Translate(Vector3.right * MovementSpeed);
    }

    void RotateTo(float targetRotation)
    {
        // calculate current turn speed
        float turnSpeedProgress = Mathf.Min((Time.time - startTime) / TurnSpeedTransitionDurationSeconds, 1.0f);
        float turnSpeed = Mathf.Lerp(InitialTurnSpeed, FinalTurnSpeed, turnSpeedProgress);

        // interpolate towards the target rotation
        Quaternion target = Quaternion.Euler(0.0f, 0.0f, targetRotation);
        transform.rotation = Quaternion.Lerp(transform.rotation, target, turnSpeed);
    }
}
