using UnityEngine;

public class NetworkLaunchManager : MonoBehaviour
{
    public static NetworkLaunchManager Instance { get; private set; }

    public bool ShouldStartHost { get; private set; }
    public bool ShouldStartSingleplayer { get; private set; }

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

    public void SetShouldStartHost(bool shouldStartHost)
    {
        ShouldStartHost = shouldStartHost;
    }

    public void SetShouldStartSingleplayer(bool shouldStartSingleplayer)
    {
        ShouldStartSingleplayer = shouldStartSingleplayer;
    }

    public void Reset()
    {
        ShouldStartHost = false;
        ShouldStartSingleplayer = false;
    }
}