using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Client.UI
{
    class UICharacterSelectButton : MonoBehaviour
    {
        [SerializeField] TMP_Text _characterNameText;
        [SerializeField] Button _selectButton;
        [SerializeField] Button _deleteButton;
        [SerializeField] Image _characterClassIcon;
        string _characterName;

        private void Awake()
        {
            _selectButton.onClick.AddListener(SelectCharacter);
            _deleteButton.onClick.AddListener(DeleteCharacter);
        }

        private void Start()
        {
            GetComponent<Button>().onClick.AddListener(DeleteCharacter);
        }

        private void SelectCharacter() => Debug.Log("Start log in.");

        private void DeleteCharacter() => ClientCharacterManager.Instance.DeleteCharacter(_characterName);

        public void Bind(PersistedCharacterData data)
        {
            _characterName = data.Name;
            GetComponentInChildren<TMP_Text>().SetText($"{data.Name} - {data.Class} ({data.Experience})");
            _characterClassIcon.sprite = PrefabsManager.Instance.GetSprite(data.Class);
        }

        [ContextMenu("Bind To Test")]
        private void BindToTest()
        {
            Bind(new PersistedCharacterData()
            {
                Name = "Lorem Ipsum",
                Class = CharacterClass.Mage.ToString(),
                Experience = 42
            });
        }
    }
}
