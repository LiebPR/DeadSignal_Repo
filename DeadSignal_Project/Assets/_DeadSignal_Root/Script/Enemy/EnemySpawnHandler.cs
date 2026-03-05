using UnityEngine;
using System.Collections;

public class EnemySpawnHandler : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] float spawnDuration = 1.5f; // Duración del spawn

    private EnemyFSM fsm;
    private HealthSystem healthSystem;
    private Coroutine currentSpawnRoutine;
    private bool isSpawning = false;

    private void Awake()
    {
        fsm = GetComponent<EnemyFSM>();
        healthSystem = GetComponent<HealthSystem>();
    }

    private void OnEnable()
    {
        if (fsm != null)
            fsm.OnStateChanged += HandleStateChanged;
    }

    private void OnDisable()
    {
        if (fsm != null)
            fsm.OnStateChanged -= HandleStateChanged;
    }

    private void HandleStateChanged(EnemyState newState)
    {
        if (newState == EnemyState.Spawn && !isSpawning)
        {
            currentSpawnRoutine = StartCoroutine(SpawnRoutine());
        }
    }

    private IEnumerator SpawnRoutine()
    {
        isSpawning = true;
        Debug.Log($"[EnemySpawnHandler] {gameObject.name} Spawn iniciado");

        healthSystem?.ActivateImmunity();

        yield return new WaitForSeconds(spawnDuration);

        healthSystem?.DeactivateImmunity();

        Debug.Log($"[EnemySpawnHandler] {gameObject.name} Spawn terminado");

        fsm?.ActionFinished();

        isSpawning = false;
        currentSpawnRoutine = null;
    }

    /// <summary>
    /// Fuerza el reinicio del spawn al reutilizar un objeto
    /// </summary>
    public void ForceResetSpawn()
    {
        if (currentSpawnRoutine != null)
        {
            StopCoroutine(currentSpawnRoutine);
            currentSpawnRoutine = null;
            isSpawning = false;
            Debug.Log($"[EnemySpawnHandler] {gameObject.name} Spawn reset forzado");
        }
    }
}