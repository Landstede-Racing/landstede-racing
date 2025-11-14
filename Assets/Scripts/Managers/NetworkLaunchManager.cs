using Unity.Netcode;
using UnityEngine;

public class NetworkLaunchManager : MonoBehaviour
{
    public static NetworkLaunchManager Instance { get; private set; }

    public bool ShouldStartHost { get; private set; }
    public bool ShouldStartSingleplayer { get; private set; }

    [SerializeField] private GameObject NetworkManagerGO;

    private void Awake()
    {
        if (NetworkManager.Singleton == null)
        {
            var nm = Instantiate(NetworkManagerGO);
            DontDestroyOnLoad(nm);
        }

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