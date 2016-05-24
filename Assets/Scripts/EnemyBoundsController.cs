using UnityEngine;
using System.Collections;

public class EnemyBoundsController : MonoBehaviour
{
    public EnemyController Parent;

    void OnCollisionEnter2D(Collision2D collision)
    {
        ManageCollisions(collision);
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        ManageCollisions(collision);
    }

    private void ManageCollisions(Collision2D collision)
    {
        Vector2 direction = (Vector2)transform.position - collision.contacts[0].point;
        Parent.AddForce(direction.normalized);
    }
}
