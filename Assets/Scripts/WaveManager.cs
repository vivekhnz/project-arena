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
    public int CurrentRound { get; private set; }

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

    public event EventHandler RoundCompleted;
    public event EventHandler RoundStarted;

    private float waveTime;
    private float spawnerTime;
    private bool isCurrentlySpawning = false;
    private int spawnersCreated = 0;

    private bool isEndOfRound = false;
    private bool isRoundTransitioning = false;
    private float roundTime;

    void Start()
    {
        waveTime = Time.time - WaveDuration;
        spawnerTime = Time.time;
        roundTime = Time.time;
        
        Waves = new List<Wave>();
        CurrentRound = 1;
    }

    void Update()
    {
        if (isEndOfRound)
        {
            if (isRoundTransitioning)
            {
                // start a new round after the intermission is complete
                if (Time.time - roundTime > RoundIntermissionDuration)
                {
                    StartNewRound();
                }
            }
            else
            {
                // wait until all enemies are destroyed before starting the intermission
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
                    // start the round intermission
                    isRoundTransitioning = true;
                    roundTime = Time.time;
                    if (RoundCompleted != null)
                    {
                        RoundCompleted(this, EventArgs.Empty);
                    }
                }
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
                // if this is the final wave, finish the round
                if (CurrentWave != null && CurrentWave.WaveNumber >= WavesPerRound)
                {
                    FinishRound();
                }
                else
                {
                    // otherwise, start a new wave
                    StartNewWave();
                }
            }
        }
    }

    private void FinishRound()
    {
        isEndOfRound = true;
        isRoundTransitioning = false;
        if (WaveChanged != null)
        {
            WaveChanged(this, new WaveChangedEventArgs(CurrentWave, null));
        }
    }

    private void StartNewRound()
    {
        // clear existing waves and notify the UI that a new round has started
        Waves.Clear();
        if (RoundStarted != null)
        {
            RoundStarted(this, EventArgs.Empty);
        }

        // notify that a new wave has started
        isEndOfRound = false;
        if (WaveChanged != null)
        {
            WaveChanged(this, new WaveChangedEventArgs(CurrentWave, null));
        }

        CurrentRound++;

        // start a new wave
        StartNewWave();
    }

    private void StartNewWave()
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

    void CreateSpawner()
    {
        var enemyController = EnemySpawner.GetComponent<EnemySpawner>();
        var spawner = enemyController.Fetch<EnemySpawner>();

        // add a WaveEnemySpawner component to this spawner if it does not have one already
        var wes = spawner.GetComponent<WaveEnemySpawner>();
        if (wes == null)
        {
            wes = spawner.gameObject.AddComponent<WaveEnemySpawner>();
        }

        // remove the WaveEnemySpawner component from this spawner after it is recycled
        spawner.InstanceRecycled += OnSpawnerRecycled;

        // calculate spawner position around edge of arena
        float angle = UnityEngine.Random.Range(0.0f, 360.0f) * Mathf.Deg2Rad;
        Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

        // create spawner and associate enemies with current wave
        spawner.Initialize(direction * ArenaRadius);
        wes.Initialize(CurrentWave.WaveNumber);

        spawnersCreated++;
        spawnerTime = Time.time;

        if (spawnersCreated == SpawnersPerWave)
        {
            // stop creating spawners
            isCurrentlySpawning = false;
            waveTime = Time.time + spawner.Lifetime;
        }
    }

    private void OnSpawnerRecycled(object sender, EventArgs e)
    {
        EnemySpawner spawner = sender as EnemySpawner;

        // remove the WaveEnemySpawner component from the spawner
        WaveEnemySpawner wes = spawner.GetComponent<WaveEnemySpawner>();
        Destroy(wes);

        // unhook the event
        spawner.InstanceRecycled -= OnSpawnerRecycled;
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
