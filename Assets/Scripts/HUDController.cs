using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text;

public class HUDController : MonoBehaviour
{
    public WaveManager WaveManager;
    public Text WaveText;
	
	void Update ()
    {
        StringBuilder sbWaveText = new StringBuilder();
        foreach (var wave in WaveManager.Waves)
        {
            sbWaveText.AppendLine(string.Format("WAVE {0}: {1} / {2}",
                wave.WaveNumber, wave.EnemiesDestroyed, wave.TotalEnemyCount));
        }
        WaveText.text = sbWaveText.ToString();
    }
}
