using UnityEngine;

/// <summary>
/// Script que se coloca en cualquier objeto que pueda ser absorbido.
/// Compatible con Object Pooling.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class Absorbable : MonoBehaviour, IAbsorbable, IReusable
{
    [Header("Configuración de absorción")]
    [SerializeField] float absorptionSpeed = 5f;      // Velocidad a la que se mueve hacia el jugador
    [SerializeField] float minDistance = 0.1f;        // Distancia mínima para activar el efecto de gameplay
    
    protected bool isBeingAbsorbed = false;

    public bool IsBeingAbsorbed => isBeingAbsorbed;

    protected Transform playerTarget;
    protected Vector3 initialScale;

    private void Awake()
    {
        initialScale = transform.localScale; // Guardamos escala original
    }

    private void FixedUpdate()
    {
        if (!isBeingAbsorbed || playerTarget == null) return;

        // Dirección hacia el jugador ignorando Z
        Vector2 targetPos2D = new Vector2(playerTarget.position.x, playerTarget.position.y);
        Vector2 currentPos2D = new Vector2(transform.position.x, transform.position.y);

        // Movimiento 2D con Lerp
        Vector2 newPos = Vector2.Lerp(currentPos2D, targetPos2D, absorptionSpeed * Time.fixedDeltaTime);
        transform.position = new Vector3(newPos.x, newPos.y, transform.position.z);

        // Escalado progresivo
        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, absorptionSpeed * Time.fixedDeltaTime);

        // Comprobar si llegó
        if (Vector2.Distance(currentPos2D, targetPos2D) <= minDistance)
        {
            ApplyGameplayEffect(playerTarget.gameObject);
            Deactivate(); // Apagar en vez de destruir
        }
    }

    /// <summary>
    /// Comprueba si el item puede iniciar absorción con este jugador.
    /// Sobrescribir en cada item para condiciones específicas.
    /// </summary>
    public virtual bool CanBeAbsorbedBy(GameObject player)
    {
        // Por defecto, siempre se puede absorber
        return true;
    }

    public void StartAbsorption(GameObject player)
    {
        if (!CanBeAbsorbedBy(player))
            return; // No iniciar absorción si la condición falla

        playerTarget = player.transform;
        isBeingAbsorbed = true;
        ApplyAbsorptionEffect(playerTarget);
    }

    /// <summary>
    /// Efecto visual de absorción (puedes sobrescribir)
    /// </summary>
    public virtual void ApplyAbsorptionEffect(Transform player)
    {
        // Por defecto, solo se reduce el tamaño en FixedUpdate
    }

    /// <summary>
    /// Efecto de gameplay al llegar al jugador
    /// </summary>
    public virtual void ApplyGameplayEffect(GameObject player)
    {
        // Sobrescribir en items concretos (ej: MedKit)
    }

    /// <summary>
    /// Apaga el objeto y lo devuelve al pool
    /// </summary>
    public virtual void Deactivate()
    {
        isBeingAbsorbed = false;
        playerTarget = null;
        transform.localScale = initialScale;
        gameObject.SetActive(false);
    }

    public virtual void CancelAbsorption()
    {
        //Cancelar la absorción y devolver item a su estado original
        isBeingAbsorbed = false;
        playerTarget = null;
        transform.localScale = initialScale;
    }

    /// <summary>
    /// Método de IReusable para reiniciar estado cuando se reutiliza desde pool
    /// </summary>
    public virtual void OnObjectReuse()
    {
        isBeingAbsorbed = false;
        playerTarget = null;
        transform.localScale = initialScale;
    }
}