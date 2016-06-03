using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ArenaManager : MonoBehaviour
{
    public GameObject BossEncounter;
    public GameObject WaveManager;
    public GameObject Player;
    public GameStateManager GameStateManager;
    public GameObject AudioController;

    public HUDController HUD;
    public float ArenaRadius = 50.0f;
    public Vector3 PlayerBossEncounterPosition;
    public float DelayDuration = 10.0f;

    private float startTime;
    private bool hasStarted;

    void Start()
    {
        startTime = Time.time;
        hasStarted = false;

        BossEncounter.SetActive(false);
        WaveManager.SetActive(false);
        AudioController.SetActive(false);

        HUD.ShowWavesUI();
    }

    void FixedUpdate()
    {
        if (!hasStarted && Time.time - startTime > DelayDuration)
        {
            WaveManager.SetActive(true);
            AudioController.SetActive(true);
            hasStarted = true;
        }
    }

    public void StartBossFight()
    {
        if (GameStateManager != null)
        {
            ApplicationModel.RegisterGameScore(GameStateManager.Score);
        }
        SceneManager.LoadScene("VictoryScene");

        //BossEncounter.SetActive(true);
        //WaveManager.SetActive(false);

        //// move the player to a safe location when the boss spawns
        //Player.transform.position = PlayerBossEncounterPosition;

        //HUD.ShowBossFightUI();
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
