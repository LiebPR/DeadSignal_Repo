using UnityEngine;

public class EnemyRotationController : MonoBehaviour
{
    #region References
    [SerializeField] EnemyData data;
    
    Rigidbody2D rb;
    #endregion

    #region Internal State
    bool canRotate = true; // Controla si el enemigo puede rotar (ej. aturdido)
    #endregion

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if(canRotate) RotateTowardsMovement();
    }

    #region Core Logic
    /// <summary>
    /// Rota al enemigo hacia la dirección de su movimiento
    /// </summary>
    private void RotateTowardsMovement()
    {
        Vector2 velocity = rb.linearVelocity;

        if (velocity.sqrMagnitude < 0.001f) return; // No rotar si está casi quieto

        // Ángulo deseado en grados
        float targetAngle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;

        // Rotación suave
        float angle = Mathf.LerpAngle(transform.eulerAngles.z, targetAngle, data.rotationSpeed * Time.fixedDeltaTime);
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
    #endregion

    #region Rotation Control API 
    /// <summary>
    /// BLoquea la rotación del enemigo
    /// </summary>
    public void StopRotation()
    {
        canRotate = false;
    }

    /// <summary>
    /// Restaura la rotación del enemigo 
    /// </summary>
    public void ResumeRotation()
    {
        canRotate = true;
    }
    #endregion
}