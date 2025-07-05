using Unity.Netcode;
using UnityEngine;

public class PlayerManager : NetworkBehaviour
{
    [SerializeField] private NetworkObject playerPrefab;
    [SerializeField] private GameObject preRacePrefab;
    private bool preRace = true;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsServer)
        {
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
            preRacePrefab.SetActive(false);
            NetworkManager.Singleton.SpawnManager.InstantiateAndSpawn(playerPrefab, OwnerClientId);
            Destroy(gameObject);
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