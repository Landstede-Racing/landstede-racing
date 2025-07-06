using Unity.Netcode;
using UnityEngine;

public class SectorController : NetworkBehaviour
{
    [SerializeField] public bool isFinish;
    // private bool _startRace = false;

    [SerializeField] private GameObject leaderBoard;

    [SerializeField] public int sectorId;

    public override void OnNetworkSpawn()
    {
        leaderBoard = GameObject.FindGameObjectWithTag("Manager");
    }
}