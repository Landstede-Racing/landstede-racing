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
                await UnityServices.InitializeAsync();
                if (!AuthenticationService.Instance.IsSignedIn)
                {
                    await AuthenticationService.Instance.SignInAnonymouslyAsync();
                }
                var maxConnections = 3;
                var connectionType = "udp";
                var allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(AllocationUtils.ToRelayServerData(allocation, connectionType));
                JoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
                CustomLogger.Log("Joincode: " + JoinCode);
                NetworkManager.Singleton.StartHost();
                NetworkLaunchManager.Instance.Reset();
            }
            else if (NetworkLaunchManager.Instance.ShouldStartSingleplayer)
            {
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(NetworkEndpoint.Parse("127.0.0.1", 7777));
                NetworkManager.Singleton.StartHost();
                NetworkLaunchManager.Instance.Reset();
            }
        }
    }
}