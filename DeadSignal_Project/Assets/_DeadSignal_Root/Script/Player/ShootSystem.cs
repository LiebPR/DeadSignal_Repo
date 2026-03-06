using System;
using UnityEngine;

public class ShootSystem : MonoBehaviour
{
    [SerializeField] FireWeaponData data;
    [SerializeField] Transform bulletSpawnPoint;

    #region References
    PlayerFSM FSM;
    #endregion

    #region Events
    public static event Action OnShoot;
    #endregion

    float lastShootTime;
    bool isShooting;

    private void Awake()
    {
        FSM = GetComponent<PlayerFSM>();
        if (FSM == null)
        {
            Debug.LogError("ShootSystem requiere un PlayerFSM en el mismo GameObject.");
        }
    }

    private void OnEnable()
    {
        FSM.OnStateChanged += HandleStateChanged;
    }

    private void OnDisable()
    {
        FSM.OnStateChanged -= HandleStateChanged;
    }

    private void Update()
    {
        if (!isShooting || data == null) return;

        // AUTOMÁTICO
        if (data.automatic)
        {
            if (Time.time - lastShootTime >= data.fireRate)
            {
                Shoot();
                lastShootTime = Time.time;
            }
        }
    }

    private void HandleStateChanged(PlayerActionState newState)
    {
        if (newState == PlayerActionState.Shooting)
        {
            isShooting = true;

            // SEMI-AUTOMÁTICO
            if (!data.automatic)
            {
                if (Time.time - lastShootTime >= data.fireRate)
                {
                    Shoot();
                    lastShootTime = Time.time;
                }
            }
        }
        else
        {
            isShooting = false;
        }
    }

    private void Shoot()
    {
        if (bulletSpawnPoint == null) return;

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
        }

        OnShoot?.Invoke();
    }

    public void EquipWeapon(FireWeaponData weapon)
    {
        data = weapon;
    }
}