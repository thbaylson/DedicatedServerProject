using Shared;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Client.UI
{
    public class UICheatsPanel : MonoBehaviour
    {
        public long experienceAmountToAdd = 10;

        [SerializeField] Button _disconnectButton;
        [SerializeField] Button _addExperienceButton;

        private void Awake()
        {
            _disconnectButton.onClick.AddListener(Disconnect);
            _addExperienceButton.onClick.AddListener(AddExperience);
        }

        private void Disconnect()
        {
            NetworkManager.Singleton.Shutdown();
            SceneManager.LoadScene("Client");
        }

        private void AddExperience()
        {
            Character.LocalCharacter.AddExperience(experienceAmountToAdd);
            Debug.Log($"{experienceAmountToAdd} Experience Added");
        }
    }
}
