using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class UiManager : NetworkBehaviour
{
    [SerializeField] private GameObject penaltyGo;
    [SerializeField] private float penaltyVisibleTime = 3;
    [SerializeField] private GameObject playerFinishedGo;
    [SerializeField] private float playerFinishedVisibleTime = 3;

    public override void OnNetworkSpawn()
    {
        EventService.PlayerPenaltyGiven += OnPlayerPenaltyGiven;
        EventService.PlayerFinished += OnPlayerFinished;
        base.OnNetworkSpawn();
    }

    public override void OnNetworkDespawn()
    {
        EventService.PlayerPenaltyGiven -= OnPlayerPenaltyGiven;
        EventService.PlayerFinished -= OnPlayerFinished;
        base.OnNetworkSpawn();
    }

    private void OnPlayerPenaltyGiven(ulong playerId, string penalty)
    {
        if (!IsClient) return;
        CustomLogger.Log("OnPlayerPenaltyGiven called on client");
        var penaltyGGo = Instantiate(penaltyGo, gameObject.transform);
        CustomLogger.Log($"Penalty UI instantiated for player {playerId} with penalty {penalty}");

        var texts = penaltyGGo.GetComponentsInChildren<TMP_Text>();
        foreach (var text in texts)
        {
            if (text.name == "playerName")
            {
                text.text = $"Player {playerId}";
            }
            else if (text.name == "duration")
            {
                if (penalty == "falseStart")
                {
                    text.text = "+5";
                }
            }
        }

        StartCoroutine(UICreated(penaltyGGo, penaltyVisibleTime));
    }

    private void OnPlayerFinished(ulong playerId, int position)
    {
        if (!IsClient) return;
        CustomLogger.Log("OnPlayerFinished called on client");
        var finishedGGo = Instantiate(playerFinishedGo, gameObject.transform);
        CustomLogger.Log($"PlayerFinished UI instantiated for player {playerId} with position {position}");

        var texts = finishedGGo.GetComponentsInChildren<TMP_Text>();
        foreach (var text in texts)
        {
            if (text.name == "playerName")
            {
                text.text = $"Player {playerId}";
            }
            else if (text.name == "position")
            {
                text.text = $"{position}";
            }
        }

        StartCoroutine(UICreated(finishedGGo, penaltyVisibleTime));
    }
    
    private IEnumerator UICreated(GameObject go, float waitTime)
    {
        yield return new WaitForSecondsRealtime(waitTime);

        Destroy(go);
    }
}