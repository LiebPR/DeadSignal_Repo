using System;
using UnityEngine;
public enum PlayerActionState { Idle, Shooting, MeleeAttack, Recoil, Interaction }

public class PlayerFSM : MonoBehaviour
{
    public PlayerActionState currentState { get; private set; } = PlayerActionState.Idle;

    #region Input Flags
    bool shootingIntent; //Mantiene intención de disparar mientras el botón esté presionado
    bool meleeIntent; //Flag de pulso, se resetea al ejecutar
    bool interactionIntent; //Flag de pulso, se resetea al ejecutar
    #endregion

    #region Events
    public event Action<PlayerActionState> OnStateChanged; //Evento que notifica cambios de estado
    #endregion

    private void OnEnable()
    {
        InputManager.OnShootEvent += HandleShootInput;
        InputManager.OnMeleeAttackEvent += HandleMeleeInput;
        InputManager.OnInteractionEvent += HandleInteractInput;
    }

    private void OnDisable()
    {
        InputManager.OnShootEvent -= HandleShootInput;
        InputManager.OnMeleeAttackEvent -= HandleMeleeInput;
        InputManager.OnInteractionEvent -= HandleInteractInput;
    }

    #region Input Handlers
    private void HandleShootInput(bool pressed)
    {
        shootingIntent = pressed;
        UpdateState();
    }

    private void HandleMeleeInput()
    {
        meleeIntent = true;
        UpdateState();
    }

    private void HandleInteractInput()
    {
        interactionIntent = true;
        UpdateState();
    }
    #endregion

    #region FSM Logic
    /// <summary>
    /// Actualiza el estado según la prioridad y las intenciones de input.
    /// Recoil siempre tiene prioridad absoluta.
    /// </summary>
    private void UpdateState()
    {
        if (meleeIntent)
        {
            SetState(PlayerActionState.MeleeAttack);
            meleeIntent = false; // Reset flag de pulso
        }
        else if (shootingIntent)
        {
            SetState(PlayerActionState.Shooting);
        }
        else if (interactionIntent)
        {
            SetState(PlayerActionState.Interaction);
            interactionIntent = false; // Reset flag de pulso
        }
        else
        {
            SetState(PlayerActionState.Idle);
        }
    }

    /// <summary>
    /// Cambia el estado de la FSM y notifica a los sistemas externos.
    /// </summary>
    /// <param name="newState">Nuevo estado</param>
    public void SetState(PlayerActionState newState)
    {
        if (currentState == newState) return;

        currentState = newState;
        OnStateChanged?.Invoke(currentState);
    }
    #endregion


}
