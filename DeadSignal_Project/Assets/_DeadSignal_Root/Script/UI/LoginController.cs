using UnityEngine;

public class LoginController : MonoBehaviour
{
    public void ConfirmLogin()
    {
        MenuDirector.Instance.ChangeState(MenuState.Desktop);
    }
}