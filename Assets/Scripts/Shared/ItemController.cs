using Client.UI;
using Server;
using Unity.Netcode;
using UnityEngine;

namespace Shared
{
    public class ItemController : NetworkBehaviour
    {
        public ServerItemInstance[] Items;
        public NetworkList<byte> ItemDefinitionInSlots;

        [SerializeField] private Character _character;

        // What's the difference between setting this here vs in Awake()?
        private void OnValidate() => _character = GetComponent<Character>();

        private void Awake()
        {
            Items = new ServerItemInstance[10];
            ItemDefinitionInSlots = new NetworkList<byte>();
        }

        private void Start()
        {
            if (IsServer)
            {
                SyncItemsToClient();
            }

            if (IsClient)
            {
                ItemDefinitionInSlots.OnListChanged += (eventParam) => Debug.Log($"Item Changed {eventParam.Index} = {eventParam.Value}");
                for(var i = 0; i < ItemDefinitionInSlots.Count; i++)
                {
                    Debug.Log($"{i} = {ItemDefinitionInSlots[i]}");
                }

                if (IsOwner)
                {
                    UIInventoryPanel.Instance.Bind(this);
                }
            }
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
            PlaceItemInSlot(firstEmptySlot, itemInstance);
        }

        public void InitializeItemsFromData()
        {
            Debug.Log($"InitializeItemsFromData for Inventory of {OwnerClientId}");
            // TODO: Player inventory is currently hardcoded to 10. Both here and in ExtractCloud. This needs to not be hardcoded in the future.
            for(byte i = 0; i < 10; i++)
            {
                if(_character.Data.ItemIds.Length < i + 1)
                {
                    Debug.LogError("Data size doesn't match for inventory");
                    break;
                }

                var itemId = _character.Data.ItemIds[i];

                if(itemId > 0)
                {
                    var itemInstance = ServerItemManager.Instance.CreateServerItem(itemId);
                    itemInstance.SlotIndex = i;
                    itemInstance.CharacterName = _character.Name.Value.Value;
                    Items[i] = itemInstance;
                }
                else
                {
                    Items[i] = null;
                }
            }
        }

        private void SyncItemsToClient()
        {
            for(int i = 0; i < Items.Length; i++)
            {
                var definitionId = Items[i]?.DefinitionId ?? 0;
                
                // If we haven't sync'd yet, then our ItemDefinitionInSlots list is empty and needs to be populated
                if(ItemDefinitionInSlots.Count < i + 1)
                {
                    // ItemDefinitionInSlots[i] = Items[i]
                    ItemDefinitionInSlots.Add(definitionId);
                }
                // If we have sync'd, then make sure our item lists haven't gotten out of sync
                else if(ItemDefinitionInSlots[i] != definitionId)
                {
                    ItemDefinitionInSlots[i] = definitionId;
                    Debug.Log($"Updated item definition in slot {i} to {definitionId}");
                }
            }
        }

        private void PlaceItemInSlot(byte slotIndex, ServerItemInstance itemInstance)
        {
            Items[slotIndex] = itemInstance;
            if(itemInstance != null)
            {
                _character.Data.ItemIds[slotIndex] = itemInstance.DefinitionId;
                itemInstance.SlotIndex = slotIndex;
                itemInstance.CharacterName = _character.Name.Value.Value;
            }
            else
            {
                _character.Data.ItemIds[slotIndex] = 0;
            }
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
