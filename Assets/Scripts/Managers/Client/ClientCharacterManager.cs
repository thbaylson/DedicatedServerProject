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
    }

    // This adds custom functionality to the context menu of this component within the editor
    [ContextMenu(nameof(SendCreateRequest))]
    public async void SendCreateRequest()
    {
        await CreateCharacter(_createRequest.CharacterName, _createRequest.ClassName);
    }

    public async Task CreateCharacter(string characterName, string className)
    {
        var result = await CloudCodeService.Instance.CallModuleEndpointAsync("ExtractCloud", "SayHello",
            new Dictionary<string, object>
            {
                {"name", characterName }
            });

        Debug.Log(result);

        var request = new CreateRequest
        {
            CharacterName = characterName,
            ClassName = className
        };
        Debug.Log($"Sending Request {request.CharacterName} {request.ClassName}");
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
    public bool Success;
    public string Message;
}