using UnityEngine;

public class DebugManager : MonoBehaviour
{
    public static DebugManager Instance { get; private set; }

    [SerializeField] private bool DebugEvents = false;
    [SerializeField] private bool DebugAerodynamics = false;
    [SerializeField] private bool DebugRace = false;
    [SerializeField] private bool DebugNetwork = false;
    [SerializeField] private bool DebugCar = false;
    [SerializeField] private bool DebugInput = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public bool ShouldDebugEvents()
    {
        return DebugEvents;
    }

    public bool ShouldDebugAerodynamics()
    {
        return DebugAerodynamics;
    }

    public bool ShouldDebugRace()
    {
        return DebugRace;
    }

    public bool ShouldDebugNetwork()
    {
        return DebugNetwork;
    }

    public bool ShouldDebugCar()
    {
        return DebugCar;
    }

    public bool ShouldDebugInput()
    {
        return DebugInput;
    }
}