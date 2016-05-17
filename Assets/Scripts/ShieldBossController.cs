using UnityEngine;
using System.Collections;

public class ShieldBossController : MonoBehaviour
{
    public float TurnSpeed = 0.1f;

    private GameObject player;

    void Start ()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

	void FixedUpdate ()
    {
        if (player != null)
        {
            Vector2 direction = player.transform.position - transform.position;
            var hit = Physics2D.Raycast(transform.position, direction);
            if (hit.collider.gameObject.tag == "Player")
            {
                RotateTo(Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (player != null)
        {
            Vector2 direction = player.transform.position - transform.position;
            var hit = Physics2D.Raycast(transform.position, direction);
            if (hit.collider.gameObject.tag == "Player")
            {
                Gizmos.color = Color.red;
            }
            else
            {
                Gizmos.color = Color.white;
            }
            
            Gizmos.DrawRay(transform.position, direction.normalized * hit.distance);
        }
    }

    void RotateTo(float targetRotation)
    {
        Quaternion target = Quaternion.Euler(0.0f, 0.0f, targetRotation);
        transform.rotation = Quaternion.Lerp(transform.rotation, target, TurnSpeed);
    }
}
