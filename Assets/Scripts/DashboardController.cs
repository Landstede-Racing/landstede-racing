using TMPro;
using UnityEngine;

public class DashboardController : MonoBehaviour
{
    public GameObject throttleIndicator;
    public GameObject brakeIndicator;
    public GameObject ERSUsageIndicator;
    public GameObject ERSChargeIndicator;
    public TMP_Text ERSChargeText;
    public GameObject ERSGeneratedIndicator;
    public VehicleController vehicleController;

    private void Update()
    {
        throttleIndicator.GetComponent<RectTransform>().localScale = new Vector3(1, vehicleController.gas, 1);
        brakeIndicator.GetComponent<RectTransform>().localScale = new Vector3(1, vehicleController.brake, 1);
        ERSUsageIndicator.GetComponent<RectTransform>().localScale =
            new Vector3(vehicleController.GetERSUsagePercentage(), 1, 1);
        ERSChargeIndicator.GetComponent<RectTransform>().localScale =
            new Vector3(vehicleController.GetERSPercentage(), 1, 1);
        ERSChargeText.SetText(vehicleController.GetERSPercentage().ToString("P0"));
        ERSGeneratedIndicator.GetComponent<RectTransform>().localScale =
            new Vector3(vehicleController.GetERSGeneratedPercentage(), 1, 1);
    }
}