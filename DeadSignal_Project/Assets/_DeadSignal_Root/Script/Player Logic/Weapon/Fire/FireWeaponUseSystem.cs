using UnityEngine;

/// <summary>
/// [FireWeaponUseSystem]: Sistema responsable de gestionar el uso de armas de fuego del jugador.
/// </summary>
public class FireWeaponUseSystem : MonoBehaviour
{
    #region Inspector References
    [Header("References")]
    [Tooltip("Máquina de estados del jugador.")]
    [SerializeField] PlayerFSM playerFSM;
    [Tooltip("Sistema que gestiona los slots de armas del jugador.")]
    [SerializeField] FireWeaponSlotHandler slotHandler;

    [Header("Weapons inside Player")]
    [Tooltip("Instancia del rifle disponible para el juagdor.")]
    [SerializeField] RifleWeapon rifle;
    [Tooltip("Instancia de la escopeta disponible para el jugador.")]
    [SerializeField] ShotgunWeapon shotgun;
    #endregion

    #region Internal States
    //Referencia al arma actualmente equipada.
    FireWeaponController currentWeapon;
    //Tipo del arma actualmente equipada.
    FireWeaponType currentType = FireWeaponType.Empty;
    #endregion

    private void Awake()
    {
        if (playerFSM == null)
            playerFSM = GetComponent<PlayerFSM>();
    }

    void Start()
    {
        //Equipamos el arma correspondiente al slot actual al iniciar el juego.
        EquipWeapon(slotHandler.GetCurrentWeapon());
    }

    private void OnEnable()
    {
        playerFSM.OnStateChanged += HandleStateChanged;
        InputManager.OnSlotSelectEvent += HandleSlotSelect;
        slotHandler.OnSlotChanged += HandleSlotChanged;
    }

    private void OnDisable()
    {
        playerFSM.OnStateChanged -= HandleStateChanged;
        InputManager.OnSlotSelectEvent -= HandleSlotSelect;
        slotHandler.OnSlotChanged -= HandleSlotChanged;
    }

    #region Input Handling
    /// <summary>
    /// Se ejecuta cuando el jugador selecicona un slot mediante input.
    /// </summary>
    void HandleSlotSelect(int slot)
    {
        slotHandler.SelectSlot(slot);
        EquipWeapon(slotHandler.GetCurrentWeapon());
    }
    #endregion

    #region PlayerState Handling
    /// <summary>
    /// Reacciona a los cambios de estado del jugador.
    /// Si el juagdor entra en estado de disparo, activa el arma actual.
    /// </summary>
    void HandleStateChanged(PlayerActionState state)
    {
        if (currentWeapon == null) return;

        if (state == PlayerActionState.Shooting)
            currentWeapon.StartShoot();
        else
            currentWeapon.StopShoot();
    }
    #endregion

    #region Weapon Equip Logic
    /// <summary>
    /// Equipa un arma según su tipo.
    /// Asigna la referencia al arma correspondiente.
    /// </summary>
    public void EquipWeapon(FireWeaponType type)
    {
        currentType = type;

        switch (type)
        {
            case FireWeaponType.AssaultRifle:
                currentWeapon = rifle;
                break;

            case FireWeaponType.Shotgun:
                currentWeapon = shotgun;
                break;

            default:
                currentWeapon = null;
                break;
        }
    }

    /// <summary>
    /// Devuelve la referencia al arma correspondiente según su tipo.
    /// </summary>
    public FireWeaponController GetWeaponFromType(FireWeaponType type)
    {
        switch (type)
        {
            case FireWeaponType.AssaultRifle: return rifle;
            case FireWeaponType.Shotgun: return shotgun;
        }

        return null;
    }
    #endregion

    #region Slot Event Handling
    /// <summary>
    /// Se ejecuta cuando el slot activo cambia en el sistema de slots.
    /// Actualiza el arma equipada.
    /// </summary>
    /// <param name="slot"></param>
    void HandleSlotChanged(int slot)
    {
        EquipWeapon(slotHandler.GetCurrentWeapon());
    }
    #endregion
}
