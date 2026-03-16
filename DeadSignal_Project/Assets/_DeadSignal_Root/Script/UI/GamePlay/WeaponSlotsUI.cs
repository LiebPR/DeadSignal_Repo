using UnityEngine;
using UnityEngine.UI;

public class WeaponSlotsUI : MonoBehaviour
{
    [Header("Systems")]
    [SerializeField] FireWeaponSlotHandler slotHandler;
    [SerializeField] FireWeaponUseSystem fireSystem;
    [SerializeField] PlayerWeaponSystem meleeSystem;
    [SerializeField] PlayerFSM playerFSM;

    [Header("Fire Slots")]
    [SerializeField] Image slot1Icon;
    [SerializeField] Image slot2Icon;

    [SerializeField] Image slot1Highlight;
    [SerializeField] Image slot2Highlight;

    [Header("Melee Slot")]
    [SerializeField] Image meleeIcon;
    [SerializeField] Image meleeHighlight;

    void Start()
    {
        RefreshIcons();
        UpdateFireHighlight(slotHandler.GetCurrentSlot());
        UpdateMeleeIcon(meleeSystem.GetCurrentWeapon());
    }

    void OnEnable()
    {
        slotHandler.OnSlotChanged += UpdateFireHighlight;
        slotHandler.OnSlotsUpdated += RefreshIcons;

        playerFSM.OnStateChanged += HandlePlayerState;

        if (meleeSystem != null)
            meleeSystem.OnWeaponChanged += UpdateMeleeIcon;
    }

    void OnDisable()
    {
        slotHandler.OnSlotChanged -= UpdateFireHighlight;
        slotHandler.OnSlotsUpdated -= RefreshIcons;

        playerFSM.OnStateChanged -= HandlePlayerState;

        if (meleeSystem != null)
            meleeSystem.OnWeaponChanged -= UpdateMeleeIcon;
    }

    #region FIRE WEAPONS

    void RefreshIcons()
    {
        UpdateSlotIcon(slotHandler.slot1, slot1Icon);
        UpdateSlotIcon(slotHandler.slot2, slot2Icon);
    }

    void UpdateSlotIcon(FireWeaponType type, Image icon)
    {
        if (type == FireWeaponType.Empty)
        {
            icon.enabled = false;
            return;
        }

        FireWeaponController weapon =
            fireSystem.GetWeaponFromType(type);

        if (weapon != null)
        {
            icon.enabled = true;
            icon.sprite = weapon.GetUIIcon();
        }
    }

    void UpdateFireHighlight(int slot)
    {
        slot1Highlight.enabled = slot == 0;
        slot2Highlight.enabled = slot == 1;

        meleeHighlight.enabled = false; // apagar highlight melee
    }

    #endregion

    #region MELEE

    void UpdateMeleeIcon(WeaponType type)
    {
        if (type == WeaponType.Empty)
        {
            meleeIcon.enabled = false;
            meleeHighlight.enabled = false;
            return;
        }

        MeleeWeapon weapon = meleeSystem.GetWeaponFromType(type);
        if (weapon != null)
        {
            meleeIcon.enabled = true;
            meleeIcon.sprite = weapon.GetUIIcon();
        }
    }

    void HandlePlayerState(PlayerActionState state)
    {
        if (state == PlayerActionState.MeleeAttack)
        {
            // Apagar highlights de armas de fuego
            slot1Highlight.enabled = false;
            slot2Highlight.enabled = false;

            // Activar highlight de melee
            meleeHighlight.enabled = true;
        }
        else
        {
            // Restaurar highlight al arma de fuego activa
            UpdateFireHighlight(slotHandler.GetCurrentSlot());
        }
    }

    #endregion
}