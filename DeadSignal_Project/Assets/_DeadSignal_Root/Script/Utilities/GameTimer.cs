using System;
using System.Collections.Generic;

/// <summary>
/// Timer global estático para la partida con soporte de acciones programadas y recurrentes.
/// </summary>
public static class GameTimer
{
    /// <summary>
    /// Tiempo transcurrido en segundos desde el inicio de la partida.
    /// </summary>
    public static float TimeElapsed { get; private set; }

    /// <summary>
    /// Evento que se dispara cada frame con el tiempo actualizado.
    /// </summary>
    public static event Action<float> OnTick;

    /// <summary>
    /// Lista de todas las acciones programadas.
    /// </summary>
    private static readonly List<ScheduledAction> scheduledActions = new List<ScheduledAction>();

    /// <summary>
    /// Clase interna que representa una acción programada.
    /// </summary>
    private class ScheduledAction
    {
        public float startTime;      // Momento de inicio
        public float interval;       // Intervalo entre repeticiones (0 = solo una vez)
        public float? endTime;       // Opcional, si se quiere limitar duración
        public Action action;        // Acción a ejecutar
        public float nextTriggerTime;// Próximo disparo
    }

    /// <summary>
    /// Inicializa el timer.
    /// </summary>
    public static void Initialize()
    {
        TimeElapsed = 0f;
        scheduledActions.Clear();
    }

    /// <summary>
    /// Debe llamarse desde un MonoBehaviour en Update para avanzar el tiempo.
    /// </summary>
    public static void Tick(float deltaTime)
    {
        TimeElapsed += deltaTime;

        OnTick?.Invoke(TimeElapsed);

        for (int i = 0; i < scheduledActions.Count; i++)
        {
            var sa = scheduledActions[i];

            if (TimeElapsed >= sa.nextTriggerTime && (sa.endTime == null || TimeElapsed <= sa.endTime))
            {
                sa.action?.Invoke();
                if (sa.interval > 0)
                {
                    // Programar siguiente ejecución
                    sa.nextTriggerTime += sa.interval;
                }
                else
                {
                    // Ejecutada una vez, eliminarla
                    sa.nextTriggerTime = float.MaxValue;
                }
            }
        }

        // Opcional: limpiar acciones que ya terminaron
        scheduledActions.RemoveAll(sa => sa.nextTriggerTime == float.MaxValue || (sa.endTime != null && TimeElapsed > sa.endTime));
    }

    /// <summary>
    /// Programa una acción que se ejecuta una sola vez.
    /// </summary>
    public static void ScheduleOnce(float time, Action action)
    {
        scheduledActions.Add(new ScheduledAction
        {
            startTime = time,
            nextTriggerTime = time,
            interval = 0f,
            action = action
        });
    }

    /// <summary>
    /// Programa una acción recurrente a intervalos específicos, opcionalmente con tiempo de fin.
    /// </summary>
    public static void ScheduleRepeated(float startTime, float interval, Action action, float? endTime = null)
    {
        if (interval <= 0)
            throw new ArgumentException("El intervalo debe ser mayor que 0 para acciones recurrentes.");

        scheduledActions.Add(new ScheduledAction
        {
            startTime = startTime,
            nextTriggerTime = startTime,
            interval = interval,
            endTime = endTime,
            action = action
        });
    }

    /// <summary>
    /// Reinicia el timer y las acciones programadas.
    /// </summary>
    public static void Reset()
    {
        TimeElapsed = 0f;
        foreach (var sa in scheduledActions)
            sa.nextTriggerTime = sa.startTime;
    }
}