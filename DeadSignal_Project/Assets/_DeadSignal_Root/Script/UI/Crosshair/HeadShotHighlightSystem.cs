using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class HeadShotHighlightSystem : MonoBehaviour
{
    #region Inspector
    [Tooltip("Layer que define las cabezas que pueden recibir HeadShot")]
    [SerializeField] LayerMask headLayerMask;
    #endregion

    #region Events
    public static event Action<bool> OnHeadshotHoverChanged;
    #endregion

    #region Internal
    bool wasHoveringHead = false;
    Camera mainCamera;
    #endregion

    #region Public API
    // Permite que otros scripts (como ShootSystem) consulten si hay headshot activo
    public static bool CurrentHoveringHead { get; private set; }
    #endregion

    #region Unity Callbacks
    private void Awake()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    private void Update()
    {
        CheckHeadHover();
    }
    #endregion

    #region Core Logic
    void CheckHeadHover()
    {
        if (Mouse.current == null) return;

        Vector2 mouseScreen = Mouse.current.position.ReadValue();

        Vector3 mouseWorld3D = mainCamera.ScreenToWorldPoint(new Vector3(mouseScreen.x, mouseScreen.y, -mainCamera.transform.position.z));
        Vector2 mouseWorld2D = new Vector2(mouseWorld3D.x, mouseWorld3D.y);

        Collider2D hit = Physics2D.OverlapPoint(mouseWorld2D, headLayerMask);
        bool isHovering = hit != null;

        if (isHovering != wasHoveringHead)
        {
            wasHoveringHead = isHovering;
            CurrentHoveringHead = isHovering;
            OnHeadshotHoverChanged?.Invoke(isHovering);
        }
    }
    #endregion
}