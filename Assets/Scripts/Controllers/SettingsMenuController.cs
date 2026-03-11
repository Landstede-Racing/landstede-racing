using UnityEngine;

public class SettingsMenuController : MonoBehaviour
{
    public GameObject settingsScreen;
    public GameObject graphicsSettings;
    public GameObject controlSettings;
    public GameObject audioSettings;
    public GameObject backButton;

    void Update()
    {
        backButton.SetActive(!settingsScreen.activeSelf);
    }

    public void ShowGraphicsSettings()
    {
        settingsScreen.SetActive(false);
        graphicsSettings.SetActive(true);
        controlSettings.SetActive(false);
        audioSettings.SetActive(false);
    }

    public void ShowControlSettings()
    {
        settingsScreen.SetActive(false);
        graphicsSettings.SetActive(false);
        controlSettings.SetActive(true);
        audioSettings.SetActive(false);
    }

    public void ShowAudioSettings()
    {
        settingsScreen.SetActive(false);
        graphicsSettings.SetActive(false);
        controlSettings.SetActive(false);
        audioSettings.SetActive(true);
    }

    public void ShowSettings()
    {
        settingsScreen.SetActive(true);
        graphicsSettings.SetActive(false);
        controlSettings.SetActive(false);
        audioSettings.SetActive(false);
    }
}