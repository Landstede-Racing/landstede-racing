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
    public static event Action RestartRace;
    public static event Action PlayerPlaced;
    public static event Action<ulong, string> PlayerPenalty;
    public static event Action<ulong, string> PlayerPenaltyGiven;
    public static event Action StartMultiplayer;
    public static event Action<string> ReceivedJoinCode;
    public static event Action<ulong> CarEnteredPit;
    public static event Action<ulong> CarExitedPit;
    public static event Action PitStopStart;
    public static event Action PitStopEnd;

    public static event Action<Location, float, float> PartDamaged;

    public static void InvokeRaceStarted()
    {
        EventCalled("RaceStarted");
        RaceStarted?.Invoke();
    }

    public static void InvokeCountdownStarted()
    {
        EventCalled("CountdownStarted");
        CountdownStarted?.Invoke();
    }

    public static void InvokeRaceEnded()
    {
        EventCalled("RaceEnded");
        RaceEnded?.Invoke();
    }

    public static void InvokeRaceReady()
    {
        EventCalled("RaceReady");
        RaceReady?.Invoke();
    }
    
    public static void InvokeRestartRace()
    {
        EventCalled("RestartRace");
        RestartRace?.Invoke();
    }

    public static void InvokePlayerPlaced()
    {
        EventCalled("PlayerPlaced");
        PlayerPlaced?.Invoke();
    }

    public static void InvokePlayerMoved(ulong playerId)
    {
        EventCalled($"PlayerMoved, playerId: {playerId}");
        PlayerMoved?.Invoke(playerId);
    }

    public static void InvokePlayerFinished(ulong playerId)
    {
        EventCalled("PlayerFinished");
        PlayerFinished?.Invoke(playerId);
    }

    public static void InvokePlayerPenalty(ulong playerId, string penalty)
    {
        EventCalled($"PlayerPenalty, playerId: {playerId}, penalty: {penalty}");
        PlayerPenalty?.Invoke(playerId, penalty);
    }

    public static void InvokePlayerPenaltyGiven(ulong playerId, string penalty)
    {
        EventCalled($"PlayerPenaltyGiven, playerId: {playerId}, penalty: {penalty}");
        PlayerPenaltyGiven?.Invoke(playerId, penalty);
    }

    public static void InvokeStartMultiplayer()
    {
        EventCalled("StartMultiplayer");
        StartMultiplayer?.Invoke();
    }

    public static void InvokeReceivedJoinCode(string code)
    {
        EventCalled($"ReceivedJoinCode, code: {code}");
        ReceivedJoinCode?.Invoke(code);
    }

    public static void InvokeCarEnteredPit(ulong clientId)
    {
        EventCalled($"CarEnteredPit, clientId: {clientId}");
        CarEnteredPit?.Invoke(clientId);
    }

    public static void InvokeCarExitedPit(ulong clientId)
    {
        EventCalled($"CarExitedPit, clientId: {clientId}");
        CarExitedPit?.Invoke(clientId);
    }

    public static void InvokePitStopStart()
    {
        EventCalled("PitStopStart");
        PitStopStart?.Invoke();
    }

    public static void InvokePitStopEnd()
    {
        EventCalled("PitStopEnd");
        PitStopEnd?.Invoke();
    }

    public static void InvokePartDamaged(Location location, float maxDamage, float currentDamage)
    {
        EventCalled($"PartDamaged, location: {location.name}, maxDamage: {maxDamage}, currentDamage: {currentDamage}");
        PartDamaged?.Invoke(location, maxDamage, currentDamage);
    }

    private static void EventCalled(string eventName)
    {
        if (DebugManager.Instance.ShouldDebugEvents())
            CustomLogger.Log($"Event {eventName} has been called.");
    }
}