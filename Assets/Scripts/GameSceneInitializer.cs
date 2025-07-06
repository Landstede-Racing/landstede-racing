using UnityEngine.SceneManagement;
using Unity.Netcode;
using UnityEngine;

public class GameSceneInitializer : MonoBehaviour
{
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name.StartsWith("Race") && NetworkLaunchManager.Instance.ShouldStartHost)
        {
            NetworkManager.Singleton.StartHost();
            NetworkLaunchManager.Instance.Reset();
        }
    }
}