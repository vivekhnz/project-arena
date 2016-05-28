using UnityEngine;
using System.Collections;

public class GameStateManager : MonoBehaviour
{
    public int Score { get; private set; }
    public int PerfectWaveBonus = 200;

	void Start ()
    {
        Score = 0;
	}

	void Update ()
    {
	}

    public void AddScore(int score)
    {
        Score += score;
    }
}
