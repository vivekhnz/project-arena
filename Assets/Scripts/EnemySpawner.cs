using UnityEngine;
using System.Collections;
using System;

public class EnemySpawner : PooledObject
{
    public EnemyController Enemy;
    public float EnemySpawnInterval = 1.0f;
    public int EnemiesPerSpawn = 5;

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
    private int enemiesSpawned = 0;

    public float Lifetime
    {
        get { return EnemySpawnInterval * EnemiesPerSpawn; }
    }

    public void Initialize(Vector3 position)
    {
        transform.position = position;
    }

    public override void ResetInstance()
    {
        enemySpawnTime = Time.time;
        enemiesSpawned = 0;

        base.ResetInstance();
    }

    void Update ()
    {
        // spawn enemies
        if (Enemy != null && Time.time - enemySpawnTime > EnemySpawnInterval)
        {
            SpawnEnemy();
        }
	}

    void SpawnEnemy()
    {
        // create enemy
        var enemy = Enemy.Fetch<EnemyController>();
        enemy.Initialize(transform.position);

        // notify subscribers that a new enemy has been spawned
        if (EnemySpawned != null)
        {
            EnemySpawned(this, new EnemySpawnedEventArgs(enemy));
        }

        enemiesSpawned++;
        enemySpawnTime = Time.time;

        if (enemiesSpawned >= EnemiesPerSpawn)
        {
            // delete spawner once all enemies have been spawned
            Recycle();
        }
    }
}
