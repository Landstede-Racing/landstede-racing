using TMPro;
using Unity.Netcode;
using UnityEngine;

public class UiManager : NetworkBehaviour
{
    [SerializeField] private GameObject penaltyGo;

    public override void OnNetworkSpawn()
    {
        EventService.PlayerPenaltyGiven += OnPlayerPenaltyGiven;
    }

    public override void OnNetworkDespawn()
    {
        EventService.PlayerPenaltyGiven -= OnPlayerPenaltyGiven;
    }

    private void OnPlayerPenaltyGiven(ulong playerId, string penalty)
    {
        if (!IsClient) return;
        Debug.Log("OnPlayerPenaltyGiven called on client");
        var penaltyGGo = Instantiate(penaltyGo, gameObject.transform);
        Debug.Log($"Penalty UI instantiated for player {playerId} with penalty {penalty}");
        
        var texts = penaltyGGo.GetComponentsInChildren<TMP_Text>();
        foreach (var text in texts)
        {
            if(text.name == "playerName")
            {
                text.text = $"Player {playerId}";
            }
            else if(text.name == "duration")
            {
                if (penalty == "falseStart")
                {
                    text.text = "+5";
                }
            }
        }
    }
}