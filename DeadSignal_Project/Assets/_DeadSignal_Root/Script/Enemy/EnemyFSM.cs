using UnityEngine;

public enum EnemyState
{
    Spawn,
    Move,
    Attack,
    Shoot,
    Invoke,
    Shout,
    Detonate,
    Death
}

public class EnemyFSM : MonoBehaviour
{
    public EnemyState CurrentState { get; private set; }

    public delegate void StateChangedHandler(EnemyState newState);
    public event StateChangedHandler OnStateChanged;

    private void Start()
    {
        // Estado inicial del enemigo: Spawn
        ChangeState(EnemyState.Spawn);
    }

    /// <summary>
    /// Cambia el estado del enemigo y notifica a todos los sistemas
    /// </summary>
    public void ChangeState(EnemyState newState, bool force = false)
    {
        // No permitir interrupción si estamos en Death o Detonate salvo force
        if (!force)
        {
            if ((CurrentState == EnemyState.Death || CurrentState == EnemyState.Detonate) &&
                newState != EnemyState.Death && newState != EnemyState.Detonate)
                return;

            // Evita que Move interrumpa acciones especiales
            if (newState == EnemyState.Move && IsActionRunning())
                return;
        }

        CurrentState = newState;
        OnStateChanged?.Invoke(newState);
    }

    private bool IsActionRunning()
    {
        return CurrentState == EnemyState.Attack ||
               CurrentState == EnemyState.Shoot ||
               CurrentState == EnemyState.Invoke ||
               CurrentState == EnemyState.Shout ||
               CurrentState == EnemyState.Spawn; // Consideramos Spawn como acción especial
    }

    /// <summary>
    /// Llamar cuando una acción especial termina para volver a Move
    /// </summary>
    public void ActionFinished()
    {
        if (CurrentState != EnemyState.Death && CurrentState != EnemyState.Detonate)
        {
            if (CurrentState == EnemyState.Spawn)
            {
                // Terminado el spawn, pasamos a Move
                ChangeState(EnemyState.Move, force: true);
            }
            else
            {
                ChangeState(EnemyState.Move);
            }
        }
    }

    /// <summary>
    /// Interrumpir cualquier estado, por ejemplo al hacer respawn
    /// </summary>
    public void InterruptState(EnemyState newState = EnemyState.Move)
    {
        ChangeState(newState, force: true);
    }
}