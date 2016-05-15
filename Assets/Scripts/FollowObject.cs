using UnityEngine;
using System.Collections;

public class FollowObject : MonoBehaviour
{
    public Transform target;

    void Start()
    {

    }

    void Update()
    {
        if (target != null)
        {
            // set the X and Y ordinates to the target's
            transform.position = new Vector3(target.position.x, target.position.y, transform.position.z);
        }
    }
}