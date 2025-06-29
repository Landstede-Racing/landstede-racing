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
    private List<PlayerStats> _players = new();
    private List<PlayerInfo> playersInfo = new();

    private NetworkList<PlayerInfo> m_playersInfo = new(
        new List<PlayerInfo>(), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private void Update()
    {
        switch (Input.inputString)
        {
            case "l":
                StartRaceServerRpc();
                break;
            case "k":
                for (var player = 0; player < _players.Count; player++)
                    for (var i = player + 1; i < _players[player].playerTimings.Count; i++)
                        Debug.Log(_players[player].GetComponent<NetworkObject>().NetworkObjectId + ", " +
                                  _players[player].playerTimings[i].Timing);

                break;
        }
    }

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
        Debug.Log("UpdateLeaderBoardServerRpc called");
        var leaderboardString = "";

        _players = _players.OrderByDescending(s => s.playerTimings[^1].Lap)
            .ThenByDescending(s => s.playerTimings[^1].SectorId)
            .ThenBy(s => s.playerTimings[^1].Timing)
            .ToList();

        Debug.Log(">>>>>>>>>>>>>>>>>>>>>>>>>>>> Player Positions");
        for (var i = 0; i < _players.Count; i++)
        {
            if (leaderboardString != "") leaderboardString += ", ";
            Debug.Log($"Index: {i}");
            var player = _players[i];
            player.position = i + 1;
            Debug.Log($"Player: {player.name}, Position: {player.position}, Time: {player.playerTimings[^1].Timing}, Sector: {player.playerTimings[^1].SectorId}, Lap: {player.playerTimings[^1].Lap}");
            leaderboardString += $"#{player.position} {player.name}";
        }

        Debug.Log(leaderboardString);

        StatsToInfo(_players);

        Debug.Log(">>>>>>>>>>>>>>>>>>>>>>>>>>>> PlayerTime:" + playersInfo[0].time);

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

        Debug.Log("StartRaceServerRpc called");
        var players = GameObject.FindGameObjectsWithTag("Player").Select(p => p.GetComponentInChildren<PlayerStats>()).ToList();
        Debug.Log(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>PlayerCount: " + players.Count);
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
            Debug.Log("Creating new player data in leaderboard");
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

        Debug.Log(leaderBoard.content.transform.childCount);
    }
    

    public void StartRace()
    {
        var players = GameObject.FindGameObjectsWithTag("Player").ToList();
        Debug.Log(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>PlayerCount: " + players.Count);
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
        Debug.Log("Client connected: " + clientId);
        if (!IsServer) return;

        var player = NetworkManager.SpawnManager.GetPlayerNetworkObject(clientId);
        if (player == null) return;
        Debug.Log("Player found: " + player.name);

        var playerStats = player.GetComponent<PlayerStats>();
        if (playerStats == null) return;
        Debug.Log("PlayerStats found: " + playerStats.name);

        AddPlayer(playerStats);
        UpdateLeaderBoardServerRpc();
    }

    private void OnPlayerInfoChanged(NetworkListEvent<PlayerInfo> changeEvent)
    {
        if (IsServer) return;
        // This method is called on clients when the player info list changes
        // We need to update the leaderboard UI based on the new player info
        switch (changeEvent.Type)
        {
            case NetworkListEvent<PlayerInfo>.EventType.Add:
                Debug.Log("PlayerInfo added: " + changeEvent.Value.shortName);
                playersInfo.Add(changeEvent.Value);
                break;
            case NetworkListEvent<PlayerInfo>.EventType.Remove:
                Debug.Log("PlayerInfo removed: " + changeEvent.Value.shortName);
                playersInfo.Remove(changeEvent.Value);
                break;
            case NetworkListEvent<PlayerInfo>.EventType.Clear:
                Debug.Log("PlayerInfo list cleared");
                playersInfo.Clear();
                break;
            case NetworkListEvent<PlayerInfo>.EventType.Full:
                Debug.Log("PlayerInfo list updated, count: " + m_playersInfo.Count);
                playersInfo = new List<PlayerInfo>();
                foreach (var info in m_playersInfo)
                {
                    playersInfo.Add(info);
                }
                break;
        }
        Debug.Log("PlayerInfo changed, updating leaderboard UI");
        if (leaderBoard == null) return;
        Debug.Log("LeaderBoard not null, updating UI");
        Debug.Log("LeaderBoard content child count: " + leaderBoard.content.transform.childCount);

        if (leaderBoard.content.transform.childCount <= 0)
        {
            Debug.Log("Creating new player data in leaderboard");
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
                    go.GetChild(index).gameObject.GetComponent<PlayerPositionUI>()
                        .UpdateUI(playersInfo[index]);
            }
        }

        Debug.Log(leaderBoard.content.transform.childCount);
    }
}