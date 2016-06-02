using UnityEngine;
using System.Collections;
using System.Linq;

public class AudioController : MonoBehaviour
{
    public SpriteRenderer Background;

    AudioSource audioSource;
    AudioLowPassFilter lowpass;
    float[] spectrum = new float[256];

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        lowpass = GetComponent<AudioLowPassFilter>();
    }

    void Update()
    {
        audioSource.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);

        float intensity = spectrum[2];
        if (intensity > 0.06f)
        {
            Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, 28.0f,
                (intensity - 0.06f) * 2.0f);
            Background.color = Color.Lerp(Background.color, new Color(0.4f, 0.4f, 0.4f),
                (intensity - 0.06f) * 3.0f);
        }
        else
        {
            Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, 25.0f, 0.05f);
            Background.color = Color.Lerp(Background.color, new Color(0.1f, 0.1f, 0.1f), 0.1f);
        }

        if (lowpass.cutoffFrequency < 5000)
        {
            lowpass.cutoffFrequency = Mathf.Lerp(lowpass.cutoffFrequency, 5000, 0.02f);
            if (lowpass.cutoffFrequency > 4995)
            {
                lowpass.cutoffFrequency = 5000;
            }
        }
    }

    public void ApplyShockwaveEffect()
    {
        lowpass.cutoffFrequency = 50;
    }
}
