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
        shortName = Random.Range(0, 999).ToString();
        // time = Random.Range(0f, 3f);
        time = 0;
        tire = RandomTire(Random.Range(0, 2));
    }

    public override void OnNetworkSpawn()
    {
        playerTimings.Add(new PlayerTiming(NetworkObjectId, 999999999, 0, 0));

        name = NetworkObjectId.ToString();
    }

    // public void NewTiming(int sectorId)
    // {
    //     PlayerTiming playerTiming = new(NetworkObjectId, stopwatch.ElapsedMilliseconds, sectorId, 1);
    //     playerTimings.Add(playerTiming);
    //     stopwatch.Restart();
    //     totalDriveTime = totalDriveTime + stopwatch.ElapsedMilliseconds;
    //     // Debug.Log(playerTimings[playerTimings.Count - 1].NetworkId + ", " + playerTimings[playerTimings.Count - 1].Timing);
    // }

    public void NewTiming(int sectorId, bool lapUp)
    {
        PlayerTiming playerTiming;
        if (lapUp)
            playerTiming = new PlayerTiming(NetworkObjectId, stopwatch.ElapsedMilliseconds, sectorId,
                playerTimings[^1].Lap + 1);
        else
            playerTiming = new PlayerTiming(NetworkObjectId, stopwatch.ElapsedMilliseconds, sectorId,
                playerTimings[^1].Lap);
        playerTimings.Add(playerTiming);
        stopwatch.Restart();
        time = stopwatch.ElapsedMilliseconds;
        totalDriveTime = totalDriveTime + stopwatch.ElapsedMilliseconds;
        // Debug.Log(playerTimings[playerTimings.Count - 1].NetworkId + ", " + playerTimings[playerTimings.Count - 1].Timing);
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
}