using UnityEngine;

public class Bullet : MonoBehaviour, IReusable
{
    [SerializeField] LayerMask obstacleLayer;
    [SerializeField] LayerMask hiteableLayer;

    [Header("Easy-Out Settings")]
    [SerializeField] bool easyOutSpeed = false;        // ¿Aplica desaceleración?
    [SerializeField] float minSpeedMultiplier = 0.5f; // Velocidad mínima relativa (50% por defecto)

    #region Internal States
    float speed;
    float initialSpeed;   // Guardamos la velocidad inicial
    float life;
    float maxLife;
    float lifeTime;
    float lifeTimer;
    #endregion

    public void Initialize(FireWeaponData data)
    {
        speed = data.bulletSpeed;
        initialSpeed = speed;
        maxLife = data.bulletLife;
        life = maxLife;
        lifeTime = data.lifeTime;
        lifeTimer = 0f;
    }

    public void OnObjectReuse()
    {
        life = maxLife;
        lifeTimer = 0f;
        speed = initialSpeed;
    }

    private void Update()
    {
        // Easy-out de velocidad
        if (easyOutSpeed)
        {
            float t = Mathf.Clamp01(lifeTimer / lifeTime); // Progreso de vida 0 → 1
            float targetMultiplier = Mathf.Lerp(1f, minSpeedMultiplier, t);
            speed = initialSpeed * targetMultiplier;
        }

        // Movimiento
        transform.position += transform.up * speed * Time.deltaTime;

        // Auto-destrucción por tiempo
        lifeTimer += Time.deltaTime;
        if (lifeTimer >= lifeTime)
        {
            Deactivate();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        int layerMask = 1 << collision.gameObject.layer;

        if ((layerMask & hiteableLayer) != 0)
        {
            HealthSystem health = collision.GetComponent<HealthSystem>();
            if (health != null)
            {
                float damageToDeal = Mathf.Min(life, health.CurrentHealth);
                health.TakeDamage(damageToDeal);
                life -= damageToDeal;

                if (life <= 0f)
                    Deactivate();
            }
        }
        else if ((layerMask & obstacleLayer) != 0)
        {
            Deactivate();
        }
    }

    public void SetLifeMultiplier(float multiplier)
    {
        life *= multiplier;
    }

    private void Deactivate()
    {
        gameObject.SetActive(false);
    }
}