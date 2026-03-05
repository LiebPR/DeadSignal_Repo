using UnityEngine;

public class MenuUIController : MonoBehaviour
{
    [SerializeField] GameObject loginCanvas;
    [SerializeField] GameObject desktopCanvas;

    private void Start()
    {
        if (MenuDirector.Instance != null)
            MenuDirector.Instance.OnStateChanged += HandleState;
    }

    private void OnDestroy()
    {
        if (MenuDirector.Instance != null)
            MenuDirector.Instance.OnStateChanged -= HandleState;
    }

    void HandleState(MenuState state)
    {
        loginCanvas.SetActive(state == MenuState.Login);
        desktopCanvas.SetActive(state == MenuState.Desktop);
    }
}