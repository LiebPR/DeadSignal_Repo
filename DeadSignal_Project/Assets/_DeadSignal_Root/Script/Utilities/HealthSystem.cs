using System;
using UnityEngine;

/// <summary>
/// HealthSystem genérico para jugadores y enemigos.
/// Emite eventos OnHit y OnDeath para que cada entidad pueda reaccionar.
/// </summary>
public class HealthSystem : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] float maxHealth = 100f;

    public float CurrentHealth { get; private set; }

    #region Events
    /// <summary>Se lanza cada vez que recibe daño.</summary>
    public event Action<float> OnHit;

    /// <summary>Se lanza cuando la vida llega a cero.</summary>
    public event Action OnDeath;
    #endregion

    #region Immunity
    /// <summary>Controla si la entidad es inmune a daño.</summary>
    bool isImmune = false;

    /// <summary>Activa la inmunidad temporal al daño.</summary>
    public void ActivateImmunity()
    {
        isImmune = true;
    }

    /// <summary>Desactiva la inmunidad y permite recibir daño nuevamente.</summary>
    public void DeactivateImmunity()
    {
        isImmune = false;
    }
    #endregion

    private void Awake()
    {
        CurrentHealth = maxHealth;
    }

    /// <summary>
    /// Aplica daño al HealthSystem. 
    /// Lanza OnHit y, si corresponde, OnDeath.
    /// </summary>
    /// <param name="amount">Cantidad de daño a aplicar</param>
    public void TakeDamage(float amount)
    {
        if (CurrentHealth <= 0f || isImmune)
            return; // Ya muerto o inmune, no hacer nada

        CurrentHealth -= amount;
        CurrentHealth = Mathf.Max(CurrentHealth, 0f);

        // Evento de hit
        OnHit?.Invoke(amount);

        // Muerte
        if (CurrentHealth <= 0f)
        {
            OnDeath?.Invoke();
        }
    }

    /// <summary>
    /// Método para curar la entidad si fuera necesario.
    /// </summary>
    /// <param name="amount">Cantidad a curar</param>
    public void Heal(float amount)
    {
        if (CurrentHealth <= 0f)
            return; // No curar si está muerto

        CurrentHealth += amount;
        CurrentHealth = Mathf.Min(CurrentHealth, maxHealth);
    }

    /// <summary>
    /// Reinicia la salud al máximo. Útil para reutilización o respawn.
    /// </summary>
    public void ResetHealth()
    {
        CurrentHealth = maxHealth;
        isImmune = false; // Opcional: resetear inmunidad también
    }
}