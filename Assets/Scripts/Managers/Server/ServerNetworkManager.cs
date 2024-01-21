using System.Collections;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ServerNetworkManager : MonoBehaviour
{
    IEnumerator Start()
    {
        yield return UnityServices.InitializeAsync();
        // This is to help us keep track of the server vs multiple clients
        AuthenticationService.Instance.SwitchProfile("Server");
        DontDestroyOnLoad(gameObject);
        SceneManager.LoadScene("Level1");
        NetworkManager.Singleton.StartServer();
    }
}
