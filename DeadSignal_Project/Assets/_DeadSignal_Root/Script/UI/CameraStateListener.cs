using UnityEngine;
using System.Collections;

public class CameraStateListener : MonoBehaviour
{
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
        switch (state)
        {
            case MenuState.Intro:

                CameraManager.Instance.ActivateApproachCamera();

                StartCoroutine(OpenLogin());

                break;

            case MenuState.Login:

                CameraManager.Instance.ActivateScreenCamera();

                break;
        }
    }

    IEnumerator OpenLogin()
    {
        yield return new WaitForSeconds(0.1f);

        MenuDirector.Instance.ChangeState(MenuState.Login);
    }
}