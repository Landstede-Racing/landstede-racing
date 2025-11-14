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
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject singlePlayerMenu;
    [SerializeField] private GameObject multiplayerMenu;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject joinMultiplayerMenu;
    [SerializeField] private TMP_InputField codeInputField;
    [SerializeField] private LobbyCamController camController;
    [SerializeField] private TrackSelectionController trackSelectionController;
    [SerializeField] private SettingsController settingsController;
    [SerializeField] private SettingsMenuController settingsMenuController;

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
        NetworkLaunchManager.Instance.SetShouldStartSingleplayer(true);
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

        var joinCode = codeInputField.text;
        CustomLogger.Log("Join Code entered: " + joinCode);

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

    public void QuitGame()
    {
        Application.Quit();
    }
}