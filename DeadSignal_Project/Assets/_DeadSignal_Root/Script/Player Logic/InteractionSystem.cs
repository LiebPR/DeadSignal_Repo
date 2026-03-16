using UnityEngine;

/// <summary>
/// InteractionSystem: Gestiona el raycast y la interacción del jugador.
/// </summary>
public class InteractionSystem : MonoBehaviour
{
    #region Inspector States
    [SerializeField] float interactRange = 2f;
    [SerializeField] LayerMask interactMask = ~0;
    [SerializeField] Transform interactionPoint;
    #endregion

    #region Internal States
    IInteractable highlightedTarget;
    PlayerFSM playerFSM;
    #endregion

    void Awake()
    {
        playerFSM = GetComponent<PlayerFSM>();
    }

    void OnEnable()
    {
        if (playerFSM != null)
            playerFSM.OnStateChanged += HandleStateChanged;
    }

    void OnDisable()
    {
        if (playerFSM != null)
            playerFSM.OnStateChanged -= HandleStateChanged;
    }

    void Update()
    {
        HandleHighlight();
    }

    #region FSM Listener
    void HandleStateChanged(PlayerActionState state)
    {
        if (state == PlayerActionState.Interaction)
        {
            ExecuteInteraction();
        }
    }
    #endregion

    #region Interaction Logic
    void ExecuteInteraction()
    {
        if (highlightedTarget == null) return;

        highlightedTarget.OnPress();
        highlightedTarget.OnRelease();
    }
    #endregion

    #region Highlight Logic
    void HandleHighlight()
    {
        if (interactionPoint == null) return;

        Vector2 origin = interactionPoint.position;
        Vector2 direction = interactionPoint.up;

        RaycastHit2D hit = Physics2D.Raycast(origin, direction, interactRange, interactMask);

        IInteractable newTarget = null;

        if (hit.collider != null)
            newTarget = hit.collider.GetComponent<IInteractable>();

        if (newTarget != highlightedTarget)
        {
            RemoveHighlight();

            highlightedTarget = newTarget;

            if (highlightedTarget != null)
                highlightedTarget.OnHighlight();
        }
    }

    void RemoveHighlight()
    {
        if (highlightedTarget == null) return;

        highlightedTarget.OnRemoveHighlight();
        highlightedTarget = null;
    }
    #endregion

    #region Gizmos Debug
    void OnDrawGizmos()
    {
        if (interactionPoint == null) return;

        Vector2 origin = interactionPoint.position;
        Vector2 direction = interactionPoint.up;

        Gizmos.color = Color.green;
        Gizmos.DrawLine(origin, origin + direction * interactRange);
    }
    #endregion
}