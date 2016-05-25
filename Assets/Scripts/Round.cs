using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[Serializable]
public class Round
{
    public EnemySpawner EnemySpawner;
    public float WaveDuration = 8.0f;
    public int WaveCount = 5;
    public int SpawnersPerWave = 5;
    public float SpawnerCreationInterval = 2.0f;
}