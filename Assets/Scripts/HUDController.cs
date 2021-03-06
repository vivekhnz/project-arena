﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text;

public class HUDController : MonoBehaviour
{
    public WaveManager WaveManager;
    public GameStateManager GameStateManager;
    public PlayerController Player;

    public Text RoundOverlayWaveNamesText;
    public Text RoundOverlayWaveScoresText;
    public Text RoundOverlayWavePerfectText;
    public Text RoundOverlayWaveBonusText;
    public Text RoundOverlayCurrentRoundText;

    public Text ScoreText;
    public Text CurrentRoundText;
    public Text CurrentWaveText;
    public Text HighscoreText;

    private int displayedScore = 0;

    private Animator animator;
    private Animator Animator
    {
        get
        {
            if (animator == null)
            {
                animator = GetComponent<Animator>();
            }
            return animator;
        }
    }

    void Start()
    {
        if (WaveManager != null)
        {
            WaveManager.RoundCompleted += OnRoundCompleted;
            WaveManager.RoundStarted += OnRoundStarted;
        }
        HighscoreText.text =
            ApplicationModel.GetValue("High Score", true).ToString();
    }

    void Update()
    {
        if (GameStateManager != null)
        {
            if (Mathf.Abs(GameStateManager.Score - displayedScore) <= 2)
            {
                displayedScore = GameStateManager.Score;
            }
            else
            {
                int increaseAmount = (int)Mathf.Ceil(
                    Mathf.Abs(GameStateManager.Score - displayedScore) / 10.0f);
                int changeDirection = (int)Mathf.Sign(GameStateManager.Score - displayedScore);
                displayedScore += (increaseAmount * changeDirection);
            }
            ScoreText.text = displayedScore.ToString().PadLeft(5, '0');

            int roundIndex = WaveManager.CurrentRound == 0 ? 1 : WaveManager.CurrentRound;
            CurrentRoundText.text = string.Format("{0}/{1}",
                roundIndex, WaveManager.Rounds.Count);
            var round = WaveManager.Rounds[roundIndex - 1];

            int wave = WaveManager.CurrentWave == 0 ? 1 : WaveManager.CurrentWave;
            CurrentWaveText.text = string.Format("{0}/{1}",
                wave, round.Waves.Count);
        }

        Animator.SetFloat("SuperEnergy", (float)Player.SuperEnergy);
    }

    private void OnRoundCompleted(object sender, System.EventArgs e)
    {
        StringBuilder sbWaveNamesText = new StringBuilder();
        StringBuilder sbWaveScoresText = new StringBuilder();
        StringBuilder sbWavePerfectText = new StringBuilder();
        StringBuilder sbWaveBonusText = new StringBuilder();

        int waveBonus = GameStateManager.PerfectWaveBonus;
        foreach (var wave in WaveManager.WaveResults)
        {
            sbWaveNamesText.AppendLine(string.Format("WAVE {0}", wave.WaveNumber));
            sbWaveScoresText.AppendLine(string.Format("{0} / {1}",
                wave.EnemiesDestroyed, wave.TotalEnemyCount));

            if (wave.IsPerfect)
            {
                sbWavePerfectText.AppendLine("Perfect!");
                sbWaveBonusText.AppendLine(string.Format("+{0}", waveBonus));

                GameStateManager.AddScore(waveBonus);
                waveBonus += GameStateManager.PerfectWaveBonus;
            }
            else
            {
                sbWavePerfectText.AppendLine();
                sbWaveBonusText.AppendLine();
            }
        }

        RoundOverlayWaveNamesText.text = sbWaveNamesText.ToString();
        RoundOverlayWaveScoresText.text = sbWaveScoresText.ToString();
        RoundOverlayWavePerfectText.text = sbWavePerfectText.ToString();
        RoundOverlayWaveBonusText.text = sbWaveBonusText.ToString();
        RoundOverlayCurrentRoundText.text = string.Format("ROUND {0}", WaveManager.CurrentRound);

        Animator.SetBool("IsRoundOverlayVisible", true);
    }

    private void OnRoundStarted(object sender, System.EventArgs e)
    {
        Animator.SetBool("IsRoundOverlayVisible", false);
    }

    public void HideRoundOverlay()
    {
        Animator.SetBool("IsRoundOverlayVisible", false);
    }
}
