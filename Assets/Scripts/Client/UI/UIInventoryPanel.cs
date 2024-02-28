using Shared;
using Unity.Netcode;
using UnityEngine;

namespace Client.UI
{
    public class UIInventoryPanel : MonoBehaviour
    {
        UIInventorySlot[] _slots;

        public static UIInventoryPanel Instance { get; private set; }
        
        void Awake() => Instance = this;

        void OnValidate() => _slots = GetComponentsInChildren<UIInventorySlot>();

        public void Bind(ItemController itemController)
        {
            for(byte i = 0; i < _slots.Length; i++)
            {
                _slots[i].Bind(itemController, i);
            }

            itemController.ItemDefinitionInSlots.OnListChanged += HandleItemDefinitionInSlotsChanged;
        }

        private void HandleItemDefinitionInSlotsChanged(NetworkListEvent<byte> changeEvent)
        {
            _slots[changeEvent.Index].RefreshItemVisuals();
        }
    }
}