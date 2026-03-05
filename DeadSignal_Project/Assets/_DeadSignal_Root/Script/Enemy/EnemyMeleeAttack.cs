using UnityEngine;

public class EnemyMeleeAttack : MonoBehaviour
{
    #region References
    EnemyFSM FSM;
    HealthSystem target;
    #endregion

    #region Attack Settings
    [Header("Attack Settings")]
    [SerializeField] float damage = 5f;
    [SerializeField] float attackCooldown = 2f;
    #endregion

    #region Runtime
    float lastAttackTime;
    #endregion

    private void Awake()
    {
        FSM = GetComponent<EnemyFSM>();
    }

    private void OnEnable()
    {
        FSM.OnStateChanged += HandleStateChanged;
    }

    private void OnDisable()
    {
        FSM.OnStateChanged -= HandleStateChanged;
    }

    #region Contact Detection
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Solo solicitar ataque si el cooldown terminó
        if (Time.time < lastAttackTime + attackCooldown)
            return;

        if (FSM.CurrentState != EnemyState.Move)
            return;

        if (collision.collider.CompareTag("Player") &&
            collision.collider.TryGetComponent(out HealthSystem health))
        {
            target = health;

            // Solicita el ataque a la FSM
            FSM.ChangeState(EnemyState.Attack);

            // Actualiza el cooldown aquí, **solo evita que se vuelva a solicitar Attack inmediatamente**
            lastAttackTime = Time.time;
        }
    }
    #endregion

    #region FSM Reaction
    void HandleStateChanged(EnemyState state)
    {
        if (state == EnemyState.Attack)
        {
            ExecuteAttack();
        }
    }
    #endregion

    #region Attack Logic
    void ExecuteAttack()
    {
        if (target != null)
        {
            target.TakeDamage(damage);
        }

        // Termina la acción inmediatamente, independiente del cooldown
        FSM.ActionFinished();

        // Limpieza de target
        target = null;
    }
    #endregion
}