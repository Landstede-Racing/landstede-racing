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
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        }
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        }
    }

    public GameObject GetPreRacePrefab()
    {
        return preRacePrefab;
    }

    public void OnRaceReady()
    {
        if (IsServer)
        {
            preRacePrefab.SetActive(false);
            Debug.Log($"Spawning player for client {OwnerClientId}");
            var newPlayerGo = NetworkManager.Singleton.SpawnManager.InstantiateAndSpawn(playerPrefab, OwnerClientId);
            var raceManager = FindAnyObjectByType<RaceManager>();
            if (raceManager != null)
            {
                raceManager.PlacePlayerOnSpawn(newPlayerGo.GetComponentInChildren<VehicleController>().gameObject, OwnerClientId);
            }
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