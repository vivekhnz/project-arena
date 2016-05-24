using UnityEngine;
using System.Collections;

[RequireComponent(typeof(EnemySpawner))]
public class WaveEnemySpawner : MonoBehaviour
{
    public EnemySpawner Spawner { get; private set; }
    private int wave;

    void Start()
    {
        Spawner = GetComponent<EnemySpawner>();
        Spawner.EnemySpawned += OnEnemySpawned;
    }

    public void Initialize(int wave)
    {
        this.wave = wave;
    }

    private void OnEnemySpawned(object sender, EnemySpawner.EnemySpawnedEventArgs e)
    {
        var waveEnemyController = e.Enemy.GetComponent<WaveEnemyController>();
        if (waveEnemyController != null)
        {
            // associate it with the wave this spawner was created for
            waveEnemyController.Initialize(wave);
        }
    }
}
