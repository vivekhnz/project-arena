using UnityEngine;
using System.Collections;

public class ParallaxScrolling : MonoBehaviour
{
    public Transform Camera;
    public float ParallaxMultiplier;

    private Vector3 offset;

	void Start ()
    {
        offset = transform.position;
	}
	
	void Update ()
    {
        transform.position = new Vector3(
            offset.x + (Camera.transform.position.x * ParallaxMultiplier),
            offset.y + (Camera.transform.position.y * ParallaxMultiplier),
            offset.z);
	}
}
