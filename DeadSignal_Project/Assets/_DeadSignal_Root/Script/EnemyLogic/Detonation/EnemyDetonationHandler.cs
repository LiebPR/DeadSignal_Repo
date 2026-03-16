using UnityEngine;

public class EnemyDetonationHandler : MonoBehaviour
{
    #region References
    HealthSystem healthSystem;
    EnemyFSM FSM;
    #endregion

    private void Awake()
    {
        healthSystem = GetComponent<HealthSystem>();
        FSM = GetComponent<EnemyFSM>();
    }

    private void OnEnable()
    {
        if (healthSystem != null) healthSystem.OnDeath += HandleDetonation;
    }

    private void OnDisable()
    {
        if (healthSystem != null) healthSystem.OnDeath -= HandleDetonation;
    }

    void HandleDetonation()
    {
        if(FSM == null) return;

        //Cambiamos al estado Detonate
        FSM.ChangeState(EnemyState.Detonate);
    }
}
