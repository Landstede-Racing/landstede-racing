using UnityEngine;

public class DashboardController : MonoBehaviour
{
    public GameObject throttleIndicator;
    public GameObject brakeIndicator;
    public GameObject ERSUsageIndicator;
    public GameObject ERSChargeIndicator;
    public GameObject ERSGeneratedIndicator;
    public VehicleController vehicleController;

    void Update()
    {
        throttleIndicator.GetComponent<RectTransform>().localScale = new(1, vehicleController.gas, 1);
        brakeIndicator.GetComponent<RectTransform>().localScale = new(1, vehicleController.brake, 1);
        ERSUsageIndicator.GetComponent<RectTransform>().localScale = new(vehicleController.GetERSUsagePercentage(), 1, 1);
        ERSChargeIndicator.GetComponent<RectTransform>().localScale = new(vehicleController.GetERSPercentage(), 1, 1);
        ERSGeneratedIndicator.GetComponent<RectTransform>().localScale = new(vehicleController.GetERSGeneratedPercentage(), 1, 1);
    }
}