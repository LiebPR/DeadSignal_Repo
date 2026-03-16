using UnityEngine;
using System.Collections;

public class EnemyMeleeAttack : MonoBehaviour
{
    #region References
    [SerializeField] EnemyData data;

    EnemyFSM FSM;
    HealthSystem target;
    #endregion

    #region Runtime
    float lastAttackTime;
    #endregion

    void Awake()
    {
        FSM = GetComponent<EnemyFSM>();
    }

    void OnEnable()
    {
        FSM.OnStateChanged += HandleStateChanged;
    }

    void OnDisable()
    {
        FSM.OnStateChanged -= HandleStateChanged;
    }

    #region Target Detection
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player") &&
            collision.collider.TryGetComponent(out HealthSystem health))
        {
            target = health;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            target = null;
        }
    }
    #endregion

    #region Attack Request
    void Update()
    {
        if (target == null)
            return;

        if (Time.time < lastAttackTime + data.attackCooldown)
            return;

        FSM.ChangeState(EnemyState.Attack);
    }
    #endregion

    void HandleStateChanged(EnemyState state)
    {
        if (state == EnemyState.Attack)
        {
            StartCoroutine(AttackRoutine());
        }
    }

    IEnumerator AttackRoutine()
    {
        // Bloqueamos movimiento automáticamente porque el FSM no está en Move
        if (target != null)
            target.TakeDamage(data.damage);

        lastAttackTime = Time.time;

        // Esperamos la duración del ataque antes de volver a Move
        yield return new WaitForSeconds(data.attackDuration);

        // Acción finalizada
        FSM.ActionFinished();
    }
}