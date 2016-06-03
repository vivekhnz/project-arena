using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ArenaManager : MonoBehaviour
{
    public GameObject BossEncounter;
    public GameObject WaveManager;
    public GameObject Player;
    public GameStateManager GameStateManager;
    public AudioController AudioController;

    public HUDController HUD;
    public float ArenaRadius = 50.0f;
    public Vector3 PlayerBossEncounterPosition;
    public float DelayDuration = 10.0f;
    public float FinishDuration = 5.0f;

    private float startTime;
    private bool hasStarted;

    private bool hasFinished;
    private float finishTime;

    void Start()
    {
        startTime = Time.time;
        hasStarted = false;
        hasFinished = false;

        BossEncounter.SetActive(false);
        WaveManager.SetActive(false);
        AudioController.gameObject.SetActive(false);

        HUD.ShowWavesUI();
    }

    void FixedUpdate()
    {
        if (!hasStarted && Time.time - startTime > DelayDuration)
        {
            WaveManager.SetActive(true);
            AudioController.gameObject.SetActive(true);
            hasStarted = true;
        }
        if (hasFinished && Time.time - finishTime > FinishDuration)
        {
            if (GameStateManager != null)
            {
                ApplicationModel.RegisterGameScore(GameStateManager.Score);
            }
            SceneManager.LoadScene("VictoryScene");
        }
    }

    public void Finish()
    {
        AudioController.FadeOut();
        HUD.HideRoundOverlay();
        hasFinished = true;
        finishTime = Time.time;
    }

    void OnDrawGizmosSelected()
    {
        // draw arena radius in the Unity editor
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(Vector3.zero, ArenaRadius);

        // draw player boss encounter position
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(PlayerBossEncounterPosition, 2.0f);
    }
}
