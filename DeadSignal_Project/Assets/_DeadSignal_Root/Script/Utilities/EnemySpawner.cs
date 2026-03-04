using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    #region Configuración
    [Header("Spawn Settings")]
    [SerializeField] float baseSpawnInterval = 3f;       // Intervalo inicial entre spawns
    [SerializeField] int baseSpawnAmount = 1;            // Enemigos iniciales por spawn
    [SerializeField] float speedIncreaseInterval = 10f; // Cada cuántos segundos reduce intervalo
    [SerializeField] float speedIncreaseFactor = 0.9f;  // Factor multiplicativo del intervalo
    [SerializeField] float amountIncreaseInterval = 20f;// Cada cuántos segundos aumenta cantidad

    [Header("Zonas de Spawn")]
    [SerializeField] List<Collider2D> spawnZones;

    [Header("Zona Segura")]
    [SerializeField] UnSpawnArea playerZone;

    [Header("Pool")]
    [SerializeField] string enemyTag;
    #endregion

    float nextSpawnTime;
    float currentSpawnInterval;
    int currentSpawnAmount;

    private void Start()
    {
        currentSpawnInterval = baseSpawnInterval;
        currentSpawnAmount = baseSpawnAmount;
        ScheduleNextSpawn();
    }

    private void Update()
    {
        float time = GameTimer.TimeElapsed;

        // Cada spawn revisa si toca aumentar velocidad y cantidad
        if (time >= nextSpawnTime)
        {
            // Actualizar intervalo progresivamente cada 10s
            int speedSteps = Mathf.FloorToInt(time / speedIncreaseInterval);
            currentSpawnInterval = baseSpawnInterval * Mathf.Pow(speedIncreaseFactor, speedSteps);
            currentSpawnInterval = Mathf.Max(0.1f, currentSpawnInterval);

            // Actualizar cantidad progresivamente cada 20s
            int amountSteps = Mathf.FloorToInt(time / amountIncreaseInterval);
            currentSpawnAmount = baseSpawnAmount + amountSteps;

            SpawnEnemies(currentSpawnAmount);
            ScheduleNextSpawn();
        }
    }

    void ScheduleNextSpawn()
    {
        nextSpawnTime = GameTimer.TimeElapsed + currentSpawnInterval;
    }

    void SpawnEnemies(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            Vector3 spawnPos = GetRandomPositionInZones();
            if (spawnPos != Vector3.zero)
                PoolManager.Instance.SpawnFromPool(enemyTag, spawnPos, Quaternion.identity);
        }
    }

    Vector3 GetRandomPositionInZones()
    {
        if (spawnZones == null || spawnZones.Count == 0)
            return Vector3.zero;

        Vector3 pos = Vector3.zero;
        int attempts = 0;
        const int maxAttempts = 10;

        do
        {
            Collider2D zone = spawnZones[Random.Range(0, spawnZones.Count)];
            Bounds bounds = zone.bounds;
            float x = Random.Range(bounds.min.x, bounds.max.x);
            float y = Random.Range(bounds.min.y, bounds.max.y);
            pos = new Vector3(x, y, 0);
            attempts++;
        }
        while (playerZone != null && playerZone.Contains(pos) && attempts < maxAttempts);

        if (playerZone != null && playerZone.Contains(pos))
            return Vector3.zero;

        return pos;
    }
}