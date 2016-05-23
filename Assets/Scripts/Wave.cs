using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Wave
{
    public Wave(int wave)
    {
        WaveNumber = wave;
    }

    public int WaveNumber { get; private set; }
    public int TotalEnemyCount { get; set; }
    public int EnemiesDestroyed { get; set; }
}