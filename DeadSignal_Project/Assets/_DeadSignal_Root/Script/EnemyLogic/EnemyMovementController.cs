using UnityEngine;

/// <summary>
/// Controla el movimiento del enemigo y bloquea su RB cuando realiza acciones especiales
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class EnemyMovementController : MonoBehaviour
{
    #region References
    [SerializeField] Transform player;
    [SerializeField] EnemyData data;

    Rigidbody2D rb;
    EnemyFSM FSM;
    EnemyRotationController rotationController;
    #endregion

    #region Internal State
    private Vector2 moveDirection;
    bool canMove = true; // Controla si el enemigo puede moverse (ej. aturdido o acción especial)
    #endregion

    #region Unity Callbacks
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
        if (FSM != null)
            FSM.OnStateChanged -= HandleStateChange;
    }

    private void FixedUpdate()
    {
        if (!canMove || player == null)
            return;

        // Moverse directo hacia el jugador
        moveDirection = ((Vector2)player.position - (Vector2)transform.position).normalized;
        rb.linearVelocity = moveDirection * data.moveSpeed;
    }
    #endregion

    #region Handle FSM Events
    void HandleStateChange(EnemyState newState)
    {
        // Si el estado es Move → permitir movimiento y física dinámica
        if (newState == EnemyState.Move)
        {
            ResumeMovement();
            rotationController.ResumeRotation();
        }
        // Para cualquier otro estado de acción especial → bloquear movimiento y poner RB cinemático
        else
        {
            StopMovement();
            rotationController.StopRotation();
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
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
        rb.bodyType = RigidbodyType2D.Dynamic; // Vuelve a física normal
    }
    #endregion
}