using UnityEngine;
using System.Collections;
using System;

public class WaveManager : MonoBehaviour
{
    public EnemySpawner EnemySpawner;
    public float WaveDuration = 10.0f;
    public float SpawnerCreationInterval = 1.0f;
    public int SpawnersPerWave = 5;
    public float ArenaRadius = 10.0f;
    public int CurrentWave = 0;

    public event EventHandler WaveChanged;

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
                // create spawners within wave
                if (Time.time - spawnerTime > SpawnerCreationInterval)
                {
                    CreateSpawner();
                }
            }
            else if (Time.time - waveTime > WaveDuration)
            {
                // start wave spawn
                isCurrentlySpawning = true;
                spawnersCreated = 0;

                // update the current wave
                CurrentWave++;
                if (WaveChanged != null)
                {
                    WaveChanged(this, EventArgs.Empty);
                }

                // create the first spawner
                CreateSpawner();
            }
        }
    }

    void CreateSpawner()
    {
        var enemy = EnemySpawner.Fetch<EnemySpawner>();

        // calculate spawner position around edge of arena
        float angle = UnityEngine.Random.Range(0.0f, 360.0f) * Mathf.Deg2Rad;
        Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

        // create spawner and associate enemies with current wave
        enemy.Initialize(direction * ArenaRadius, CurrentWave);

        spawnersCreated++;
        spawnerTime = Time.time;

        if (spawnersCreated >= SpawnersPerWave)
        {
            // stop creating spawners
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
