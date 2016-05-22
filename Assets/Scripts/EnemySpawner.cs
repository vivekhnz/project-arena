using UnityEngine;
using System.Collections;

public class EnemySpawner : PooledObject
{
    public SeekerEnemyController Enemy;
    public float EnemySpawnInterval = 1.0f;
    public int EnemiesPerSpawn = 5;
    
    private float enemySpawnTime;
    private int enemiesSpawned = 0;

    public override void ResetInstance()
    {
        enemySpawnTime = Time.time;
        enemiesSpawned = 0;

        base.ResetInstance();
    }

    void Update ()
    {
        if (Enemy != null && Time.time - enemySpawnTime > EnemySpawnInterval)
        {
            SpawnEnemy();
        }
	}

    void SpawnEnemy()
    {
        var enemy = Enemy.Fetch<SeekerEnemyController>();
        enemy.transform.position = transform.position;

        enemiesSpawned++;
        enemySpawnTime = Time.time;

        if (enemiesSpawned >= EnemiesPerSpawn)
        {
            Recycle();
        }
    }
}
