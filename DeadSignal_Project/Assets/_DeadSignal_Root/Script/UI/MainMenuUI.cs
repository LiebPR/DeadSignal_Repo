using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
  public void StartPressed()
    {
        MainMenuEvents.OnStartPressed?.Invoke();
    }

    public void ExitPressed()
    {
        MainMenuEvents.OnExitPressed?.Invoke();
    }

}
