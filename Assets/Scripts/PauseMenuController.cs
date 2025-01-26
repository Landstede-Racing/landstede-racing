using UnityEngine;

public class PauseMenuController : MonoBehaviour
{
    public GameObject pauseMenu;

    void Update()
    {
        switch (SettingsController.DeviceController)
        {
            case 0:
                
                break;
            case 1:
                if(Input.GetKeyDown(KeyCode.Escape)) {
                    TogglePauseMenu();
                }
                break;
            case 2:
                if(LogitechGSDK.LogiButtonReleased(0, Controls.PauseButton.button)) {
                    TogglePauseMenu();
                }
                break;
        }
    }

    public void TogglePauseMenu() {
        pauseMenu.SetActive(!pauseMenu.activeSelf);
    }
}