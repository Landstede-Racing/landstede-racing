using UnityEngine;

public class InputController : MonoBehaviour
{
    public VehicleController vehicleController;
    public CameraController cameraController;
    private GamepadController gamepadControls;
    private KeyboardController keyboardControls;

    private void Awake()
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

        if (vehicleController == null) Debug.LogError("VehicleController reference is not set in InputController.");
    }

    private void Update()
    {
        if (SettingsController.DeviceController == "gamepadController")
            if (vehicleController != null && gamepadControls != null)
                ProcessGamepadInputs();

        if (SettingsController.DeviceController == "keyboardController")
            if (vehicleController != null && keyboardControls != null)
                ProcessKeyboardInputs();
    }

    private void OnEnable()
    {
        gamepadControls?.Enable();
        keyboardControls?.Enable();
    }

    private void OnDisable()
    {
        gamepadControls?.Disable();
        keyboardControls?.Disable();
    }

    private void ProcessGamepadInputs()
    {
        // Accelerate
        var gas = gamepadControls.vehicleControls.Accelerate.ReadValue<float>();
        vehicleController.SetGas(gas);


        // Brake
        var brake = gamepadControls.vehicleControls.Brake.ReadValue<float>();
        vehicleController.SetBrake(brake);

        // Steering
        var steerVector = gamepadControls.vehicleControls.Steer.ReadValue<Vector2>();
        var steer = steerVector.x;
        vehicleController.SetSteeringAngle(steer);

        // Gear Up
        if (gamepadControls.vehicleControls.GearUp.triggered) StartCoroutine(vehicleController.ChangeGear(1));

        // Gear Down
        if (gamepadControls.vehicleControls.GearDown.triggered) StartCoroutine(vehicleController.ChangeGear(-1));

        if (gamepadControls.vehicleControls.DRS.triggered) vehicleController.ToggleDRS();

        if (gamepadControls.vehicleControls.LookBack.triggered)
            cameraController.SetReverseCam(true);
        else if (gamepadControls.vehicleControls.LookBack.WasReleasedThisFrame()) cameraController.SetReverseCam(false);

        if (gamepadControls.vehicleControls.NextCam.triggered) cameraController.NextCamera();
    }

    private void ProcessKeyboardInputs()
    {
        // Accelerate
        var gas = keyboardControls.vehicleControls.Accelerate.ReadValue<float>();
        vehicleController.SetGas(gas);


        // Brake
        var brake = keyboardControls.vehicleControls.Brake.ReadValue<float>();
        vehicleController.SetBrake(brake);

        // Steering
        var steer = keyboardControls.vehicleControls.Steer.ReadValue<float>();
        vehicleController.SetSteeringAngle(steer);

        // Gear Up
        if (keyboardControls.vehicleControls.GearUp.triggered) StartCoroutine(vehicleController.ChangeGear(1));

        // Gear Down
        if (keyboardControls.vehicleControls.GearDown.triggered) StartCoroutine(vehicleController.ChangeGear(-1));

        if (keyboardControls.vehicleControls.DRS.triggered) vehicleController.ToggleDRS();

        if (keyboardControls.vehicleControls.NextCam.triggered) cameraController.NextCamera();
    }
}