using UnityEngine;
using System;

/// <summary>
/// [FireWeaponType]: Tipos de armas de fuego disponibles en el sistema. 
/// Se utiliza para identificar armas en slots y sistemas de equipamiento.
/// </summary>
public enum FireWeaponType
{
    Empty, 
    AssaultRifle,
    Shotgun, 
    Sniper, 
    Flamethrower
}

/// <summary>
/// [FireWeaponController]: Clase base abstracta para todas las armas de fuego. 
/// Define la interfaz común y la lógica compartida entre armas.
/// Cada arma concreta debe implementar su propio comportamiento de disparo.
/// </summary>
public abstract class FireWeaponController : MonoBehaviour
{
    #region Inspector References
    [Tooltip("Datos configurables del arma (daño, velocidad de bala, icono UI, etc...")]
    [SerializeField] protected FireWeaponData data;
    [Tooltip("Punto desde el cual se instancian las balas.")]
    [SerializeField] protected Transform bulletSpawnPoint;
    #endregion

    #region Events
    /// <summary>
    /// Evento global que se dispara cada vez que un arma genera una bala.
    /// Puse ser usado por sistemas de sonido, efectos, UI, etc.
    /// </summary>
    public static event Action OnWeaponFired;
    #endregion

    #region Abstract Shoot Interface
    /// <summary>
    /// Inicia el proceso de disparo del arma.
    /// La implementación depende del tipo de arma. 
    /// </summary>
    public abstract void StartShoot();

    /// <summary>
    /// Detiene el proceso de disparo del arma.
    /// No todas las armas necesitan implementar lógica aquí.
    /// </summary>
    public abstract void StopShoot();
    #endregion

    #region UI Utilities
    /// <summary>
    /// Devuelce el icono del arma para su representación en la UI [WeaponSlotsUI]
    /// </summary>
    public Sprite GetUIIcon() => data.uiIcon;
    #endregion

    #region Internal Shoot Utilities
    /// <summary>
    /// Invoca el evento global de disparo.
    /// </summary>
    protected void InvokeOnWeaponFired()
    {
        OnWeaponFired?.Invoke();
    }

    /// <summary>
    /// Aplica modificadores de headShot a la bala si el sistema de highlight
    /// indica que el cursor está sobre la cabeza de un enemigo.
    /// </summary>
    protected void HandleHeadShot(Bullet bullet)
    {
        if (bullet == null) return;

        if (HeadShotHighlightSystem.CurrentHoveringHead)
            bullet.SetLifeMultiplier(2f); //duplica el daño o vida efectiva de la bala
    }
    
    /// <summary>
    /// GEnera una bala utilizando el sistema de pooling.
    /// Inicializa sus parámetros y aplcia lógica de headShot si corresponde.
    /// </summary>
    protected void SpawnBullet()
    {
        GameObject bulletGO = PoolManager.Instance.SpawnFromPool(data.bulletName, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
        if (bulletGO == null) return;

        Bullet bullet = bulletGO.GetComponent<Bullet>();
        if (bullet != null)
        {
            //Inicializa parámetros de la bala usando los datos del arma
            bullet.Initialize(data);
            HandleHeadShot(bullet); //aplica bonus de headShot si procede
        }

        // Notifica que el arma ha disparado
        InvokeOnWeaponFired();
    }
    #endregion
}
