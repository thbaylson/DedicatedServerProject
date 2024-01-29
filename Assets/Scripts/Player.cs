using Cloud;
using System;
using System.Collections;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

// This will be shared between Client and Server
public class Player : NetworkBehaviour
{
    public static Player LocalPlayer { get; private set; }

    // TODO: This should be its own object. Same data as what's found in ServerPlayerManager.
    public NetworkVariable<FixedString32Bytes> PlayerId;
    public NetworkVariable<FixedString32Bytes> PlayerName;
    public NetworkVariable<FixedString32Bytes> CharacterName;
    
    private Character _character;

    private void Awake()
    {
        // Important: These need to be initialized outside their field declaration or else it'll cause a memory leak
        // Important (cont.): We init the NetworkVariables, but we can't set their Values yet. It's best to do that in OnNetworkSpawn
        PlayerId = new();
        PlayerName = new();
        CharacterName = new();
    }

    public override void OnNetworkSpawn()
    {
        // This only runs server-side
        if (IsServer)
        {
            PlayerId.Value = ServerPlayerManager.GetPlayerId(OwnerClientId);
            PlayerName.Value = ServerPlayerManager.GetPlayerName(OwnerClientId);
            CharacterName.Value = ServerPlayerManager.GetCharacterName(OwnerClientId);

            StartCoroutine(LoadCharacterAsync());
        }

        // This only runs client-side
        if (IsOwner)
        {
            LocalPlayer = this;
        }

        gameObject.name = $"({PlayerName.Value}) {CharacterName.Value}";
    }

    public override void OnNetworkDespawn()
    {
        if(IsServer && _character != null)
        {
            // This gives ownership of the object to the server.
            _character.NetworkObject.ChangeOwnership(0);
        }
    }

    private async Awaitable LoadCharacterAsync()
    {
        if (RetakeExistingCharacter())
        {
            return;
        }

        var persistedCharacterData = await PlayerSaveWrapper.LoadCharacterOnServer(PlayerId.Value.Value, CharacterName.Value.Value);
        _character = await ServerCharacterManager.SpawnCharacterFromCloudData(persistedCharacterData, this);
        Debug.Log($"Loaded {persistedCharacterData.Name} {persistedCharacterData.Class}");
    }

    /// <summary>
    /// Attempts to allow players to retake control of a character that may be left on the server.
    /// TODO: This one method is really doing two different things. This logic should probably be split.
    /// </summary>
    /// <returns>True if the retake was successful, otherwise returns false.</returns>
    private bool RetakeExistingCharacter()
    {
        bool result = false;

        // Check to see if the character object is still hanging around on the server.
        var existing = ServerCharacterManager.GetExistingCharacter(PlayerId.Value.Value);
        if(existing != null)
        {
            _character = existing;
            _character.NetworkObject.ChangeOwnership(OwnerClientId);
            result = true;
        }

        return result;
    }

    // Using an expression body to call the RPC for the sake of abstaction
    public void ClickedNavMesh(Vector3 navHitPosition) => ClickedNavMeshServerRpc(navHitPosition);

    [ServerRpc]
    private void ClickedNavMeshServerRpc(Vector3 navHitPosition)
    {
        _character.SetDestinationOnNavMesh(navHitPosition);
    }

    public void LeaveServer() => StartCoroutine(SaveThenLeave());

    private IEnumerator SaveThenLeave()
    {
        // Save Here
        yield return null;
        Debug.LogWarning("Save Complete");

        LeaveServerClientRpc();
        Destroy(_character.gameObject);
    }

    [ClientRpc]
    private void LeaveServerClientRpc()
    {
        NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene("Client");
    }
}
