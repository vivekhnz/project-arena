using UnityEngine;
using System.Collections;

public class SuperMeterController : MonoBehaviour
{
    public PlayerController Player;

	void Start ()
    {

	}
	
	void FixedUpdate ()
    {
        transform.position = Player.transform.position;
	}
}
