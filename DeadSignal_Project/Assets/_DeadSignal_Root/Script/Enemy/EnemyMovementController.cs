using UnityEngine;

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

        //EnemyFSM
        if (FSM != null) 
            FSM.OnStateChanged += HandleStateChange;
    }

    private void OnDisable()
    {
        //ENemyFSM
        if(FSM != null)
            FSM.OnStateChanged -= HandleStateChange;
    }

    private void FixedUpdate()
    {
        if (!canMove || player == null) return;

        // Moverse directo hacia el jugador
        Vector2 moveDirection = ((Vector2)player.position - (Vector2)transform.position).normalized;
        rb.linearVelocity = moveDirection * data.moveSpeed;
    }

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