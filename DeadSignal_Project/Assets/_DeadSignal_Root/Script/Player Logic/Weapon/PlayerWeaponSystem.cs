using System.Collections;
using UnityEngine;

public enum WeaponType
{
    Empty,
    Axe,
    Bat
}
public class PlayerWeaponSystem : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] PlayerFSM playerFSM;

    [Header("Armas dentro del jugador")]
    [SerializeField] AxeWeapon axe;
    [SerializeField] BatWeapon bat;

    [Header("Ataque")]
    [SerializeField] float attackCooldown = 0.5f;

    MeleeWeapon currentWeapon;
    WeaponType currentType = WeaponType.Empty;
    bool canAttack = true;

    private void Awake()
    {
        if (playerFSM == null)
            playerFSM = GetComponent<PlayerFSM>();
    }

    private void OnEnable()
    {
        playerFSM.OnStateChanged += HandleStateChanged;
    }

    private void OnDisable()
    {
        playerFSM.OnStateChanged -= HandleStateChanged;
    }

    void HandleStateChanged(PlayerActionState state)
    {
        if (state == PlayerActionState.MeleeAttack && canAttack)
        {
            currentWeapon?.PerformAttack();
            StartCoroutine(ResetAttackCooldown());
        }
    }

    IEnumerator ResetAttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;

        // Volvemos a Idle automáticamente
        playerFSM.SetState(PlayerActionState.Idle);
    }

    public void EquipWeapon(WeaponType type)
    {
        // Desactiva arma anterior
        currentWeapon?.DeactivateWeapon();

        currentType = type;

        switch (type)
        {
            case WeaponType.Axe: currentWeapon = axe; break;
            case WeaponType.Bat: currentWeapon = bat; break;
            case WeaponType.Empty: currentWeapon = null; break;
        }

        currentWeapon?.ActivateWeapon();
    }

    public WeaponType GetCurrentWeapon() => currentType;
}