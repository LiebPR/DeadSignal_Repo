using UnityEngine;

public class AxeWeapon : MeleeWeapon
{
    [Header("Attack Settings")]
    [SerializeField] Vector2 hitboxSize = new Vector2(0.8f, 1.2f);

    public override void PerformAttack()
    {
        Vector2 center = attackPoint.position;

        Collider2D[] hits = Physics2D.OverlapBoxAll(center, hitboxSize, 0f, enemyLayer);

        float damage = GetDamage();

        foreach (var hit in hits)
        {
            HealthSystem health = hit.GetComponent<HealthSystem>();
            if (health != null)
                health.TakeDamage(damage);
        }
    }
}
