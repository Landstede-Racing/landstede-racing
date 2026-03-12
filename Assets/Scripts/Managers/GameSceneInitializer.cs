using UnityEngine.SceneManagement;
using UnityEngine;
using System.Threading.Tasks;

public class GameSceneInitializer : MonoBehaviour
{
    public string JoinCode { get; private set; }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += async (scene, mode) => await OnSceneLoadedAsync(scene, mode);
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= async (scene, mode) => await OnSceneLoadedAsync(scene, mode);
    }

    private async Task OnSceneLoadedAsync(Scene scene, LoadSceneMode mode)
    {
        if (scene.name.StartsWith("Race"))
        {
            if (NetworkLaunchManager.Instance.ShouldStartHost)
            {
                JoinCode = await NetworkUtils.StartMultiplayerHost();
            }
            else if (NetworkLaunchManager.Instance.ShouldStartSingleplayer)
            {
                NetworkUtils.StartSingleplayerHost();
            }
        }
    }
}