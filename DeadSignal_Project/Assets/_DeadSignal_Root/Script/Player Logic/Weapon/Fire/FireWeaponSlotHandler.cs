using UnityEngine;

public class FireWeaponSlotHandler : MonoBehaviour
{
    public FireWeaponType slot1 = FireWeaponType.AssaultRifle;
    public FireWeaponType slot2 = FireWeaponType.Empty;

    int currentSlot = 0;

    public FireWeaponType GetCurrentWeapon()
    {
        return currentSlot == 0 ? slot1 : slot2;
    }

    public void SetWeaponToSlot(FireWeaponType type)
    {
        if (slot1 == FireWeaponType.Empty)
            slot1 = type;
        else
            slot2 = type;
    }

    public void SelectSlot(int slot)
    {
        currentSlot = slot;
    }
}
