using UnityEngine;

public class DashboardController : MonoBehaviour
{
    public GameObject throttleIndicator;
    public VehicleController vehicleController;

    void Start()
    {

    }

    void Update()
    {
        throttleIndicator.GetComponent<RectTransform>().localScale = new(1, vehicleController.gas, 1);
    }
}