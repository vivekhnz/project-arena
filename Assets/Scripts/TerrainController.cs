using UnityEngine;
using System.Collections;

public class TerrainController : MonoBehaviour
{
    public SpriteRenderer Background;
    public SpriteRenderer BackLayer;
    public SpriteRenderer MidLayer1;
    public SpriteRenderer MidLayer2;
    public SpriteRenderer FrontLayer;

    public AudioController AudioController;
    public WaveManager WaveManager;

    public Color LowIntensityBackground = new Color(0.1f, 0.1f, 0.1f);
    public Color HighIntensityBackground = new Color(0.4f, 0.4f, 0.4f);
    public Color ForegroundColor = new Color(0.0f, 0.0f, 0.0f, 1.0f);
    public Sprite ForegroundSprite;

    private Animator animator;

    void Start ()
    {
        animator = GetComponent<Animator>();
	}
	
	void FixedUpdate ()
    {
        if (AudioController.Intensity > 0.06f)
        {
            Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, 28.0f,
                (AudioController.Intensity - 0.06f) * 2.0f);
            Background.color = Color.Lerp(Background.color, HighIntensityBackground,
                (AudioController.Intensity - 0.06f) * 3.0f);
        }
        else
        {
            Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, 25.0f, 0.05f);
            Background.color = Color.Lerp(Background.color, LowIntensityBackground, 0.1f);
        }

        Color backLayer = ForegroundColor;
        Color midLayer = new Color(
            ForegroundColor.r,
            ForegroundColor.g,
            ForegroundColor.b,
            ForegroundColor.a * 0.5f);
        Color frontLayer = new Color(
            ForegroundColor.r,
            ForegroundColor.g,
            ForegroundColor.b,
            ForegroundColor.a * 0.25f);

        BackLayer.color = backLayer;
        MidLayer1.color = midLayer;
        MidLayer2.color = midLayer;
        FrontLayer.color = frontLayer;

        BackLayer.sprite = ForegroundSprite;
        MidLayer1.sprite = ForegroundSprite;
        MidLayer2.sprite = ForegroundSprite;
        FrontLayer.sprite = ForegroundSprite;

        animator.SetInteger("CurrentRound", WaveManager.CurrentRound);
    }
}
