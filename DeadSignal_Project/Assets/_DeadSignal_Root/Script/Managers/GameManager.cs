using UnityEngine;

/// <summary>
/// GameManager central de la partida.
/// Controla el inicio del timer y otras mecánicas globales.
/// </summary>
public class GameManager : MonoBehaviour
{
    [Header("Opciones de partida")]
    public bool startTimerOnAwake = true; // Si el timer inicia automáticamente

    void Awake()
    {
        if (startTimerOnAwake)
        {
            StartGame();
        }
    }

    /// <summary>
    /// Llama al inicio de la partida: inicializa timer y otros sistemas.
    /// </summary>
    public void StartGame()
    {
        // Inicializar GameTimer
        GameTimer.Initialize();

        // Aquí puedes inicializar otros sistemas globales
        // Ej: Spawn de enemigos, UI, música, etc.
        Debug.Log("Partida iniciada. Timer activado.");
    }

    void Update()
    {
        // Avanza el GameTimer cada frame
        GameTimer.Tick(Time.deltaTime);
    }
}