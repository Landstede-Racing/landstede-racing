using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class RaceEndManager : NetworkBehaviour
{
    [SerializeField] private GameObject timeTrialEndedPrefab;
    [SerializeField] private GameObject raceEndedPrefab;
    [SerializeField] private GameObject leaderboardEntryPrefab;
    [SerializeField] private GameObject finishCamera;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        EventService.RaceEnded += OnRaceEnded;
        finishCamera.SetActive(false);
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        EventService.RaceEnded -= OnRaceEnded;
    }

    private void OnRaceEnded(RaceType raceType, Dictionary<int, PlayerInfo> players)
    {
        OnRaceEndedRpc(raceType, players.Values.ToArray());
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void OnRaceEndedRpc(RaceType raceType, PlayerInfo[] players)
    {
        Dictionary<int, PlayerInfo> playerDictionary = new();
        for (int i = 0; i < players.Length; i++)
        {
            playerDictionary.Add(i, players[i]);
        }

        switch (raceType)
        {
            case RaceType.Race:
                ShowRaceEndedUI(playerDictionary);
                break;

            case RaceType.TimeTrial:
                ShowTimeTrialEndedUI(playerDictionary);
                break;

            default:
                ShowRaceEndedUI(playerDictionary);
                break;
        }

        finishCamera.SetActive(true);
    }

    private void ShowRaceEndedUI(Dictionary<int, PlayerInfo> players)
    {
        if(!IsClient) return;
        var raceEndedGo = Instantiate(raceEndedPrefab, gameObject.transform);

        for (int i = 0; i < raceEndedGo.transform.childCount; i++)
        {
            var obj = raceEndedGo.transform.GetChild(i);

            switch (obj.name)
            {
                case "Player1":
                {
                    if(players.ContainsKey(0))
                    {
                        SetRaceEndedText(obj.gameObject, players[0]);
                    } else
                    {
                        obj.gameObject.SetActive(false);
                    }
                    break;
                }
                case "Player2":
                {
                    if(players.ContainsKey(1))
                    {
                        SetRaceEndedText(obj.gameObject, players[1]);
                    } else
                    {
                        obj.gameObject.SetActive(false);
                    }
                    break;
                }
                case "Player3":
                {
                    if(players.ContainsKey(2))
                    {
                        SetRaceEndedText(obj.gameObject, players[2]);
                    } else
                    {
                        obj.gameObject.SetActive(false);
                    }
                    break;
                }
            }
        }

        VerticalLayoutGroup scrollView = gameObject.GetComponentInChildren<VerticalLayoutGroup>();

        if(scrollView != null)
        {
            for (int i = 0; i < players.Count; i++)
            {
                var player = players[i];

                GameObject entryGo = Instantiate(leaderboardEntryPrefab, scrollView.transform);
                var texts = entryGo.GetComponentsInChildren<TMP_Text>();

                foreach (var text in texts)
                {
                    switch (text.name)
                    {
                        case "Position":
                        {
                            text.SetText(player.position.ToString());
                            break;
                        }
                        case "Name":
                        {
                            text.SetText(player.shortName.ToString());
                            break;
                        }
                        case "Time":
                        {
                            text.SetText(i == 0 ? "-" : (player.totalTime - players[i - 1].totalTime).ToString());
                            break;
                        }
                    }
                }
            }
        }
    }

    private void SetRaceEndedText(GameObject go, PlayerInfo player)
    {
        var texts = go.GetComponentsInChildren<TMP_Text>();
        foreach (var text in texts)
        {
            if(text.name == "Name")
            {
                text.SetText(player.shortName.ToString());
            }
        }
    }

    private void ShowTimeTrialEndedUI(Dictionary<int, PlayerInfo> players)
    {
        if(!IsClient) return;
        var timeTrialEndedGo = Instantiate(timeTrialEndedPrefab, gameObject.transform);

        var texts = timeTrialEndedGo.GetComponentsInChildren<TMP_Text>();
        foreach (var text in texts)
        {
            if (text.name == "LapTime")
            {
                var lapTime = TimeSpan.FromMilliseconds(players[0].lapTime).ToString(@"mm\:ss\.fff");
                text.text = $"Lap Time: {lapTime}";
            }
        }
    }
}