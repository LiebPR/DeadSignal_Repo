// GameDirector.cs
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyUnlock
{
    public string enemyTag;
    public int unlockCycle;
}

public class GameDirector : MonoBehaviour
{
    [SerializeField] EnemySpawner spawner;
    [SerializeField] WaveUI waveUI;
    [SerializeField] WaveAlertUI waveAlertUI;
    [SerializeField] List<EnemyUnlock> enemyUnlocks;

    const float cycleDuration = 120f;
    int currentCycle = -1;

    float nextNormalSpawn;
    float nextWaveSpawn;
    bool cycleCompletedDisplayed = false;

    private void Update()
    {
        float t = GameTimer.TimeElapsed % cycleDuration;
        int cycle = Mathf.FloorToInt(GameTimer.TimeElapsed / cycleDuration);

        if (cycle != currentCycle)
        {
            currentCycle = cycle;
            UpdateEnemyPool(cycle);
            cycleCompletedDisplayed = false;
        }

        if (t < 57f)
        {
            waveAlertUI.Play("HOSTILE SWARM DETECTED");
            TrySpawnNormal();
        }
        else if (t < 87f)
        {
            TrySpawnWave();
        }
        else
        {
            if (!cycleCompletedDisplayed)
            {
                waveUI.ShowWave(currentCycle, currentCycle + 1);
                cycleCompletedDisplayed = true;
            }
        }
    }

    void TrySpawnNormal()
    {
        if (GameTimer.TimeElapsed >= nextNormalSpawn)
        {
            spawner.SpawnNormal(2);
            nextNormalSpawn = GameTimer.TimeElapsed + 3f;
        }
    }

    void TrySpawnWave()
    {
        if (GameTimer.TimeElapsed >= nextWaveSpawn)
        {
            spawner.SpawnWave(3);
            nextWaveSpawn = GameTimer.TimeElapsed + 1f;
        }
    }

    void UpdateEnemyPool(int cycle)
    {
        List<string> activeTags = new List<string>();
        foreach (var e in enemyUnlocks)
        {
            if (cycle >= e.unlockCycle)
                activeTags.Add(e.enemyTag);
        }
        spawner.SetActiveEnemies(activeTags);
    }
}