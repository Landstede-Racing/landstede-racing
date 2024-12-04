using UnityEngine;

public class DashboardController : MonoBehaviour
{
    public GameObject throttleIndicator;
    public GameObject brakeIndicator;
    public VehicleController vehicleController;

    void Start()
    {

    }

    void Update()
    {
        throttleIndicator.GetComponent<RectTransform>().localScale = new(1, vehicleController.gas, 1);
        brakeIndicator.GetComponent<RectTransform>().localScale = new(1, vehicleController.brake, 1);
    }
}