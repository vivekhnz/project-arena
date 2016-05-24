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
        // add a WaveEnemyController component to this enemy if it does not have one already
        var waveEnemyController = e.Enemy.GetComponent<WaveEnemyController>();
        if (waveEnemyController == null)
        {
            waveEnemyController = e.Enemy.gameObject.AddComponent<WaveEnemyController>();
        }

        // associate it with the wave this spawner was created for
        waveEnemyController.Initialize(wave);

        // remove the WaveEnemyController component from this enemy after it is recycled
        e.Enemy.InstanceRecycled += OnEnemyRecycled;
    }

    private void OnEnemyRecycled(object sender, System.EventArgs e)
    {
        EnemyController enemy = sender as EnemyController;

        // remove the WaveEnemyController component from the enemy
        WaveEnemyController wec = enemy.GetComponent<WaveEnemyController>();
        Destroy(wec);

        // unhook the event
        enemy.InstanceRecycled -= OnEnemyRecycled;
    }

    void OnDestroy()
    {
        // unhook from the enemy spawn event so we don't get duplicate notifications when the spawner is pooled
        Spawner.EnemySpawned -= OnEnemySpawned;
    }
}
