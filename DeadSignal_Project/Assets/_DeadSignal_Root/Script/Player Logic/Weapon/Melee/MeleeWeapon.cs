using UnityEngine;

public abstract class MeleeWeapon : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] Sprite uiIcon;

    [Header("Settings")]
    [SerializeField] protected float damage = 25f;
    [SerializeField] protected LayerMask enemyLayer;

    [Header("Reference Point")]
    [SerializeField] protected Transform attackPoint; // siempre el centro del hitbox

    public Sprite GetUIIcon()
    {
        return uiIcon;
    }

    #region Damage Calculation
    protected float GetDamage()
    {
        if (HeadShotHighlightSystem.CurrentHoveringHead)
            return damage * 2;

        return damage;
    }
    #endregion

    public abstract void PerformAttack();

    public virtual void ActivateWeapon() => gameObject.SetActive(true);
    public virtual void DeactivateWeapon() => gameObject.SetActive(false);
}
