using UnityEngine;
using System.Collections;

public class GameStateManager : MonoBehaviour
{
    public int Score = 0;

	void Start ()
    {
	
	}

	void Update ()
    {
        Score += 100;
	}
}
