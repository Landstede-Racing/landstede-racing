using System;
using UnityEngine;

public static class EventService
{
    public static event Action<ulong> PlayerMoved;
    public static event Action<ulong> PlayerFinished;
    public static event Action RaceStarted;
    public static event Action CountdownStarted;
    public static event Action RaceEnded;
    public static event Action RaceReady;
    public static event Action PlayerPlaced;
    public static event Action<ulong, string> PlayerPenalty;
    public static event Action<ulong, string> PlayerPenaltyGiven;
    public static event Action StartMultiplayer;
    public static event Action<string> ReceivedJoinCode;

    public static void InvokeRaceStarted()
    {
        RaceStarted?.Invoke();
    }

    public static void InvokeCountdownStarted()
    {
        CountdownStarted?.Invoke();
    }

    public static void InvokeRaceEnded()
    {
        RaceEnded?.Invoke();
    }

    public static void InvokeRaceReady()
    {
        RaceReady?.Invoke();
    }

    public static void InvokePlayerPlaced()
    {
        PlayerPlaced?.Invoke();
    }

    public static void InvokePlayerMoved(ulong playerId)
    {
        PlayerMoved?.Invoke(playerId);
    }

    public static void InvokePlayerFinished(ulong playerId)
    {
        PlayerFinished?.Invoke(playerId);
    }

    public static void InvokePlayerPenalty(ulong playerId, string penalty)
    {
        PlayerPenalty?.Invoke(playerId, penalty);
    }

    public static void InvokePlayerPenaltyGiven(ulong playerId, string penalty)
    {
        PlayerPenaltyGiven?.Invoke(playerId, penalty);
    }

    public static void InvokeStartMultiplayer()
    {
        StartMultiplayer?.Invoke();
    }

    public static void InvokeReceivedJoinCode(string code)
    {
        ReceivedJoinCode?.Invoke(code);
    }
}