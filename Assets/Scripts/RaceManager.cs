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
    [SerializeField] private AudioSource lightBoopSound;
    private NetworkVariable<int> currentLap = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private Dictionary<ulong, List<string>> playerPenalties = new Dictionary<ulong, List<string>>();
    public override void OnNetworkSpawn()
    {
        currentLap.OnValueChanged += OnCurrentLapChanged;

        if (IsServer)
        {
            EventService.PlayerMoved += PlayerMoved;
            EventService.PlayerPenalty += PlayerPenaltyGiven;
        }

        if (!IsServer)
        {
            enabled = false;
            return;
        }
    }

    public override void OnNetworkDespawn()
    {
        currentLap.OnValueChanged -= OnCurrentLapChanged;
        if (IsServer)
        {
            EventService.PlayerMoved -= PlayerMoved;
            EventService.PlayerPenalty -= PlayerPenaltyGiven;
        }
        base.OnNetworkDespawn();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            if (IsHost)
            {
                StartRaceRpc();
            }
        }
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

    [Rpc(SendTo.Server)]
    public void StartRaceRpc()
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

    private IEnumerator StartRaceCoroutine()
    {
        EventService.InvokeCountdownStarted();
        RaceCountdownStartedClientRpc();
        yield return new WaitForSeconds(1f);
        for (int i = 0; i < startingLights.Count; i++)
        {
            startingLights[i].GetComponent<MeshRenderer>().material = lightOnMaterial;
            SetLightsOnClientRpc(i, true);
            yield return new WaitForSeconds(1f);
        }
        yield return new WaitForSeconds(1f);
        for (int i = 0; i < startingLights.Count; i++)
        {
            startingLights[i].GetComponent<MeshRenderer>().material = lightOffMaterial;
            SetLightsOnClientRpc(i, false);
        }
        raceStarted = true;
        EventService.InvokeRaceStarted();
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void SetLightsOnClientRpc(int lightIndex, bool isOn)
    {
        if (lightIndex < 0 || lightIndex >= startingLights.Count) return;
        var renderer = startingLights[lightIndex].GetComponent<MeshRenderer>();
        renderer.material = isOn ? lightOnMaterial : lightOffMaterial;
        lightBoopSound.Play();
    }

    private void PlayerMoved(ulong playerId)
    {
        if (!IsServer) return;

        if (!raceStarted)
        {
            EventService.InvokePlayerPenalty(playerId, "falseStart");
        }
    }

    private void PlayerPenaltyGiven(ulong playerId, string penalty)
    {
        if (!IsServer) return;

        if (!playerPenalties.ContainsKey(playerId))
        {
            playerPenalties[playerId] = new List<string>();
        }
        if (!playerPenalties[playerId].Contains(penalty))
        {
            playerPenalties[playerId].Add(penalty);
            Debug.Log($"Player {playerId} received penalty: {penalty}");
            PlayerPenaltyGivenClientRpc(playerId, penalty);
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void RaceCountdownStartedClientRpc()
    {
        if (!IsClient) return;
        EventService.InvokeCountdownStarted();
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void PlayerPenaltyGivenClientRpc(ulong playerId, string penalty)
    {
        if (!IsClient) return;
        Debug.Log("PlayerPenaltyGivenClientRpc called on client");
        EventService.InvokePlayerPenaltyGiven(playerId, penalty);
    }
}