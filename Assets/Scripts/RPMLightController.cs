using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RPMLightController : MonoBehaviour
{
    public VehicleController vehicleController;
    private Image[] lights;
    public Color greenColor;
    public Color redColor;
    public Color blueColor;

    public TMP_Text drsText;

    void Start()
    {
        lights = GetComponentsInChildren<Image>();

        greenColor = new Color(0f, 1f, 0f, 0.5f);
    }

    void Update()
    {
        // Calculate how many lights should be on
        float i = Mathf.InverseLerp(vehicleController.firstLightOn, vehicleController.redLine, vehicleController.currentEngineRPM) * 13;
        int j = 0;
        foreach (Image light in lights)
        {
            // Check if RPM lights should be on
            if (i >= j + 1) light.color = j < 5 ? greenColor : j < 10 ? redColor : blueColor;
            else light.color = Color.gray;

            // Check light 14 for DRS enabled
            if (j == 13 && vehicleController.drsEnabled)
            {
                light.color = blueColor;
            }

            // Check light 15 for DRS available
            if (j == 14 && vehicleController.drsAvailable)
            {
                light.color = blueColor;
            }
            j++;
        }

        //TODO: DRS zone distence?! instead of the text.
        if (vehicleController.drsEnabled)
        {
            drsText.text = "DRS";
            drsText.color = Color.white;
        }
        else if (vehicleController.drsAvailable)
        {
            drsText.text = "DRS Available";
            drsText.color = Color.black;

        }
        else
        {
            drsText.text = "DRS off";
            drsText.color = Color.red;

        }
    }
}