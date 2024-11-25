using UnityEngine;
using UnityEngine.InputSystem;

public class GamepadMonoScript : MonoBehaviour
{
    GamepadController controls;

    void Awake()
    {
        controls = new GamepadController();

        controls.vehicleControls.Accelerate.performed += ctx => Accelerate();
        Debug.Log("awake from gamepadscript!");
    }

    void OnEnable()
    {
        controls.Enable();
    }

    void OnDisable()
    {
        controls.Disable();
    }

    void Accelerate()
    {
        Debug.Log("accelerate Pressed?!?"); // This should now work
    }
}