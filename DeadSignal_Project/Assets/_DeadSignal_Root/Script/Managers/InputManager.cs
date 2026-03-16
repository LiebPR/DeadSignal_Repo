using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour, PlayerInputActions.IGameplayActions
{
    #region PlayerActions Events
    public static event Action<Vector2> OnMoveEvent;
    public static event Action<bool> OnRunEvent;
    public static event Action<bool> OnShootEvent;
    public static event Action OnMeleeAttackEvent;
    public static event Action OnInteractionEvent;
    #endregion

    #region Weapon Events
    public static event Action<int> OnSlotSelectEvent; //Slot1 = 0, Slot2 = 1
    public static event Action<int> OnWeaponScrollEvent; // +1 = siguiente slot, -1 = anterior slot.
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

    #region PlayerActions Input Callbacks
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

    #region Weapon Input Callbacks
    public void OnSlot1(InputAction.CallbackContext context)
    {
        if (context.performed)
            OnSlotSelectEvent?.Invoke(0); //Slot 1
    }

    public void OnSlot2(InputAction.CallbackContext context)
    {
        if(context.performed)
            OnSlotSelectEvent?.Invoke(1); //Slot 2
    }

    public void OnWeaponScroll(InputAction.CallbackContext context)
    {
        float scroll = context.ReadValue<float>();
        if (scroll > 0f) OnWeaponScrollEvent?.Invoke(1); //Siguiente slot
        else if (scroll < 0f) OnWeaponScrollEvent?.Invoke(-1); //Anterior slot
    }
    #endregion
}
