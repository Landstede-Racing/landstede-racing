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

    // private void OnTriggerEnter(Collider other)
    // {
    //     // Debug.Log("player entered the trigger");
    //     if (other.tag != "Player" || !IsServer) return;
    //     Debug.Log("player entered the trigger and is server");
    //     var player = other.GetComponentInChildren<PlayerStats>();
    //     if (player.stopwatch.ElapsedMilliseconds > 0 && player.playerTimings[^1].SectorId < sectorId)
    //     {
    //         Debug.Log("New timing for sector " + sectorId);
    //         player.NewTiming(sectorId, false);
    //     }
    //     else if (player.playerTimings[^1].SectorId < sectorId)
    //     {
    //         Debug.Log("Starting stopwatch on sector " + sectorId);
    //         player.stopwatch.Start();
    //         // _startRace = true;
    //     }
    //     else if (player.stopwatch.ElapsedMilliseconds > 0 && isFinish)
    //     {
    //         Debug.Log("Finishing timing for sector " + sectorId);
    //         player.NewTiming(sectorId, true);
    //     }
    //     leaderBoard.GetComponent<LeaderBoardPosition>().UpdateLeaderBoardServerRpc();
    // }
}