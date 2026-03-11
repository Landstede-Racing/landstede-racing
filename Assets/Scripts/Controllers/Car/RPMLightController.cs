using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RPMLightController : MonoBehaviour
{
    public VehicleController vehicleController;
    public Color greenColor;
    public Color redColor;
    public Color blueColor;

    public TMP_Text drsText;
    private Image[] lights;

    private void Start()
    {
        lights = GetComponentsInChildren<Image>();

        greenColor = new Color(0f, 1f, 0f, 0.5f);
    }

    private void Update()
    {
        // Calculate how many lights should be on
        var i = Mathf.InverseLerp(vehicleController.firstLightOn, vehicleController.redLine,
            vehicleController.currentEngineRPM) * 13;
        var j = 0;
        foreach (var light in lights)
        {
            // Check if RPM lights should be on
            if (i >= j + 1) light.color = j < 5 ? greenColor : j < 10 ? redColor : blueColor;
            else light.color = Color.gray;

            // Check light 14 for DRS enabled
            if (j == 13 && vehicleController.drsEnabled) light.color = blueColor;

            // Check light 15 for DRS available
            if (j == 14 && vehicleController.drsAvailable) light.color = blueColor;
            j++;
        }

        //TODO: DRS zone distence?! instead of the text.
        if (drsText != null)
        {
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
}