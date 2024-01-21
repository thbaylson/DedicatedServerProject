using System.Collections.Generic;
using System.Text;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// A manager to allow the server to keep track of player data. Some player data may include secrets, so handle them with care.
/// </summary>
public class ServerPlayerManager: MonoBehaviour
{
    // Three separate collections to keep in sync... why not make an object at this point?? (Answer: Course material simplicity)
    // TODO: Fix this.
    static readonly Dictionary<ulong, string> _connectionToId = new();
    static readonly Dictionary<ulong, string> _connectionToCharacterName = new();
    static readonly Dictionary<ulong, string> _connectionToPlayerName = new();

    private void Awake()
    {
        // This overrides the default method that approves everything. This will let us write our own approval logic.
        NetworkManager.Singleton.ConnectionApprovalCallback = HandleConnectionApproval;
        DontDestroyOnLoad(gameObject);
    }

    private void HandleConnectionApproval(
        NetworkManager.ConnectionApprovalRequest request, 
        NetworkManager.ConnectionApprovalResponse response)
    {
        // This is trusting that ClientNetworkManager doesn't change.
        // TODO: Find a more robust way to handle this data. Probably JSON serialization/deserialization
        var args = Encoding.ASCII.GetString(request.Payload).Split("|");
        _connectionToId[request.ClientNetworkId] = args[0];
        _connectionToCharacterName[request.ClientNetworkId] = args[1];
        _connectionToPlayerName[request.ClientNetworkId] = args[2];

        response.CreatePlayerObject = true;
        response.Approved = true;
        Debug.LogWarning($"Approved Player: {request.ClientNetworkId} {args[0]} {args[1]} {args[2]}");
    }

    public static string GetPlayerId(ulong ownerClientId) =>
    _connectionToId.GetValueOrDefault(ownerClientId);

    public static string GetCharacterName(ulong ownerClientId) =>
        _connectionToCharacterName.GetValueOrDefault(ownerClientId);

    public static string GetPlayerName(ulong ownerClientId) =>
        _connectionToPlayerName.GetValueOrDefault(ownerClientId);
}
