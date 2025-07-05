using Unity.Netcode;
using Unity.Networking.Transport.Error;
using UnityEngine;

public class PreRaceUI : NetworkBehaviour
{
    [SerializeField] private GameObject startButton;
    public void Show()
    {
        gameObject.SetActive(true);
        if (IsHost)
        {
            startButton.SetActive(true);
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void StartButtonClicked()
    {
        if (IsHost)
        {
            RaceReadyRpc();
        }
    }

    [Rpc(SendTo.Server)]
    public void RaceReadyRpc()
    {
        if (!IsServer) return;
        var players = FindObjectsByType<PlayerManager>(FindObjectsSortMode.None);
        // For loop through all players
        foreach (var player in players)
        {
            if (player != null && player.IsServer)
            {
                player.OnRaceReady();
            }
        }
        EventService.InvokeRaceReady();
    }

    public void LeaveButtonClicked()
    {
        NetworkManager.Singleton.Shutdown();
    }
}