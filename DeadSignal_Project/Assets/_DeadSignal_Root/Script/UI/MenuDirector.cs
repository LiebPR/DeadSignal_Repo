using System;
using UnityEngine;

/// <summary>
/// Controla los estados del menú principal.
/// </summary>
public class MenuDirector : MonoBehaviour
{
    public static MenuDirector Instance { get; private set; }

    public MenuState CurrentState { get; private set; } = MenuState.Boot;

    public event Action<MenuState> OnStateChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        ChangeState(MenuState.Intro);
    }

    public void ChangeState(MenuState newState)
    {
        if (CurrentState == newState) return;

        CurrentState = newState;

        Debug.Log("Menu State → " + newState);

        OnStateChanged?.Invoke(newState);
    }
}

public enum MenuState
{
    Boot,
    Intro,
    Login,
    Desktop
}