using UnityEngine;

public class Weapon : MonoBehaviour, IInteractable
{
    [SerializeField] WeaponType weaponType;
    PlayerWeaponSystem weaponSystem;

    private void Awake()
    {
        weaponSystem = FindAnyObjectByType<PlayerWeaponSystem>();
    }

    public void OnHighlight() { /* UI opcional */ }
    public void OnRemoveHighlight() { /* UI opcional */ }

    public void OnPress()
    {
        weaponSystem?.EquipWeapon(weaponType);
        Destroy(gameObject); // Se recoge
    }

    public void OnRelease() { }
}