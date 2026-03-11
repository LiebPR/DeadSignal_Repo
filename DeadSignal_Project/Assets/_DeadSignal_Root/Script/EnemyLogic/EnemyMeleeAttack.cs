using UnityEngine;

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
            Debug.Log($"Attacking {target.name} for {data.damage} damage");
            target.TakeDamage(data.damage);
        }
        else
        {
            Debug.Log("No target to attack");
        }

        lastAttackTime = Time.time;
        FSM.ActionFinished();
    }
    #endregion
}