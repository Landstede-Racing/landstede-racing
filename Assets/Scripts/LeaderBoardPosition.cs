using System;
using System.Collections.Generic;
using System.Linq;
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
        if (!IsServer) return;
        var leaderboardString = "";

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
            .UpdateUI(new PlayerInfo(player.position, player.name, player.time, player.tire));
    }

    public void AddPlayer(PlayerStats player, int position)
    {
        var newPlayerData = Instantiate(playerData, leaderBoard.content);
        _players.Add(player);
        player.position = position;
        newPlayerData.GetComponent<PlayerPositionUI>()
            .UpdateUI(new PlayerInfo(player.position, player.name, player.time, player.tire));
    }

    [Rpc(SendTo.Server)]
    public void StartRaceServerRpc()
    {
        if (!IsServer) return;

        var players = GameObject.FindGameObjectsWithTag("Player").Select(p => p.GetComponentInChildren<PlayerStats>()).ToList();
        _players = players;
        UpdateLeaderBoardServerRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void UpdateLeaderBoardGUIClientRpc(PlayerInfo[] players)
    {
        playersInfo = new List<PlayerInfo>(players);
        if (leaderBoard == null) return;

        if (leaderBoard.content.transform.childCount == 0)
        {
            for (var i = 0; i < playersInfo.Count; i++)
            {
                var newPlayerData = Instantiate(playerData, leaderBoard.content);
                newPlayerData.GetComponent<PlayerPositionUI>().UpdateUI(playersInfo[i]);
            }
        }
        else
        {
            for (var player = 0; player < leaderBoard.content.transform.childCount; player++)
            {
                var go = leaderBoard.content.transform;

                for (var index = 0; index < go.childCount; index++)
                    go.GetChild(index).gameObject.GetComponent<PlayerPositionUI>().UpdateUI(playersInfo[index]);
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
            playersInfo.Add(new PlayerInfo(player.position, player.name, player.playerTimings[^1].Timing, player.tire));
            Debug.Log(player.playerTimings[^1].Timing);
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