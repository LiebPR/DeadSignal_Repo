using UnityEngine;

public class EnemyDeathHandler : MonoBehaviour
{
    HealthSystem healthSystem;
    EnemyFSM FSM;

    private void Awake()
    {
        healthSystem = GetComponent<HealthSystem>();
        FSM = GetComponent<EnemyFSM>();
    }

    private void OnEnable()
    {
        if(healthSystem != null) 
            healthSystem.OnDeath += HandleDeath;
    }

    private void OnDisable()
    {
        if(healthSystem != null) 
            healthSystem.OnDeath -= HandleDeath;
    }

    void HandleDeath()
    {
        if (FSM != null)
        {
            // Forzamos estado Death sin que nada lo interrumpa
            FSM.ChangeState(EnemyState.Death);
            ExecuteDeath();

            EnemyDeathCounter counter = Object.FindFirstObjectByType<EnemyDeathCounter>();
            if (counter != null)
                counter.AddKill();
        }
    }

    void ExecuteDeath()
    {
        if(FSM == null || FSM.CurrentState != EnemyState.Death) return;

        // Desactivar colisionadores 2D
        Collider2D col2d = GetComponent<Collider2D>();
        if(col2d != null) col2d.enabled = false;

        // Apagar el objeto para que PoolManager lo recicle
        gameObject.SetActive(false);
    }
}