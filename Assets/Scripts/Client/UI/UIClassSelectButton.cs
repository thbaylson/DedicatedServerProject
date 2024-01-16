using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Note: The course file lists a namespace for this class only. May want to add that?
public class UIClassSelectButton : MonoBehaviour
{
    public static CharacterClass SelectedClass;

    public CharacterClass CharacterClass;
    Toggle _toggle;
    [SerializeField] TMP_Text _text;
    [SerializeField] Image _selectedBadge;

    private void OnValidate()
    {
        _text = GetComponentInChildren<TMP_Text>();
        _text.text = CharacterClass.ToString();
        gameObject.name = "Class Button " + CharacterClass;
    }

    private void Awake()
    {
        _toggle = GetComponent<Toggle>();
        _toggle.onValueChanged.AddListener(HandleClick);
    }

    void HandleClick(bool arg0)
    {
        if (arg0)
        {
            SelectedClass = CharacterClass;
        }

        _selectedBadge.color = arg0 ? Color.green : Color.white;
    }
}

public enum CharacterClass
{
    Mage, Warrior, Cleric, Ranger
}