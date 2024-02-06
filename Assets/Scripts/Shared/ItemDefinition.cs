using UnityEngine;

namespace Shared
{
    // TODO: This could probably be renamed to just "Item"
    [CreateAssetMenu(fileName = "ItemDefinition", menuName = "Game/Item")]
    public class ItemDefinition : ScriptableObject
    {
        public int Health;
        public int Mana;
        public string Model;
        public Sprite Sprite;
    }
}
