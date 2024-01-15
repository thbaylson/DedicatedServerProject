using TMPro;
using UnityEngine;
using UnityEngine.UI;

class UICharacterSelectButton : MonoBehaviour
{
    private PersistedCharacterData _persistedCharacterData;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(DeleteCharacter);
    }

    private void DeleteCharacter()
    {
        ClientCharacterManager.Instance.DeleteCharacter(_persistedCharacterData.Name);
    }

    public void Bind(PersistedCharacterData persistedCharacterData)
    {
        _persistedCharacterData = persistedCharacterData;
        GetComponentInChildren<TMP_Text>().SetText($"{_persistedCharacterData.Name} - {_persistedCharacterData.Class} ({_persistedCharacterData.Experience})");
    }
}
