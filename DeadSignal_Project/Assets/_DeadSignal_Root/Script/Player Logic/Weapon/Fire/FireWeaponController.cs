using UnityEngine;

public enum FireWeaponType
{
    Empty, 
    AssaultRifle,
    Shotgun, 
    Sniper, 
    Flamethrower
}

public abstract class FireWeaponController : MonoBehaviour
{
    [SerializeField] protected FireWeaponData data;
    [SerializeField] protected Transform bulletSpawnPoint;

    public abstract void StartShoot();
    public abstract void StopShoot();

    protected void SpawnBullet()
    {
        GameObject bulletGO = PoolManager.Instance.SpawnFromPool(
            data.bulletName,
            bulletSpawnPoint.position,
            bulletSpawnPoint.rotation
        );

        if (bulletGO == null) return;

        Bullet bullet = bulletGO.GetComponent<Bullet>();

        if (bullet != null)
        {
            bullet.Initialize(data);

            if (HeadShotHighlightSystem.CurrentHoveringHead)
                bullet.SetLifeMultiplier(2f);
        }
    }
}
