using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(EnemyController))]
public class WaveEnemyController : MonoBehaviour
{
    private EnemyController enemyController;

    private WaveManager waveManager;
    private int wave;

    void Start()
    {
        enemyController = GetComponent<EnemyController>();
        enemyController.EnemyDestroyed += OnEnemyDestroyed;
        enemyController.EnemyEscaped += OnEnemyEscaped;

        // subscribe to wave changed event so we are notified when this enemy's wave has ended
        var waveManagerObj = GameObject.FindGameObjectWithTag("WaveManager");
        if (waveManagerObj != null)
        {
            waveManager = waveManagerObj.GetComponent<WaveManager>();
            if (waveManager != null)
            {
                waveManager.WaveChanged += OnWaveChanged;
                waveManager.NotifyEnemyCreated(wave);
            }
        }
    }

    public void Initialize(int wave)
    {
        this.wave = wave;
    }

    private void OnEnemyDestroyed(object sender, EventArgs e)
    {
        if (waveManager != null)
        {
            waveManager.NotifyEnemyDestroyed(wave);
        }
    }

    private void OnEnemyEscaped(object sender, EventArgs e)
    {
        if (waveManager != null)
        {
            waveManager.NotifyEnemyEscaped(wave);
        }
    }

    private void OnWaveChanged(object sender, WaveManager.WaveChangedEventArgs e)
    {
        // tell the enemy controller to escape once it's wave has ended
        if (e.PreviousWave.WaveNumber >= wave)
        {
            enemyController.Escape();
        }
    }

    void OnDestroy()
    {
        // unhook from events so we don't get duplicate notifications when the enemy is pooled
        enemyController.EnemyDestroyed -= OnEnemyDestroyed;
        enemyController.EnemyEscaped -= OnEnemyEscaped;

        if (waveManager != null)
        {
            waveManager.WaveChanged -= OnWaveChanged;
        }
    }
}