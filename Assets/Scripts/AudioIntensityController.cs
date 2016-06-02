using UnityEngine;
using System.Collections;
using System.Linq;

public class AudioIntensityController : MonoBehaviour
{
    public SpriteRenderer Background;

    AudioSource audioSource;
    float[] spectrum = new float[256];

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        audioSource.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);

        float intensity = spectrum[2];
        if (intensity > 0.06f)
        {
            Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, 28.0f,
                (intensity - 0.06f) * 2.0f);
            Background.color = Color.Lerp(Background.color, Color.white,
                (intensity - 0.06f) * 3.0f);
        }
        else
        {
            Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, 23.0f, 0.05f);
            Background.color = Color.Lerp(Background.color, new Color(0.1f, 0.1f, 0.1f), 0.1f);
        }
    }
}
