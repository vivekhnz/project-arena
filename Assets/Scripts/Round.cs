using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[Serializable]
public class Round
{
    public List<WaveSpawns> Waves = new List<WaveSpawns>();
}

[Serializable]
public class WaveSpawns
{
    public int SpawnerCount = 3;
    public float WaveDuration = 10.0f;
    public float SpawnerCreationInterval = 2.0f;

    public float EnemySpawnInterval = 0.5f;
    public List<EnemySpawn> Spawns = new List<EnemySpawn>();
}