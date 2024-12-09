using UnityEngine.InputSystem;
using UnityEngine;


public class InputController : MonoBehaviour
{
    public VehicleController vehicleController;
    private GamepadController gamepadControls;
    private KeyboardController keyboardControls;
    public CameraController cameraController;

    void Awake()
    {
        gamepadControls = new GamepadController();
        keyboardControls = new KeyboardController();

        if (gamepadControls == null)
        {
            Debug.LogError("GamepadController failed to initialize.");
            return;
        }
        if (keyboardControls == null)
        {
            Debug.LogError("KeyboardController failed to initialize.");
            return;
        }

        Debug.Log("Awake from InputController!");

        if (vehicleController == null)
        {
            Debug.LogError("VehicleController reference is not set in InputController.");
        }
    }

    void OnEnable()
    {
        gamepadControls?.Enable();
        keyboardControls?.Enable();
    }

    void OnDisable()
    {
        gamepadControls?.Disable();
        keyboardControls?.Disable();
    }

    void Update()
    {
        if (SettingsController.DeviceController == "gamepadController")
        {
            if (vehicleController != null && gamepadControls != null)
            {
                ProcessGamepadInputs();

                // Vector2 steerVector = gamepadControls.vehicleControls.Steer.ReadValue<Vector2>();
                // float steer = steerVector.x;
                // Debug.Log($"SteerDebug: {steer}");
            }
        }

        if (SettingsController.DeviceController == "keyboardController")
        {
            if (vehicleController != null && keyboardControls != null)
            {
                ProcessKeyboardInputs();
            }
        }
    }

    private void ProcessGamepadInputs()
    {
        // Accelerate
        float gas = gamepadControls.vehicleControls.Accelerate.ReadValue<float>();
        vehicleController.SetGas(gas);


        // Brake
        float brake = gamepadControls.vehicleControls.Brake.ReadValue<float>();
        vehicleController.SetBrake(brake);

        // Steering
        Vector2 steerVector = gamepadControls.vehicleControls.Steer.ReadValue<Vector2>();
        float steer = steerVector.x;
        vehicleController.SetSteeringAngle(steer);

        // Gear Up
        if (gamepadControls.vehicleControls.GearUp.triggered)
        {
            StartCoroutine(vehicleController.ChangeGear(1));
            Debug.Log("GearUp!");
        }

        // Gear Down
        if (gamepadControls.vehicleControls.GearDown.triggered)
        {
            StartCoroutine(vehicleController.ChangeGear(-1));
            Debug.Log("GearDown!");
        }

        if (gamepadControls.vehicleControls.DRS.triggered)
        {
            Debug.Log("DRS TOGGLE!");
            vehicleController.ToggleDRS();
        }

        if (gamepadControls.vehicleControls.LookBack.triggered)
        {
            Debug.Log("Gamepad Lookback toggled!");
            cameraController.SetReverseCam(true);
        }
        else if (gamepadControls.vehicleControls.LookBack.WasReleasedThisFrame())
        {
            Debug.Log("Gamepad Lookback Released");
            cameraController.SetReverseCam(false);
        }

        if (gamepadControls.vehicleControls.NextCam.triggered)
        {
            Debug.Log("Next Cam pressed.");
            cameraController.NextCamera();
        }


    }

    private void ProcessKeyboardInputs()
    {
        // Accelerate
        float gas = keyboardControls.vehicleControls.Accelerate.ReadValue<float>();
        vehicleController.SetGas(gas);


        // Brake
        float brake = keyboardControls.vehicleControls.Brake.ReadValue<float>();
        vehicleController.SetBrake(brake);

        // Steering
        float steer = keyboardControls.vehicleControls.Steer.ReadValue<float>();
        vehicleController.SetSteeringAngle(steer);

        // Gear Up
        if (keyboardControls.vehicleControls.GearUp.triggered)
        {
            StartCoroutine(vehicleController.ChangeGear(1));
            Debug.Log("GearUp!");
        }

        // Gear Down
        if (keyboardControls.vehicleControls.GearDown.triggered)
        {
            StartCoroutine(vehicleController.ChangeGear(-1));
            Debug.Log("GearDown!");
        }

        if (keyboardControls.vehicleControls.DRS.triggered)
        {
            Debug.Log("DRS TOGGLE!");
            vehicleController.ToggleDRS();
        }
    }
}
