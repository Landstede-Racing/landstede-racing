using System;
using UnityEngine;
using UnityEngine.UI;

public class RPMLightController : MonoBehaviour
{
    public VehicleController vehicleController;
    private Image[] lights;
    public Color greenColor;
    public Color redColor;
    public Color blueColor;

    void Start()
    {
        lights = GetComponentsInChildren<Image>();
    }

    void Update()
    {
        // Calculate how many lights should be on
        float i = Mathf.InverseLerp(vehicleController.firstLightOn, vehicleController.redLine, vehicleController.currentEngineRPM) * 13;
        Debug.Log("Lights: " + i);
        int j = 0;
        foreach (Image light in lights)
        {
            // Check if RPM lights should be on
            if (i >= j + 1) light.color = j < 5 ? greenColor : j < 10 ? redColor : blueColor;
            else light.color = Color.gray;

            // Check light 14 for DRS enabled
            if (j == 13)
            {
                if (vehicleController.drsEnabled) light.color = blueColor;
            }

            // Check light 15 for DRS available
            if (j == 14)
            {
                if (vehicleController.drsAvailable) light.color = blueColor;
            }
            j++;
        }
    }
}