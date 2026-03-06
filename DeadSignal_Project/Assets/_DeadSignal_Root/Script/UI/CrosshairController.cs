using UnityEngine;
using UnityEngine.InputSystem;

public class CrosshairController : MonoBehaviour
{
    #region Inspector Variables & References
    [Tooltip("Referencia al RecTransform del Crosshair.")]
    [SerializeField] RectTransform crosshair;

    [Header("Movement Effect")]
    [Tooltip("Máxima rotación permitida")]
    [SerializeField] float maxRotation = 15f;
    [Tooltip("Velocidad a la que rota el Crosshair al moverse.")]
    [SerializeField] float rotationSpeed = 10f;
    [Tooltip("Velocidad a la que vuelve a su rotación original. (En caso de que se quede quieto.")]
    [SerializeField] float returnSpeed = 6f;
    [Tooltip("Motiplicador para exagerar la rotación.")]
    [SerializeField] float movementMultiplier = 6f;

    [Header("Shoot Effect")]
    [Tooltip("Punto máximo al que escala el Crosshair cuando dispara")]
    [SerializeField] float pulseScale = 1.4f;
    [Tooltip("Velocidad a la que hace el efecto de disparo.")]
    [SerializeField] float pulseSpeed = 10f;
    #endregion

    #region Internal Variables & References
    Vector3 currentRotation;
    Vector3 originalScale;

    bool isPulsing;
    float pulseTimer;
    #endregion


    #region Initialization

    void Start()
    {
        if (crosshair == null)
        {
            Debug.LogError("[CrosshairController]: Crosshair no asignado, el sistema no podra ejecutar los efectos.");
            return;
        }

        originalScale = crosshair.localScale;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
    }

    private void OnEnable()
    {
        ShootSystem.OnShoot += TriggerPulse;
    }

    private void OnDisable()
    {
        ShootSystem.OnShoot -= TriggerPulse;
    }
    #endregion


    #region Update

    void Update()
    {
        FollowMouse();
        HandleRotation();
        HandlePulse();
    }

    #endregion


    #region Mouse Follow

    /// <summary>
    /// Hace que el crosshair siga exactamente al ratón
    /// </summary>
    void FollowMouse()
    {
        if (Mouse.current == null)
            return;

        Vector2 mousePos = Mouse.current.position.ReadValue();
        crosshair.position = mousePos;
    }

    #endregion


    #region Rotation Logic

    /// <summary>
    /// Aplica rotación reactiva según el movimiento del ratón
    /// </summary>
    void HandleRotation()
    {
        if (Mouse.current == null)
            return;

        Vector2 delta = Mouse.current.delta.ReadValue() * movementMultiplier;

        bool mouseMoving = delta.sqrMagnitude > 0.01f;

        if (mouseMoving)
        {
            float targetZ = Mathf.Clamp(-delta.x, -maxRotation, maxRotation);
            float targetX = Mathf.Clamp(delta.y, -maxRotation, maxRotation);

            Vector3 targetRotation = new Vector3(targetX, 0f, targetZ);

            currentRotation = Vector3.Lerp(currentRotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
        else
        {
            currentRotation = Vector3.Lerp(currentRotation, Vector3.zero, Time.deltaTime * returnSpeed);
        }

        crosshair.localRotation = Quaternion.Euler(currentRotation);
    }

    #endregion


    #region Pulse Effect

    /// <summary>
    /// Llamar a este método cuando el jugador dispare
    /// </summary>
    public void TriggerPulse()
    {
        isPulsing = true;
        pulseTimer = 0f;
    }

    void HandlePulse()
    {
        if (!isPulsing)
            return;

        pulseTimer += Time.deltaTime * pulseSpeed;

        float scale = Mathf.Lerp(pulseScale, 1f, pulseTimer);
        crosshair.localScale = originalScale * scale;

        if (pulseTimer >= 1f)
        {
            crosshair.localScale = originalScale;
            isPulsing = false;
        }
    }

    #endregion
}