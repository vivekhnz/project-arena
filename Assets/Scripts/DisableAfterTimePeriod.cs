using UnityEngine;
using System.Collections;

public class DisableAfterTimePeriod : MonoBehaviour
{
    public float DurationSeconds = 1.0f;

    private float startTime;

	void Start ()
    {
        startTime = Time.time;
	}

	void FixedUpdate ()
    {
        if (gameObject.activeSelf && Time.time - startTime > DurationSeconds)
        {
            gameObject.SetActive(false);
        }
	}
}
