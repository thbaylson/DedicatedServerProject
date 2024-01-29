using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Services.CloudCode.Apis;
using Unity.Services.CloudCode.Core;
using Unity.Services.CloudSave.Model;

namespace ExtractCloud
{
    public class ServerFunctions
    {
        [CloudCodeFunction("LoadCharacterOnServer")]
        public async Task<PersistedCharacterData> LoadCharacterOnServer(IExecutionContext ctx, IGameApiClient gameApiClient, string playerId, string characterName)
        {
            var response = await gameApiClient.CloudSaveData.GetProtectedItemsAsync(
                ctx, ctx.ServiceToken, ctx.ProjectId, playerId, new List<string>() { characterName });

            if (response.Data.Results.Count == 0)
            {
                throw new Exception($"No player returned searching player {playerId} for {characterName}");
            }

            var json = response.Data.Results[0].Value.ToString();
            var character = JsonConvert.DeserializeObject<PersistedCharacterData>(json);
            if (character == null)
            {
                throw new Exception($"Invalid Player data, unable to deserialize{Environment.NewLine}{response.Data.Results[0].Value}");
            }

            return character;
        }

        [CloudCodeFunction("SaveCharacterOnServer")]
        public async Task<CreateResult> SaveCharacterOnServer(IExecutionContext ctx, IGameApiClient gameApiClient, PersistedCharacterData data)
        {
            try
            {
                SetItemBody setItemBody = new SetItemBody(data.Name, data);
                var response = await gameApiClient.CloudSaveData.SetProtectedItemAsync(
                    ctx, ctx.ServiceToken, ctx.ProjectId, data.PlayerId, setItemBody);

                if(response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return new CreateResult { Data = data, Success = true, Message = response.StatusCode.ToString() };
                }
                else
                {
                    var persistedCharacter = await LoadCharacterOnServer(ctx, gameApiClient, data.PlayerId, data.Name);
                    return new CreateResult { Data = data, Success = false, Message = response.StatusCode.ToString() };
                }
            }
            catch (Exception ex)
            {
                return new CreateResult { Data = null, Success = false, Message = $"Exception Thrown - {ex}" };
            }
        }
    }
}
