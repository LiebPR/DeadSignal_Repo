using UnityEngine;

public class EnemyMovementController : MonoBehaviour
{
    #region References
    Rigidbody2D rb;
    EnemyFSM FSM;
    EnemyRotationController rotationController;
    [SerializeField] Transform player;
    #endregion

    #region Settings
    [Header("Movement Settings")]
    [SerializeField] float moveSpeed = 2f;
    [Tooltip("Distancia para detectar obstáculos")]
    [SerializeField]  float obstacleCheckDistance = 0.5f;
    [Tooltip("Ángulo para evasión lateral (en grados)")]
    [SerializeField] float sideCheckAngle = 30f;
    [SerializeField] LayerMask obstacleLayer;
    #endregion

    #region Internal State
    private Vector2 moveDirection;
    bool canMove = true; //Controla si el enemigo puede moverse (ej. aturdido)
    #endregion

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        FSM = GetComponent<EnemyFSM>();
        rotationController = GetComponent<EnemyRotationController>();
    }

    private void OnEnable()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;

        if (FSM != null) 
            FSM.OnStateChanged += HandleStateChange;
    }

    private void OnDisable()
    {
        if(FSM != null)
            FSM.OnStateChanged -= HandleStateChange;
    }

    void FixedUpdate()
    {
        if (player == null || !canMove) return; // Sin jugador o movimiento bloqueado

        moveDirection = (player.position - transform.position).normalized; // Evita obstáculos y ajusta dirección
        moveDirection = AvoidObstacles(moveDirection); // Agrega fuerza de separación entre enemigos
        moveDirection = moveDirection.normalized; // Normaliza para mantener velocidad constante

        rb.linearVelocity = moveDirection * moveSpeed; // <- cambio crítico
    }

    #region Obstacle Detection
    /// <summary>
    /// Ajusta la dirección para evitar obstáculos frontales
    /// </summary>
    private Vector2 AvoidObstacles(Vector2 dir)
    {
        // Raycast frontal
        if (!Physics2D.Raycast(transform.position, dir, obstacleCheckDistance, obstacleLayer))
            return dir; // Nada en frente

        // Si hay obstáculo, prueba direcciones laterales
        float[] angles = { sideCheckAngle, -sideCheckAngle, 2 * sideCheckAngle, -2 * sideCheckAngle };
        foreach (float angle in angles)
        {
            Vector2 newDir = RotateVector(dir, angle);
            if (!Physics2D.Raycast(transform.position, newDir, obstacleCheckDistance, obstacleLayer))
                return newDir;
        }

        // Si todas las direcciones bloqueadas, se detiene
        return Vector2.zero;
    }

    /// <summary>
    /// Rota un vector 2D en grados
    /// </summary>
    private Vector2 RotateVector(Vector2 v, float degrees)
    {
        float rad = degrees * Mathf.Deg2Rad;
        float cos = Mathf.Cos(rad);
        float sin = Mathf.Sin(rad);
        return new Vector2(v.x * cos - v.y * sin, v.x * sin + v.y * cos).normalized;
    }
    #endregion

    #region Handle Events FSM 
    void HandleStateChange(EnemyState newState)
    {
        //FSM dice Move y canMove es false -> activar movimiento
        if (newState == EnemyState.Move && !canMove)
        {
            ResumeMovement();
            rotationController.ResumeRotation();
        }
        //FSM dice otro estado y canMove es true -> detener movimiento
        else if (newState != EnemyState.Move && canMove)
        {
            StopMovement();
            rotationController.StopRotation();
        }
    }
    #endregion

    #region Movement Control API
    /// <summary>
    /// Bloquea el movimiento del enemigo
    /// </summary>
    public void StopMovement()
    {
        canMove = false;
        rb.linearVelocity = Vector2.zero; // Detiene instantáneamente
    }

    /// <summary>
    /// Restaura el movimiento del enemigo
    /// </summary>
    public void ResumeMovement()
    {
        canMove = true;
    }
    #endregion
}