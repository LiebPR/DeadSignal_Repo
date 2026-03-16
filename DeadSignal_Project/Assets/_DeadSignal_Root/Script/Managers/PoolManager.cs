using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// [PoolManager] - Sistema de Object Pooling para instanciar y reutilizar objetos en memoria.
/// Editable en Inspector. No persiste entre escenas.
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

        // Inicializar diccionarios
        poolDictionary = new Dictionary<string, Queue<PooledObject>>();
        prefabDictionary = new Dictionary<string, GameObject>();
        poolContainers = new Dictionary<string, Transform>();

        // Crear pools desde el Inspector
        foreach (Pool pool in pools)
        {
            if (pool.prefab == null)
            {
                Debug.LogError($"[PoolManager] Pool {pool.tag} no tiene prefab asignado!");
                continue;
            }

            // Crear contenedor vacío para organizar jerarquía
            GameObject poolContainer = new GameObject(pool.tag + "_Pool");

            // Usar SetParent para UI y mantener escala/posición local
            poolContainer.transform.SetParent(this.transform, false);

            poolContainers.Add(pool.tag, poolContainer.transform);

            // Guardar prefab para auto-expansión
            if (!prefabDictionary.ContainsKey(pool.tag))
                prefabDictionary.Add(pool.tag, pool.prefab);

            // Crear cola de objetos
            Queue<PooledObject> objectPool = new Queue<PooledObject>();
            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);

                // Organización: hijo del contenedor usando SetParent
                obj.transform.SetParent(poolContainer.transform, false);

                // Desactivar objeto
                obj.SetActive(false);

                PooledObject pooledObject = new PooledObject
                {
                    gameObject = obj,
                    reusableComponents = obj.GetComponents<IReusable>()
                };

                objectPool.Enqueue(pooledObject);
            }

            poolDictionary.Add(pool.tag, objectPool);
        }
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

    /// <summary>
    /// Estructura interna optimizada del Pool
    /// </summary>
    public class PooledObject
    {
        public GameObject gameObject;
        public IReusable[] reusableComponents;
    }

    #endregion

    #region Variables

    [SerializeField] private List<Pool> pools;

    // Diccionarios internos
    private Dictionary<string, Queue<PooledObject>> poolDictionary;
    private Dictionary<string, GameObject> prefabDictionary;
    private Dictionary<string, Transform> poolContainers;

    #endregion

    #region API

    /// <summary>
    /// Spawnea un objeto desde el pool correspondiente
    /// </summary>
    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogError($"[PoolManager] No existe un pool con el tag: {tag}");
            return null;
        }

        Queue<PooledObject> poolQueue = poolDictionary[tag];
        PooledObject pooledObject = null;

        // Tomar el primer objeto disponible
        if (poolQueue.Count > 0)
            pooledObject = poolQueue.Dequeue();

        // Si está activo o cola vacía → crear nuevo objeto (auto-expansión)
        if (pooledObject == null || pooledObject.gameObject.activeInHierarchy)
        {
            if (!prefabDictionary.ContainsKey(tag))
            {
                Debug.LogError($"[PoolManager] No existe prefab para el tag: {tag}");
                return null;
            }

            GameObject prefab = prefabDictionary[tag];
            GameObject newObj = Instantiate(prefab);

            // Hacer hijo del contenedor usando SetParent para evitar warnings de UI
            if (poolContainers.ContainsKey(tag))
                newObj.transform.SetParent(poolContainers[tag], false);

            newObj.SetActive(false);

            pooledObject = new PooledObject
            {
                gameObject = newObj,
                reusableComponents = newObj.GetComponents<IReusable>()
            };
        }

        GameObject objectToSpawn = pooledObject.gameObject;

        // Posicionar y rotar
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        // Notificar a componentes reutilizables
        foreach (var reusable in pooledObject.reusableComponents)
            reusable.OnObjectReuse();

        // Activar objeto
        objectToSpawn.SetActive(true);

        // Volver a encolar
        poolQueue.Enqueue(pooledObject);

        return objectToSpawn;
    }

    /// <summary>
    /// Devuelve la cola de un pool para consultas externas
    /// </summary>
    public Queue<PooledObject> GetPoolQueue(string tag)
    {
        if (poolDictionary.ContainsKey(tag))
            return poolDictionary[tag];
        return null;
    }

    #endregion
}