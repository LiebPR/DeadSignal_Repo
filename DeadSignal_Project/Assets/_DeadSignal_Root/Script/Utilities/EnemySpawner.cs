using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    #region Configuración
    [Header("Spawn Settings")]
    [Tooltip("Intervalo inicial entre spawns")]
    [SerializeField] float baseSpawnInterval = 3f;
    [Tooltip("Cantidad inicial de enemigos por spawn")]
    [SerializeField] int baseSpawnAmount = 1;
    [Tooltip("Cada cuantos segundos se reduce el intervalo de spawn")]
    [SerializeField] float speedIncreaseInterval = 10f;
    [Tooltip("Factor multiplicativo del intervalo de spawn cada vez que se reduce")]
    [SerializeField] float speedIncreaseFactor = 0.9f;
    [Tooltip("Cada cuantos segundos aumenta la cantidad de enemigos por spawn")]
    [SerializeField] float amountIncreaseInterval = 20f;
    [Tooltip("Máximo enemigos activos en pantalla")]
    [SerializeField] int maxEnemies = 300;
    [Tooltip("Máximo enemigos por spawn")]
    [SerializeField] int maxSpawnAmount = 10;
    [Tooltip("Mínimo intervalo entre spawns (límite inferior)")]
    [SerializeField] float minSpawnInterval = 3f;

    [Header("Zonas de Spawn")]
    [SerializeField] List<Collider2D> spawnZones;

    [Header("Zona Segura")]
    [SerializeField] UnSpawnArea playerZone;

    [Header("Pool")]
    [SerializeField] string enemyTag;
    #endregion

    #region Internal States
    float nextSpawnTime;
    float currentSpawnInterval;
    int currentSpawnAmount;
    #endregion

    private void Start()
    {
        currentSpawnInterval = baseSpawnInterval;
        currentSpawnAmount = baseSpawnAmount;
        ScheduleNextSpawn();
    }

    void Update()
    {
        float time = GameTimer.TimeElapsed;

        // Mostrar debug de enemigos activos
        if (PoolManager.Instance != null)
        {
            int activeEnemies = CountActiveEnemies();
            Debug.Log($"[EnemySpawner] Enemigos activos: {activeEnemies}");
        }

        if (time >= nextSpawnTime)
        {
            // Lógica de spawn (igual que antes)
            int speedSteps = Mathf.FloorToInt(time / speedIncreaseInterval);
            currentSpawnInterval = baseSpawnInterval * Mathf.Pow(speedIncreaseFactor, speedSteps);
            currentSpawnInterval = Mathf.Max(minSpawnInterval, currentSpawnInterval);

            int amountSteps = Mathf.FloorToInt(time / amountIncreaseInterval);
            currentSpawnAmount = baseSpawnAmount + amountSteps;
            currentSpawnAmount = Mathf.Min(maxSpawnAmount, currentSpawnAmount);

            SpawnEnemies(currentSpawnAmount);
            ScheduleNextSpawn();
        }
    }

    int CountActiveEnemies()
    {
        if (PoolManager.Instance == null) return 0;

        Queue<PoolManager.PooledObject> poolQueue = PoolManager.Instance.GetPoolQueue(enemyTag);
        if (poolQueue == null) return 0;

        int activeCount = 0;
        foreach (var obj in poolQueue)
        {
            if (obj.gameObject.activeInHierarchy)
                activeCount++;
        }
        return activeCount;
    }

    void ScheduleNextSpawn()
    {
        nextSpawnTime = GameTimer.TimeElapsed + currentSpawnInterval;
    }

    void SpawnEnemies(int amount)
    {
        if (PoolManager.Instance == null) return;

        Queue<PoolManager.PooledObject> poolQueue = PoolManager.Instance.GetPoolQueue(enemyTag);
        if (poolQueue == null) return;

        // Contar enemigos activos en el pool
        int activeCount = 0;
        foreach (var obj in poolQueue)
        {
            if (obj.gameObject.activeInHierarchy)
                activeCount++;
        }

        if (activeCount >= maxEnemies)
            return; // No spawneamos si ya hay demasiados

        // Solo spawneamos hasta alcanzar el máximo global
        int spawnable = Mathf.Min(amount, maxEnemies - activeCount);

        for (int i = 0; i < spawnable; i++)
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