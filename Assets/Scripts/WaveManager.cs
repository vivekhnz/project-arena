using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

public class WaveManager : MonoBehaviour
{
    public EnemySpawner EnemySpawner;
    public float RoundIntermissionDuration = 10.0f;
    public List<Round> Rounds = new List<Round>();

    public List<WaveResults> WaveResults = new List<WaveResults>();
    public int CurrentRound { get; private set; }
    public int CurrentWave { get; private set; }

    public class WaveChangedEventArgs : EventArgs
    {
        public WaveResults PreviousWave { get; private set; }
        public WaveResults NewWave { get; private set; }

        public WaveChangedEventArgs(WaveResults prevWave, WaveResults newWave)
        {
            PreviousWave = prevWave;
            NewWave = newWave;
        }
    }
    public event EventHandler<WaveChangedEventArgs> WaveChanged;

    public event EventHandler RoundCompleted;
    public event EventHandler RoundStarted;

    private ArenaManager arena;

    private float waveTime;
    private float spawnerTime;
    private bool isCurrentlySpawning = false;
    private int spawnersCreated = 0;

    private bool isEndOfRound = false;
    private bool isRoundTransitioning = false;
    private float roundTime;
    private bool allRoundsComplete = false;

    public bool AllRoundsComplete
    {
        get { return allRoundsComplete; }
    }

    void Start()
    {
        var arenaObj = GameObject.FindGameObjectWithTag("ArenaManager");
        if (arenaObj == null)
        {
            throw new Exception("Could not find ArenaManager!");
        }
        else
        {
            arena = arenaObj.GetComponent<ArenaManager>();
        }

        waveTime = Time.time - Rounds[0].Waves[0].WaveDuration;
        spawnerTime = Time.time;
        roundTime = Time.time;
        
        WaveResults = new List<WaveResults>();
        CurrentRound = 0;
        CurrentWave = 0;

        StartNewRound();
    }

    void FixedUpdate()
    {
        if (allRoundsComplete)
        {
            return;
        }
        if (isEndOfRound)
        {
            if (isRoundTransitioning)
            {
                // wait until the intermission is complete
                if (Time.time - roundTime > RoundIntermissionDuration)
                {
                    if (CurrentRound == Rounds.Count)
                    {
                        // start the boss fight
                        arena.StartBossFight();

                        // don't progress to the next wave
                        allRoundsComplete = true;
                    }
                    else
                    {
                        // start a new round
                        StartNewRound();
                    }
                }
            }
            else
            {
                // wait until all enemies are destroyed before starting the intermission
                bool allEnemiesInactive = true;
                foreach (var wave in WaveResults)
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
        else if (CurrentRound > 0)
        {
            Round currentRound = Rounds[CurrentRound - 1];
            WaveSpawns currentWave = currentRound.Waves[CurrentWave - 1];
            if (currentWave.Spawns != null)
            {
                if (isCurrentlySpawning)
                {
                    // create spawners within wave
                    if (Time.time - spawnerTime > currentWave.SpawnerCreationInterval)
                    {
                        CreateSpawner();
                    }
                }
                else if (Time.time - waveTime > currentWave.WaveDuration)
                {
                    // if this is the final wave, finish the round
                    if (CurrentWave >= currentRound.Waves.Count)
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
    }

    private void FinishRound()
    {
        isEndOfRound = true;
        isRoundTransitioning = false;
        if (WaveChanged != null)
        {
            WaveChanged(this, new WaveChangedEventArgs(WaveResults[CurrentWave - 1], null));
        }
    }

    private void StartNewRound()
    {
        // clear existing waves and notify the UI that a new round has started
        WaveResults.Clear();
        if (RoundStarted != null)
        {
            RoundStarted(this, EventArgs.Empty);
        }

        // notify that a new wave has started
        isEndOfRound = false;
        if (WaveChanged != null)
        {
            WaveChanged(this, new WaveChangedEventArgs(WaveResults[CurrentWave - 1], null));
        }

        CurrentRound++;
        CurrentWave = 0;

        // start a new wave
        StartNewWave();
    }

    private void StartNewWave()
    {
        // start wave spawn
        isCurrentlySpawning = true;
        spawnersCreated = 0;

        // create a new wave
        WaveResults newWave = new WaveResults(CurrentWave + 1);
        if (WaveChanged != null)
        {
            WaveChanged(this, new WaveChangedEventArgs(WaveResults[CurrentWave - 1], newWave));
        }
        WaveResults.Add(newWave);
        CurrentWave++;

        // create the first spawner
        CreateSpawner();
    }

    void CreateSpawner()
    {
        var currentWaveSpawns = Rounds[CurrentRound - 1].Waves[CurrentWave - 1];
        
        var spawner = EnemySpawner.Fetch<EnemySpawner>();

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
        spawner.Initialize(direction * arena.ArenaRadius,
            currentWaveSpawns.EnemySpawnInterval, currentWaveSpawns.Spawns);
        wes.Initialize(CurrentWave);

        spawnersCreated++;
        spawnerTime = Time.time;

        if (spawnersCreated == currentWaveSpawns.SpawnerCount)
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
        WaveResults[wave - 1].TotalEnemyCount++;
    }

    public void NotifyEnemyDestroyed(int wave)
    {
        WaveResults[wave - 1].EnemiesDestroyed++;
    }

    public void NotifyEnemyEscaped(int wave)
    {
        WaveResults[wave - 1].EnemiesEscaped++;
    }
}
