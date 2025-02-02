using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.UI;

public class Control
{
    public int controlNumber = 0;
    public int button = 0;
    public string controlName;
}

public class Controls 
{
    public static Control MfdButton = new() { controlNumber = 0, button = 2, controlName = "MFD" };
    public static Control NextGearButton = new() { controlNumber = 1, button = 4, controlName = "Next Gear" };
    public static Control PreviousGearButton = new() { controlNumber = 2, button = 5, controlName = "Previous Gear" };
    public static Control DrsButton = new() { controlNumber = 3, button = 7, controlName = "DRS" };
    public static Control NextCamButton = new() { controlNumber = 4, button = 10, controlName = "Next Camera" };
    public static Control ReverseCamButton = new() { controlNumber = 5, button = 6, controlName = "Reverse Camera" };
    public static Control NextErsModeButton = new() { controlNumber = 6, button = 19, controlName = "Next ERS Mode" };
    public static Control PreviousErsModeButton = new() { controlNumber = 7, button = 20, controlName = "Previous ERS Mode" };
    public static Control PauseButton = new() { controlNumber = 8, button = 9, controlName = "Pause" };

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
    public TMP_Dropdown resolutionDropdown;
    public TMP_Dropdown qualityDropdown;
    public TMP_Dropdown textureDropdown;
    public TMP_Dropdown aaDropdown;
    public TMP_Dropdown cloudsDropdown;
    public TMP_Dropdown ssrDropdown;
    public TMP_Dropdown shadowsDropdown;
    public Toggle ssLensFlareToggle;

    public Slider volumeSlider;
    float currentVolume;
    public Resolution[] resolutions;

    public VolumeProfile volumeProfile;

    public GameObject controlGo;
    public ScrollRect controlScrollView;

    public TMP_Dropdown controllerDropdown;
    public static int DeviceController { get; set; } = 2;
    public Text connectedDeviceText;
    private bool listening = false;
    private int listeningForControl = -1;


    // Start is called before the first frame update
    void Start()
    {
        LogitechGSDK.LogiSteeringInitialize(false);
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
        UpdateControls();
    }

    void Update()
    {
        if(LogitechGSDK.LogiUpdate() && LogitechGSDK.LogiIsConnected(0)) {
            if(listening) {
                for (int i = 0; i < 26; i++)
                {
                    if(LogitechGSDK.LogiButtonReleased(0, i)) {
                        listening = false;
                        Controls.SetControl(listeningForControl, i);
                        UpdateControls();
                    }
                }
            }
        }
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

    void UpdateControls() {
        for (int i = 0; i < controlScrollView.content.transform.childCount; i++)
        {
            Destroy(controlScrollView.content.transform.GetChild(i).gameObject);
        }
        foreach (Control control in Controls.Values)
        {
            GameObject controlButton = Instantiate(controlGo, controlScrollView.content.transform);
            controlButton.GetComponent<ControlButtonUI>().control = control;
            controlButton.GetComponent<ControlButtonUI>().button = ControllerButtons.GetButton(control.button);
        }
    }

    public void StartListeningForInput(int controlNumber) {
        listening = true;
        listeningForControl = controlNumber;
        Controls.SetControl(controlNumber, -1);
        UpdateControls();
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

    public void SetCloudsQuality(int cloudsIndex)
    {
        if(volumeProfile.TryGet<VisualEnvironment>(out var visualEnvironment)) {
            visualEnvironment.cloudType.value = cloudsIndex == 1 ? 1 : 0;
        }
        if(volumeProfile.TryGet<VolumetricClouds>(out var volumetricClouds)) {
            volumetricClouds.enable.value = cloudsIndex >= 2;
            volumetricClouds.cloudSimpleMode.value = cloudsIndex == 3 ? VolumetricClouds.CloudSimpleMode.Quality : VolumetricClouds.CloudSimpleMode.Performance;
            Debug.Log(volumetricClouds.enable.value);
        }
        qualityDropdown.value = 6;
    }

    public void SetSSR(int ssrIndex)
    {
        if(volumeProfile.TryGet<ScreenSpaceReflection>(out var screenSpaceReflection)) {
            screenSpaceReflection.active = ssrIndex != 0;
            screenSpaceReflection.quality.value = ssrIndex;
        }
        qualityDropdown.value = 6;
    }

    public void SetShadows(int shadowIndex)
    {
        if(volumeProfile.TryGet<HDShadowSettings>(out var hdShadowSettings)) {
            switch (shadowIndex)
            {
                case 0:
                    hdShadowSettings.maxShadowDistance.value = 0;
                    break;
                case 1:
                    hdShadowSettings.maxShadowDistance.value = 300;
                    break;
                case 2:
                    hdShadowSettings.maxShadowDistance.value = 600;
                    break;
                case 3:
                    hdShadowSettings.maxShadowDistance.value = 1000;
                    break;
            }
        }
        qualityDropdown.value = 6;
    }

    public void SetSSLensFlare(bool ssLensFlare)
    {
        if(volumeProfile.TryGet<ScreenSpaceLensFlare>(out var screenSpaceLensFlare)) {
            screenSpaceLensFlare.active = ssLensFlare;
        }
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
                cloudsDropdown.value = 0;
                ssrDropdown.value = 0;
                shadowsDropdown.value = 0;
                ssLensFlareToggle.isOn = false;
                break;
            case 1: // quality level - low
                textureDropdown.value = 2;
                aaDropdown.value = 0;
                cloudsDropdown.value = 1;
                ssrDropdown.value = 0;
                shadowsDropdown.value = 1;
                ssLensFlareToggle.isOn = false;
                break;
            case 2: // quality level - medium
                textureDropdown.value = 1;
                aaDropdown.value = 0;
                cloudsDropdown.value = 1;
                ssrDropdown.value = 1;
                shadowsDropdown.value = 2;
                ssLensFlareToggle.isOn = false;
                break;
            case 3: // quality level - high
                textureDropdown.value = 0;
                aaDropdown.value = 1;
                cloudsDropdown.value = 2;
                ssrDropdown.value = 2;
                shadowsDropdown.value = 2;
                ssLensFlareToggle.isOn = true;
                break;
            case 4: // quality level - very high
                textureDropdown.value = 0;
                aaDropdown.value = 2;
                cloudsDropdown.value = 2;
                ssrDropdown.value = 2;
                shadowsDropdown.value = 2;
                ssLensFlareToggle.isOn = true;
                break;
            case 5: // quality level - ultra
                textureDropdown.value = 0;
                aaDropdown.value = 3;
                cloudsDropdown.value = 3;
                ssrDropdown.value = 3;
                shadowsDropdown.value = 3;
                ssLensFlareToggle.isOn = true;
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
        PlayerPrefs.SetInt("CloudsQualityPreference", cloudsDropdown.value);
        PlayerPrefs.SetInt("SSRQualityPreference", ssrDropdown.value);
        PlayerPrefs.SetInt("ShadowsQualityPreference", shadowsDropdown.value);
        PlayerPrefs.SetInt("SSLensFlarePreference", Convert.ToInt32(ssLensFlareToggle.isOn));
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
        if(PlayerPrefs.HasKey("CloudsQualityPreference"))
            cloudsDropdown.value = PlayerPrefs.GetInt("CloudsQualityPreference");
        else
            cloudsDropdown.value = 2;
        if(PlayerPrefs.HasKey("SSRQualityPreference"))
            ssrDropdown.value = PlayerPrefs.GetInt("SSRQualityPreference");
        else
            ssrDropdown.value = 2;
        if(PlayerPrefs.HasKey("ShadowsQualityPreference"))
            shadowsDropdown.value = PlayerPrefs.GetInt("ShadowsQualityPreference");
        else
            shadowsDropdown.value = 2;
        if(PlayerPrefs.HasKey("SSLensFlarePreference"))
            ssLensFlareToggle.isOn = Convert.ToBoolean(PlayerPrefs.GetInt("SSLensFlarePreference"));
        else
            ssLensFlareToggle.isOn = true;
    }
}
