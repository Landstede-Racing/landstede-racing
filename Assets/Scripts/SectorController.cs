using Unity.Netcode;
using UnityEngine;

public class SectorController : NetworkBehaviour
{
    [SerializeField] private bool isFinish = false;
    private bool _startRace = false;

    [SerializeField] private GameObject leaderBoard;

    [SerializeField] public int sectorId;
    
    private void Start()
    {
        Debug.Log("Detecting objects in sector!");
    }
    
    private void OnTriggerEnter(Collider other)
    {
        PlayerStats player = other.GetComponent<PlayerStats>();
        if (other.tag != "Player") return;
        leaderBoard.GetComponent<LeaderBoardPosition>().UpdateLeaderBoard();
        if (player.stopwatch.ElapsedMilliseconds > 0 && player.playerTimings[^1].SectorId < sectorId)
        {
            if (_startRace)
            {
                player.NewTiming(sectorId, _startRace);
            }
            player.NewTiming(sectorId);
        }
        else if(player.playerTimings[^1].SectorId < sectorId)
        {
            player.stopwatch.Start();
            _startRace = true;
        } else if (isFinish)
        {
            
        }
    }
}
