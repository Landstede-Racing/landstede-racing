using Unity.Netcode;
using UnityEngine;

public class SectorController : NetworkBehaviour
{
    [SerializeField] private bool isFinish;
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


    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("player entered the trigger");
        if (other.tag != "Player" || !IsServer) return;
        var player = other.GetComponent<PlayerStats>();
        leaderBoard.GetComponent<LeaderBoardPosition>().UpdateLeaderBoardServerRpc();
        if (player.stopwatch.ElapsedMilliseconds > 0 && player.playerTimings[^1].SectorId < sectorId)
            // if (_startRace)
            // {
            //     player.NewTiming(sectorId, _startRace);
            // }
            player.NewTiming(sectorId, false);
        else if (player.playerTimings[^1].SectorId < sectorId)
            player.stopwatch.Start();
        // _startRace = true;
        else if (player.stopwatch.ElapsedMilliseconds > 0 && isFinish) player.NewTiming(sectorId, true);
    }
}