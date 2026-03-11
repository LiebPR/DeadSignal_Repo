using UnityEngine;

public class BatWeapon : MeleeWeapon
{
    [Header("Attack Settings")]
    [SerializeField] float radius = 1.5f;

    public override void PerformAttack()
    {
        if (attackPoint == null) return;

        Vector2 center = (Vector2)attackPoint.position;
        Collider2D[] hits = Physics2D.OverlapCircleAll(center, radius, enemyLayer);

        float damage = GetDamage();

        foreach (var hit in hits)
        {
            HealthSystem health = hit.GetComponent<HealthSystem>();
            if (health != null) health.TakeDamage(damage);
        }
    }
}
