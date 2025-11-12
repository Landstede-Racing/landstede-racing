using UnityEngine.SceneManagement;
using Unity.Netcode;
using UnityEngine;
using Unity.Services.Relay.Models;
using System.Threading.Tasks;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Networking.Transport;

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
                NetworkLaunchManager.Instance.Reset();
            }
            else if (NetworkLaunchManager.Instance.ShouldStartSingleplayer)
            {
                NetworkUtils.StartSingleplayerHost();
                NetworkLaunchManager.Instance.Reset();
            }
        }
    }
}