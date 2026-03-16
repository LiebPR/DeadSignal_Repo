using UnityEngine;

/// <summary>
/// [ShotgunWeapon]: Implementación de un arma tipo escopeta.
/// Dispara múltiples proyectiles en un patrón de dispersión en cada disparo.
/// </summary>
public class ShotgunWeapon : FireWeaponController
{
    #region Internal State
    float lastShootTime; //Momento en el que se realizó el último disparo. Limitante de la cadencia de disparo
    #endregion

    #region Shoot Interface Implementation
    /// <summary>
    /// Inicia el disparo de la escopeta. 
    /// Adiferencia de armas automáticas, la escopeta dispara una sola vez
    /// cada vez que se activa este método.
    /// </summary>
    public override void StartShoot()
    {
        if (Time.time - lastShootTime < data.fireRate) return;

        ShootSpread();
        lastShootTime = Time.time;
    }

    /// <summary>
    /// La escopeta no requiere lógica al detener el disparo, 
    /// ya que cada activación produce un único disparo.
    /// </summary>
    public override void StopShoot() { }
    #endregion

    #region Shoot Logic
    /// <summary>
    /// Genera múltiples proyectiles aplicando un ángulo de dispersión
    /// aleatorio a cada uno de ellos.
    /// </summary>
    void ShootSpread()
    {
        for (int i = 0; i < data.bulletsPerShot; i++)
        {
            //Generamos un ángulo aleatorio dentro del rango de dispersión
            float angle = Random.Range(-data.spreadAngle, data.spreadAngle);

            //Calculamos la rotación final del proyectil
            Quaternion rot = bulletSpawnPoint.rotation * Quaternion.Euler(0, 0, angle);

            //Obtenemos una bala del sistema de pooling
            GameObject bulletGO = PoolManager.Instance.SpawnFromPool(data.bulletName, bulletSpawnPoint.position, rot);
            
            if (bulletGO == null) return;

            Bullet bullet = bulletGO.GetComponent<Bullet>();

            if (bullet != null)
            {
                //Inicializa la bala con los datos del arma
                bullet.Initialize(data);

                //Aplica bonus de headShot si corresponde
                HandleHeadShot(bullet);
            }

            //Notifica que el arma ha generado un disparo
            InvokeOnWeaponFired();
        }
    }
    #endregion
}