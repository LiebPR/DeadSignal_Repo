using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRotationController : MonoBehaviour
{
    Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
            Debug.LogWarning("No se encontró la cámara principal. Asegúrate de que haya una cámara en la escena con la etiqueta 'MainCamera'.");
    }

    private void Update()
    {
        HandleRotation();
    }

    #region Rotation Logic
    /// <summary>
    /// Gira al jugador para que siga al puntero del mouse usando Input System
    /// Proyectando el mouse a un plano que pase por el jugador
    /// </summary>
    void HandleRotation()
    {
        if (mainCamera == null || Mouse.current == null)
            return;

        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();

        // Crear un rayo desde la cámara hacia la posición del mouse
        Ray ray = mainCamera.ScreenPointToRay(mouseScreenPos);

        // Crear un plano en Z=0 (plano XY)
        Plane plane = new Plane(Vector3.forward, Vector3.zero);

        // Calcular punto de intersección del rayo con el plano
        if (plane.Raycast(ray, out float distance))
        {
            Vector3 mouseWorldPos = ray.GetPoint(distance);

            // Dirección del jugador al mouse
            Vector2 fromPlayerToMouse = mouseWorldPos - transform.position;

            // Calcula ángulo y aplica rotación
            float angle = Mathf.Atan2(fromPlayerToMouse.y, fromPlayerToMouse.x) * Mathf.Rad2Deg - 90f;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }
    }
    #endregion
}