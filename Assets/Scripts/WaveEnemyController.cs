using UnityEngine;
using System.Collections;
using System;

public class WaveEnemyController : MonoBehaviour
{
    private WaveManager waveManager;
    private int wave;

    public event EventHandler WaveEnded;

    public void Initialize(int wave)
    {
        this.wave = wave;

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

    public void Cleanup()
    {
        // unhook from events so we don't get duplicate notifications when this object is pooled
        if (waveManager != null)
        {
            waveManager.WaveChanged -= OnWaveChanged;
        }
    }

    private void OnWaveChanged(object sender, WaveManager.WaveChangedEventArgs e)
    {
        // notify subscribers if this enemy's wave has ended
        if (e.PreviousWave.WaveNumber >= wave && WaveEnded != null)
        {
            WaveEnded(this, EventArgs.Empty);
        }
    }

    public void NotifyEnemyDestroyed()
    {
        if (waveManager != null)
        {
            waveManager.NotifyEnemyDestroyed(wave);
        }
    }

    public void NotifyEnemyEscaped()
    {
        if (waveManager != null)
        {
            waveManager.NotifyEnemyEscaped(wave);
        }
    }
}