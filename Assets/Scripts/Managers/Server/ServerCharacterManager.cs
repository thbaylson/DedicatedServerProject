using Client.UI;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public static class ServerCharacterManager
{
    // Store player characters in a Dictionary using playerId as the key
    static readonly Dictionary<string, Character> PlayerCharacters = new();
    static readonly Dictionary<ulong, Character> CharactersByNetworkObjectId = new();

    public static async Task<Character> SpawnCharacterFromCloudData(PersistedCharacterData persistedCharacterData, Player player)
    {
        var character = Object.Instantiate(PrefabsManager.Instance.CharacterPrefab);
        character.gameObject.name = $"Character {persistedCharacterData.Name}";
        character.NetworkObject.SpawnWithOwnership(player.OwnerClientId);

        PlayerCharacters[player.PlayerId.Value.Value] = character;
        CharactersByNetworkObjectId[character.NetworkObjectId] = character;

        // Polling. Gross.
        while (!character.NetworkObject.IsSpawned)
        {
            await Awaitable.NextFrameAsync();
            Debug.Log("Waiting for character spawn to complete");
        }

        return character;
    }
}
