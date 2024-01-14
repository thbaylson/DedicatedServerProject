using System;
using System.Threading.Tasks;
using Unity.Services.CloudCode.Core;
using Unity.Services.Lobby.Model;

namespace HelloWorld;

public class MyModule
{
    [CloudCodeFunction("SayHello")]
    public string Hello(string name, string className)
    {
        return $"Hello, {name}! You're a {className}!";
    }

    [CloudCodeFunction("CreateCharacter")]
    public async Task<CreateResult> CreateCharacter(IExecutionContext ctx, CreateRequest request)
    {
        return new CreateResult()
        {
            Data = new PersistedCharacterData()
            {
                PlayerId = ctx.PlayerId,
                Name = request.CharacterName,
                Class = request.ClassName,
                Experience = 99
            },
            Message = "We didn't really do anything",
            Success = true
        };
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
