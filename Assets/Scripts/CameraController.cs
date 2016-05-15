using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    public Transform target;
    public float Stickiness = 1.0f;
    public Rect Bounds;

    void Start()
    {

    }

    void Update()
    {
        if (target != null)
        {
            // calculate view bounds
            float viewHeight = Camera.main.orthographicSize;
            float viewWidth = viewHeight * (Screen.width / (float)Screen.height);

            // calculate target position
            Vector3 targetPosition = new Vector3(
                Mathf.Max(Mathf.Min(target.position.x, Bounds.xMax - viewWidth), Bounds.xMin + viewWidth),
                Mathf.Max(Mathf.Min(target.position.y, Bounds.yMax - viewHeight), Bounds.yMin + viewHeight),
                transform.position.z);
            // interpolate to the target position based on the stickiness factor
            transform.position = Vector3.Lerp(transform.position, targetPosition, Stickiness);
        }
    }

    void OnDrawGizmosSelected()
    {
        // draw bounds in the Unity editor
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(Bounds.center, Bounds.size);
    }
}