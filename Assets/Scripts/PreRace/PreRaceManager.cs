using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PreRaceManager : NetworkBehaviour
{
    [SerializeField] private List<GameObject> spawnPositions;
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        }
        base.OnNetworkSpawn();
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        }
        base.OnNetworkDespawn();
    }

    private void OnClientConnected(ulong clientId)
    {
        if (IsServer)
        {
            // Here you can handle the logic when a client connects, such as updating UI or game state.
            if(DebugManager.Instance.ShouldDebugRace())
                CustomLogger.Log($"Client {clientId} connected.");

            // Get Player's spawned prefab and assign a spawn position
            if (NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(clientId) is NetworkObject playerObject)
            {
                // Assign a spawn position from the list
                if (spawnPositions.Count > 0)
                {
                    // Get player number based on clientId


                    int index = (int)(clientId % (ulong)spawnPositions.Count); // Simple round-robin assignment
                    playerObject.GetComponent<PlayerManager>().GetPreRacePrefab().transform.SetPositionAndRotation(spawnPositions[index].transform.position, spawnPositions[index].transform.rotation);
                }
                else
                {
                    CustomLogger.LogWarning("No spawn positions available.");
                }
            }
            else
            {
                CustomLogger.LogError($"Player object for client {clientId} not found.");
            }
        }
    }
}