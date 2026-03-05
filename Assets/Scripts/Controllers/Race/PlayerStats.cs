using System;
using System.Collections.Generic;
using System.Diagnostics;
using LandstedeRacing.Types;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class PlayerStats : NetworkBehaviour, INetworkSerializeByMemcpy
{
    public List<PlayerTiming> playerTimings = new();

    public int position;

    public long totalDriveTime;

    public string shortName;
    public float time;
    public string tire;
    private Rigidbody rb;

    public Stopwatch stopwatch = new();

    private void Start()
    {
        playerTimings.Add(new PlayerTiming(NetworkObjectId, 0, -1, 0));
        shortName = Random.Range(0, 999).ToString();
        // time = Random.Range(0f, 3f);
        time = 0;
        tire = RandomTire(Random.Range(0, 2));
    }

    public override void OnNetworkSpawn()
    {
        playerTimings.Add(new PlayerTiming(NetworkObjectId, 0, 0, 0));

        name = NetworkObjectId.ToString();
        base.OnNetworkSpawn();
    }

    // public void NewTiming(int sectorId)
    // {
    //     PlayerTiming playerTiming = new(NetworkObjectId, stopwatch.ElapsedMilliseconds, sectorId, 1);
    //     playerTimings.Add(playerTiming);
    //     stopwatch.Restart();
    //     totalDriveTime = totalDriveTime + stopwatch.ElapsedMilliseconds;
    //     // CustomLogger.Log(playerTimings[playerTimings.Count - 1].NetworkId + ", " + playerTimings[playerTimings.Count - 1].Timing);
    // }
    
    public void NewTiming(int sectorId, bool lapUp, bool ignoreStopwatch)
    {
        PlayerTiming playerTiming;
        if(stopwatch.ElapsedMilliseconds < 5 && !ignoreStopwatch) return;
        if (lapUp)
            playerTiming = new PlayerTiming(NetworkObjectId, stopwatch.ElapsedMilliseconds, sectorId,
                playerTimings[^1].Lap + 1);
        
        else
            playerTiming = new PlayerTiming(NetworkObjectId, stopwatch.ElapsedMilliseconds, sectorId,
                playerTimings[^1].Lap);
        playerTimings.Add(playerTiming);
        time = stopwatch.ElapsedMilliseconds;
        totalDriveTime = totalDriveTime + stopwatch.ElapsedMilliseconds;

        if(lapUp)
        {
            LapUpRpc(playerTiming.Lap);
        }
        
        stopwatch.Restart();
        
        if(DebugManager.Instance.ShouldDebugRace())
            CustomLogger.Log(playerTimings[playerTimings.Count - 1].NetworkId + ", " + playerTimings[playerTimings.Count - 1].Timing);
    }

    [Rpc(SendTo.Server)]
    private void LapUpRpc(int lap)
    {
        EventService.InvokePlayerFinishedLap(OwnerClientId, lap);
    }

    private static string RandomTire(int tireIndex)
    {
        return tireIndex switch
        {
            0 => "S",
            1 => "M",
            2 => "H",
            _ => "M"
        };
    }

    void OnTriggerEnter(Collider other)
    {
        var sectorController = other.GetComponent<SectorController>();
        if (sectorController == null || !IsServer) return;

        if (stopwatch.ElapsedMilliseconds > 0 && playerTimings[^1].SectorId < sectorController.sectorId)
        {
            NewTiming(sectorController.sectorId, false, false);
        }
        else if (playerTimings[^1].SectorId < sectorController.sectorId)
        {
            stopwatch.Start();
            NewTiming(sectorController.sectorId, true, true);
        }
        else if (stopwatch.ElapsedMilliseconds > 0 && sectorController.sectorType == SectorTypeEnum.Finish)
        {
            NewTiming(sectorController.sectorId, true, false);
        }

        switch (sectorController.sectorType)
        {
            case SectorTypeEnum.PitEntrance:
                PitEnteredClientRpc(OwnerClientId);
                break;
            case SectorTypeEnum.PitExit:
                PitExitedClientRpc(OwnerClientId);
                break;
            default:
                break;
        }
        
        GameObject.FindGameObjectWithTag("Manager").GetComponent<LeaderBoardPosition>().UpdateLeaderBoardServerRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    void PitEnteredClientRpc(ulong clientId)
    {
        EventService.InvokeCarEnteredPit(clientId);
    }

    [Rpc(SendTo.ClientsAndHost)]
    void PitExitedClientRpc(ulong clientId)
    {
        EventService.InvokeCarExitedPit(clientId);
    }
}