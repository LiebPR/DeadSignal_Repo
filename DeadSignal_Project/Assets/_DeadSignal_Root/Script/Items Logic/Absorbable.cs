using UnityEngine;

/// <summary>
/// Script que se coloca en cualquier objeto que pueda ser absorbido.
/// Compatible con Object Pooling.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class Absorbable : MonoBehaviour, IAbsorbable, IReusable
{
    #region Configuración
    [Header("Configuración de absorción")]
    [SerializeField] float absorptionSpeed = 5f;
    [SerializeField] float minDistance = 0.1f;
    #endregion

    #region Estado
    protected bool isBeingAbsorbed = false;
    public bool IsBeingAbsorbed => isBeingAbsorbed;
    protected Transform playerTarget;
    #endregion

    #region Referencias
    SpriteRenderer spriteRenderer;
    #endregion

    #region Valores iniciales
    protected Vector3 initialScale;
    float initialAlpha;
    #endregion

    void Awake()
    {
        initialScale = transform.localScale;

        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
            initialAlpha = spriteRenderer.color.a;
    }

    void FixedUpdate()
    {
        if (!isBeingAbsorbed || playerTarget == null) return;

        // Dirección hacia el jugador ignorando Z
        Vector2 targetPos2D = new Vector2(playerTarget.position.x, playerTarget.position.y);
        Vector2 currentPos2D = new Vector2(transform.position.x, transform.position.y);

        // Movimiento
        Vector2 newPos = Vector2.Lerp(currentPos2D, targetPos2D, absorptionSpeed * Time.fixedDeltaTime);
        transform.position = new Vector3(newPos.x, newPos.y, transform.position.z);

        // Escala progresiva
        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, absorptionSpeed * Time.fixedDeltaTime);

        // Transparencia progresiva
        if (spriteRenderer != null)
        {
            Color c = spriteRenderer.color;
            c.a = Mathf.Lerp(c.a, 0f, absorptionSpeed * Time.fixedDeltaTime);
            spriteRenderer.color = c;
        }

        // Comprobar si llegó
        if (Vector2.Distance(currentPos2D, targetPos2D) <= minDistance)
        {
            ApplyGameplayEffect(playerTarget.gameObject);
            Deactivate();
        }
    }

    #region Absorción
    public virtual bool CanBeAbsorbedBy(GameObject player)
    {
        return true;
    }

    public void StartAbsorption(GameObject player)
    {
        if (!CanBeAbsorbedBy(player))
            return;

        playerTarget = player.transform;
        isBeingAbsorbed = true;

        ApplyAbsorptionEffect(playerTarget);
    }

    public virtual void ApplyAbsorptionEffect(Transform player)
    {
        // Sobrescribir si se quiere añadir VFX extra
    }

    public virtual void ApplyGameplayEffect(GameObject player)
    {
        // Sobrescribir en items concretos
    }
    #endregion

    #region Reset / Pool
    public virtual void Deactivate()
    {
        isBeingAbsorbed = false;
        playerTarget = null;

        transform.localScale = initialScale;

        ResetAlpha();

        gameObject.SetActive(false);
    }

    public virtual void CancelAbsorption()
    {
        isBeingAbsorbed = false;
        playerTarget = null;

        transform.localScale = initialScale;

        ResetAlpha();
    }

    public virtual void OnObjectReuse()
    {
        isBeingAbsorbed = false;
        playerTarget = null;

        transform.localScale = initialScale;

        ResetAlpha();
    }

    void ResetAlpha()
    {
        if (spriteRenderer != null)
        {
            Color c = spriteRenderer.color;
            c.a = initialAlpha;
            spriteRenderer.color = c;
        }
    }
    #endregion
}