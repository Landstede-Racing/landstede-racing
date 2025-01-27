using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
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
        StartCoroutine(CustomSceneManager.LoadScene("LobbyScene"));
    }
}