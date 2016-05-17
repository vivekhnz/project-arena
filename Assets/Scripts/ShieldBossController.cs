using UnityEngine;
using System.Collections;

public class ShieldBossController : MonoBehaviour
{
    private GameObject player;

	void Start ()
    {
        player = GameObject.FindGameObjectWithTag("Player");
	}

	void FixedUpdate ()
    {
        Vector3 direction = player.transform.position - transform.position;
        transform.rotation = Quaternion.Euler(0.0f, 0.0f,
            Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
    }
}
