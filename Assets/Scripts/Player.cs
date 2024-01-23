using Cloud;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

// This will be shared between Client and Server
public class Player : NetworkBehaviour
{
    // TODO: This should be its own object. Same data as what's found in ServerPlayerManager.
    public NetworkVariable<FixedString32Bytes> PlayerId;
    public NetworkVariable<FixedString32Bytes> PlayerName;
    public NetworkVariable<FixedString32Bytes> CharacterName;

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
        if (IsServer)
        {
            PlayerId.Value = ServerPlayerManager.GetPlayerId(OwnerClientId);
            PlayerName.Value = ServerPlayerManager.GetPlayerName(OwnerClientId);
            CharacterName.Value = ServerPlayerManager.GetCharacterName(OwnerClientId);

            StartCoroutine(LoadCharacterAsync());
        }
        gameObject.name = $"({PlayerName.Value}) {CharacterName.Value}";
    }

    private async Awaitable LoadCharacterAsync()
    {
        var persistedCharacterData = await PlayerSaveWrapper.LoadCharacterOnServer(PlayerId.Value.Value, CharacterName.Value.Value);
        Debug.Log($"{persistedCharacterData.Name} {persistedCharacterData.Class}");
    }
}
