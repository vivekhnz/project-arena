using UnityEngine;
using System.Collections;

public class ArenaManager : MonoBehaviour
{
    public GameObject BossEncounter;
    public GameObject WaveManager;

    public HUDController HUD;
    public float ArenaRadius = 50.0f;

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

        HUD.ShowBossFightUI();
    }

    void OnDrawGizmosSelected()
    {
        // draw arena radius in the Unity editor
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(Vector3.zero, ArenaRadius);
    }
}
