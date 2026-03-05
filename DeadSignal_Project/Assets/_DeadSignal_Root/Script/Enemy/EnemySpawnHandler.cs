using UnityEngine;
using System.Collections;

public class EnemySpawnHandler : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] float spawnDuration = 1.5f; // Duración del spawn

    EnemyFSM fsm;
    HealthSystem healthSystem;
    Rigidbody2D rb;
    Coroutine currentSpawnRoutine;
    bool isSpawning = false;

    private void Awake()
    {
        fsm = GetComponent<EnemyFSM>();
        healthSystem = GetComponent<HealthSystem>();
        rb = GetComponent<Rigidbody2D>();
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

        healthSystem?.ActivateImmunity();
        rb.bodyType = RigidbodyType2D.Kinematic; // Evita que la física afecte durante el spawn

        yield return new WaitForSeconds(spawnDuration);

        healthSystem?.DeactivateImmunity();
        rb.bodyType = RigidbodyType2D.Dynamic; // Vuelve a la física normal

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
        }
    }
}