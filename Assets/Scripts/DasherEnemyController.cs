using UnityEngine;
using System.Collections;

[RequireComponent(typeof(EnemyController))]
public class DasherEnemyController : MonoBehaviour
{
    public GameObject FollowTarget;

    private EnemyController enemyController;
    private GameObject player;

    private Vector2 velocity;

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
        RotateToTarget();
    }

    void FixedUpdate()
    {
        if (!enemyController.IsEscaping)
        {
            transform.position = new Vector3(
                transform.position.x + velocity.x,
                transform.position.y + velocity.y,
                transform.position.z);

            if (velocity.magnitude > 0.01f)
            {
                velocity *= 0.9f;
            }
            else
            {
                velocity = RotateToTarget() * 0.5f;
            }
        }
    }

    private Vector2 RotateToTarget()
    {
        GameObject target = FollowTarget;
        if (target == null)
        {
            target = player;
        }
        if (target != null)
        {
            Vector2 direction = (target.transform.position - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            angle += Random.Range(-30.0f, 30.0f);
            direction = new Vector2(
                Mathf.Cos(angle * Mathf.Deg2Rad),
                Mathf.Sin(angle * Mathf.Deg2Rad));

            transform.rotation = Quaternion.Euler(0.0f, 0.0f, angle);
            return direction;
        }
        else
        {
            return Vector2.zero;
        }
    }
}
