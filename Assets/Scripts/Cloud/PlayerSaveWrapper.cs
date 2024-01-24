using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.CloudCode;
using UnityEngine;

namespace Cloud
{
    public class PlayerSaveWrapper
    {
        public static async Task<PersistedCharacterData> LoadCharacterOnServer(string playerId, string characterName)
        {
            if (string.IsNullOrWhiteSpace(characterName))
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

        public static async Task<List<PersistedCharacterData>> LoadAllCharactersOnClient()
        {
            var playerId = AuthenticationService.Instance.PlayerId;
            var result = await CloudCodeService.Instance.CallModuleEndpointAsync<List<PersistedCharacterData>>(
                "ExtractCloud",
                "LoadAllCharactersOnClient");

            return result;
        }

        public static async Task<CreateResult> CreateCharacterFromClient(string characterName, CharacterClass characterClass)
        {
            var request = new CreateRequest
            {
                CharacterName = characterName,
                ClassName = characterClass.ToString()
            };

            Debug.Log($"Sending CreateRequest: CharacterName {request.CharacterName}; ClassName: {request.ClassName}");
            var result = await CloudCodeService.Instance.CallModuleEndpointAsync<CreateResult>(
                "ExtractCloud",
                "CreateCharacter",
                new Dictionary<string, object>
                {
                {"request", request }
                });

            Debug.Log($"Received CreateResult: {result.Success}; {result.Message}");
            return result;
        }

        public static async Task<bool> DeleteCharacterFromClient(string characterName)
        {
            // Delete the character from CloudCode
            return await CloudCodeService.Instance.CallModuleEndpointAsync<bool>(
                "ExtractCloud",
                "DeleteCharacter",
                new Dictionary<string, object>
                {
                {"characterName", characterName }
                });
        }
    }
}
