using System.Linq;
using Unity.Multiplayer.Playmode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EditorPlayModeManager : MonoBehaviour
{
    /// Description from https://docs.unity3d.com/ScriptReference/RuntimeInitializeOnLoadMethodAttribute.html documentation:
    /// "Use this attribute to get a callback when the runtime is starting up and loading the first scene. Use the various options 
    /// for RuntimeInitializeLoadType to control when the method is invoked in the startup sequence."
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void LoadMultiplayerScene()
    {
        // Because this script will run on every scene, we need to check which scene is actually loaded
        if(!SceneManager.GetSceneByName("Startup").isLoaded)
        {
            // If the Startup scene is not yet loaded, we need to load it now
            // Kinda hate these string references. TODO: look into a more robust way of doing this
            SceneManager.LoadScene("Startup");
            Debug.Log("Loaded Startup Scene");
        }

        // Check to see if the "player" (ie: user) is the Server
        bool isServer = CurrentPlayer.ReadOnlyTags().Contains("Server");
        if(isServer)
        {
            if (!SceneManager.GetSceneByName("Server").isLoaded)
            {
                SceneManager.LoadScene("Server", LoadSceneMode.Additive);
                Debug.Log("Loaded Server Scene");
            }

            return;
        }

        // Check to see if the "player" (ie: user) is a Client
        bool isClient = CurrentPlayer.ReadOnlyTags().Contains("Client");
        if(isClient)
        {
            if (!SceneManager.GetSceneByName("Client").isLoaded)
            {
                SceneManager.LoadScene("Client", LoadSceneMode.Additive);
                Debug.Log("Loaded Client Scene");
            }

            return;
        }
    }
}
