using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ClientCharacterManager : MonoBehaviour
{
    [SerializeField] CreateRequest _createRequest;

    // This adds custom functionality to the context menu of this component within the editor
    [ContextMenu(nameof(SendCreateRequest))]
    public async void SendCreateRequest()
    {
        await CreateCharacter(_createRequest.CharacterName, _createRequest.ClassName);
    }

    public async Task CreateCharacter(string characterName, string className)
    {
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