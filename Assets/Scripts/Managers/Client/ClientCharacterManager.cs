using Client.UI;
using Cloud;
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

        // If we are loading back into this from a level, then skip sign-in logic. We're already signed in
        if (AuthenticationService.Instance.IsAuthorized)
        {
            Debug.LogWarning("Already Authorized.");
            await LoadAllCharactersOnClient();
            return;
        }

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

        await LoadAllCharactersOnClient();
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
        CreateResult result = await PlayerSaveWrapper.CreateCharacterFromClient(characterName, characterClass);
        if (result.Success)
        {
            PersistedCharacterDatas.Add(result.Data);
            Debug.Log($"Player: {result.Data.PlayerId}, {result.Data.Name}, {result.Data.Class}, {result.Data.Experience}");

            // Refresh the UI element. This logic will be moved in the future
            FindFirstObjectByType<UICharacterSelectPanel>().Bind(PersistedCharacterDatas);
        }
    }

    public async void DeleteCharacter(string characterName)
    {
        bool result = await PlayerSaveWrapper.DeleteCharacterFromClient(characterName);
        // If the character was successfully deleted, remove them from the list
        if (result)
        {
            PersistedCharacterDatas.Remove(PersistedCharacterDatas.Find(character => character.Name == characterName));
        }

        // Refresh the UI element. This logic will be moved in the future
        FindFirstObjectByType<UICharacterSelectPanel>().Bind(PersistedCharacterDatas);
    }

    private async Task LoadAllCharactersOnClient()
    {
        PersistedCharacterDatas = await PlayerSaveWrapper.LoadAllCharactersOnClient();
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
