using System;
using System.Collections.Generic;
using System.Linq;
using LandstedeRacing.Types;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using GameObject = UnityEngine.GameObject;

public class LeaderBoardPosition : NetworkBehaviour
{
    public int test = 1;

    [SerializeField] private ScrollRect leaderBoard;
    [SerializeField] private GameObject playerData;
    public List<PlayerStats> _players = new();
    private List<PlayerInfo> playersInfo = new();

    private NetworkList<PlayerInfo> m_playersInfo = new(
        new List<PlayerInfo>(), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public override void OnNetworkSpawn()
    {
        // StartRace();
        NetworkManager.OnClientConnectedCallback += OnClientConnected;
        m_playersInfo.OnListChanged += OnPlayerInfoChanged;
        base.OnNetworkSpawn();
    }

    public override void OnNetworkDespawn()
    {
        NetworkManager.OnClientConnectedCallback -= OnClientConnected;
        m_playersInfo.OnListChanged -= OnPlayerInfoChanged;
        base.OnNetworkDespawn();
    }

    [Rpc(SendTo.Server)]
    public void UpdateLeaderBoardServerRpc()
    {
        CustomLogger.Log("UpdateLeaderBoardServerRpc called");
        if (!IsServer) return;
        CustomLogger.Log("UpdateLeaderBoardServerRpc executed on server");
        var leaderboardString = "";

        CustomLogger.Log($"Updating leaderboard with {_players.Count} players");
        _players = _players.OrderByDescending(s => s.playerTimings[^1].Lap)
            .ThenByDescending(s => s.playerTimings[^1].SectorId)
            .ThenBy(s => s.playerTimings[^1].Timing)
            .ToList();

        for (var i = 0; i < _players.Count; i++)
        {
            if (leaderboardString != "") leaderboardString += ", ";
            var player = _players[i];
            player.position = i + 1;
            leaderboardString += $"#{player.position} {player.name}";
            CustomLogger.Log($"Player: {player.name}, Position: {player.position}, Time: {player.playerTimings[^1].Timing}, Tire: {player.tire}");
        }

        CustomLogger.Log(leaderboardString);

        StatsToInfo(_players);

        UpdateLeaderBoardGUIClientRpc(playersInfo.ToArray());
    }

    public void AddPlayer(PlayerStats player)
    {
        var newPlayerData = Instantiate(playerData, leaderBoard.content);
        _players.Add(player);

        PlayerTiming lastTiming = player.playerTimings[^1];
        float currentLapTime = player.playerTimings.FindAll((t) => t.Lap == lastTiming.Lap).Sum((t) => t.Timing);
        newPlayerData.GetComponent<PlayerPositionUI>()
            .UpdateUI(new PlayerInfo(player.position, player.name, player.time, player.tire, lastTiming.Lap, currentLapTime));
    }

    public void AddPlayer(PlayerStats player, int position)
    {
        var newPlayerData = Instantiate(playerData, leaderBoard.content);
        _players.Add(player);
        player.position = position;
        
        PlayerTiming lastTiming = player.playerTimings[^1];
        float currentLapTime = player.playerTimings.FindAll((t) => t.Lap == lastTiming.Lap).Sum((t) => t.Timing);
        newPlayerData.GetComponent<PlayerPositionUI>()
            .UpdateUI(new PlayerInfo(player.position, player.name, player.time, player.tire, lastTiming.Lap, currentLapTime));
    }

    [Rpc(SendTo.Server)]
    public void StartRaceServerRpc()
    {
        CustomLogger.Log("StartRaceServerRpc called");
        if (!IsServer) return;
        CustomLogger.Log("StartRaceServerRpc executed on server");
        StartRace();

        var players = GameObject.FindGameObjectsWithTag("Player").Select(p => p.GetComponentInChildren<PlayerStats>()).ToList();
        if (players.Count == 0)
        {
            CustomLogger.LogWarning("No players found to start");
            return;
        }
        CustomLogger.Log($"Starting with {players.Count} players");
        _players = players;
        UpdateLeaderBoardServerRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void UpdateLeaderBoardGUIClientRpc(PlayerInfo[] players)
    {
        CustomLogger.Log("UpdateLeaderBoardGUIClientRpc called");
        playersInfo = new List<PlayerInfo>(players);
        if (leaderBoard == null) return;
        CustomLogger.Log("Leaderboard is not null");

        if (leaderBoard.content.transform.childCount == 0)
        {
            CustomLogger.Log("No existing player data, creating new ones");
            for (var i = 0; i < playersInfo.Count; i++)
            {
                CustomLogger.Log($"Creating player data for {playersInfo[i].shortName} at position {playersInfo[i].position}");
                var newPlayerData = Instantiate(playerData, leaderBoard.content);
                newPlayerData.GetComponent<PlayerPositionUI>().UpdateUI(playersInfo[i]);
            }
        }
        else
        {
            CustomLogger.Log("Updating existing player data");
            for (var player = 0; player < leaderBoard.content.transform.childCount; player++)
            {
                CustomLogger.Log($"Updating player data for index {player}");
                var go = leaderBoard.content.transform;

                for (var index = 0; index < go.childCount; index++)
                {
                    go.GetChild(index).gameObject.GetComponent<PlayerPositionUI>().UpdateUI(playersInfo[index]);
                }
            }
        }
    }
    

    public void StartRace()
    {
        var players = GameObject.FindGameObjectsWithTag("Player").ToList();
        for (var i = 0; i < players.Count; i++)
        {
            var playerInfo = players[i].GetComponentInChildren<PlayerStats>();
            AddPlayer(playerInfo, i + 1);
        }
    }

    private void StatsToInfo(List<PlayerStats> players)
    {
        playersInfo.Clear();

        foreach (var player in players)
        {
            PlayerTiming lastTiming = player.playerTimings[^1];
            float currentLapTime = player.playerTimings.FindAll((t) => t.Lap == lastTiming.Lap).Sum((t) => t.Timing);
            playersInfo.Add(new PlayerInfo(player.position, player.name, lastTiming.Timing, player.tire, lastTiming.Lap, currentLapTime));
            CustomLogger.Log(player.playerTimings[^1].Timing);
        }

        m_playersInfo.Clear();
        foreach (var info in playersInfo)
        {
            m_playersInfo.Add(info);
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        if (!IsServer) return;

        var player = NetworkManager.SpawnManager.GetPlayerNetworkObject(clientId);
        if (player == null) return;

        var playerStats = player.GetComponent<PlayerStats>();
        if (playerStats == null) return;

        AddPlayer(playerStats);
        UpdateLeaderBoardServerRpc();
    }

    private void OnPlayerInfoChanged(NetworkListEvent<PlayerInfo> changeEvent)
    {
        if (IsServer) return;

        switch (changeEvent.Type)
        {
            case NetworkListEvent<PlayerInfo>.EventType.Add:
                playersInfo.Add(changeEvent.Value);
                break;
            case NetworkListEvent<PlayerInfo>.EventType.Remove:
                playersInfo.Remove(changeEvent.Value);
                break;
            case NetworkListEvent<PlayerInfo>.EventType.Clear:
                playersInfo.Clear();
                break;
            case NetworkListEvent<PlayerInfo>.EventType.Full:
                playersInfo = new List<PlayerInfo>();
                foreach (var info in m_playersInfo)
                {
                    playersInfo.Add(info);
                }
                break;
        }
    }
}