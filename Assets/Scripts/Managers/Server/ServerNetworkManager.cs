using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Authentication.Server;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ServerNetworkManager : MonoBehaviour
{
    // Secrets in the codebase? *gasps and clutches pearls*
    string apiKey = "5a12269c-9c64-4090-b8a7-8bb414f09964";
    string apiSecret = "vm13R8LK0Fb9tHw7jsB2bBX_-aMCfAAC";

    async void Start()
    {
        await UnityServices.InitializeAsync();
        // This is to help us keep track of the server vs multiple clients
        AuthenticationService.Instance.SwitchProfile("Server");
        // This line needs the project to be setup as a Dedicated Server (Windows) in Build Settings
        await ServerAuthenticationService.Instance.SignInWithServiceAccountAsync(apiKey, apiSecret);
        // *Important: This sends the tokens to the authentication service. This must be done or nothing will work!
        AuthenticationService.Instance.ProcessAuthenticationTokens(ServerAuthenticationService.Instance.AccessToken);
        Debug.Log(ServerAuthenticationService.Instance.State);
        Debug.Log(ServerAuthenticationService.Instance.AccessToken);

        DontDestroyOnLoad(gameObject);
        SceneManager.LoadScene("Level1");
        NetworkManager.Singleton.StartServer();
    }
}
