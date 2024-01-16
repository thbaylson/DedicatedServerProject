using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICharacterCreatePanel : MonoBehaviour
{
    [SerializeField] Button _createButton;
    [SerializeField] TMP_InputField _nameInputField;

    void Awake() => _createButton.onClick.AddListener(CreateCharacter);

    async void CreateCharacter()
    {
        await ClientCharacterManager.Instance.CreateCharacter(_nameInputField.text, UIClassSelectButton.SelectedClass);
    }
}