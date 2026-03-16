using UnityEngine;
using System.Collections;

public class EnemyShootAttack : MonoBehaviour
{
    [SerializeField] FireWeaponData weaponData;
    [SerializeField] Transform firePoint;
    [SerializeField] string bulletPoolTag = "EnemyBullet";
    [SerializeField] EnemyDetection detection; // Detector independiente

    EnemyFSM fsm;
    EnemyMovementController movementController;
    EnemyRotationController rotationController;
    float nextShootTime;

    private void Awake()
    {
        fsm = GetComponent<EnemyFSM>();
        movementController = GetComponent<EnemyMovementController>(); 
        rotationController = GetComponent<EnemyRotationController>();
    }

    private void OnEnable()
    {
        fsm.OnStateChanged += HandleStateChanged;
    }

    private void OnDisable()
    {
        fsm.OnStateChanged -= HandleStateChanged;
    }

    private void Update()
    {
        if (detection.PlayerInRange)
        {
            // Detener movimiento al respetar el margen
            movementController.StopMovement();
            rotationController.ResumeRotation();

            // Cambiar inmediatamente a Shoot si no está ya en Shoot
            if (fsm.CurrentState != EnemyState.Shoot)
            {
                fsm.ChangeState(EnemyState.Shoot);
            }
        }
    }

    private void HandleStateChanged(EnemyState state)
    {
        if (state == EnemyState.Shoot)
        {
            StartCoroutine(ShootRoutine());
        }
        else
        {
            // Reiniciar movimiento si sale del estado Shoot
            movementController.ResumeMovement(); 
        }
    }

    private IEnumerator ShootRoutine()
    {
        // Solo disparar si ha pasado el cooldown
        if (Time.time >= nextShootTime)
        {
            GameObject bulletGO = PoolManager.Instance.SpawnFromPool(
                bulletPoolTag,
                firePoint.position,
                firePoint.rotation
            );

            Bullet bullet = bulletGO.GetComponent<Bullet>();
            bullet.Initialize(weaponData);

            nextShootTime = Time.time + weaponData.fireRate;
        }

        // Esperar duración de la acción antes de liberar movimiento
        yield return new WaitForSeconds(weaponData.shootDuration);

        fsm.ActionFinished();
    }
}