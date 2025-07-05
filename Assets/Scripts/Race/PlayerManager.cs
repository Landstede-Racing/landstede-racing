using Unity.Netcode;
using UnityEngine;

public class PlayerManager : NetworkBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject preRacePrefab;
    private bool preRace = true;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsServer)
        {
            preRacePrefab.SetActive(true);
            playerPrefab.SetActive(false);
            SetPreRacePrefabEnabledClientRpc(true);
            EventService.RaceReady += OnRaceReady;
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        }
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        if (IsServer)
        {
            EventService.RaceReady -= OnRaceReady;
        }
    }

    public GameObject GetPreRacePrefab()
    {
        return preRacePrefab;
    }

    private void OnRaceReady()
    {
        if (IsServer)
        {
            playerPrefab.SetActive(true);
            preRacePrefab.SetActive(false);
            preRace = false;
            SetPreRacePrefabEnabledClientRpc(false);
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void SetPreRacePrefabEnabledClientRpc(bool enabled)
    {
        if (IsClient)
        {
            preRacePrefab.SetActive(enabled);
            playerPrefab.SetActive(!enabled);
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        if (IsServer)
        {
            SetPreRacePrefabEnabledClientRpc(preRace);
        }
    }
}