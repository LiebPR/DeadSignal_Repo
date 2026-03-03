using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// [PoolManager] - Sistema de Object Pooling para instanciar y reutilizar objetos en memoria.
/// No persiste entre escenas.
/// </summary>
public class PoolManager : MonoBehaviour
{
    #region Singleton (No persistente)
    public static PoolManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
    #endregion

    #region Clases
    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }
    #endregion

    #region Variables
    [SerializeField] private List<Pool> pools;
    private Dictionary<string, Queue<GameObject>> poolDictionary;
    #endregion

    #region Inicialización
    private void Start()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.tag, objectPool);
        }
    }
    #endregion

    #region API
    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogError($"[PoolManager] No existe un pool con el tag: {tag}");
            return null;
        }

        Queue<GameObject> poolQueue = poolDictionary[tag];
        GameObject objectToSpawn = null;

        // Buscar objeto inactivo
        foreach (var obj in poolQueue)
        {
            if (!obj.activeInHierarchy)
            {
                objectToSpawn = obj;
                break;
            }
        }

        // Si todos están activos, recicla el más antiguo
        if (objectToSpawn == null)
        {
            objectToSpawn = poolQueue.Dequeue();
            objectToSpawn.SetActive(false);
        }

        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        // Notificar reutilización
        var reusableComponents = objectToSpawn.GetComponents<IReusable>();
        foreach (var reusable in reusableComponents)
        {
            reusable.OnObjectReuse();
        }

        objectToSpawn.SetActive(true);
        poolQueue.Enqueue(objectToSpawn);

        return objectToSpawn;
    }
    #endregion
}
