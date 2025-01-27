using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseController : MonoBehaviour
{
    public string pauseScene;
    public bool isPaused = false;

    void Update()
    {
        switch (SettingsController.DeviceController)
        {
            case 0:
                
                break;
            case 2:
                if(LogitechGSDK.LogiButtonReleased(0, Controls.PauseButton.button)) {
                    TogglePauseMenu();
                }
                break;
        }

        if(Input.GetKeyDown(KeyCode.Escape)) {
            TogglePauseMenu();
        }
    }

    public void TogglePauseMenu() {
        bool wasPaused = isPaused;
        isPaused = !isPaused;
        // Check for singleplayer
        Time.timeScale = wasPaused ? 1 : 0;
        if(wasPaused) {
            SceneManager.UnloadSceneAsync(pauseScene);
        } else {
            SceneManager.LoadScene(pauseScene, LoadSceneMode.Additive);
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(pauseScene));
        }
    }
}