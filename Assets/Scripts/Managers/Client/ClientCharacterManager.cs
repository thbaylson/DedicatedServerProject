using Client.UI;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.CloudCode;
using Unity.Services.Core;
using UnityEngine;

public class ClientCharacterManager : MonoBehaviour
{
    [SerializeField] CreateRequest _createRequest;
    public List<PersistedCharacterData> PersistedCharacterDatas;
    public static ClientCharacterManager Instance { get; private set; }

    void Awake() => Instance = this;

    async void Start()
    {
        await UnityServices.InitializeAsync();
        try
        {
            await AuthenticationService.Instance.SignInWithUsernamePasswordAsync("Player1", "Test1234!");
        }
        catch (Exception ex)
        {
            await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync("Player1", "Test1234!");
        }

        if (AuthenticationService.Instance.IsAuthorized)
        {
            Debug.Log($"Logged in as {AuthenticationService.Instance.PlayerId}");
        }
        else
        {
            Debug.Log("Failed to login");
        }

        PersistedCharacterDatas = await LoadAllCharactersOnClient();
        
        // Refresh the UI element. This logic will be moved in the future
        FindFirstObjectByType<UICharacterSelectPanel>().Bind(PersistedCharacterDatas);
    }

    // This adds custom functionality to the context menu of this component within the editor
    [ContextMenu(nameof(SendCreateRequest))]
    public async void SendCreateRequest()
    {
        // This is basically deprecated code now. The UI makes this obsolete
        await CreateCharacter(_createRequest.CharacterName, CharacterClass.Warrior);
    }

    public async Task CreateCharacter(string characterName, CharacterClass characterClass)
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
        if (result.Success)
        {
            PersistedCharacterDatas.Add(result.Data);
            Debug.Log($"Player: {result.Data.PlayerId}, {result.Data.Name}, {result.Data.Class}, {result.Data.Experience}");

            // Refresh the UI element. This logic will be moved in the future
            FindFirstObjectByType<UICharacterSelectPanel>().Bind(PersistedCharacterDatas);
        }
    }

    public async Task<List<PersistedCharacterData>> LoadAllCharactersOnClient()
    {
        var playerId = AuthenticationService.Instance.PlayerId;
        var result = await CloudCodeService.Instance.CallModuleEndpointAsync<List<PersistedCharacterData>>(
            "ExtractCloud",
            "LoadAllCharactersOnClient");

        return result;
    }

    public async void DeleteCharacter(string characterName)
    {
        // Delete the character from CloudCode
        var result = await CloudCodeService.Instance.CallModuleEndpointAsync<bool>(
            "ExtractCloud",
            "DeleteCharacter",
            new Dictionary<string, object>
            {
                {"characterName", characterName }
            });

        // If the character was successfully deleted, remove them from the list
        if (result)
        {
            PersistedCharacterDatas.Remove(PersistedCharacterDatas.Find(character => character.Name == characterName));
        }

        // Refresh the UI element. This logic will be moved in the future
        FindFirstObjectByType<UICharacterSelectPanel>().Bind(PersistedCharacterDatas);
    }
}

// This is sent to cloud code server to create a new character
[Serializable]
public class CreateRequest
{
    public string CharacterName;
    public string ClassName;
}

[Serializable]
public class CreateResult
{
    public PersistedCharacterData Data;
    public string Message;
    public bool Success;
}
