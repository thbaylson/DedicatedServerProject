using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Netcode;
using Unity.Services.CloudCode;
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

    async Awaitable LoadCharacterAsync()
    {
        var persistedCharacterData = await LoadCharacterOnServer(PlayerId.Value.Value, CharacterName.Value.Value);
        Debug.Log($"{persistedCharacterData.Name} {persistedCharacterData.Class}");
    }

    public async Task<PersistedCharacterData> LoadCharacterOnServer(string playerId, string characterName)
    {
        if(string.IsNullOrWhiteSpace(characterName))
        {
            Debug.LogError("Can't load empty character");
            return null;
        }

        Debug.LogWarning($"Loading character on server for playerid {playerId} and character {characterName}");

        var result = await CloudCodeService.Instance.CallModuleEndpointAsync<PersistedCharacterData>(
            "ExtractCloud",
            "LoadCharacterOnServer",
            new Dictionary<string, object>
            {
                {"playerId", playerId },
                {"characterName", characterName }
            });

        return result;
    }
}
