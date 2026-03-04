using UnityEngine;
using System.Collections;

public class EnemySpawnHandler : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] float spawnDuration = 1.5f; // Tiempo que dura el spawn

    private EnemyFSM fsm;
    private HealthSystem healthSystem;
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
        // Cuando entra en Spawn, activa la lógica de aparición
        if (newState == EnemyState.Spawn && !isSpawning)
        {
            StartCoroutine(SpawnRoutine());
        }
    }

    private IEnumerator SpawnRoutine()
    {
        isSpawning = true;

        // Activar inmunidad para no recibir daño
        if (healthSystem != null)
            healthSystem.ActivateImmunity();

        // Aquí podrías poner animaciones, efectos visuales, etc.
        yield return new WaitForSeconds(spawnDuration);

        // Desactivar inmunidad
        if (healthSystem != null)
            healthSystem.DeactivateImmunity();

        // Notificar a la FSM que terminó la acción de Spawn
        if (fsm != null)
            fsm.ActionFinished();

        isSpawning = false;
    }
}
