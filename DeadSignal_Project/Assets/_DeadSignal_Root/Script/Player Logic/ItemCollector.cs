using UnityEngine;

public class ItemCollectorTest : MonoBehaviour
{
    public float detectionRadius = 2f;

    void Update()
    {
        // Detecta items cercanos en 2D
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRadius);
        foreach (Collider2D hit in hits)
        {
            Absorbable absorbable = hit.GetComponent<Absorbable>();
            if (absorbable != null && !absorbable.IsBeingAbsorbed)
            {
                absorbable.StartAbsorption(gameObject);
            }
        }
    }

    // Ver la zona de detección en la escena
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}