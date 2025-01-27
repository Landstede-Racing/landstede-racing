using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Control
{
    public int controlNumber = 0;
    public int button = 0;
}

public class Controls 
{
    public static Control MfdButton = new() { controlNumber = 0, button = 2 };
    public static Control NextGearButton = new() { controlNumber = 1, button = 4 };
    public static Control PreviousGearButton = new() { controlNumber = 2, button = 5 };
    public static Control DrsButton = new() { controlNumber = 3, button = 7 };
    public static Control NextCamButton = new() { controlNumber = 4, button = 10 };
    public static Control ReverseCamButton = new() { controlNumber = 5, button = 6 };
    public static Control NextErsModeButton = new() { controlNumber = 6, button = 19 };
    public static Control PreviousErsModeButton = new() { controlNumber = 7, button = 20 };
    public static Control PauseButton = new() { controlNumber = 8, button = 10 };

    public static IEnumerable<Control> Values
    {
        get
        {
            yield return MfdButton;
            yield return NextGearButton;
            yield return PreviousGearButton;
            yield return DrsButton;
            yield return NextCamButton;
            yield return ReverseCamButton;
            yield return NextErsModeButton;
            yield return PreviousErsModeButton;
            yield return PauseButton;
        }
    }

    public static void SetControl(int controlNumber, int button)
    {
        foreach (Control control in Values)
        {
            if (control.controlNumber == controlNumber)
            {
                control.button = button;
            }
        }
    }
}

public class SettingsController : MonoBehaviour
{
    public AudioMixer audioMixer;
    public Dropdown resolutionDropdown;
    public Dropdown qualityDropdown;
    public Dropdown textureDropdown;
    public Dropdown aaDropdown;
    public Slider volumeSlider;
    float currentVolume;
    public Resolution[] resolutions;

    public Dropdown controllerDropdown;
    public static int DeviceController { get; set; } = 2;
    public Text connectedDeviceText;
    private bool listening = false;
    private int listeningForControl = -1;


    // Start is called before the first frame update
    void Start()
    {
        int currentResolutionIndex = 0;
        if(resolutionDropdown != null) {
            resolutionDropdown.ClearOptions();
            List<string> options = new List<string>();
            resolutions = Screen.resolutions;

            for (int i = 0; i < resolutions.Length; i++)
            {
                string option = resolutions[i].width + " x " + resolutions[i].height;
                options.Add(option);

                if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
                    currentResolutionIndex = i;
            }

            resolutionDropdown.AddOptions(options);
            resolutionDropdown.RefreshShownValue();
        }
        LoadSettings(currentResolutionIndex);
    }

    void FixedUpdate()
    {
        connectedDeviceText.gameObject.SetActive(DeviceController == 2);
        if(DeviceController == 2) {
            if(LogitechGSDK.LogiIsConnected(0)) {
                StringBuilder deviceName = new StringBuilder(256);
                LogitechGSDK.LogiGetFriendlyProductName(0, deviceName, 256);
                connectedDeviceText.text = "Connected: " + deviceName;
            } else {
                connectedDeviceText.text = "No device connected";
            }
        }
    }

    private IEnumerator WaitForInput() {
        for (int i = 0; i < 26; i++)
        {
            if(LogitechGSDK.LogiButtonTriggered(0, i)) {
                Debug.Log("Button " + i + " triggered");
                listening = false;
                Controls.SetControl(listeningForControl, i);
                CancelInvoke("WaitForInput");
            }
        }

        if(listening) {
            yield return null;
        }
    }

    // Controls
    public void SetDevice(int deviceIndex)
    {
        DeviceController = deviceIndex;
    }


    // Audio
    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("MasterVolume", volume);
        currentVolume = volume;
    }

    // Graphics
    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void SetTextureQuality(int textureIndex)
    {
        QualitySettings.globalTextureMipmapLimit = textureIndex;
        qualityDropdown.value = 6;
    }

    public void SetAntiAliasing(int aaIndex)
    {
        QualitySettings.antiAliasing = aaIndex;
        qualityDropdown.value = 6;
    }

    public void SetQuality(int qualityIndex)
    {
        if (qualityIndex != 6) // if the user is not using any of the presets
            QualitySettings.SetQualityLevel(qualityIndex);

        switch (qualityIndex)
        {
            case 0: // quality level - very low
                textureDropdown.value = 3;
                aaDropdown.value = 0;
                break;
            case 1: // quality level - low
                textureDropdown.value = 2;
                aaDropdown.value = 0;
                break;
            case 2: // quality level - medium
                textureDropdown.value = 1;
                aaDropdown.value = 0;
                break;
            case 3: // quality level - high
                textureDropdown.value = 0;
                aaDropdown.value = 0;
                break;
            case 4: // quality level - very high
                textureDropdown.value = 0;
                aaDropdown.value = 1;
                break;
            case 5: // quality level - ultra
                textureDropdown.value = 0;
                aaDropdown.value = 2;
                break;
        }
        
        qualityDropdown.value = qualityIndex;
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetInt("QualitySettingPreference", qualityDropdown.value);
        PlayerPrefs.SetInt("ResolutionPreference", resolutionDropdown.value);
        PlayerPrefs.SetInt("TextureQualityPreference", textureDropdown.value);
        PlayerPrefs.SetInt("AntiAliasingPreference", aaDropdown.value);
        PlayerPrefs.SetInt("FullscreenPreference", Convert.ToInt32(Screen.fullScreen));
        PlayerPrefs.SetFloat("VolumePreference", currentVolume);
        PlayerPrefs.SetInt("DeviceController", DeviceController);
        foreach (Control control in Controls.Values)
        {
            PlayerPrefs.SetInt("Control" + control.controlNumber, control.button);
        }
        PlayerPrefs.Save();
    }

    public void LoadSettings(int currentResolutionIndex)
    {
        if (PlayerPrefs.HasKey("QualitySettingPreference"))
            qualityDropdown.value = PlayerPrefs.GetInt("QualitySettingPreference");
        else
            qualityDropdown.value = 3;

        if (PlayerPrefs.HasKey("ResolutionPreference"))
            resolutionDropdown.value = PlayerPrefs.GetInt("ResolutionPreference");
        else
            resolutionDropdown.value = currentResolutionIndex;

        if (PlayerPrefs.HasKey("TextureQualityPreference"))
            textureDropdown.value = PlayerPrefs.GetInt("TextureQualityPreference");
        else
            textureDropdown.value = 0;

        if (PlayerPrefs.HasKey("AntiAliasingPreference"))
            aaDropdown.value = PlayerPrefs.GetInt("AntiAliasingPreference");
        else
            aaDropdown.value = 0;

        if (PlayerPrefs.HasKey("FullscreenPreference"))
            Screen.fullScreen = Convert.ToBoolean(PlayerPrefs.GetInt("FullscreenPreference"));
        else
            Screen.fullScreen = true;

        if (PlayerPrefs.HasKey("VolumePreference"))
            volumeSlider.value = PlayerPrefs.GetFloat("VolumePreference");
        else
            volumeSlider.value = PlayerPrefs.GetFloat("VolumePreference");

        if (PlayerPrefs.HasKey("DeviceController"))
            controllerDropdown.value = PlayerPrefs.GetInt("DeviceController");
        else
            controllerDropdown.value = 2;

        foreach (Control control in Controls.Values)
        {
            if (PlayerPrefs.HasKey("Control" + control.controlNumber))
            {
                control.button = PlayerPrefs.GetInt("Control" + control.controlNumber);
            }
        }
    }
}
