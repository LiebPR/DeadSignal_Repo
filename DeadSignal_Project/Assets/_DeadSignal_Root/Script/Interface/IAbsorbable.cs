using UnityEngine;

/// <summary>
/// Interfaz que define la funcionalidad de un objeto absorbible.
/// </summary>
public interface IAbsorbable
{
    /// <summary>
    /// Efecto visual de absorción hacia el jugador (ej: hacerse más pequeño, brillar, etc.)
    /// </summary>
    /// <param name="player">Transform del jugador</param>
    void ApplyAbsorptionEffect(Transform player);

    /// <summary>
    /// Efecto de gameplay cuando se completa la absorción (ej: curar, aumentar level, etc.)
    /// </summary>
    /// <param name="player">Referencia al jugador o stats</param>
    void ApplyGameplayEffect(GameObject player);
}
