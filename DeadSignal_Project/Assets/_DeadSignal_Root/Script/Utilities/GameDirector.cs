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
    [SerializeField] WaveUIManager waveUI;
    [SerializeField] List<EnemyUnlock> enemyUnlocks;

    const float cycleDuration = 120f;
    int currentCycle = -1;
    float nextSpawn;
    bool enemiesCleared;

    bool cycleCompletedDisplayed = false;

    private void Update()
    {
        float t = GameTimer.TimeElapsed % cycleDuration;

        int cycle = Mathf.FloorToInt(GameTimer.TimeElapsed / cycleDuration);

        if (cycle != currentCycle)
        {
            currentCycle = cycle;
            UpdateEnemyPool(cycle);

            // Reset flags al inicio de cada ciclo
            enemiesCleared = false;
            cycleCompletedDisplayed = false;
        }

        //Spawn normal
        if (t < 57f)
        {
            TrySpawnNormal();
        }
        //Limpieza
        else if (t < 60f)
        {
            if (!enemiesCleared)
            {
                spawner.KillAllEnemies();
                enemiesCleared = true;
            }
        }
        //Oleada
        else if (t < 87f)
        {
            TrySpawnWave();
        }
        //Pausa NPC
        else
        {
            if (!cycleCompletedDisplayed)
            {
                // Limpieza final de la zona por si quedó algún enemigo
                spawner.KillAllEnemies();

                // Mostrar el texto de la oleada actual
                waveUI.ShowWave(currentCycle, currentCycle + 1); // +1 para que la primera oleada sea Wave 1

                cycleCompletedDisplayed = true;
                enemiesCleared = true; // Reforzar que la zona está limpia
            }
        }
    }

    void TrySpawnNormal()
    {
        if (GameTimer.TimeElapsed >= nextSpawn)
        {
            spawner.SpawnNormal(2);
            nextSpawn = GameTimer.TimeElapsed + 3f;
        }
    }

    void TrySpawnWave()
    {
        if (GameTimer.TimeElapsed >= nextSpawn)
        {
            spawner.SpawnWave(3);
            nextSpawn = GameTimer.TimeElapsed + 1f;
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
