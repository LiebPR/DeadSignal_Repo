using UnityEngine;

public class EnemyReusable : MonoBehaviour, IReusable
{
    HealthSystem healthSystem;
    EnemyFSM FSM;
    EnemySpawnHandler spawnHandler;
    EnemyMovementController movement;
    EnemyRotationController rotation;
    Collider2D col2D;

    private void Awake()
    {
        healthSystem = GetComponent<HealthSystem>();
        FSM = GetComponent<EnemyFSM>();
        spawnHandler = GetComponent<EnemySpawnHandler>();
        movement = GetComponent<EnemyMovementController>();
        rotation = GetComponent<EnemyRotationController>();
        col2D = GetComponent<Collider2D>();
    }

    /// <summary>
    /// Se llama cuando el PoolManager reutiliza este objeto.
    /// </summary>
    public void OnObjectReuse()
    {
        Debug.Log($"[EnemyReusable] {gameObject.name} OnObjectReuse()");

        // Reset de salud
        healthSystem?.ResetHealth();

        // Reactivar collider
        if (col2D != null) col2D.enabled = true;

        // Detener coroutines de spawn pendientes
        if (spawnHandler != null)
            spawnHandler.ForceResetSpawn();

        // Restablecer movimiento y rotación
        movement?.ResumeMovement();
        rotation?.ResumeRotation();

        // Reiniciar FSM a Spawn
        FSM?.InterruptState(EnemyState.Spawn);
    }
}