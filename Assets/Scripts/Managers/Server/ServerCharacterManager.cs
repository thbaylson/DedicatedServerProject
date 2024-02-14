using Client.UI;
using Shared;
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
        var entryPoint = GameObject.FindWithTag("EntryPoint").transform;
        var character = Object.Instantiate(PrefabsManager.Instance.CharacterPrefab, entryPoint.position, entryPoint.rotation);
        character.Bind(persistedCharacterData);

        character.NetworkObject.SpawnWithOwnership(player.OwnerClientId);

        PlayerCharacters[player.PlayerId.Value.Value] = character;
        CharactersByNetworkObjectId[character.NetworkObjectId] = character;

        // Polling. Gross. TODO: Find a better way?
        while (!character.NetworkObject.IsSpawned)
        {
            await Awaitable.NextFrameAsync();
            Debug.Log("Waiting for character spawn to complete");
        }

        return character;
    }

    /// <summary>
    /// Gets a Character for a given playerId or returns null. Intentionally (per the course lecture), if a player connects with
    /// Character A, disconnects, and then rejoins with Character B, the end result is that player being forced to rejoin as Character A.
    /// </summary>
    /// <param name="playerId"></param>
    /// <returns>A Character if the given playerId matches one in PlayerCharacters dict, otherwise returns null.</returns>
    public static Character GetExistingCharacter(string playerId) => PlayerCharacters.GetValueOrDefault(playerId);
}
