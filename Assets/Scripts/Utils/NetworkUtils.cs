using System.Collections;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class NetworkUtils
{
    public static async Task<string> StartMultiplayerHost()
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
        string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
        CustomLogger.Log("Joincode: " + joinCode);
        NetworkManager.Singleton.StartHost();
        return joinCode;
    }

    public static void StartSingleplayerHost()
    {
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(NetworkEndpoint.Parse("127.0.0.1", 7777));
        NetworkManager.Singleton.StartHost();
    }

    public static async void StopHost()
    {
        if (NetworkManager.Singleton == null)
            return;

        NetworkManager.Singleton.Shutdown();

        await Task.Yield();

        foreach (var obj in Object.FindObjectsByType<NetworkObject>(FindObjectsSortMode.None))
        {
            if (obj != null && obj.IsSpawned)
                obj.Despawn(true);
            else
                Object.Destroy(obj.gameObject);
        }

        var spawnManager = NetworkManager.Singleton.SpawnManager;
        spawnManager.SpawnedObjects.Clear();
    }
}