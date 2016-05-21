using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    public SeekerEnemyController Enemy;
    public float TimeBetweenSpawns = 5.0f;
    public float EnemySpawnInterval = 1.0f;
    public int EnemiesPerSpawn = 5;

    private float spawnTime;
    private float enemySpawnTime;
    private bool isCurrentlySpawning = false;
    private int enemiesSpawned = 0;

	void Start ()
    {
        spawnTime = Time.time;
        enemySpawnTime = Time.time;
	}

	void Update ()
    {
        if (Enemy != null)
        {
            if (isCurrentlySpawning)
            {
                if (Time.time - enemySpawnTime > EnemySpawnInterval)
                {
                    SpawnEnemy();
                }
            }
            else if (Time.time - spawnTime > TimeBetweenSpawns)
            {
                isCurrentlySpawning = true;
                enemiesSpawned = 0;
                SpawnEnemy();
            }
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
            isCurrentlySpawning = false;
            spawnTime = Time.time;
        }
    }
}
