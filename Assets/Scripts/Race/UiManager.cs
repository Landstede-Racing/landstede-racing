using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class UiManager : NetworkBehaviour
{
    [SerializeField] private GameObject penaltyGo;
    [SerializeField] private float penaltyVisibleTime = 3;

    public override void OnNetworkSpawn()
    {
        EventService.PlayerPenaltyGiven += OnPlayerPenaltyGiven;
        base.OnNetworkSpawn();
    }

    public override void OnNetworkDespawn()
    {
        EventService.PlayerPenaltyGiven -= OnPlayerPenaltyGiven;
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

        StartCoroutine(PenaltyUICreated(penaltyGGo));
    }
    
    private IEnumerator PenaltyUICreated(GameObject penaltyGGo)
    {
        yield return new WaitForSecondsRealtime(penaltyVisibleTime);

        Destroy(penaltyGGo);
    }
}