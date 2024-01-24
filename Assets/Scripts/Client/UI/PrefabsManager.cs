using System.Collections.Generic;
using UnityEngine;

namespace Client.UI
{
    public class PrefabsManager : MonoBehaviour
    {
        public static PrefabsManager Instance {  get; private set; }
        public Character CharacterPrefab;
        [SerializeField] List<Sprite> _sprites;

        private void Awake() => Instance = this;

        public Sprite GetSprite(string spriteName)
        {
            return _sprites.Find(s => s.name == spriteName);
        }
    }
}
