using Unity.Netcode;
using UnityEngine;

public class SectorController : NetworkBehaviour
{
    [SerializeField] private bool isFinish = false;

    [SerializeField] private GameObject leaderBoard;

    [SerializeField] public int sectorId;
    private void Start()
    {
        Debug.Log("Detecting objects in sector!");
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player") return;
        other.GetComponent<PlayerStats>().NewTiming(sectorId);
        leaderBoard.GetComponent<LeaderBoardPosition>().UpdateLeaderBoard();
    }
}
