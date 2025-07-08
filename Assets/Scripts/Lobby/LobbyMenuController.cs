using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class LobbyMenuController : NetworkBehaviour
{
    public GameObject mainMenu;
    public GameObject singlePlayerMenu;
    public GameObject multiplayerMenu;
    public GameObject settingsMenu;
    public GameObject joinMultiplayerMenu;
    public LobbyCamController camController;
    public TrackSelectionController trackSelectionController;
    public SettingsController settingsController;
    public SettingsMenuController settingsMenuController;
    private string joinCode;

    public void SetMainMenu()
    {
        mainMenu.SetActive(true);
        singlePlayerMenu.SetActive(false);
        multiplayerMenu.SetActive(false);
        joinMultiplayerMenu.SetActive(false);
        settingsMenu.SetActive(false);
    }

    public void SetSinglePlayerMenu()
    {
        mainMenu.SetActive(false);
        singlePlayerMenu.SetActive(true);
        multiplayerMenu.SetActive(false);
        joinMultiplayerMenu.SetActive(false);
        settingsMenu.SetActive(false);
    }

    public void SetMultiplayerMenu()
    {
        mainMenu.SetActive(false);
        singlePlayerMenu.SetActive(false);
        multiplayerMenu.SetActive(true);
        joinMultiplayerMenu.SetActive(false);
        settingsMenu.SetActive(false);
    }

    public void SetSettingsMenu()
    {
        mainMenu.SetActive(false);
        singlePlayerMenu.SetActive(false);
        multiplayerMenu.SetActive(false);
        joinMultiplayerMenu.SetActive(false);
        settingsMenu.SetActive(true);
    }

    public void SetJoinMultiplayerMenu()
    {
        mainMenu.SetActive(false);
        singlePlayerMenu.SetActive(false);
        multiplayerMenu.SetActive(false);
        joinMultiplayerMenu.SetActive(true);
        settingsMenu.SetActive(false);
    }

    public void ExitSettingsMenu(bool save)
    {
        if (save) settingsController.SaveSettings();
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
        _ = JoinOnlineGameAsync();
    }

    public async Task JoinOnlineGameAsync()
    {
        await UnityServices.InitializeAsync();
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        var allocation = await RelayService.Instance.JoinAllocationAsync(joinCode: joinCode);
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(AllocationUtils.ToRelayServerData(allocation, "udp"));
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

    public void SetJoinCodeInput(string code)
    {
        joinCode = code;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}