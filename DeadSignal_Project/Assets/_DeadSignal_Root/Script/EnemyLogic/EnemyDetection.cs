using UnityEngine;
using System.Collections;
public class EnemyDetection : MonoBehaviour
{
    [SerializeField] string playerTag = "Player"; // Tag del jugador
    [SerializeField] float detectionDelay = 0.5f; // Tiempo antes de considerar al jugador en rango

    public bool PlayerInRange { get; private set; } = false;
    private Coroutine detectionCoroutine;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Ignorar si el objeto no está activo o si el enemigo está muerto
        if (!gameObject.activeInHierarchy || !isActiveAndEnabled)
            return;

        if (collision.CompareTag(playerTag))
        {
            if (detectionCoroutine != null)
                StopCoroutine(detectionCoroutine);

            detectionCoroutine = StartCoroutine(ActivateAfterDelay());
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(playerTag))
        {
            if (detectionCoroutine != null)
                StopCoroutine(detectionCoroutine);

            PlayerInRange = false;
        }
    }

    private IEnumerator ActivateAfterDelay()
    {
        yield return new WaitForSeconds(detectionDelay);
        PlayerInRange = true;
    }

    private void OnDisable()
    {
        if (detectionCoroutine != null)
        {
            StopCoroutine(detectionCoroutine);
            detectionCoroutine = null;
        }
    }
}
