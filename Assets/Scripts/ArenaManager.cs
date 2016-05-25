using UnityEngine;
using System.Collections;

public class ArenaManager : MonoBehaviour
{
    public GameObject BossEncounter;
    public GameObject WaveManager;
    public GameObject Player;

    public HUDController HUD;
    public float ArenaRadius = 50.0f;
    public Vector3 PlayerBossEncounterPosition;

    void Start()
    {
        BossEncounter.SetActive(false);
        WaveManager.SetActive(true);

        HUD.ShowWavesUI();
    }

    public void StartBossFight()
    {
        BossEncounter.SetActive(true);
        WaveManager.SetActive(false);

        // move the player to a safe location when the boss spawns
        Player.transform.position = PlayerBossEncounterPosition;

        HUD.ShowBossFightUI();
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
