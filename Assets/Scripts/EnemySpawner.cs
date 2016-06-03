using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

public class EnemySpawner : PooledObject
{
    public class EnemySpawnedEventArgs : EventArgs
    {
        public EnemyController Enemy { get; private set; }

        public EnemySpawnedEventArgs(EnemyController enemy)
        {
            Enemy = enemy;
        }
    }
    public event EventHandler<EnemySpawnedEventArgs> EnemySpawned;

    public HazardController Hazard;

    private float enemySpawnInterval = 1.0f;
    private List<EnemySpawn> spawns = new List<EnemySpawn>();
    private float enemySpawnTime;
    private int[] enemiesSpawned;

    public float Lifetime
    {
        get { return enemySpawnInterval * spawns.Sum(p => p.EnemiesPerSpawn); }
    }

    public void Initialize(Vector3 position, float enemySpawnInterval, List<EnemySpawn> spawns)
    {
        transform.position = position;
        this.enemySpawnInterval = enemySpawnInterval;
        this.spawns = spawns;

        enemiesSpawned = new int[spawns.Count];

        var hazard = Hazard.Fetch<HazardController>();
        hazard.Initialize(transform.position);
    }

    public override void ResetInstance()
    {
        enemySpawnTime = Time.time;

        base.ResetInstance();
    }

    void Update ()
    {
        // spawn enemies
        if (spawns.Count > 0 && Time.time - enemySpawnTime > enemySpawnInterval)
        {
            SpawnEnemy();
        }
	}

    void SpawnEnemy()
    {
        // pick an enemy type to spawn
        int enemyType = PickEnemyTypeToSpawn();

        if (enemyType != -1)
        {
            // create enemy
            var enemy = spawns[enemyType].Enemy.Fetch<EnemyController>();
            enemy.Initialize(transform.position);

            // notify subscribers that a new enemy has been spawned
            if (EnemySpawned != null)
            {
                EnemySpawned(this, new EnemySpawnedEventArgs(enemy));
            }

            enemiesSpawned[enemyType]++;
            enemySpawnTime = Time.time;
        }

        if (enemiesSpawned.Sum() >= spawns.Sum(p => p.EnemiesPerSpawn))
        {
            // delete spawner once all enemies have been spawned
            Recycle();
        }
    }

    private int PickEnemyTypeToSpawn()
    {
        for (int i = 0; i < spawns.Count; i++)
        {
            var spawn = spawns[i];
            if (enemiesSpawned[i] < spawn.EnemiesPerSpawn)
            {
                return i;
            }
        }
        return -1;
    }
}
