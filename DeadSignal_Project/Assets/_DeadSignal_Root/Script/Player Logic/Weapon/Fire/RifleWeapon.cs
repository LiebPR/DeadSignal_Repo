using UnityEngine;

/// <summary>
/// [RifleWeapon]: Implementación concreta de un arma automática tipo rifle.
/// Dispara de forma continua mientras el jugador mantiene el estado de disparo. 
/// Incluye un sistema de "ease-in" para que la cadencia aumente progresivamente
/// al comenzar a disparar
/// </summary>
public class RifleWeapon : FireWeaponController
{
    #region Fire Rate Settings
    [Tooltip("Tiempo que tarda el arma en alcanzar suu cadencia normal. (Durante este periodo el disparo se acelera progresivamente)")]
    [SerializeField] float easeInDuration = 0.3f; // tiempo que tarda en alcanzar la cadencia normal
    [Tooltip("Multiplicador de retraso inicial del primer disparo. (Un valor mayor significa que el arma disparando más lento)")]
    [SerializeField] float maxInitialDelayMultiplier = 2f; // cuánto más lento empieza
    #endregion

    #region Internal States
    bool shooting; //india si el arma está actualmente disparado.
    float lastShootTime; //momento de último disparo realizado.
    #endregion

    /// <summary>
    /// Controla la lógica de disparo continuo del rifle. 
    /// Se ejecuta cada frame mientras el arma esté en estado de disparo.
    /// </summary>
    void Update()
    {
        
        if (!shooting) return;

        //Tiempo transcurrido desde el último disparo
        float timeSinceStart = Time.time - lastShootTime;

        //Cálculamos el progreso del "ease-in"
        float t = Mathf.Clamp01(timeSinceStart / easeInDuration);

        //Interpolamos el intervalo de disparo desde un valor lento hacia la  cadencia real.
        float interval = data.fireRate * Mathf.Lerp(maxInitialDelayMultiplier, 1f, t);

        //Si ha apsado el intervalo requerido, generamos una nueva bala
        if (Time.time >= lastShootTime + interval)
        {
            SpawnBullet();
            lastShootTime = Time.time;
        }
    }

    #region Shoot Control
    /// <summary>
    /// Incia el estado de disparo automático del rifle.
    /// </summary>
    public override void StartShoot()
    {
        shooting = true;
        lastShootTime = Time.time; // Inicializamos el temporizador
    }

    /// <summary>
    /// Detiene el disparo automático.
    /// </summary>
    public override void StopShoot()
    {
        shooting = false;
    }
    #endregion
}