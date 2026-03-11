using UnityEngine;

public class RifleWeapon : FireWeaponController
{
    bool shooting;
    float nextShootTime;

    void Update()
    {
        if (!shooting) return;

        if (Time.time >= nextShootTime)
        {
            SpawnBullet();
            nextShootTime = Time.time + data.fireRate;
        }
    }

    public override void StartShoot()
    {
        shooting = true;
    }

    public override void StopShoot()
    {
        shooting = false;
    }
}
