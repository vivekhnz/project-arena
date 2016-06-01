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

    public static int GetValue(string name, bool isPersisted)
    {
        if (isPersisted)
        {
            return PlayerPrefs.GetInt("High Score", 0);
        }
        else
        {
            return GetSessionValue(name);
        }
    }

    private static int GetSessionValue(string name)
    {
        switch (name)
        {
            case "GameScore":
                return GameScore;
        }
        return 0;
    }
}