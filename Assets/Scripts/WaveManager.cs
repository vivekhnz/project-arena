using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

public class WaveManager : MonoBehaviour
{
    public EnemySpawner EnemySpawner;
    public float WaveDuration = 10.0f;
    public float SpawnerCreationInterval = 1.0f;
    public int SpawnersPerWave = 5;
    public float ArenaRadius = 10.0f;
    public int WavesPerRound = 5;
    public float RoundIntermissionDuration = 10.0f;

    public List<Wave> Waves = new List<Wave>();
    public Wave CurrentWave
    {
        get { return Waves.LastOrDefault(); }
    }

    public class WaveChangedEventArgs : EventArgs
    {
        public Wave PreviousWave { get; private set; }
        public Wave NewWave { get; private set; }

        public WaveChangedEventArgs(Wave prevWave, Wave newWave)
        {
            PreviousWave = prevWave;
            NewWave = newWave;
        }
    }
    public event EventHandler<WaveChangedEventArgs> WaveChanged;

    private float waveTime;
    private float spawnerTime;
    private bool isCurrentlySpawning = false;
    private int spawnersCreated = 0;
    private bool isEndOfRound = false;

    void Start()
    {
        waveTime = Time.time - WaveDuration;
        spawnerTime = Time.time;
        
        Waves = new List<Wave>();
    }

    void Update()
    {
        if (isEndOfRound)
        {
            bool allEnemiesInactive = true;
            foreach (var wave in Waves)
            {
                if (!wave.AllEnemiesInactive)
                {
                    allEnemiesInactive = false;
                    break;
                }
            }
            if (allEnemiesInactive)
            {
                print("round end!");
            }
        }
        else if (EnemySpawner != null)
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
                if (CurrentWave != null && CurrentWave.WaveNumber >= WavesPerRound)
                {
                    isEndOfRound = true;
                    if (WaveChanged != null)
                    {
                        WaveChanged(this, new WaveChangedEventArgs(CurrentWave, null));
                    }
                }
                else
                {
                    // start wave spawn
                    isCurrentlySpawning = true;
                    spawnersCreated = 0;

                    // create a new wave
                    Wave newWave = new Wave(CurrentWave == null ? 1 : CurrentWave.WaveNumber + 1);
                    if (WaveChanged != null)
                    {
                        WaveChanged(this, new WaveChangedEventArgs(CurrentWave, newWave));
                    }
                    Waves.Add(newWave);

                    // create the first spawner
                    CreateSpawner();
                }
            }
        }
    }

    void CreateSpawner()
    {
        var spawner = EnemySpawner.Fetch<EnemySpawner>();

        // calculate spawner position around edge of arena
        float angle = UnityEngine.Random.Range(0.0f, 360.0f) * Mathf.Deg2Rad;
        Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

        // create spawner and associate enemies with current wave
        spawner.Initialize(direction * ArenaRadius, CurrentWave.WaveNumber);

        spawnersCreated++;
        spawnerTime = Time.time;

        if (spawnersCreated == SpawnersPerWave)
        {
            // stop creating spawners
            isCurrentlySpawning = false;
            waveTime = Time.time + spawner.Lifetime;
        }
    }

    public void NotifyEnemyCreated(int wave)
    {
        Waves[wave - 1].TotalEnemyCount++;
    }

    public void NotifyEnemyDestroyed(int wave)
    {
        Waves[wave - 1].EnemiesDestroyed++;
    }

    public void NotifyEnemyEscaped(int wave)
    {
        Waves[wave - 1].EnemiesEscaped++;
    }

    void OnDrawGizmosSelected()
    {
        // draw arena radius in the Unity editor
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(Vector3.zero, ArenaRadius);
    }
}
