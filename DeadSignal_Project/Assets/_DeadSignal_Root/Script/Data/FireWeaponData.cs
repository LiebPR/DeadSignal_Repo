using UnityEngine;

[CreateAssetMenu(fileName = "FireWeaponData", menuName = "Scriptable Objects/FireWeaponData")]
public class FireWeaponData : ScriptableObject
{
    public string weaponName;
    public Sprite uiIcon;

    public string bulletName; // Nombre de la bullet para el PoolManager
    public GameObject bulletPrefab; // Prefab de la bala
    public float fireRate = 0.2f; // Tiempo entre disparos
    public float bulletSpeed = 20f;
    public float bulletLife = 5f; // Vida de l bala
    public float lifeTime = 1f; //Tiempo de vida de la bala antes de destruirse
    public bool automatic = true; // Disparo sostenido o por click

    [Header("Shotgun Settings")]
    public int bulletsPerShot = 1; //Cuantas balas salen por disparo
    public float spreadAngle = 15f; //Ángulo máximo de dispersión en grados 
}
