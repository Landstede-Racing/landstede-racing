using UnityEngine.InputSystem;
using UnityEngine;


public class GamepadMonoScript : MonoBehaviour
{
    public VehicleController vehicleController;
    private GamepadController controls;

    void Awake()
    {
        controls = new GamepadController();

        if (controls == null)
        {
            Debug.LogError("GamepadController failed to initialize.");
            return;
        }

        Debug.Log("Awake from GamepadMonoScript!");

        if (vehicleController == null)
        {
            Debug.LogError("VehicleController reference is not set in GamepadMonoScript.");
        }
    }

    void OnEnable()
    {
        controls?.Enable();
    }

    void OnDisable()
    {
        controls?.Disable();
    }

    void Update()
    {
        if (SettingsController.DeviceController == "gamepadController")
        {
            if (vehicleController != null && controls != null)
            {
                ProcessInputs();

                // Vector2 steerVector = controls.vehicleControls.Steer.ReadValue<Vector2>();
                // float steer = steerVector.x;
                // Debug.Log($"SteerDebug: {steer}");
            }
        }
    }

    private void ProcessInputs()
    {
        // Accelerate
        float gas = controls.vehicleControls.Accelerate.ReadValue<float>();
        vehicleController.SetGas(gas);


        // Brake
        float brake = controls.vehicleControls.Brake.ReadValue<float>();
        vehicleController.SetBrake(brake);

        // Steering
        Vector2 steerVector = controls.vehicleControls.Steer.ReadValue<Vector2>();
        float steer = steerVector.x;
        vehicleController.SetSteeringAngle(steer);

        // Gear Up
        if (controls.vehicleControls.GearUp.triggered)
        {
            StartCoroutine(vehicleController.ChangeGear(1));
            Debug.Log("GearUp!");
        }

        // Gear Down
        if (controls.vehicleControls.GearDown.triggered)
        {
            StartCoroutine(vehicleController.ChangeGear(-1));
            Debug.Log("GearDown!");
        }

        if (controls.vehicleControls.DRS.triggered)
        {
            Debug.Log("DRS TOGGLE!");
            vehicleController.ToggleDRS();
        }
    }
}
