using UnityEngine;
using System;
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

    public static event Action OnWeaponFired;

    public abstract void StartShoot();
    public abstract void StopShoot();

    public Sprite GetUIIcon() => data.uiIcon;
   
    protected void InvokeOnWeaponFired()
    {
        OnWeaponFired?.Invoke();
    }

    protected void HandleHeadShot(Bullet bullet)
    {
        if (bullet == null) return;

        if (HeadShotHighlightSystem.CurrentHoveringHead)
            bullet.SetLifeMultiplier(2f); // Duplicar vida o daño si es headshot
    }

    protected void SpawnBullet()
    {
        GameObject bulletGO = PoolManager.Instance.SpawnFromPool(data.bulletName, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
        if (bulletGO == null) return;

        Bullet bullet = bulletGO.GetComponent<Bullet>();
        if (bullet != null)
        {
            bullet.Initialize(data);
            HandleHeadShot(bullet);
        }

        // Dispara el evento al crear la bala
        InvokeOnWeaponFired();
    }
}
