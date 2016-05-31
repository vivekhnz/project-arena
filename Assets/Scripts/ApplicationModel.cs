using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class ApplicationModel
{
    public static int GameScore { get; private set; }

    public static void RegisterGameScore(int score)
    {
        GameScore = score;

        int highscore = PlayerPrefs.GetInt("High Score", 0);
        if (score > highscore)
        {
            PlayerPrefs.SetInt("High Score", score);
        }
    }

    public static void ResetGameScore()
    {
        GameScore = 0;
    }
}