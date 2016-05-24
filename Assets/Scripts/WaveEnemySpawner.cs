using UnityEngine;
using System.Collections;

public class WaveEnemySpawner : MonoBehaviour
{
    private int wave;

    public void Initialize(int wave)
    {
        this.wave = wave;
    }

    public void AssociateEnemyWithWave(EnemyController enemy)
    {
        enemy.AssociateWithWave(wave);
    }
}
