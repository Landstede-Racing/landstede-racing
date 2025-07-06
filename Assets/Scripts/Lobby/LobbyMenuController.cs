using Unity.Netcode;
using UnityEngine;

public class LobbyMenuController : NetworkBehaviour
{
    public GameObject mainMenu;
    public GameObject singlePlayerMenu;
    public GameObject multiplayerMenu;
    public GameObject settingsMenu;
    public LobbyCamController camController;
    public TrackSelectionController trackSelectionController;
    public SettingsController settingsController;
    public SettingsMenuController settingsMenuController;

    public void SetMainMenu()
    {
        mainMenu.SetActive(true);
        singlePlayerMenu.SetActive(false);
        multiplayerMenu.SetActive(false);
        settingsMenu.SetActive(false);
    }

    public void SetSinglePlayerMenu()
    {
        mainMenu.SetActive(false);
        singlePlayerMenu.SetActive(true);
        multiplayerMenu.SetActive(false);
        settingsMenu.SetActive(false);
    }

    public void SetMultiplayerMenu()
    {
        mainMenu.SetActive(false);
        singlePlayerMenu.SetActive(false);
        multiplayerMenu.SetActive(true);
        settingsMenu.SetActive(false);
    }

    public void SetSettingsMenu()
    {
        mainMenu.SetActive(false);
        singlePlayerMenu.SetActive(false);
        multiplayerMenu.SetActive(false);
        settingsMenu.SetActive(true);
    }

    public void ExitSettingsMenu(bool save) {
        if(save) settingsController.SaveSettings();
        else settingsController.LoadSettings(settingsController.resolutions.Length - 1);
        settingsMenuController.ShowSettings();
        SetMainMenu();
    }

    public void StartTrackSelection(bool multiplayer)
    {
        NetworkLaunchManager.Instance.SetShouldStartHost(multiplayer);
        camController.SetTrackCamera(0);
        trackSelectionController.multiplayer = multiplayer;
        trackSelectionController.UpdateButtons();
        trackSelectionController.UpdateText();
    }

    public void JoinOnlineGame()
    {
        NetworkManager.Singleton.StartClient();
    }

    public void Back()
    {
        NetworkLaunchManager.Instance.Reset();
        if (camController.currentTrackCam != -1 || camController.garageCamera.gameObject.activeSelf)
        {
            camController.DisableCameras();
            camController.SetScreenCamera();
        }
        else
        {
            SetMainMenu();
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}