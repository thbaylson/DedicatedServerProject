using Client.UI;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Server
{
    public class ServerItemManager : MonoBehaviour
    {
        public static ServerItemManager Instance { get; private set; }
        public List<ServerItemInstance> SpawnedItems;

        void Awake() => Instance = this;

        void Start() => Invoke(nameof(SpawnItem), 1f);

        [ContextMenu("SpawnItem")]
        public void SpawnItem()
        {
            if (!NetworkManager.Singleton.IsServer) { return; }

            var id = (byte)Random.Range(1, PrefabsManager.Instance.Items.Count);
            var serverItemInstance = CreateServerItem(id);
            SpawnWorldItemForInstance(serverItemInstance, Vector3.zero);
        }

        public void SpawnWorldItemForInstance(ServerItemInstance serverItemInstance, Vector3 position)
        {
            var worldItem = Instantiate(PrefabsManager.Instance.WorldItemPrefab, position, Quaternion.identity);
            worldItem.Initialize(serverItemInstance);
            worldItem.NetworkObject.Spawn();
        }

        public ServerItemInstance CreateServerItem(byte itemDefinitionId)
        {
            var item = new ServerItemInstance
            {
                DefinitionId = itemDefinitionId,
                // Items that have spawned but have yet to be picked up belong to the "world"
                CharacterName = "World",
                // Not sure what it would achieve functionally, but SlotIndex could relate to SpawnedItems.Count to know where this item is in the List
                SlotIndex = 0
            };
            SpawnedItems.Add(item);
            return item;
        }

        public void RemoveItems(List<ServerItemInstance> items)
        {
            // Why don't we just pass items instead of items.Contains?
            SpawnedItems.RemoveAll(items.Contains);
        }
    }
}