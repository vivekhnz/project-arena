using UnityEngine;
using System.Collections;

public class ArenaManager : MonoBehaviour
{
    public float ArenaRadius = 50.0f;

    void OnDrawGizmosSelected()
    {
        // draw arena radius in the Unity editor
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(Vector3.zero, ArenaRadius);
    }
}
