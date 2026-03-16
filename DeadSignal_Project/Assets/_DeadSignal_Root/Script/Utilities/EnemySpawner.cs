using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class EnemySpawnEntry
{
    [Tooltip("Tag del Pool del enemigo.")]
    public string enemyTag;
    [Tooltip("Probabilidad relativa de aparición.")]
    [Range(0f, 100f)]
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
    [SerializeField] int maxEnemies = 140;      // límite real en escena
    [SerializeField] int maxSpawnAmount = 10;   // máximo por spawn

    [Header("Zonas de Spawn")]
    [SerializeField] List<Collider2D> spawnZones;

    [Header("Zona Segura")]
    [SerializeField] UnSpawnArea playerZone;

    [Header("Pool Enemigos")]
    [SerializeField] List<EnemySpawnEntry> enemies;

    List<EnemySpawnEntry> activeEnemyEntries = new List<EnemySpawnEntry>();
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
    }

    public void SetActiveEnemies(List<string> tags)
    {
        activeEnemyEntries.Clear();
        if (tags == null || tags.Count == 0) return;

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

        // Obtener enemigos activos reales desde la pool
        int actualActive = GetActualActiveEnemies();

        int spawnable = Mathf.Min(amount, maxEnemies - actualActive);
        spawnable = Mathf.Min(spawnable, maxSpawnAmount);

        if (spawnable <= 0)
        {
            return;
        }

        for (int i = 0; i < spawnable; i++)
        {
            Vector3 pos = GetRandomPositionInZones();
            if (pos == Vector3.zero)
            {
                continue;
            }

            string tag = GetRandomEnemyTag();
            if (string.IsNullOrEmpty(tag))
            {
                continue;
            }

            GameObject enemy = PoolManager.Instance.SpawnFromPool(tag, pos, Quaternion.identity);
        }
    }

    int GetActualActiveEnemies()
    {
        int count = 0;
        foreach (var entry in activeEnemyEntries)
        {
            var poolQueue = PoolManager.Instance.GetPoolQueue(entry.enemyTag);
            if (poolQueue == null) continue;

            foreach (var obj in poolQueue)
                if (obj.gameObject.activeInHierarchy)
                    count++;
        }
        return count;
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
        } while (playerZone != null && playerZone.Contains(pos) && attempts < maxAttempts);

        if (playerZone != null && playerZone.Contains(pos)) return Vector3.zero;
        return pos;
    }
    #endregion

    #region Enemy Selection
    string GetRandomEnemyTag()
    {
        if (activeEnemyEntries.Count == 0) return null;

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