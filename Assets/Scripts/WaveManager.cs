using UnityEngine;
using System.Collections;

public class WaveManager : MonoBehaviour
{
    public EnemySpawner EnemySpawner;
    public float WaveDuration = 10.0f;
    public float SpawnerCreationInterval = 1.0f;
    public int SpawnersPerWave = 5;
    public float ArenaRadius = 10.0f;
    public int CurrentWave = 0;

    private float waveTime;
    private float spawnerTime;
    private bool isCurrentlySpawning = false;
    private int spawnersCreated = 0;

    void Start()
    {
        waveTime = Time.time - WaveDuration;
        spawnerTime = Time.time;
    }

    void Update()
    {
        if (EnemySpawner != null)
        {
            if (isCurrentlySpawning)
            {
                if (Time.time - spawnerTime > SpawnerCreationInterval)
                {
                    CreateSpawner();
                }
            }
            else if (Time.time - waveTime > WaveDuration)
            {
                isCurrentlySpawning = true;
                spawnersCreated = 0;
                CurrentWave++;
                CreateSpawner();
            }
        }
    }

    void CreateSpawner()
    {
        var enemy = EnemySpawner.Fetch<EnemySpawner>();

        // calculate spawner position around edge of arena
        float angle = Random.Range(0.0f, 360.0f) * Mathf.Deg2Rad;
        Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        enemy.transform.position = direction * ArenaRadius;

        spawnersCreated++;
        spawnerTime = Time.time;

        if (spawnersCreated >= SpawnersPerWave)
        {
            isCurrentlySpawning = false;
            waveTime = Time.time;
        }
    }

    void OnDrawGizmosSelected()
    {
        // draw arena radius in the Unity editor
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(Vector3.zero, ArenaRadius);
    }
}
