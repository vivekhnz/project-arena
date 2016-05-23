using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text;

public class HUDController : MonoBehaviour
{
    public WaveManager WaveManager;
    public Text WaveNamesText;
    public Text WaveScoresText;

    void Update ()
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
    }
}
