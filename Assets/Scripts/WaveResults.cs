using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class WaveResults
{
    public WaveResults(int wave)
    {
        WaveNumber = wave;
    }

    public int WaveNumber { get; private set; }
    public int TotalEnemyCount { get; set; }
    public int EnemiesDestroyed { get; set; }
    public int EnemiesEscaped { get; set; }

    public bool AllEnemiesInactive
    {
        get { return EnemiesDestroyed + EnemiesEscaped >= TotalEnemyCount; }
    }
    public bool IsPerfect
    {
        get { return EnemiesDestroyed == TotalEnemyCount; }
    }
}