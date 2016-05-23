using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text;

public class HUDController : MonoBehaviour
{
    public WaveManager WaveManager;
    public Text WaveNamesText;
    public Text WaveScoresText;
    public Text CurrentRoundText;

    private Animator animator;

    void Start()
    {
        if (WaveManager != null)
        {
            WaveManager.RoundCompleted += OnRoundCompleted;
            WaveManager.RoundStarted += OnRoundStarted;
        }
        animator = GetComponent<Animator>();
    }

    private void OnRoundCompleted(object sender, System.EventArgs e)
    {
        StringBuilder sbWaveNamesText = new StringBuilder();
        StringBuilder sbWaveScoresText = new StringBuilder();

        foreach (var wave in WaveManager.Waves)
        {
            sbWaveNamesText.AppendLine(string.Format("WAVE {0}", wave.WaveNumber));
            sbWaveScoresText.AppendLine(string.Format("{0} / {1}",
                wave.EnemiesDestroyed, wave.TotalEnemyCount));
        }

        WaveNamesText.text = sbWaveNamesText.ToString();
        WaveScoresText.text = sbWaveScoresText.ToString();
        CurrentRoundText.text = string.Format("ROUND {0}", WaveManager.CurrentRound);

        animator.SetBool("IsRoundOverlayVisible", true);
    }

    private void OnRoundStarted(object sender, System.EventArgs e)
    {
        animator.SetBool("IsRoundOverlayVisible", false);
    }
}
