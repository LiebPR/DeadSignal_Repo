using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour, PlayerInputActions.IGameplayActions
{
    #region Events
    public static event Action<Vector2> OnMoveEvent;
    public static event Action<bool> OnRunEvent;
    public static event Action<bool> OnShootEvent;
    public static event Action OnMeleeAttackEvent;
    public static event Action OnInteractionEvent;
    #endregion

    #region Fields
    PlayerInputActions inputActions;
    #endregion

    private void Awake()
    {
        inputActions = new PlayerInputActions();
        inputActions.Gameplay.SetCallbacks(this);
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    private void OnDestroy()
    {
        inputActions.Dispose();
    }

    #region Input Callbacks
    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 value = context.ReadValue<Vector2>();
        OnMoveEvent?.Invoke(value);
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        if (context.performed)
            OnRunEvent?.Invoke(true);

        if (context.canceled)
            OnRunEvent?.Invoke(false);
    }

    public void OnShoot(InputAction.CallbackContext context)
    {
        if (context.performed)
            OnShootEvent?.Invoke(true);
        if (context.canceled)
            OnShootEvent?.Invoke(false);
    }

    public void OnMeleeAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
            OnMeleeAttackEvent?.Invoke();
    }

    public void OnInteraction(InputAction.CallbackContext context)
    {
        if (context.performed)
            OnInteractionEvent?.Invoke();
    }
    #endregion
}
