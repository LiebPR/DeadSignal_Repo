using UnityEngine;
using TMPro;

/// <summary>
/// Controla un Timer visual en la partida, mostrando el tiempo transcurrido al jugador.
/// </summary>
public class UITimer : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI timerText; // Puede ser TextMeshPro o Text de UnityEngine.UI

    [Header("Formato de tiempo")]
    public bool showMinutes = true;
    public bool showSeconds = true;
    public bool showMilliseconds = false;

    void OnEnable()
    {
        GameTimer.OnTick += UpdateTimerUI;
    }

    void OnDisable()
    {
        GameTimer.OnTick -= UpdateTimerUI;
    }

    /// <summary>
    /// Actualiza el texto del timer en pantalla.
    /// </summary>
    void UpdateTimerUI(float timeElapsed)
    {
        timerText.text = FormatTime(timeElapsed);
    }

    /// <summary>
    /// Convierte el tiempo en un string legible: mm:ss:ms
    /// </summary>
    string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        int milliseconds = Mathf.FloorToInt((time * 1000f) % 1000f);

        string text = "";
        if (showMinutes)
            text += minutes.ToString("00") + ":";
        if (showSeconds)
            text += seconds.ToString("00");
        if (showMilliseconds)
            text += ":" + milliseconds.ToString("000");

        return text;
    }
}