using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Client.UI
{
    public class UICharacterSelectPanel : MonoBehaviour
    {
        [SerializeField] UICharacterSelectButton _characterSelectButtonPrefab;
        [SerializeField] Transform _characterButtonsRoot;

        Dictionary<string, UICharacterSelectButton> _buttons = new();

        public void Bind(List<PersistedCharacterData> characters)
        {
            var buttonsList = _buttons.Values.ToList();
            foreach (var button in buttonsList)
            {
                Destroy(button.gameObject);
            }
            _buttons.Clear();

            foreach (var character in characters)
            {
                AddCharacter(character);
            }
        }

        private void AddCharacter(PersistedCharacterData persistedCharacterData)
        {
            var button = Instantiate(_characterSelectButtonPrefab, _characterButtonsRoot);
            button.Bind(persistedCharacterData);
            _buttons[persistedCharacterData.Name] = button;
        }
    }
}
