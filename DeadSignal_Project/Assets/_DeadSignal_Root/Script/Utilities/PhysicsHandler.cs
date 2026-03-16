using UnityEngine;

/// <summary>
/// [PhysicsHandler2D] - Gestiona la activación y desactivación de físicas
/// para objetos que usan Object Pooling.
/// </summary>
public class PhysicsHandler : MonoBehaviour, IReusable
{
    #region Variables

    Rigidbody2D rb;
    Collider2D[] colliders;

    bool originalSimulated;
    bool[] originalColliderState;

    #endregion

    #region Initialization

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        colliders = GetComponentsInChildren<Collider2D>();

        if (rb != null)
            originalSimulated = rb.simulated;

        originalColliderState = new bool[colliders.Length];

        for (int i = 0; i < colliders.Length; i++)
            originalColliderState[i] = colliders[i].enabled;
    }

    #endregion

    #region Pool Interface

    public void OnObjectReuse()
    {
        // Reset de velocidades al reutilizar el objeto
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
    }

    #endregion

    #region Unity Events

    void OnEnable()
    {
        RestorePhysics();
    }

    void OnDisable()
    {
        DisablePhysics();
    }

    #endregion

    #region Physics Control

    void DisablePhysics()
    {
        if (rb != null)
            rb.simulated = false;

        foreach (var col in colliders)
            col.enabled = false;
    }

    void RestorePhysics()
    {
        if (rb != null)
            rb.simulated = originalSimulated;

        for (int i = 0; i < colliders.Length; i++)
            colliders[i].enabled = originalColliderState[i];
    }

    #endregion
}