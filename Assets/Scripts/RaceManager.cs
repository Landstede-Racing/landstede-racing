using UnityEngine;
using Unity.Netcode;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class RaceManager : NetworkBehaviour
{
    public int lap = 1;
    public int maxLaps = 3;
    public bool raceStarted = false;
    [SerializeField] private TMP_Text raceLapText;
    [SerializeField] private LeaderBoardPosition leaderBoardPosition;
    [SerializeField] private List<GameObject> startingLights;
    [SerializeField] private Material lightOnMaterial;
    [SerializeField] private Material lightOffMaterial;
    private NetworkVariable<int> currentLap = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public override void OnNetworkSpawn()
    {
        currentLap.OnValueChanged += OnCurrentLapChanged;
        if (!IsServer)
        {
            enabled = false;
            return;
        }
    }

    public override void OnNetworkDespawn()
    {
        currentLap.OnValueChanged -= OnCurrentLapChanged;
        base.OnNetworkDespawn();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
            StartRace();
        raceLapText.text = $"{lap}/{maxLaps}";
        UpdateLap();
    }

    private void UpdateLap()
    {
        if (!IsServer) return;
        var newLap = lap = leaderBoardPosition._players.Count > 0 ? leaderBoardPosition._players[0].playerTimings[^1].Lap : 1;
        if (newLap > lap)
        {
            lap = newLap;
            currentLap.Value = lap;
        }
    }

    public void StartRace()
    {
        if (!IsServer) return;

        leaderBoardPosition.StartRaceServerRpc();
        StartCoroutine(StartRaceCoroutine());
    }

    private void OnCurrentLapChanged(int oldLap, int newLap)
    {
        if (IsServer) return;
        lap = newLap;
    }

    // Function to handle:
    //     - Starting lights
    //     - Countdown
    //     - Starting
    private IEnumerator StartRaceCoroutine()
    {
        Debug.Log("Start");
        yield return new WaitForSeconds(1f);
        Debug.Log("Starting lights sequence");
        for (int i = 0; i < startingLights.Count; i++)
        {
            Debug.Log($"Light {i + 1} ON");
            startingLights[i].GetComponent<MeshRenderer>().material = lightOnMaterial;
            SetLightsOnClientRpc(i, true);
            yield return new WaitForSeconds(1f);
        }
        Debug.Log("Countdown started");
        yield return new WaitForSeconds(1f);
        Debug.Log("GO!");
        for (int i = 0; i < startingLights.Count; i++)
        {
            Debug.Log($"Light {i + 1} OFF");
            startingLights[i].GetComponent<MeshRenderer>().material = lightOffMaterial;
            SetLightsOnClientRpc(i, false);
        }
        raceStarted = true;
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void SetLightsOnClientRpc(int lightIndex, bool isOn)
    {
        Debug.Log($"Setting light {lightIndex} to {(isOn ? "ON" : "OFF")}");
        if (lightIndex < 0 || lightIndex >= startingLights.Count) return;
        var renderer = startingLights[lightIndex].GetComponent<MeshRenderer>();
        renderer.material = isOn ? lightOnMaterial : lightOffMaterial;
    }
}