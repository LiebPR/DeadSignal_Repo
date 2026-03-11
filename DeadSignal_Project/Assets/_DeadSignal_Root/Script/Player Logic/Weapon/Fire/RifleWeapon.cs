using UnityEngine;

public class RifleWeapon : FireWeaponController
{
    bool shooting;
    float lastShootTime;

    [SerializeField] float easeInDuration = 0.3f; // tiempo que tarda en alcanzar la cadencia normal
    [SerializeField] float maxInitialDelayMultiplier = 2f; // cuánto más lento empieza

    void Update()
    {
        if (!shooting) return;

        float timeSinceStart = Time.time - lastShootTime;

        // Calculamos cuánto debería ser el intervalo actual usando ease-in progresivo
        float t = Mathf.Clamp01(timeSinceStart / easeInDuration);
        float interval = data.fireRate * Mathf.Lerp(maxInitialDelayMultiplier, 1f, t);

        if (Time.time >= lastShootTime + interval)
        {
            SpawnBullet();
            lastShootTime = Time.time;
        }
    }

    public override void StartShoot()
    {
        shooting = true;
        lastShootTime = Time.time; // Inicializamos el temporizador
    }

    public override void StopShoot()
    {
        shooting = false;
    }
}