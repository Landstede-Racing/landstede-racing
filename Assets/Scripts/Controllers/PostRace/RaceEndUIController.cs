using UnityEngine;

public class RaceEndUIController : MonoBehaviour
{
    public void Retry()
    {
        EventService.InvokeRestartRace();
    }

    public void BackToMenu()
    {
        NetworkUtils.StopHost();
        StartCoroutine(CustomSceneUtils.LoadScene("LobbyScene"));
    }
}