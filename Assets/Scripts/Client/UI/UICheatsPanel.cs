using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Client.UI
{
    public class UICheatsPanel : MonoBehaviour
    {
        [SerializeField] Button _disconnectButton;
        [SerializeField] Button _addExperienceButton;

        private float experienceToAdd = 10f;

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
            Debug.Log($"{experienceToAdd} Experience Added");
        }
    }
}
