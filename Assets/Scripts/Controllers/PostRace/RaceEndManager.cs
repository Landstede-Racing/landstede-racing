using System;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class RaceEndManager : NetworkBehaviour
{
    [SerializeField] private GameObject timeTrialEndedPrefab;
    [SerializeField] private GameObject raceEndedPrefab;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        EventService.RaceEnded += OnRaceEnded;
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        EventService.RaceEnded -= OnRaceEnded;
    }

    private void OnRaceEnded(RaceType raceType, Dictionary<int, PlayerInfo> players)
    {
        switch (raceType)
        {
            case RaceType.Race:
                ShowRaceEndedUI(players);
                break;

            case RaceType.TimeTrial:
                ShowTimeTrialEndedUI(players);
                break;

            default:
                ShowRaceEndedUI(players);
                break;
        }
    }

    private void ShowRaceEndedUI(Dictionary<int, PlayerInfo> players)
    {
        
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