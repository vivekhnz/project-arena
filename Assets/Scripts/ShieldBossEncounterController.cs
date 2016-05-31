using UnityEngine;
using System.Collections;
using System;

public class ShieldBossEncounterController : MonoBehaviour
{
    public EnemySpawner EnemySpawner;
    public float EnemySpawnInterval = 10.0f;

    private ArenaManager arena;

    private float spawnTime;
    private bool initialized = false;

    void Start ()
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

        spawnTime = Time.time;
	}

	void Update ()
    {
        if (!initialized)
        {
            spawnTime = Time.time;
            initialized = true;
        }

        if (Time.time - spawnTime > EnemySpawnInterval)
        {
            CreateSpawner();
        }
	}

    void CreateSpawner()
    {
        //var enemyController = EnemySpawner.GetComponent<EnemySpawner>();
        //var spawner = enemyController.Fetch<EnemySpawner>();

        //// calculate spawner position around edge of arena
        //float angle = UnityEngine.Random.Range(0.0f, 360.0f) * Mathf.Deg2Rad;
        //Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

        //// create spawner
        //spawner.Initialize(direction * arena.ArenaRadius);

        // reset spawn timer
        spawnTime = Time.time;
    }
}
