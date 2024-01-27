using Cloud;
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

    private async Awaitable LoadCharacterAsync()
    {
        var persistedCharacterData = await PlayerSaveWrapper.LoadCharacterOnServer(PlayerId.Value.Value, CharacterName.Value.Value);
        _character = await ServerCharacterManager.SpawnCharacterFromCloudData(persistedCharacterData, this);
        Debug.Log($"{persistedCharacterData.Name} {persistedCharacterData.Class}");
    }

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
