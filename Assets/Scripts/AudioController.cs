﻿using UnityEngine;

public class AudioController : MonoBehaviour
{
    AudioSource audioSource;
    AudioLowPassFilter lowpass;
    private MusicSpectrum spectrum;
    private bool isFadingOut = false;

    public float Intensity { get; private set; }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        lowpass = GetComponent<AudioLowPassFilter>();
        isFadingOut = false;

        spectrum = new MusicSpectrum(audioSource);
    }

    void FixedUpdate()
    {
        Intensity = spectrum.GetIntensity();

        if (isFadingOut)
        {
            lowpass.cutoffFrequency = Mathf.Lerp(lowpass.cutoffFrequency, 0, 0.02f);
            audioSource.volume = Mathf.Lerp(audioSource.volume, 0, 0.02f);
        }
        else
        {
            if (lowpass.cutoffFrequency < 5000)
            {
                lowpass.cutoffFrequency = Mathf.Lerp(lowpass.cutoffFrequency, 5000, 0.02f);
                if (lowpass.cutoffFrequency > 4995)
                {
                    lowpass.cutoffFrequency = 5000;
                }
            }
            if (audioSource.volume < 0.5f)
            {
                audioSource.volume = Mathf.Lerp(audioSource.volume, 0.5f, 0.015f);
                if (audioSource.volume > 0.499f)
                {
                    audioSource.volume = 0.5f;
                }
            }
        }
    }

    public void ApplyShockwaveEffect()
    {
        if (!isFadingOut)
        {
            lowpass.cutoffFrequency = 50;
            audioSource.volume = 0.01f;
        }
    }

    public void FadeOut()
    {
        isFadingOut = true;
    }
}
