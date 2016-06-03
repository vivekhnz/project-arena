using UnityEngine;
using System.Collections;

[RequireComponent(typeof(EnemyController))]
public class SwarmEnemyController : MonoBehaviour
{
    private EnemyController enemyController;
    private bool initialized = false;

    void Start()
    {
        enemyController = GetComponent<EnemyController>();
        enemyController.InstanceReset += OnInstanceReset;

        initialized = false;
    }

    private void OnInstanceReset(object sender, System.EventArgs e)
    {
        initialized = false;
    }

    void FixedUpdate()
    {
        if (!initialized)
        {
            Vector2 directionToCenter = -transform.position.normalized;
            float angle = Mathf.Atan2(directionToCenter.y, directionToCenter.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0.0f, 0.0f, angle);

            initialized = true;
        }
        if (!enemyController.IsEscaping)
        {
            transform.Translate(Vector3.right * enemyController.MovementSpeed);
        }
    }
}
