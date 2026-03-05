using UnityEngine;
using UnityEngine.SceneManagement;

public class DesktopIcon : MonoBehaviour
{
    public DesktopAction action;

    public void Open()
    {
        switch (action)
        {
            case DesktopAction.PlayGame:
                SceneManager.LoadScene("SCN_Gameplay");
                break;

            case DesktopAction.Ranking:
                SceneManager.LoadScene("SCN_Ranking");
                break;

            case DesktopAction.Lore:
                Debug.Log("Abrir Lore");
                break;
        }
    }
}

public enum DesktopAction
{
    PlayGame,
    Ranking,
    Lore
}