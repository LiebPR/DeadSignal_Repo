using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/EnemyData")]
public class EnemyData : ScriptableObject
{
    [Header("Enemy Base Stadistics")]
    [Tooltip("Velocidad de movimiento base del enemigo.")]
    public float moveSpeed = 2f;
    [Tooltip("Velocidad de rotación del enemigo al perseguir al jugador.")]
    public float rotationSpeed = 10f;
    [Tooltip("Daño que el enemigo inflige al jugador al atacar.")]
    public float damage = 5f;
    [Tooltip("Tiempo de enfriamiento entre ataques del enemigo.")]
    public float attackCooldown = 1f;
    public float attackDuration = 0.5f;

    [Header("Enemy Spawn Settings")]
    [Tooltip("Duración del estado de spawn, durante el cual el enemigo es invulnerable y no afecta ni es afectado por la física.")]
    public float spawnDuration = 1.5f;

}
