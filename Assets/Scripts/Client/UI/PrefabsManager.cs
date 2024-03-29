﻿using Shared;
using System.Collections.Generic;
using UnityEngine;

namespace Client.UI
{
    public class PrefabsManager : MonoBehaviour
    {
        public static PrefabsManager Instance {  get; private set; }
        public Character CharacterPrefab;
        public WorldItem WorldItemPrefab;
        public List<ItemDefinition> Items;

        [SerializeField] private List<Sprite> _sprites;

        private void Awake() => Instance = this;

        public Sprite GetSprite(string spriteName)
        {
            return _sprites.Find(s => s.name == spriteName);
        }
    }
}
