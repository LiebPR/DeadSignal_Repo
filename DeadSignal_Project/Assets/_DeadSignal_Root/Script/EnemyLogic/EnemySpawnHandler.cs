using UnityEngine;
using System.Collections;

public class EnemySpawnHandler : MonoBehaviour
{
    [SerializeField] EnemyData data;

    EnemyFSM fsm;
    HealthSystem healthSystem;
    Rigidbody2D rb;
    Coroutine currentSpawnRoutine;

    private void Awake()
    {
        fsm = GetComponent<EnemyFSM>();
        healthSystem = GetComponent<HealthSystem>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        if (fsm != null)
        {
            fsm.OnStateChanged += HandleStateChanged;

            // Verificar si el FSM ya está en Spawn al habilitar
            if (fsm.CurrentState == EnemyState.Spawn)
                currentSpawnRoutine = StartCoroutine(SpawnRoutine());
        }
    }

    private void OnDisable()
    {
        if (fsm != null)
            fsm.OnStateChanged -= HandleStateChanged;
    }

    private void HandleStateChanged(EnemyState newState)
    {
        if (newState == EnemyState.Spawn)
        {
            currentSpawnRoutine = StartCoroutine(SpawnRoutine());
        }
    }

    private IEnumerator SpawnRoutine()
    {

        // Bloquear movimiento y rotación
        var movement = GetComponent<EnemyMovementController>();
        var rotation = GetComponent<EnemyRotationController>();
        movement?.StopMovement();
        rotation?.StopRotation();

        healthSystem?.ActivateImmunity();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(data.spawnDuration);

        healthSystem?.DeactivateImmunity();
        rb.bodyType = RigidbodyType2D.Dynamic;

        // Restaurar movimiento y rotación
        movement?.ResumeMovement();
        rotation?.ResumeRotation();

        fsm?.ActionFinished();

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
        }
    }
}