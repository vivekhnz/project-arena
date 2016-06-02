using UnityEngine;
using System.Collections;
using System;

public class GameStateManager : MonoBehaviour
{
    public int Score { get; private set; }
    public int PerfectWaveBonus = 200;
    public int DeathScorePenalty = 1000;

    public event EventHandler Supercharged;

	void Start ()
    {
        Score = 0;
        ApplicationModel.ResetGameScore();
	}

	void Update ()
    {
	}

    public void AddScore(int score)
    {
        Score += score;
        if (Score < 0)
        {
            Score = 0;
        }
    }

    public void NotifySupercharged()
    {
        if (Supercharged != null)
        {
            Supercharged(this, EventArgs.Empty);
        }
    }
}
