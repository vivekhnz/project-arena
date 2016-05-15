using UnityEngine;
using System.Collections;

public class FollowObject : MonoBehaviour
{
    public Transform target;
    public float Stickiness = 1.0f;

    void Start()
    {

    }

    void Update()
    {
        if (target != null)
        {
            // calculate target position
            Vector3 targetPosition = new Vector3(target.position.x, target.position.y, transform.position.z);
            // interpolate to the target position based on the stickiness factor
            transform.position = Vector3.Lerp(transform.position, targetPosition, Stickiness);
        }
    }
}