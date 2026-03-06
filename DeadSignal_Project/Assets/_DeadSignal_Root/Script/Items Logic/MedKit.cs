using UnityEngine;

/// <summary>
/// MediKit que cura al jugadro cuando se absorbe.
/// </summary>
public class MedKit : Absorbable
{
    [Header("Configuración de curación")]
    [SerializeField] float healAmount = 25f;

    public override bool CanBeAbsorbedBy(GameObject player)
    {
        HealthSystem health = player.GetComponent<HealthSystem>();
        if (health == null) return false;

        // Solo se puede absorber si no está a máxima vida
        return health.CurrentHealth < health.MaxHealth;
    }

    public override void ApplyGameplayEffect(GameObject player)
    {
        HealthSystem health = player.GetComponent<HealthSystem>();
        if (health != null)
        {
            health.Heal(healAmount);
        }

        Deactivate();
    }
}
