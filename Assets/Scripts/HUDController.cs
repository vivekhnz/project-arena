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

        Animator.SetBool("IsRoundOverlayVisible", true);
    }

    private void OnRoundStarted(object sender, System.EventArgs e)
    {
        Animator.SetBool("IsRoundOverlayVisible", false);
    }

    public void ShowWavesUI()
    {
        Animator.SetBool("IsBossFightActive", false);
    }

    public void ShowBossFightUI()
    {
        Animator.SetBool("IsRoundOverlayVisible", false);
        Animator.SetBool("IsBossFightActive", true);
    }
}
