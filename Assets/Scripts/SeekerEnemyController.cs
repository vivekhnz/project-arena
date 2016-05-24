using UnityEngine;
using System.Collections;

[RequireComponent(typeof(EnemyController))]
public class SeekerEnemyController : MonoBehaviour
{
    public GameObject FollowTarget;

    private EnemyController enemyController;
    private GameObject player;

    void Start()
    {
        enemyController = GetComponent<EnemyController>();
        enemyController.InstanceReset += OnInstanceReset;

        Initialize();
    }

    private void OnInstanceReset(object sender, System.EventArgs e)
    {
        Initialize();
    }

    private void Initialize()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        RotateToTarget(1.0f);
    }

    void FixedUpdate()
    {
        if (!enemyController.IsEscaping)
        {
            // rotate and move towards the target
            RotateToTarget(enemyController.TurnSpeed);
            transform.Translate(Vector3.right * enemyController.MovementSpeed);
        }
    }

    private void RotateToTarget(float turnSpeed)
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

    private void RotateTo(float targetRotation, float turnSpeed)
    {
        // interpolate towards the target rotation
        Quaternion target = Quaternion.Euler(0.0f, 0.0f, targetRotation);
        transform.rotation = Quaternion.Lerp(transform.rotation, target, turnSpeed);
    }
}
