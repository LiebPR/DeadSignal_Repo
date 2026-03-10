using System.Collections;
using UnityEngine;

public class ProximityDetonation : MonoBehaviour
{
    [SerializeField] float detectionRadius = 3f;
    [SerializeField] float detonationDelay = 0.5f;
    [SerializeField] LayerMask playerLayer;

    #region References
    EnemyFSM FSM;
    #endregion

    bool playerDetected;
    Coroutine detonationRoutine;

    void Awake()
    {
        FSM = GetComponent<EnemyFSM>();
    }

    private void Update()
    {
        CheckPlayerRange();
    }

    void CheckPlayerRange()
    {
        Collider2D player = Physics2D.OverlapCircle(transform.position, detectionRadius, playerLayer);
        if (player != null && !playerDetected)
        {
            playerDetected = true;
            detonationRoutine = StartCoroutine(DetonationDelay());
        }

        if (player == null && playerDetected)
        {
            playerDetected = false;

            if(detonationRoutine != null)
                StopCoroutine(detonationRoutine);
        }
    }

    IEnumerator DetonationDelay()
    {
        yield return new WaitForSeconds(detonationDelay);
        if (playerDetected && FSM != null)
            FSM.ChangeState(EnemyState.Detonate);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
