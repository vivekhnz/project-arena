using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ShieldGeneratorManager : MonoBehaviour
{
    public ShieldGeneratorController ShieldGenerator;
    public float SpawnIntervalSeconds = 10;
    public int MaxGeneratorCount = 3;

    private int generatorCount = 0;
    private float spawnTime;
    private Dictionary<Vector3, bool> spawnPoints;

    void Start()
    {
        // find all spawn point objects and store their position and availability
        var spawnPointObjects = GameObject.FindGameObjectsWithTag("ShieldGeneratorSpawnPoint");
        spawnPoints = spawnPointObjects.ToDictionary(p => p.transform.position, p => true);

        // spawn the maximum number of shield generators at the beginning
        for (int i = 0; i < MaxGeneratorCount; i++)
        {
            SpawnShieldGenerator();
        }
    }

    public void FixedUpdate()
    {
        // if there are fewer than the maximum number of generators, spawn generators at a certain interval
        if (generatorCount < MaxGeneratorCount && Time.time - spawnTime > SpawnIntervalSeconds)
        {
            SpawnShieldGenerator();

            // if there are still fewer than the max, reset the spawn timer
            if (generatorCount < MaxGeneratorCount)
            {
                spawnTime = Time.time;
            }
        }
    }

    public void SpawnShieldGenerator()
    {
        // we can only spawn a shield generator if a prefab has been specified and there are available spawn points
        var availableSpawnPoints = GetAvailableSpawnPoints();
        if (ShieldGenerator != null && availableSpawnPoints.Count > 0)
        {
            // create a new shield generator at a random available spawn point
            var generator = ShieldGenerator.Fetch<ShieldGeneratorController>();
            Vector3 spawnPos = availableSpawnPoints[Random.Range(0, availableSpawnPoints.Count)];
            generator.transform.position = spawnPos;
            // mark the spawn point as unavailable
            spawnPoints[spawnPos] = false;
            // subscribe to the generator's Destroyed event so we can mark the spawn point as available after it is destroyed
            generator.Destroyed += OnGeneratorDestroyed;

            generatorCount++;
        }
    }

    private void OnGeneratorDestroyed(object sender, System.EventArgs e)
    {
        var generator = sender as ShieldGeneratorController;
        // mark the spawn point as available
        spawnPoints[generator.transform.position] = true;

        // reset the spawn timer if this was the first generator destroyed
        if (generatorCount == MaxGeneratorCount)
        {
            spawnTime = Time.time;
        }
        generatorCount--;
    }

    private List<Vector3> GetAvailableSpawnPoints()
    {
        return (from spawnKVP in spawnPoints
                where spawnKVP.Value
                select spawnKVP.Key).ToList();
    }
}
