using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICharacterSelectPanel : MonoBehaviour
{
    [SerializeField] UICharacterSelectButton _characterSelectButtonPrefab;
    [SerializeField] Transform _characterButtonsRoot;

    public void Bind(List<PersistedCharacterData> characters)
    {
        foreach(var character in characters)
        {
            AddCharacter(character);
        }
    }

    private void AddCharacter(PersistedCharacterData persistedCharacterData)
    {
        var button = Instantiate(_characterSelectButtonPrefab, _characterButtonsRoot);
        button.Bind(persistedCharacterData);
        //_buttons[persistedCharacterData.Name] = button;
    }
}
