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
    #region Singleton
    public static EnemySpawner Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    #endregion

    #region Configuración
    [Header("Spawn Limits")]
    [SerializeField] int maxEnemies = 140;
    [SerializeField] int maxSpawnAmount = 10;

    [Header("Zonas de Spawn")]
    [SerializeField] List<Collider2D> spawnZones;

    [Header("Zona Segura")]
    [SerializeField] UnSpawnArea playerZone;

    [Header("Pool Enemigos")]
    [SerializeField] List<EnemySpawnEntry> enemies;

    // Lista de enemigos disponibles para spawn según ciclo
    List<EnemySpawnEntry> activeEnemyEntries = new List<EnemySpawnEntry>();
    #endregion

    #region Estado runtime
    int activeEnemyCount; // contador de enemigos activos
    #endregion

    #region API pública
    public void SpawnNormal(int amount) => SpawnEnemies(amount);
    public void SpawnWave(int amount) => SpawnEnemies(amount);

    public void KillAllEnemies()
    {
        foreach (var entry in enemies)
        {
            var poolQueue = PoolManager.Instance.GetPoolQueue(entry.enemyTag);
            if (poolQueue == null) continue;

            foreach (var obj in poolQueue)
                if (obj.gameObject.activeInHierarchy)
                    obj.gameObject.SetActive(false);
        }

        activeEnemyCount = 0;
    }

    public void RegisterEnemyDeath()
    {
        activeEnemyCount--;
        activeEnemyCount = Mathf.Max(0, activeEnemyCount);
    }

    public void SetActiveEnemies(List<string> tags)
    {
        if (tags == null || tags.Count == 0)
        {
            activeEnemyEntries.Clear();
            return;
        }

        activeEnemyEntries.Clear();
        foreach (var tag in tags)
        {
            EnemySpawnEntry entry = enemies.Find(e => e.enemyTag == tag);
            if (entry != null)
                activeEnemyEntries.Add(entry);
        }
    }
    #endregion

    #region Spawn Core
    void SpawnEnemies(int amount)
    {
        if (PoolManager.Instance == null || activeEnemyEntries.Count == 0) return;

        int spawnable = Mathf.Min(amount, maxEnemies - activeEnemyCount);
        spawnable = Mathf.Min(spawnable, maxSpawnAmount);

        for (int i = 0; i < spawnable; i++)
        {
            Vector3 pos = GetRandomPositionInZones();
            if (pos == Vector3.zero) continue;

            string tag = GetRandomEnemyTag();
            if (string.IsNullOrEmpty(tag)) continue;

            GameObject enemy = PoolManager.Instance.SpawnFromPool(tag, pos, Quaternion.identity);
            if (enemy != null) activeEnemyCount++;
        }
    }
    #endregion

    #region Spawn Position
    Vector3 GetRandomPositionInZones()
    {
        if (spawnZones == null || spawnZones.Count == 0) return Vector3.zero;

        int attempts = 0;
        const int maxAttempts = 10;
        Vector3 pos = Vector3.zero;

        do
        {
            var zone = spawnZones[Random.Range(0, spawnZones.Count)];
            Bounds b = zone.bounds;
            float x = Random.Range(b.min.x, b.max.x);
            float y = Random.Range(b.min.y, b.max.y);
            pos = new Vector3(x, y, 0);
            attempts++;
        }
        while (playerZone != null && playerZone.Contains(pos) && attempts < maxAttempts);

        if (playerZone != null && playerZone.Contains(pos)) return Vector3.zero;
        return pos;
    }
    #endregion

    #region Enemy Selection
    string GetRandomEnemyTag()
    {
        if (activeEnemyEntries == null || activeEnemyEntries.Count == 0) return null;

        float totalWeight = 0f;
        foreach (var e in activeEnemyEntries) totalWeight += e.weight;

        float rnd = Random.Range(0f, totalWeight);
        float current = 0f;

        foreach (var e in activeEnemyEntries)
        {
            current += e.weight;
            if (rnd <= current) return e.enemyTag;
        }

        return activeEnemyEntries[0].enemyTag;
    }
    #endregion
}