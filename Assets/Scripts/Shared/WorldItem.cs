using Server;
using Unity.Netcode;

namespace Shared
{
    public class WorldItem : NetworkBehaviour
    {
        public ServerItemInstance ServerItemInstance { get; private set;}

        public void Initialize(ServerItemInstance sharedServerItemInstance)
        {
            ServerItemInstance = sharedServerItemInstance;
        }
    }
}