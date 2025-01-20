using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Unity.VisualScripting;
using LandstedeRacing.Components.Stopwatch;
using LandstedeRacing.Types;

public class PlayerStats : NetworkBehaviour
{
    [SerializeField]
    private float pushPower = 20f;

    [SerializeField] private Rigidbody rb;

    [DoNotSerialize] public Stopwatch stopwatch = new();
    
    [DoNotSerialize] public List<PlayerTiming> playerTimings = new();
    
    [DoNotSerialize] public int position = 0;

    [DoNotSerialize] public long totalDriveTime = 0;

    public string shortName;
    public float time;
    public string tire;
    
    

    private void Start()
    {
        shortName = Random.Range(0, 999).ToString();
        time = Random.Range(0f, 3f);
        tire = RandomTire(Random.Range(0, 2));
    }
    
    // private void Update()
    // {
    //     if (!rb || !IsServer) return;
    //     if (Input.GetKeyDown(KeyCode.M))
    //     {
    //         _stopwatch.Start();
    //         rb.AddForce(-pushPower, 0f, 0f, ForceMode.Impulse);
    //         Debug.Log("Adding force to coob");
    //     }
    // }

    public override void OnNetworkSpawn()
    {
        // Debug.Log(NetworkObjectId);
        
        playerTimings.Add(new(NetworkObjectId, 999999999, 0, 0));

        name = NetworkObjectId.ToString();
    }

    public void NewTiming(int sectorId)
    {
        PlayerTiming playerTiming = new(NetworkObjectId, stopwatch.ElapsedMilliseconds, sectorId, 1);
        playerTimings.Add(playerTiming);
        stopwatch.Restart();
        totalDriveTime = totalDriveTime + stopwatch.ElapsedMilliseconds;
        Debug.Log(playerTimings[playerTimings.Count - 1].NetworkId + ", " + playerTimings[playerTimings.Count - 1].Timing);
    }
    
    public void NewTiming(int sectorId, bool lapUp)
    {
        PlayerTiming playerTiming = new(NetworkObjectId, stopwatch.ElapsedMilliseconds, sectorId, playerTimings[^1].Lap + 1);
        playerTimings.Add(playerTiming);
        stopwatch.Restart();
        totalDriveTime = totalDriveTime + stopwatch.ElapsedMilliseconds;
        Debug.Log(playerTimings[playerTimings.Count - 1].NetworkId + ", " + playerTimings[playerTimings.Count - 1].Timing);
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
