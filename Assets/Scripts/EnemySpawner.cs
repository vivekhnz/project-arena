using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

public class EnemySpawner : PooledObject
{
    public float EnemySpawnInterval = 1.0f;
    public List<EnemySpawn> Spawns = new List<EnemySpawn>();

    public class EnemySpawnedEventArgs : EventArgs
    {
        public EnemyController Enemy { get; private set; }

        public EnemySpawnedEventArgs(EnemyController enemy)
        {
            Enemy = enemy;
        }
    }
    public event EventHandler<EnemySpawnedEventArgs> EnemySpawned;

    private float enemySpawnTime;
    private int[] enemiesSpawned;

    public float Lifetime
    {
        get { return EnemySpawnInterval
                * Spawns.Sum(p => p.EnemiesPerSpawn); }
    }

    public void Initialize(Vector3 position)
    {
        transform.position = position;
    }

    public override void ResetInstance()
    {
        enemySpawnTime = Time.time;
        enemiesSpawned = new int[Spawns.Count];

        base.ResetInstance();
    }

    void Update ()
    {
        // spawn enemies
        if (Spawns.Count > 0 && Time.time - enemySpawnTime > EnemySpawnInterval)
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
            var enemy = Spawns[enemyType].Enemy.Fetch<EnemyController>();
            enemy.Initialize(transform.position);

            // notify subscribers that a new enemy has been spawned
            if (EnemySpawned != null)
            {
                EnemySpawned(this, new EnemySpawnedEventArgs(enemy));
            }

            enemiesSpawned[enemyType]++;
            enemySpawnTime = Time.time;
        }

        if (enemiesSpawned.Sum() >= Spawns.Sum(p => p.EnemiesPerSpawn))
        {
            // delete spawner once all enemies have been spawned
            Recycle();
        }
    }

    private int PickEnemyTypeToSpawn()
    {
        for (int i = 0; i < Spawns.Count; i++)
        {
            var spawn = Spawns[i];
            if (enemiesSpawned[i] < spawn.EnemiesPerSpawn)
            {
                return i;
            }
        }
        return -1;
    }
}
