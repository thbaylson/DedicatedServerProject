using Shared;
using TMPro;
using UnityEngine;

namespace Client.UI
{
    public class UICharacterInfoPanel : MonoBehaviour
    {
        [SerializeField] TMP_Text _nameText;
        [SerializeField] TMP_Text _classText;
        [SerializeField] TMP_Text _experienceText;

        private Character _boundCharacter;

        public static UICharacterInfoPanel Instance { get; private set; }

        void Awake() => Instance = this;

        public void ToggleVisibility() => gameObject.SetActive(!gameObject.activeSelf);

        public void Bind(Character sharedCharacter)
        {
            if(_boundCharacter != null)
            {
                // If we're already bound to a character, unsubscribe that character from RefreshExperience
                _boundCharacter.Experience.OnValueChanged -= RefreshExperience;
            }

            _boundCharacter = sharedCharacter;
            if(_boundCharacter != null)
            {
                _nameText.text = _boundCharacter.Name.Value.Value;
                _classText.text = _boundCharacter.CharacterClass.Value.ToString();
                _experienceText.text = _boundCharacter.Experience.Value.ToString();

                _boundCharacter.Experience.OnValueChanged += RefreshExperience;
                Debug.LogWarning($"Bound To: {_boundCharacter}; Exp: {_boundCharacter.Experience.Value}");
            }
            else
            {
                _nameText.text = "No Character Selected";
            }
        }

        private void RefreshExperience(long previousValue, long newValue) => _experienceText.text = newValue.ToString();
    }
}