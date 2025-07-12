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
    }

    [Rpc(SendTo.Server)]
    public void UpdateLeaderBoardServerRpc()
    {
        Debug.Log("UpdateLeaderBoardServerRpc called");
        if (!IsServer) return;
        Debug.Log("UpdateLeaderBoardServerRpc executed on server");
        var leaderboardString = "";

        Debug.Log($"Updating leaderboard with {_players.Count} players");
        _players = _players.OrderByDescending(s => s.playerTimings[^1].Lap)
            .ThenByDescending(s => s.playerTimings[^1].SectorId)
            .ThenBy(s => s.playerTimings[^1].SectorTimestamp)
            .ToList();

        for (var i = 0; i < _players.Count; i++)
        {
            var player = _players[i];
            player.position = i + 1;

            if (i == 0)
            {
                player.gapToFront = 0f; // Leader
            }
            else
            {
                var front = _players[i - 1];
                PlayerTiming playerTiming = player.playerTimings[^1];
                PlayerTiming frontPlayerTiming = front.playerTimings.Where(t => t.SectorId == playerTiming.SectorId).Last();

                player.gapToFront = playerTiming.SectorTimestamp - frontPlayerTiming.SectorTimestamp;
            }
        }

        Debug.Log(leaderboardString);

        StatsToInfo(_players);

        UpdateLeaderBoardGUIClientRpc(playersInfo.ToArray());
    }

    public void AddPlayer(PlayerStats player)
    {
        var newPlayerData = Instantiate(playerData, leaderBoard.content);
        _players.Add(player);
        newPlayerData.GetComponent<PlayerPositionUI>()
            .UpdateUI(new PlayerInfo(player.position, player.name, 0, player.tire));
    }

    public void AddPlayer(PlayerStats player, int position)
    {
        var newPlayerData = Instantiate(playerData, leaderBoard.content);
        player.position = position;
        _players.Add(player);
        Debug.Log($"Updating UI for player {player.OwnerClientId} at position {position}");
        newPlayerData.GetComponent<PlayerPositionUI>()
            .UpdateUI(new PlayerInfo(player.position, player.name, 0, player.tire));
    }

    [Rpc(SendTo.Server)]
    public void StartRaceServerRpc()
    {
        Debug.Log("StartRaceServerRpc called");
        if (!IsServer) return;
        Debug.Log("StartRaceServerRpc executed on server");
        StartRace();

        var players = GameObject.FindGameObjectsWithTag("Player").Select(p => p.GetComponentInChildren<PlayerStats>()).ToList();
        if (players.Count == 0)
        {
            Debug.LogWarning("No players found to start");
            return;
        }
        Debug.Log($"Starting with {players.Count} players");
        _players = players;
        UpdateLeaderBoardServerRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void UpdateLeaderBoardGUIClientRpc(PlayerInfo[] players)
    {
        Debug.Log("UpdateLeaderBoardGUIClientRpc called");
        playersInfo = new List<PlayerInfo>(players);
        if (leaderBoard == null) return;
        Debug.Log("Leaderboard is not null");

        if (leaderBoard.content.transform.childCount == 0)
        {
            Debug.Log("No existing player data, creating new ones");
            for (var i = 0; i < playersInfo.Count; i++)
            {
                Debug.Log($"Creating player data for {playersInfo[i].shortName} at position {playersInfo[i].position}");
                var newPlayerData = Instantiate(playerData, leaderBoard.content);
                newPlayerData.GetComponent<PlayerPositionUI>().UpdateUI(playersInfo[i]);
            }
        }
        else
        {
            Debug.Log("Updating existing player data");
            for (var player = 0; player < leaderBoard.content.transform.childCount; player++)
            {
                Debug.Log($"Updating player data for index {player}");
                var go = leaderBoard.content.transform;

                for (var index = 0; index < go.childCount; index++)
                    go.GetChild(index).gameObject.GetComponent<PlayerPositionUI>().UpdateUI(playersInfo[index]);
            }
        }
    }
    

    public void StartRace()
    {
        var players = GameObject.FindGameObjectsWithTag("Player").ToList();
        Debug.Log($"Found {players.Count} players");
        for (var i = 0; i < players.Count; i++)
        {
            var playerInfo = players[i].GetComponentInChildren<PlayerStats>();
            Debug.Log($"Adding player {playerInfo.OwnerClientId}");
            AddPlayer(playerInfo, i + 1);
        }
    }

    private void StatsToInfo(List<PlayerStats> players)
    {
        playersInfo.Clear();

        foreach (var player in players)
        {
            Debug.Log($"Player {player.shortName}, position: {player.position}, totalTime: {player.totalDriveTime}, gapToFront: {player.gapToFront}");
            playersInfo.Add(
                new PlayerInfo
                {
                    shortName = player.name, 
                    position = player.position,
                    gapToFront = player.gapToFront,
                    tire = player.tire
                });
            Debug.Log($"Player {player.shortName}, total time: {player.totalDriveTime}, gap to front: {player.gapToFront}");
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