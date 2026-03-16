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
        ChangeState(EnemyState.Spawn);
    }

    public void ChangeState(EnemyState newState, bool force = false)
    {
        if (!force && CurrentState == newState)
            return;

        if (!force)
        {
            if ((CurrentState == EnemyState.Death || CurrentState == EnemyState.Detonate) &&
                newState != EnemyState.Death && newState != EnemyState.Detonate)
                return;

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
               CurrentState == EnemyState.Spawn;
    }

    /// <summary>
    /// Llamar cuando una acción especial termina para volver a Move
    /// </summary>
    public void ActionFinished()
    {
        if (CurrentState == EnemyState.Death || CurrentState == EnemyState.Detonate)
            return;

        // Solo volver a Move cuando la acción termina
        if (IsActionRunning())
            ChangeState(EnemyState.Move, force: true);
    }

    public void InterruptState(EnemyState newState = EnemyState.Move)
    {
        ChangeState(newState, force: true);
    }
}