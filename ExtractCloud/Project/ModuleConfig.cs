using Microsoft.Extensions.DependencyInjection;
using Unity.Services.CloudCode.Apis;
using Unity.Services.CloudCode.Core;

namespace ExtractCloud
{
    // Ayo? A little bit of DI happening here
    public class ModuleConfig : ICloudCodeSetup
    {
        public void Setup(ICloudCodeConfig config)
        {
            config.Dependencies.AddSingleton(GameApiClient.Create());
        }
    }
}
