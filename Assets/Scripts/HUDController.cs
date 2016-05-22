using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    public WaveManager WaveManager;
    public Text WaveText;
	
	void Update ()
    {
        WaveText.text = string.Format("WAVE {0}", WaveManager.CurrentWave);
	}
}
