using UnityEngine;

public class FireWeaponPickUp : MonoBehaviour, IInteractable
{
    [SerializeField] FireWeaponType weaponType;

    FireWeaponSlotHandler slotHandler;

    private void Awake()
    {
        slotHandler = FindAnyObjectByType<FireWeaponSlotHandler>();
    }

    public void OnHighlight() { }

    public void OnRemoveHighlight() { }

    public void OnPress()
    {
        slotHandler.SetWeaponToSlot(weaponType);
        Destroy(gameObject);
    }

    public void OnRelease() { }
}
