using UnityEngine;

public class FireWeaponUseSystem : MonoBehaviour
{
    [Header("References")]
    [SerializeField] PlayerFSM playerFSM;
    [SerializeField] FireWeaponSlotHandler slotHandler;

    [Header("Weapons inside Player")]
    [SerializeField] RifleWeapon rifle;
    [SerializeField] ShotgunWeapon shotgun;

    FireWeaponController currentWeapon;
    FireWeaponType currentType = FireWeaponType.Empty;

    private void Awake()
    {
        if (playerFSM == null)
            playerFSM = GetComponent<PlayerFSM>();
    }

    void Start()
    {
        EquipWeapon(slotHandler.GetCurrentWeapon());
    }

    private void OnEnable()
    {
        playerFSM.OnStateChanged += HandleStateChanged;
        InputManager.OnSlotSelectEvent += HandleSlotSelect;
    }

    private void OnDisable()
    {
        playerFSM.OnStateChanged -= HandleStateChanged;
        InputManager.OnSlotSelectEvent -= HandleSlotSelect;
    }

    void HandleSlotSelect(int slot)
    {
        slotHandler.SelectSlot(slot);
        EquipWeapon(slotHandler.GetCurrentWeapon());
    }

    void HandleStateChanged(PlayerActionState state)
    {
        if (currentWeapon == null) return;

        if (state == PlayerActionState.Shooting)
            currentWeapon.StartShoot();
        else
            currentWeapon.StopShoot();
    }

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
}
