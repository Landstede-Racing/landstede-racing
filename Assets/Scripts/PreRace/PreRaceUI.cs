using Unity.Netcode;
using Unity.Networking.Transport.Error;
using UnityEngine;

public class PreRaceUI : NetworkBehaviour
{
    [SerializeField] private GameObject startButton;
    public void Show()
    {
        gameObject.SetActive(true);
        if (IsHost)
        {
            startButton.SetActive(true);
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void LeaveButtonClicked()
    {
        NetworkManager.Singleton.Shutdown();
    }
}