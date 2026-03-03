using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyMovementController : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] float speed = 5f;
    [SerializeField] float stopDistance = 1f;
    [SerializeField] LayerMask forbiddenZones; // Capas de zonas que el enemigo no puede pisar
    [SerializeField] float checkDistance = 0.5f; // Distancia para comprobar si se va a meter en zona prohibida

    Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (player == null)
        {
            GameObject target = GameObject.FindGameObjectWithTag("Player");
            if (target != null) player = target.transform;
        }
    }

    void FixedUpdate()
    {
        if (player == null) return;

        Vector2 direction = (player.position - transform.position);
        float distanceToPlayer = direction.magnitude;

        if (distanceToPlayer > stopDistance)
        {
            Vector2 moveDirection = direction.normalized;

            // Chequeamos si el siguiente paso estaría dentro de una zona prohibida
            RaycastHit2D hit = Physics2D.Raycast(rb.position, moveDirection, checkDistance, forbiddenZones);
            if (hit.collider != null)
            {
                // Zona prohibida detectada: calculamos dirección perpendicular para esquivar
                Vector2 perp = Vector2.Perpendicular(moveDirection).normalized;

                RaycastHit2D side1 = Physics2D.Raycast(rb.position, perp, checkDistance, forbiddenZones);
                RaycastHit2D side2 = Physics2D.Raycast(rb.position, -perp, checkDistance, forbiddenZones);

                if (side1.collider == null)
                    moveDirection = (moveDirection + perp).normalized;
                else if (side2.collider == null)
                    moveDirection = (moveDirection - perp).normalized;
                else
                    moveDirection = Vector2.zero; // Bloqueado, espera un poco
            }

            rb.linearVelocity = moveDirection * speed;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    void OnDrawGizmosSelected()
    {
        if (rb == null || player == null) return;
        Gizmos.color = Color.red;
        Vector2 dir = (player.position - transform.position).normalized;
        Gizmos.DrawLine(rb.position, rb.position + dir * checkDistance);
    }
}