using UnityEngine;

public class EnemyReusable : MonoBehaviour, IReusable
{
    HealthSystem healthSystem;
    EnemyFSM FSM;
    Collider2D col2D;

    private void Awake()
    {
        healthSystem = GetComponent<HealthSystem>();
        col2D = GetComponent<Collider2D>();
        FSM = GetComponent<EnemyFSM>();
    }

    /// <summary>
    /// Se llama cuando el PoolManager reutiliza este objeto.
    /// Reinicia la salud, activa el collider y reinicia la FSM al estado Spawn.
    /// </summary>
    public void OnObjectReuse()
    {
        if(healthSystem != null) healthSystem.ResetHealth(); // Resetea la salud al máximo, en caso de que haya HealthSystem
        col2D.enabled = true; //Reactiva el collider en caso de que este apagado
        FSM.InterruptState(EnemyState.Spawn); //Pasa Spawn para que el enemigo vuelva a su estado inicial
    }
}
