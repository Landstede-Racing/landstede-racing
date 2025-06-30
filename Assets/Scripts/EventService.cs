using System;
using UnityEngine;

public static class EventService
{
    public static event Action<ulong> PlayerMoved;
    public static event Action<ulong> PlayerFinished;
    public static event Action RaceStarted;
    public static event Action RaceEnded;
    public static event Action<ulong, string> PlayerPenalty;
    public static event Action<ulong, string> PlayerPenaltyGiven;

    public static void InvokeRaceStarted()
    {
        RaceStarted?.Invoke();
    }

    public static void InvokeRaceEnded()
    {
        RaceEnded?.Invoke();
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
}