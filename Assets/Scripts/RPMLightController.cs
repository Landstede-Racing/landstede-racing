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
    private Color offColor;

    // public Image drsBar;

    void Start()
    {
        lights = GetComponentsInChildren<Image>();

        offColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        greenColor = new Color(0f, 1f, 0f, 0.5f);
    }

    void Update()
    {
        // Calculate how many lights should be on
        float i = Mathf.InverseLerp(vehicleController.firstLightOn, vehicleController.redLine, vehicleController.currentEngineRPM) * 13;
        // Debug.Log("Lights: " + i);
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
            // drsBar.color = greenColor;
            drsText.color = Color.white;
        }
        else if (vehicleController.drsAvailable)
        {
            drsText.text = "DRS Available";
            // drsBar.color = Color.yellow;
            drsText.color = Color.black;

        }
        else
        {
            drsText.text = "DRS off";
            drsText.color = Color.red;
            // drsBar.color = offColor;

        }
    }
}