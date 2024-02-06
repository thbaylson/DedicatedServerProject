using Server;
using Unity.Netcode;
using UnityEngine;

namespace Shared
{
    public class ItemController : NetworkBehaviour
    {
        public ServerItemInstance[] Items;

        [SerializeField] private Character _character;

        // What's the difference between setting this here vs in Awake()?
        private void OnValidate() => _character = GetComponent<Character>();

        private void Awake()
        {
            Items = new ServerItemInstance[10];
        }

        public void ClickedWorldItemOnServer(WorldItem item)
        {
            if(!TryGetFirstEmptySlot(out var firstEmptySlot))
            {
                Debug.LogError("No empty slots");
                return;
            }

            item.NetworkObject.Despawn();

            var itemInstance = item.ServerItemInstance;

            if(itemInstance.DefinitionId == 0 )
            {
                Debug.LogError("ItemInstance.Definition is not set.");
                return;
            }

            Debug.Log($"Searching for {itemInstance.DefinitionId}");

            itemInstance.CharacterName = _character.Name.Value.Value;
            Items[firstEmptySlot] = itemInstance;
        }

        // This is a fun way to functionally return 2 data points while technically only returning 1 boolean.
        private bool TryGetFirstEmptySlot(out byte slotIndex)
        {
            bool slotIndexFound = false;
            slotIndex = 0;
            for(byte i = 0; i < Items.Length && !slotIndexFound; i++)
            {
                // Find the first index where the data is null of the DefinitionId is 0.
                if (Items[i] == null || Items[i].DefinitionId == 0)
                {
                    slotIndex = i;
                    slotIndexFound = true;
                }
            }

            return slotIndexFound;
        }
    }
}
