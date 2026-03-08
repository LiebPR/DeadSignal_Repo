using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public class EnemySpawnEntry
{
    [Tooltip("Tag del Pool del enemigo.")]
    public string enemyTag;
    [Tooltip("Probabilidad relativa de aparición.")]
    [Range(0f, 1f)]
    public float weight = 50f;
}
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
    [SerializeField] List<EnemySpawnEntry> enemies;
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

        if (time >= nextSpawnTime)
        {
            // Actualizamos intervalo y cantidad de spawn según tiempo
            int speedSteps = Mathf.FloorToInt(time / speedIncreaseInterval);
            currentSpawnInterval = Mathf.Max(minSpawnInterval, baseSpawnInterval * Mathf.Pow(speedIncreaseFactor, speedSteps));

            int amountSteps = Mathf.FloorToInt(time / amountIncreaseInterval);
            currentSpawnAmount = Mathf.Min(maxSpawnAmount, baseSpawnAmount + amountSteps);

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
        if (PoolManager.Instance == null) return;

        // Contar enemigos activos en todos los pools configurados
        int activeCount = 0;

        foreach (var entry in enemies)
        {
            Queue<PoolManager.PooledObject> poolQueue = PoolManager.Instance.GetPoolQueue(entry.enemyTag);
            if (poolQueue == null) continue;

            foreach (var obj in poolQueue)
                if (obj.gameObject.activeInHierarchy)
                    activeCount++;
        }

        // Asegurarse de que el spawnable nunca sea negativo
        int spawnable = Mathf.Max(0, Mathf.Min(amount, maxEnemies - activeCount));
        Debug.Log($"Enemigos activos actualmente: {activeCount}");
        for (int i = 0; i < spawnable; i++)
        {
            Vector3 spawnPos = GetRandomPositionInZones();
            if (spawnPos == Vector3.zero) continue;

            string tag = GetRandomEnemyTag();
            if (!string.IsNullOrEmpty(tag))
                PoolManager.Instance.SpawnFromPool(tag, spawnPos, Quaternion.identity);
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

    string GetRandomEnemyTag()
    {
        if (enemies == null || enemies.Count == 0)
            return null;

        float totalWeight = 0f;

        foreach (var e in enemies)
            totalWeight += e.weight;

        float randomValue = Random.Range(0, totalWeight);
        float current = 0f;

        foreach (var e in enemies)
        {
            current += e.weight;
            if (randomValue <= current)
                return e.enemyTag;
        }

        return enemies[0].enemyTag;
    }
}