using TMPro;
using Unity.Netcode;
using Unity.Networking.Transport.Error;
using UnityEngine;

public class PreRaceUI : NetworkBehaviour
{
    [SerializeField] private GameObject startButton;
    [SerializeField] private TMP_Text joinCodeText;
    private string joinCode;

    public override void OnNetworkSpawn()
    {
        EventService.ReceivedJoinCode += OnJoinCodeReceive;

        var gameSceneInit = FindFirstObjectByType<GameSceneInitializer>();
        if (gameSceneInit != null)
        {
            joinCode = gameSceneInit.JoinCode;
            joinCodeText.text = joinCode;
        }
    }

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
        StartCoroutine(CustomSceneManager.LoadScene("LobbyScene", false));
    }

    private void OnJoinCodeReceive(string code)
    {
        joinCode = code;
        joinCodeText.text = code;
    }
}