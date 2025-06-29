using Unity.Netcode;
using UnityEngine;

public class SectorController : NetworkBehaviour
{
    [SerializeField] public bool isFinish;
    // private bool _startRace = false;

    [SerializeField] private GameObject leaderBoard;

    [SerializeField] public int sectorId;

    private void Start()
    {
        Debug.Log("Detecting objects in sector!");
    }

    public override void OnNetworkSpawn()
    {
        leaderBoard = GameObject.FindGameObjectWithTag("Manager");
    }
}