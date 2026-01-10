using UnityEngine;
using Unity.Netcode;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Unity.Netcode.Transports.UTP;
using System;

public class RaceManager : NetworkBehaviour
{
    public int lap = 1;
    public bool raceStarted = false;
    [SerializeField] private TMP_Text raceLapText;
    [SerializeField] private LeaderBoardPosition leaderBoardPosition;
    [SerializeField] private List<GameObject> startingLights;
    [SerializeField] private Material lightOnMaterial;
    [SerializeField] private Material lightOffMaterial;
    [SerializeField] private AudioSource lightBoopSound;
    [SerializeField] private GameObject startingPositions;
    [SerializeField] private RaceType raceType;
    [SerializeField] private int maxLaps;

    [SerializeField] private GameObject timeTrialEndedPrefab;
    [SerializeField] private GameObject raceEndedPrefab;
    
    private List<ulong> finishedPlayers = new();

    private NetworkVariable<int> currentLap = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private Dictionary<ulong, List<string>> playerPenalties = new Dictionary<ulong, List<string>>();
    public override void OnNetworkSpawn()
    {
        currentLap.OnValueChanged += OnCurrentLapChanged;

        if (IsServer)
        {
            EventService.PlayerMoved += PlayerMoved;
            EventService.PlayerFinishedLap += PlayerFinishedLap;
            EventService.PlayerPenalty += PlayerPenaltyGiven;
            EventService.RaceReady += StartRaceRpc;
            EventService.SetupRace += SetupRace;
        }

        if(IsHost)
        {
            EventService.RestartRace += RestartRaceCheck;
        }

        if (!IsServer)
        {
            enabled = false;
            return;
        }
        base.OnNetworkSpawn();
    }

    public override void OnNetworkDespawn()
    {
        currentLap.OnValueChanged -= OnCurrentLapChanged;
        if (IsServer)
        {
            EventService.PlayerMoved -= PlayerMoved;
            EventService.PlayerFinishedLap -= PlayerFinishedLap;
            EventService.PlayerPenalty -= PlayerPenaltyGiven;
            EventService.RaceReady -= StartRaceRpc;
            EventService.SetupRace -= SetupRace;
        }

        if(IsHost)
        {
            EventService.RestartRace -= RestartRaceCheck;
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

    private void PlayerFinishedLap(ulong playerId, int lap)
    {
        if(!IsServer) return;

        if(lap > maxLaps)
        {
            if(!finishedPlayers.Contains(playerId))
            {
                finishedPlayers.Add(playerId);

                if(finishedPlayers.Count >= NetworkManager.Singleton.ConnectedClientsIds.Count)
                {
                    if(DebugManager.Instance.ShouldDebugRace())
                        CustomLogger.Log("All players finished");
                    EndRace();
                }

                PlayerFinishedRpc(playerId, finishedPlayers.Count);
            }
            
            return;
        }

        if(lap > this.lap)
        {
            this.lap = lap;
            currentLap.Value = lap;
        }
    }

    private void SetupRace()
    {
        raceType = NetworkLaunchManager.Instance.raceType;
        maxLaps = NetworkLaunchManager.Instance.raceLaps;
        NetworkLaunchManager.Instance.Reset();
    }

    [Rpc(SendTo.Server)]
    public void StartRaceRpc()
    {
        if(DebugManager.Instance.ShouldDebugRace())
            CustomLogger.Log("StartRaceRpc called");
        if (!IsServer) return;
        if(DebugManager.Instance.ShouldDebugRace())
            CustomLogger.Log("StartRaceRpc executed on server");

        leaderBoardPosition.StartRaceServerRpc();
        StartCoroutine(StartRaceCoroutine());
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void PlayerFinishedRpc(ulong playerId, int position)
    {
        EventService.InvokePlayerFinished(playerId, position);
    }

    private void EndRace()
    {
        // TODO: Implement
        //  - Despawn cars
        //  - Show "podium" (seperate scene or just ui?)
        var _players = leaderBoardPosition._players;

        Dictionary<int, PlayerInfo> playerInfoDictionary = new();
        
        for (int i = 0; i < finishedPlayers.Count; i++)
        {
            var player = finishedPlayers[i];

            playerInfoDictionary[i] = PlayerInfoUtils.StatsToInfo(_players.Find((playerStat) => playerStat.OwnerClientId == player)); 
        }

        OnRaceEnded(raceType, playerInfoDictionary);

        EventService.InvokeRaceEnded(raceType, playerInfoDictionary);
    }

    private void RestartRaceCheck()
    {
        if(DebugManager.Instance.ShouldDebugRace())
            CustomLogger.Log("RestartRaceCheck called");
        if(!IsHost) return;
        if(DebugManager.Instance.ShouldDebugRace())
            CustomLogger.Log("RestartRaceCheck called on host");

        RestartRaceRpc();
    }

    [Rpc(SendTo.Server)]
    private void RestartRaceRpc()
    {
        if(DebugManager.Instance.ShouldDebugRace())
            CustomLogger.Log("RestartRaceRpc called");
        if(!IsServer) return;
        if(DebugManager.Instance.ShouldDebugRace())
            CustomLogger.Log("RestartRaceRpc executed on server");

        NetworkLaunchManager.Instance.SetShouldStartSingleplayer(true);

        NetworkUtils.StopHost();
        StartCoroutine(CustomSceneUtils.LoadScene(SceneManager.GetActiveScene().name));
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
            if(DebugManager.Instance.ShouldDebugRace())
                CustomLogger.Log($"Player {playerId} received penalty: {penalty}");
            PlayerPenaltyGivenClientRpc(playerId, penalty);
        }
    }

    private void OnRaceEnded(RaceType raceType, Dictionary<int, PlayerInfo> players)
    {
        switch (raceType)
        {
            case RaceType.Race:
                ShowRaceEndedUI(players);
                break;

            case RaceType.TimeTrial:
                ShowTimeTrialEndedUI(players);
                break;

            default:
                ShowRaceEndedUI(players);
                break;
        }
    }

    private void ShowRaceEndedUI(Dictionary<int, PlayerInfo> players)
    {
        
    }

    private void ShowTimeTrialEndedUI(Dictionary<int, PlayerInfo> players)
    {
        if(!IsClient) return;
        var timeTrialEndedGo = Instantiate(timeTrialEndedPrefab, gameObject.transform);

        var texts = timeTrialEndedGo.GetComponentsInChildren<TMP_Text>();
        foreach (var text in texts)
        {
            if (text.name == "LapTime")

            {
                var lapTime = TimeSpan.FromMilliseconds(players[0].lapTime).ToString(@"mm\:ss\.fff");
                text.text = $"Lap Time: {lapTime}";
            }
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
        EventService.InvokePlayerPenaltyGiven(playerId, penalty);
    }

    public void PlacePlayerOnSpawn(GameObject playerGo, ulong clientId)
    {
        if (startingPositions != null && startingPositions.transform.childCount > 0)
        {
            var index = (int)(clientId % (ulong)startingPositions.transform.childCount);
            if (DebugManager.Instance.ShouldDebugRace())
                CustomLogger.Log($"Placing player {clientId}\'s object {playerGo.name} on spawn position {index}.");
            Transform spawnPosition = startingPositions.transform.GetChild(index);
            if (spawnPosition == null)
            {
                CustomLogger.LogError($"Spawn position {index} is null for client {clientId}.");
                return;
            }
            if(DebugManager.Instance.ShouldDebugRace())
                CustomLogger.Log($"Setting player {clientId} position to {spawnPosition.position} and rotation to {spawnPosition.rotation}");
            playerGo.transform.SetPositionAndRotation(spawnPosition.position, spawnPosition.rotation);
            // SetPlayerPositionClientRpc(clientId, spawnPosition.position, spawnPosition.rotation);
            EventService.InvokePlayerPlaced();
        }
        else
        {
            CustomLogger.LogWarning("No starting positions available.");
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void SetPlayerPositionClientRpc(ulong clientId, Vector3 position, Quaternion rotation)
    {
        if (!IsClient) return;
        if (DebugManager.Instance.ShouldDebugRace())
            CustomLogger.Log($"Setting player position for client {clientId} to {position} and rotation {rotation}");

        var playerObject = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(clientId).GetComponentInChildren<VehicleController>().gameObject;
        if (playerObject != null)
        {
            if(DebugManager.Instance.ShouldDebugRace())
                CustomLogger.Log($"Found player object for client {clientId}: {playerObject.name}");
            playerObject.transform.SetPositionAndRotation(position, rotation);
        }
        else
        {
            CustomLogger.LogError($"Player object for client {clientId} not found.");
        }
    }



}