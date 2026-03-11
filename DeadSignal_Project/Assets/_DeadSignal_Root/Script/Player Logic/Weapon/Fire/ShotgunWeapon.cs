using UnityEngine;

public class ShotgunWeapon : FireWeaponController
{
    float lastShootTime;

    public override void StartShoot()
    {
        if (Time.time - lastShootTime < data.fireRate) return;

        ShootSpread();
        lastShootTime = Time.time;
    }

    public override void StopShoot() { }

    void ShootSpread()
    {
        for (int i = 0; i < data.bulletsPerShot; i++)
        {
            float angle = Random.Range(-data.spreadAngle, data.spreadAngle);

            Quaternion rot =
                bulletSpawnPoint.rotation *
                Quaternion.Euler(0, 0, angle);

            GameObject bulletGO = PoolManager.Instance.SpawnFromPool(
                data.bulletName,
                bulletSpawnPoint.position,
                rot
            );

            Bullet bullet = bulletGO.GetComponent<Bullet>();

            if (bullet != null)
                bullet.Initialize(data);
        }
    }
}