using UnityEngine;

public class Bullet : MonoBehaviour, IReusable
{
    [SerializeField] LayerMask obstacleLayer;   // Capas que bloquean la bala
    [SerializeField] LayerMask hiteableLayer;   // Capas que la bala puede dañar

    #region Internal States
    float speed;
    float life;        // Vida actual de la bala (cuánto daño le queda)
    float maxLife;     // Vida total inicial (para resetear al reutilizar)
    float lifeTime;    // Tiempo máximo de vida antes de desactivarse
    float lifeTimer;
    #endregion

    /// <summary>
    /// Inicializa la bala con los datos del arma
    /// </summary>
    public void Initialize(FireWeaponData data)
    {
        speed = data.bulletSpeed;
        maxLife = data.bulletLife;
        life = maxLife;
        lifeTime = data.lifeTime;
        lifeTimer = 0f;
    }

    #region IReusable Method
    /// <summary>
    /// Resetea la bala cuando se reutiliza desde PoolManager
    /// </summary>
    public void OnObjectReuse()
    {
        life = maxLife;
        lifeTimer = 0f;
    }
    #endregion

    private void Update()
    {
        // Movimiento
        transform.position += transform.up * speed * Time.deltaTime;

        // Auto-destrucción por vida máxima
        lifeTimer += Time.deltaTime;
        if (lifeTimer >= lifeTime)
        {
            Deactivate();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        int layerMask = 1 << collision.gameObject.layer;

        // Colisión con hiteables
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
        // Colisión con obstáculos
        else if ((layerMask & obstacleLayer) != 0)
        {
            Deactivate();
        }
    }

    private void Deactivate()
    {
        gameObject.SetActive(false);
    }
}