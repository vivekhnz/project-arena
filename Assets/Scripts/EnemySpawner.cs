using UnityEngine;
using System.Collections;

public class EnemySpawner : PooledObject
{
    public EnemyController Enemy;
    public float EnemySpawnInterval = 1.0f;
    public int EnemiesPerSpawn = 5;

    private WaveEnemySpawner waveEnemySpawner;
    
    private float enemySpawnTime;
    private int enemiesSpawned = 0;

    public float Lifetime
    {
        get { return EnemySpawnInterval * EnemiesPerSpawn; }
    }

    public void Initialize(Vector3 position, int wave)
    {
        transform.position = position;
        if (waveEnemySpawner != null)
        {
            waveEnemySpawner.Initialize(wave);
        }
    }

    public override void ResetInstance()
    {
        enemySpawnTime = Time.time;
        enemiesSpawned = 0;

        if (waveEnemySpawner == null)
        {
            waveEnemySpawner = GetComponent<WaveEnemySpawner>();
        }

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
        // create enemy and associate it with the wave this spawner was created for
        var enemy = Enemy.Fetch<EnemyController>();
        enemy.Initialize(transform.position);
        waveEnemySpawner.AssociateEnemyWithWave(enemy);

        enemiesSpawned++;
        enemySpawnTime = Time.time;

        if (enemiesSpawned >= EnemiesPerSpawn)
        {
            // delete spawner once all enemies have been spawned
            Recycle();
        }
    }
}
