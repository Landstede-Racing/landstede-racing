using Unity.Netcode;
using UnityEngine;

public class PauseMenuController : NetworkBehaviour
{
    public GameObject pauseGo;
    public GameObject settingsGo;

    public void ResumeGame() {
        FindFirstObjectByType<PauseController>().TogglePauseMenu();
    }

    public void ShowSettings() {
        pauseGo.SetActive(false);
        settingsGo.SetActive(true);
    }

    public void ShowPause() {
        pauseGo.SetActive(true);
        settingsGo.SetActive(false);
    }

    public void ExitToMainMenu() {
        NetworkManager.Singleton.Shutdown();
        StartCoroutine(CustomSceneManager.LoadScene("LobbyScene", false));
    }
}