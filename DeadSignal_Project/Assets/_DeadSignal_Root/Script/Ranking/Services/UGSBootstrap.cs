using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using System.Threading.Tasks;

public class UGSBootstrap : MonoBehaviour
{
    public static bool IsReady { get; private set; }

    private async void Awake()
    {
        await Initialize();
    }

    private async Task Initialize()
    {
        if (IsReady) return;

        await UnityServices.InitializeAsync();

        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        Debug.Log("UGS listo. PlayerID: " + AuthenticationService.Instance.PlayerId);

        IsReady = true;
    }
}