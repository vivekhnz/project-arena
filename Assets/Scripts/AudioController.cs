using UnityEngine;
using System.Collections;
using System.Linq;

public class AudioController : MonoBehaviour
{
    AudioSource audioSource;
    AudioLowPassFilter lowpass;
    float[] spectrum = new float[256];
    private bool isFadingOut = false;

    public float Intensity { get; private set; }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        lowpass = GetComponent<AudioLowPassFilter>();
        isFadingOut = false;
    }

    void FixedUpdate()
    {
        audioSource.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);
        Intensity = spectrum[2];

        if (isFadingOut)
        {
            lowpass.cutoffFrequency = Mathf.Lerp(lowpass.cutoffFrequency, 0, 0.02f);
        }
        else if (lowpass.cutoffFrequency < 5000)
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
        if (!isFadingOut)
        {
            lowpass.cutoffFrequency = 50;
        }
    }

    public void FadeOut()
    {
        isFadingOut = true;
    }
}
