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

    private void Update()
    {
        switch (Input.inputString)
        {
            case "l":
                StartRace();
                break;
            case "K":
                for (var player = 0; player < _players.Count; player++)
                for (var i = player + 1; i < _players[player].playerTimings.Count; i++)
                    Debug.Log(_players[player].GetComponent<NetworkObject>().NetworkObjectId + ", " +
                              _players[player].playerTimings[i].Timing);

                break;
        }
    }

    public override void OnNetworkSpawn()
    {
        StartRace();
    }

    [Rpc(SendTo.Server)]
    public void UpdateLeaderBoardServerRpc()
    {
        if (!IsServer) return;
        var players = _players.OrderBy(s => s.playerTimings[^1].Timing).ThenBy(s => s.playerTimings[^1].SectorId)
            .ThenBy(s => s.playerTimings[^1].Lap).ToList();
        var isEqual = true;
        var leaderboardString = "";

        isEqual = _players.SequenceEqual(players);

        if (isEqual) _players = players;

        for (var i = 0; i < _players.Count; i++)
        {
            if (leaderboardString != "") leaderboardString += ", ";
            var player = _players[i];
            player.position = i + 1;
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

    [Rpc(SendTo.ClientsAndHost)]
    private void UpdateLeaderBoardGUIClientRpc(PlayerInfo[] players)
    {
        playersInfo = new List<PlayerInfo>(players);
        if (leaderBoard == null) return;

        for (var player = 0; player < leaderBoard.content.transform.childCount; player++)
        {
            var go = leaderBoard.content.transform;

            for (var index = 0; index < go.childCount; index++)
                go.GetChild(index).gameObject.GetComponent<PlayerPositionUI>().UpdateUI(playersInfo[index]);
        }

        Debug.Log(leaderBoard.content.transform.childCount);
    }


    public void StartRace()
    {
        var players = GameObject.FindGameObjectsWithTag("Player").ToList();
        Debug.Log(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>PlayerCount: " + players.Count);
        for (var i = 0; i < players.Count; i++)
        {
            var playerInfo = players[i].GetComponent<PlayerStats>();
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
    }
}