using System.Text;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Client
{
    // Note: This is NOT a NetworkBehaviour. It doesn't need to be for any reason
    public class ClientNetworkManager : MonoBehaviour
    {
        public static ClientNetworkManager Instance {  get; private set; }

        private void Awake() => Instance = this;

        public void ConnectToServer(string characterName)
        {
            var playerId = AuthenticationService.Instance.PlayerId;
            var playerName = AuthenticationService.Instance.PlayerName;
            Debug.LogWarning($"Sending Connection Data: {playerId},{characterName}  name: {playerName}");
            
            // This only sets up the configuration data for this client. This does not start the client
            NetworkManager.Singleton.NetworkConfig.ConnectionData =
                // This formats relavent data to be parsed on the server
                Encoding.ASCII.GetBytes($"{playerId}|{characterName}|{playerName}");

            // This is where the magic for connecting/starting the client happens
            var result = NetworkManager.Singleton.StartClient();
            Debug.Log($"Client Started {result}");

            // Unsubscribe any existing HandleDisconnect callbacks that may be hanging around for any reason
            NetworkManager.Singleton.OnClientDisconnectCallback -= HandleDisconnect;
            // Subscribe to the callback
            NetworkManager.Singleton.OnClientDisconnectCallback += HandleDisconnect;
        }

        private void HandleDisconnect(ulong obj)
        {
            // Unsubscribe from the existing HandleDisconnect callback bc we are handling disconnect right now.
            // There's no reason to keep the callback if we're already disconnected
            NetworkManager.Singleton.OnClientDisconnectCallback -= HandleDisconnect;
            SceneManager.LoadScene("Client");
        }
    }
}
