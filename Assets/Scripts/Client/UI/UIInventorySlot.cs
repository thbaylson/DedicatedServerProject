using Shared;
using UnityEngine;
using UnityEngine.UI;

namespace Client.UI
{
    public class UIInventorySlot : MonoBehaviour
    {
        [SerializeField] private Image Icon;

        private ItemController _itemController;
        private byte _index;

        public void Bind(ItemController itemController, byte index)
        {
            _index = index;
            _itemController = itemController;
            byte itemDefinitionId = itemController.ItemDefinitionInSlots.Count > index ? itemController.ItemDefinitionInSlots[index] : new byte();
            ShowItemDefinition(itemDefinitionId);
        }

        public void RefreshItemVisuals()
        {
            byte itemDefinitionId = _itemController.ItemDefinitionInSlots.Count > _index ? _itemController.ItemDefinitionInSlots[_index] : new byte();
            ShowItemDefinition(itemDefinitionId);
        }

        private void ShowItemDefinition(byte itemDefinitionId)
        {
            // This is why other places say "itemDefinitionId". Here, we grab the actual instance of the item
            var itemDefinition = PrefabsManager.Instance.Items[itemDefinitionId];

            if (Icon == null)
            {
                Debug.Log("Icon is null");
            }
            else
            {
                Icon.sprite = itemDefinition?.Sprite;
            }
        }
    }
}