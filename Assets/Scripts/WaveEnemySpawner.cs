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

        // subscribe to enemy spawn events so we can associate new enemies with a wave
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

    void OnDestroy()
    {
        // unhook from the enemy spawn event so we don't get duplicate notifications when the spawner is pooled
        Spawner.EnemySpawned -= OnEnemySpawned;
    }
}
