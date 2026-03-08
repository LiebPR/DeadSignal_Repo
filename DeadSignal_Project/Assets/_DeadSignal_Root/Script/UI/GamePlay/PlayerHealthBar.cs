using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Barra de vida del jugador usando Image tipo Filled.
/// Actualiza instantáneamente al cambiar la vida.
/// </summary>
public class PlayerHealthBar : MonoBehaviour
{
    #region References 
    [Header("References")]
    [SerializeField] Image healthFill;          // Imagen tipo Filled (Horizontal)
    [SerializeField] HealthSystem healthSystem; // Sistema de vida del jugador
    #endregion

    private void Awake()
    {
        if (!healthFill) Debug.LogWarning("Asigna un Image tipo Filled al PlayerHealthBar.");
        if (!healthSystem) Debug.LogWarning("Asigna un HealthSystem al PlayerHealthBar.");
    }

    private void Start()
    {
        // Inicializa la barra al valor máximo
        UpdateHealthBar();

        // Suscribirse a eventos del HealthSystem
        healthSystem.OnHit += OnHit;
        healthSystem.OnDeath += OnDeath;
        healthSystem.OnHeal += OnHeal;
    }

    private void OnDestroy()
    {
        // Desuscribirse para evitar errores
        if (healthSystem != null)
        {
            healthSystem.OnHit -= OnHit;
            healthSystem.OnDeath -= OnDeath;
        }
    }

    #region Event Handlers
    private void OnHit(float damage)
    {
        UpdateHealthBar();
    }

    private void OnDeath()
    {
        healthFill.fillAmount = 0f;
    }

    private void OnHeal(float amount)
    {
        UpdateHealthBar();
    }
    #endregion

    /// <summary>
    /// Actualiza el fill de la barra según la vida actual
    /// </summary>
    private void UpdateHealthBar()
    {
        // fillAmount siempre va de 0 a 1
        healthFill.fillAmount = Mathf.Clamp01(healthSystem.CurrentHealth / healthSystem.MaxHealth);
    }
}