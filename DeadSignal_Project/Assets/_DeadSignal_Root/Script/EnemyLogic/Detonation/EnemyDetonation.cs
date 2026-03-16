using System.Collections;
using UnityEngine;

public class EnemyDetonation : MonoBehaviour
{
    #region Settings Inspector
    [Header("Explosion")]
    [SerializeField] float damage = 40f;

    [Header("Timing")]
    [SerializeField] float expandDuration = 2f;
    [SerializeField] float maxSizeDuration = 1f;

    [Header("Warning Area")]
    [SerializeField] SpriteRenderer warningRenderer;

    [Header("Explosion Collider")]
    [SerializeField] Collider2D explosionTrigger;
    #endregion

    #region References
    EnemyFSM FSM;
    Rigidbody2D rb;
    HealthSystem healthSystem;
    #endregion

    #region Internal Variables
    Vector2 targetScale;
    #endregion

    void Awake()
    {
        FSM = GetComponent<EnemyFSM>();
        rb = GetComponent<Rigidbody2D>();
        healthSystem = GetComponent<HealthSystem>();

        if (warningRenderer != null)
        {
            targetScale = warningRenderer.transform.localScale;

            warningRenderer.enabled = false;
            warningRenderer.transform.localScale = Vector3.zero;
        }

        if (explosionTrigger != null)
            explosionTrigger.enabled = false;
    }

    void OnEnable()
    {
        if (FSM != null)
            FSM.OnStateChanged += OnStateChanged;
    }

    void OnDisable()
    {
        if (FSM != null)
            FSM.OnStateChanged -= OnStateChanged;
    }

    void OnStateChanged(EnemyState state)
    {
        if (state == EnemyState.Detonate)
            StartCoroutine(DetonationRoutine());
    }

    IEnumerator DetonationRoutine()
    {
        warningRenderer.enabled = true;

        healthSystem.ActivateImmunity();

        yield return StartCoroutine(ExpandArea());

        yield return new WaitForSeconds(maxSizeDuration);

        warningRenderer.enabled = false;

        Explode();
    }

    IEnumerator ExpandArea()
    {
        float time = 0f;

        Transform area = warningRenderer.transform;
        area.localScale = Vector3.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;

        while (time < expandDuration)
        {
            float t = time / expandDuration;

            area.localScale = Vector3.Lerp(Vector3.zero, targetScale, t);

            time += Time.deltaTime;
            yield return null;
        }

        area.localScale = targetScale;
    }

    void Explode()
    {
        explosionTrigger.enabled = true;
        rb.bodyType = RigidbodyType2D.Dynamic;
        healthSystem.DeactivateImmunity();
        StartCoroutine(DisableExplosion());
    }

    IEnumerator DisableExplosion()
    {
        yield return new WaitForSeconds(0.05f);

        explosionTrigger.enabled = false;

        gameObject.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        HealthSystem health = other.GetComponent<HealthSystem>();

        if (health != null)
            health.TakeDamage(damage);
    }
}