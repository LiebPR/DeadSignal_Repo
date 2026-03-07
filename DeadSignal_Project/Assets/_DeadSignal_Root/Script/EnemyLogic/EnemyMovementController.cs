using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyMovementController : MonoBehaviour
{
    #region References
    [SerializeField] Transform player;
    [SerializeField] EnemyData data;

    Rigidbody2D rb;
    #endregion

    #region Internal State
    private bool canMove = true; //Controla si el enemigo puede moverse (ej. aturdido)
    #endregion

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void FixedUpdate()
    {
        if (!canMove || player == null) return;

        // Movimiento simple hacia el jugador
        Vector2 moveDirection = ((Vector2)player.position - (Vector2)transform.position).normalized;
        rb.linearVelocity = moveDirection * data.moveSpeed;
    }

    #region Movement Control API
    /// <summary>
    /// Bloquea el movimiento del enemigo
    /// </summary>
    public void StopMovement()
    {
        canMove = false;
        rb.linearVelocity = Vector2.zero;
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