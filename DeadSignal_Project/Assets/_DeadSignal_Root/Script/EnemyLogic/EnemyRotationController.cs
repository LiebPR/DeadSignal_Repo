using UnityEngine;

public class EnemyRotationController : MonoBehaviour
{
    #region References
    [SerializeField] EnemyData data;
    [SerializeField] Transform player; // Transform del jugador
    Rigidbody2D rb;
    #endregion

    #region Internal State
    bool canRotate = true; // Controla si el enemigo puede rotar (ej. aturdido)
    float currentVelocity; // Para SmoothDampAngle cuando está quieto
    #endregion

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        // Buscar automáticamente al jugador si no está asignado
        if (player == null)
        {
            GameObject playerGO = GameObject.FindGameObjectWithTag("Player");
            if (playerGO != null)
                player = playerGO.transform;
        }
    }

    private void FixedUpdate()
    {
        if (canRotate) RotateEnemy();
    }

    #region Core Logic
    /// <summary>
    /// Rota al enemigo hacia la dirección de movimiento o hacia el jugador si está quieto
    /// </summary>
    private void RotateEnemy()
    {
        if (player == null) return;

        Vector2 velocity = rb.linearVelocity;
        float targetAngle;

        if (velocity.sqrMagnitude > 0.001f)
        {
            // Rotar según la dirección de movimiento (como estaba antes)
            targetAngle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg - 90f;
            float angle = Mathf.LerpAngle(transform.eulerAngles.z, targetAngle, data.rotationSpeed * Time.fixedDeltaTime);
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }
        else
        {
            // Rotar hacia el jugador usando SmoothDampAngle cuando está quieto
            Vector2 direction = (player.position - transform.position).normalized;
            targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.z, targetAngle, ref currentVelocity, data.rotationSpeed * Time.fixedDeltaTime);
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }
    }
    #endregion

    #region Rotation Control API 
    public void StopRotation()
    {
        canRotate = false;
    }

    public void ResumeRotation()
    {
        canRotate = true;
    }
    #endregion
}