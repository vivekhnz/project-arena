using UnityEngine;
using System.Collections;
using System.Linq;

public class AudioController : MonoBehaviour
{
    AudioSource audioSource;
    AudioLowPassFilter lowpass;
    float[] spectrum = new float[256];

    public float Intensity { get; private set; }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        lowpass = GetComponent<AudioLowPassFilter>();
    }

    void FixedUpdate()
    {
        audioSource.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);
        Intensity = spectrum[2];

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
