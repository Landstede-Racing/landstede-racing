using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.Netcode;
using UnityEngine.UI;

public class LeaderBoardPosition : NetworkBehaviour
{
    private List<PlayerStats> _players = new();

    public int test = 1;

    [SerializeField] private ScrollRect leaderBoard;
    [SerializeField] private GameObject playerData;

    public override void OnNetworkSpawn()
    {
        List<GameObject> players = GameObject.FindGameObjectsWithTag("Player").ToList();
        Debug.Log(players.Count);
        for (int i = 0; i < players.Count; i++)
        {
            PlayerStats playerInfo = players[i].GetComponent<PlayerStats>();
            AddPlayer(playerInfo, i + 1);
        }
    }

    private void Update()
    {
        switch(Input.inputString)
        {
            case "L":
                StartRace();
                break;
            case "K":
                for (int player = 0; player < _players.Count; player++)
                {
                    for (int i = player + 1; i < _players[player].playerTimings.Count; i++)
                    {
                        Debug.Log(_players[player].NetworkObjectId + ", " + _players[player].playerTimings[i].Timing);
                    }
                }
                break;
        }
    }

    public void UpdateLeaderBoard()
    {
        List<PlayerStats> players = _players.OrderBy(s => s.playerTimings[^1].Lap).ThenBy(s => s.playerTimings[^1].SectorId).ThenBy(s => s.playerTimings[^1].Timing).ToList(); // .ThenBy(s => s.totalDriveTime)
        bool isEqual = true;
        string leaderboardString = "";
        
        isEqual = _players.SequenceEqual(players);
        
        if (isEqual)
        {
            _players = players;
            //TODO: implement a function that updates the leaderboard on screen when people change position
        }
    
        for(int i = 0; i < _players.Count; i++)
        {
            if(leaderboardString != "") leaderboardString += ", ";
            PlayerStats player = _players[i];
            player.position = i + 1;
            leaderboardString += $"#{player.position} {player.name}";
        }
        
        Debug.Log(leaderboardString);
        
        UpdateLeaderBoardGUI();
    }

    public void AddPlayer(PlayerStats player)
    {
        GameObject newPlayerData = Instantiate(playerData, leaderBoard.content);
        _players.Add(player);
        newPlayerData.GetComponent<PlayerPositionUI>().UpdateUI(player);
    }
    
    public void AddPlayer(PlayerStats player, int position)
    {
        GameObject newPlayerData = Instantiate(playerData, leaderBoard.content);
        _players.Add(player);
        player.position = position;
        newPlayerData.GetComponent<PlayerPositionUI>().UpdateUI(player);
    }

    private void UpdateLeaderBoardGUI()
    {
        if (leaderBoard == null) return;

        for (int player = 0; player < leaderBoard.content.transform.childCount; player++)
        {
            Transform go = leaderBoard.content.transform;

            for (int index = 0; index < go.childCount; index++)
            {
                go.GetChild(index).gameObject.GetComponent<PlayerPositionUI>().UpdateUI(_players[index]);
            }
        }
        
        Debug.Log(leaderBoard.content.transform.childCount);
    }

    public void StartRace()
    {
        
    }
}