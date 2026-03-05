using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private string gameSceneName = "GameScene";
   
    private void OnEnable()
    {
        MainMenuEvents.OnStartPressed += HandleStart;
        MainMenuEvents.OnExitPressed += HandleExit;
    }

    private void OnDisable()
    {
        MainMenuEvents.OnStartPressed -= HandleStart;
        MainMenuEvents.OnExitPressed -= HandleExit;
    }

    private void HandleStart()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    private void HandleExit()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
