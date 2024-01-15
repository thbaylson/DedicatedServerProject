using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.CloudCode.Apis;
using Unity.Services.CloudCode.Core;
using Unity.Services.CloudSave.Model;
using Unity.Services.Lobby.Model;

namespace HelloWorld;

public class MyModule
{
    [CloudCodeFunction("SayHello")]
    public string Hello(string name, string className)
    {
        return $"Hello, {name}! You're a {className}!";
    }

    // IExecutionContext is being injected from somewhere else. The parameter name has to be exact for it to work (gross)!
    // IGameApiClient is the same as above. Injection is defined in part at this bottom of this script (for some reason).
    [CloudCodeFunction("CreateCharacter")]
    public async Task<CreateResult> CreateCharacter(IExecutionContext ctx, IGameApiClient gameApiClient, CreateRequest request)
    {
        // Make sure the character doesn't already exist before we create a new one.
        bool characterExists = await DoesCharacterExist(ctx, gameApiClient, request);
        if (characterExists)
        {
            return new CreateResult() { Success = false, Message = $"CharacterName {request.CharacterName} already exists." };
        }

        // Now we know the character doesn't already exist, so we can continue with creation logic.
        var persistedCharacterData = new PersistedCharacterData()
        {
            PlayerId = ctx.PlayerId,
            Name = request.CharacterName,
            Class = request.ClassName,
            Experience = 1
        };

        // This SetItemBody object is how we set data in cloud code.
        // CharacterName is being used as a DB key here. Shouldn't we be using Id?
        SetItemBody setItemBody = new SetItemBody(persistedCharacterData.Name, persistedCharacterData);
        
        // Protected in this context means it can only be set by the server (CloudCode).
        var response = await gameApiClient.CloudSaveData.SetProtectedItemAsync(
            ctx, ctx.ServiceToken, ctx.ProjectId, ctx.PlayerId, setItemBody);

        if(response.StatusCode == System.Net.HttpStatusCode.OK)
        {
            return new CreateResult() { Data = persistedCharacterData, Success = true };
        }
        else
        {
            return new CreateResult() { Data = persistedCharacterData, Success = false, Message = response.StatusCode.ToString() };
        }
    }

    [CloudCodeFunction("LoadAllCharactersOnClient")]
    public async Task<List<PersistedCharacterData>> LoadAllCharactersOnClient(IExecutionContext ctx, IGameApiClient gameApiClient)
    {
        // Gets all of the keys
        var names = await gameApiClient.CloudSaveData.GetProtectedKeysAsync(
            ctx, ctx.ServiceToken, ctx.ProjectId, ctx.PlayerId);

        // For each key, get all of the data and add it to the list
        List<PersistedCharacterData> characters = new();
        foreach(var characterName in names.Data.Results)
        {
            characters.Add(await LoadCharacterForClientPlayer(ctx, gameApiClient, characterName.Key));
        }

        return characters;
    }

    /// <summary>
    /// Gets character data for a given character's name.
    /// </summary>
    /// <returns>PersistedCharacterData object from GameApiClient.CloudSaveData.</returns>
    private async Task<PersistedCharacterData> LoadCharacterForClientPlayer(IExecutionContext ctx, IGameApiClient gameApiClient, string characterName)
    {
        var getItemsResponse = await gameApiClient.CloudSaveData.GetProtectedItemsAsync(
            ctx, ctx.ServiceToken, ctx.ProjectId, ctx.PlayerId, new List<string>() { characterName });

        // Items are stored as json serialized strings
        string json = getItemsResponse.Data.Results[0].Value.ToString();
        var persistedCharacterData = JsonConvert.DeserializeObject<PersistedCharacterData>(json);

        return persistedCharacterData;
    }

    /// <summary>
    /// Checks if CloudSaveData contains the given request's character name.
    /// </summary>
    /// <returns>True if any such characters with the given request name exist.</returns>
    private async Task<bool> DoesCharacterExist(IExecutionContext ctx, IGameApiClient gameApiClient, CreateRequest request)
    {
        var existingCharacter = await gameApiClient.CloudSaveData.GetProtectedItemsAsync(
            ctx, ctx.ServiceToken, ctx.ProjectId, ctx.PlayerId, new List<string>() { request.CharacterName });

        return existingCharacter.Data.Results.Count > 0;
    }
}

// These models are copy/pasted directly from the unity MonoBehaviours. Breaks DRY principle.
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

[Serializable]
public class PersistedCharacterData
{
    public string PlayerId;
    public string Name;
    public string Class;
    public long Experience;
}

// Ayo? A little bit of DI happening down here!
public class ModuleConfig : ICloudCodeSetup
{
    public void Setup(ICloudCodeConfig config)
    {
        config.Dependencies.AddSingleton(GameApiClient.Create());
    }
}
