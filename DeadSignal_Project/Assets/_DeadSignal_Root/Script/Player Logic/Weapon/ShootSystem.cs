using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShootSystem : MonoBehaviour
{
    [SerializeField] FireWeaponData data;
    [SerializeField] Transform bulletSpawnPoint;

    #region References
    PlayerFSM FSM;
    Camera mainCamera;
    #endregion

    #region Events
    public static event Action OnShoot;
    #endregion

    float lastShootTime;
    bool isShooting;

    #region Unity Callbacks
    private void Awake()
    {
        FSM = GetComponent<PlayerFSM>();
        if (FSM == null)
            Debug.LogError("[ShootSystem]: ShootSystem requiere un PlayerFSM en el mismo GameObject.");

        if (mainCamera == null)
            mainCamera = Camera.main;
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
        if (data.automatic && Time.time - lastShootTime >= data.fireRate)
        {
            Shoot();
            lastShootTime = Time.time;
        }
    }
    #endregion

    #region FSM Event Handle
    private void HandleStateChanged(PlayerActionState newState)
    {
        if (newState == PlayerActionState.Shooting)
        {
            isShooting = true;

            // SEMI-AUTOMÁTICO
            if (!data.automatic && Time.time - lastShootTime >= data.fireRate)
            {
                Shoot();
                lastShootTime = Time.time;
            }
        }
        else
        {
            isShooting = false;
        }
    }
    #endregion

    #region Shoot Core
    private void Shoot()
    {
        if (bulletSpawnPoint == null) return;

        // Consultamos la propiedad del sistema de highlight
        bool headShot = HeadShotHighlightSystem.CurrentHoveringHead;

        GameObject bulletGO = PoolManager.Instance.SpawnFromPool(data.bulletName, bulletSpawnPoint.position, bulletSpawnPoint.rotation);

        if (bulletGO == null)
        {
            Debug.LogError("PoolManager no devolvió bala");
            return;
        }

        Bullet bullet = bulletGO.GetComponent<Bullet>();
        if (bullet != null)
        {
            bullet.Initialize(data);

            if (headShot)
                bullet.SetLifeMultiplier(2f);
        }

        OnShoot?.Invoke();
    }
    #endregion

    #region Utilities
    public void EquipWeapon(FireWeaponData weapon)
    {
        data = weapon;
    }
    #endregion
}