using UnityEngine;
using System.Text;
using UnityEngine.SceneManagement;
using System;

public class LogitechSteeringWheel : MonoBehaviour
{
    public VehicleController vehicleController;
    public MfdController mfdController;
    private CameraController cameraController;
    LogitechGSDK.LogiControllerPropertiesData properties;
    private string actualState;
    private string activeForces;
    private string propertiesEdit;
    private string buttonStatus;
    private string forcesLabel;
    private float steeringAngle;
    private int changeLights;
    private float gas;
    private float brake;
    string[] activeForceAndEffect;

    [Header("Force Feedback")]
    private float vibrationTimer = 0f;
    private bool vibrationState = false;
    public float centeringForceMultiplier = 50f; // Strength of centering force
    public float slipForceMultiplier = 100f;    // Strength of slip feedback

    // Use this for initialization
    void Start()
    {
        // Find all child GameObjects that have the WheelControl script attached
        cameraController = GetComponent<CameraController>();

        Debug.Log($"[LogitechSteeringWheel] Device Name: {SettingsController.DeviceController}");

        activeForces = "";
        propertiesEdit = "";
        actualState = "";
        buttonStatus = "";
        forcesLabel = "Press the following keys to activate forces and effects on the steering wheel / gaming controller \n";
        forcesLabel += "Spring force : S\n";
        forcesLabel += "Constant force : C\n";
        forcesLabel += "Damper force : D\n";
        forcesLabel += "Side collision : Left or Right Arrow\n";
        forcesLabel += "Front collision : Up arrow\n";
        forcesLabel += "Dirt road effect : I\n";
        forcesLabel += "Bumpy road effect : B\n";
        forcesLabel += "Slippery road effect : L\n";
        forcesLabel += "Surface effect : U\n";
        forcesLabel += "Car Airborne effect : A\n";
        forcesLabel += "Soft Stop Force : O\n";
        forcesLabel += "Set example controller properties : PageUp\n";
        forcesLabel += "Play Leds : P\n";
        activeForceAndEffect = new string[9];
        if(SettingsController.DeviceController == 2) {
            Debug.Log("SteeringInit:" + LogitechGSDK.LogiSteeringInitialize(false));
        }
    }

    void OnApplicationQuit()
    {
        Debug.Log("SteeringShutdown:" + LogitechGSDK.LogiSteeringShutdown());
    }

    void OnGUI()
    {
        activeForces = GUI.TextArea(new Rect(10, 10, 180, 200), activeForces, 400);
        propertiesEdit = GUI.TextArea(new Rect(200, 10, 200, 200), propertiesEdit, 400);
        actualState = GUI.TextArea(new Rect(410, 10, 300, 200), actualState, 1000);
        buttonStatus = GUI.TextArea(new Rect(720, 10, 300, 200), buttonStatus, 1000);
        GUI.Label(new Rect(10, 400, 800, 400), forcesLabel);
    }

    // Update is called once per frame
    void Update()
    {
        if (SettingsController.DeviceController == 2) //Check for what controller the user wants to use. For now hardcoded in SettingsController.cs
        {
            if (LogitechGSDK.LogiUpdate() && LogitechGSDK.LogiIsConnected(0))
            {
                ApplyForceFeedback();

                LogitechGSDK.DIJOYSTATE2ENGINES rec;
                rec = LogitechGSDK.LogiGetStateUnity(0);
                // Get steer and pedal states
                // lX: Steer (-32768 to 32767f)
                // lY: Gas pedal (32767 to -32768)
                // lRz: Brake pedal (32767 to -32766)


                // Calculate steering angle with a max of 180 degrees, mapped to a value from -1 to 1
                steeringAngle = Mathf.InverseLerp(-32768f / 2.5f, 32767f / 2.5f, rec.lX) * 2 - 1;
                vehicleController.SetSteeringAngle(steeringAngle);

                // Calculate gas amount mapped to a value from 0 to 1
                gas = Mathf.InverseLerp(32767, -32768, rec.lY);
                vehicleController.SetGas(gas);

                // Calculate brake amount mapped to a value from 0 to 1
                brake = Mathf.InverseLerp(32767, -32766, rec.lRz);
                vehicleController.SetBrake(brake);

                if(LogitechGSDK.LogiButtonTriggered(0, Controls.MfdButton.button)) {
                    mfdController.NextPage();
                }
                if (LogitechGSDK.LogiButtonTriggered(0, Controls.NextGearButton.button))
                {
                    StartCoroutine(vehicleController.ChangeGear(1));
                }
                if (LogitechGSDK.LogiButtonTriggered(0, Controls.PreviousGearButton.button))
                {
                    StartCoroutine(vehicleController.ChangeGear(-1));
                }
                if (LogitechGSDK.LogiButtonTriggered(0, Controls.DrsButton.button))
                {
                    vehicleController.ToggleDRS();
                }
                if (LogitechGSDK.LogiButtonTriggered(0, Controls.NextCamButton.button))
                {
                    cameraController.NextCamera();
                }
                if (LogitechGSDK.LogiButtonTriggered(0, Controls.ReverseCamButton.button))
                {
                    cameraController.SetReverseCam(true);
                }
                if (LogitechGSDK.LogiButtonReleased(0, Controls.ReverseCamButton.button))
                {
                    cameraController.SetReverseCam(false);
                }
                if(LogitechGSDK.LogiButtonReleased(0, Controls.NextErsModeButton.button)) {
                    vehicleController.NextERSMode();
                }
                if(LogitechGSDK.LogiButtonReleased(0, Controls.PreviousErsModeButton.button)) {
                    vehicleController.PreviousERSMode();
                }

                if (LogitechGSDK.LogiButtonTriggered(0, 23))
                {
                    SceneManager.LoadScene(0);
                }


                // |-------------------------------------------|
                // | Default LogitechSDK code (debug and such) |
                // |-------------------------------------------|

                //CONTROLLER PROPERTIES
                StringBuilder deviceName = new StringBuilder(256);
                LogitechGSDK.LogiGetFriendlyProductName(0, deviceName, 256);
                propertiesEdit = "Current Controller : " + deviceName + "\n";
                propertiesEdit += "Current controller properties : \n\n";
                LogitechGSDK.LogiControllerPropertiesData actualProperties = new LogitechGSDK.LogiControllerPropertiesData();
                LogitechGSDK.LogiGetCurrentControllerProperties(0, ref actualProperties);
                propertiesEdit += "forceEnable = " + actualProperties.forceEnable + "\n";
                propertiesEdit += "overallGain = " + actualProperties.overallGain + "\n";
                propertiesEdit += "springGain = " + actualProperties.springGain + "\n";
                propertiesEdit += "damperGain = " + actualProperties.damperGain + "\n";
                propertiesEdit += "defaultSpringEnabled = " + actualProperties.defaultSpringEnabled + "\n";
                propertiesEdit += "combinePedals = " + actualProperties.combinePedals + "\n";
                propertiesEdit += "wheelRange = " + actualProperties.wheelRange + "\n";
                propertiesEdit += "gameSettingsEnabled = " + actualProperties.gameSettingsEnabled + "\n";
                propertiesEdit += "allowGameSettings = " + actualProperties.allowGameSettings + "\n";

                //CONTROLLER STATE
                actualState = "Steering wheel current state : \n\n";

                actualState += "x-axis position :" + rec.lX + "\n";
                actualState += "y-axis position :" + rec.lY + "\n";
                actualState += "z-axis position :" + rec.lZ + "\n";
                actualState += "x-axis rotation :" + rec.lRx + "\n";
                actualState += "y-axis rotation :" + rec.lRy + "\n";
                actualState += "z-axis rotation :" + rec.lRz + "\n";
                actualState += "extra axes positions 1 :" + rec.rglSlider[0] + "\n";
                actualState += "extra axes positions 2 :" + rec.rglSlider[1] + "\n";
                switch (rec.rgdwPOV[0])
                {
                    case (0): actualState += "POV : UP\n"; break;
                    case (4500): actualState += "POV : UP-RIGHT\n"; break;
                    case (9000): actualState += "POV : RIGHT\n"; break;
                    case (13500): actualState += "POV : DOWN-RIGHT\n"; break;
                    case (18000): actualState += "POV : DOWN\n"; break;
                    case (22500): actualState += "POV : DOWN-LEFT\n"; break;
                    case (27000): actualState += "POV : LEFT\n"; break;
                    case (31500): actualState += "POV : UP-LEFT\n"; break;
                    default: actualState += "POV : CENTER\n"; break;
                }

                //Button status :

                buttonStatus = "Button pressed : \n\n";
                for (int i = 0; i < 128; i++)
                {
                    if (rec.rgbButtons[i] == 128)
                    {
                        buttonStatus += "Button " + i + " pressed\n";
                    }

                }


                /* THIS AXIS ARE NEVER REPORTED BY LOGITECH CONTROLLERS 
                * 
                * actualState += "x-axis velocity :" + rec.lVX + "\n";
                * actualState += "y-axis velocity :" + rec.lVY + "\n";
                * actualState += "z-axis velocity :" + rec.lVZ + "\n";
                * actualState += "x-axis angular velocity :" + rec.lVRx + "\n";
                * actualState += "y-axis angular velocity :" + rec.lVRy + "\n";
                * actualState += "z-axis angular velocity :" + rec.lVRz + "\n";
                * actualState += "extra axes velocities 1 :" + rec.rglVSlider[0] + "\n";
                * actualState += "extra axes velocities 2 :" + rec.rglVSlider[1] + "\n";
                * actualState += "x-axis acceleration :" + rec.lAX + "\n";
                * actualState += "y-axis acceleration :" + rec.lAY + "\n";
                * actualState += "z-axis acceleration :" + rec.lAZ + "\n";
                * actualState += "x-axis angular acceleration :" + rec.lARx + "\n";
                * actualState += "y-axis angular acceleration :" + rec.lARy + "\n";
                * actualState += "z-axis angular acceleration :" + rec.lARz + "\n";
                * actualState += "extra axes accelerations 1 :" + rec.rglASlider[0] + "\n";
                * actualState += "extra axes accelerations 2 :" + rec.rglASlider[1] + "\n";
                * actualState += "x-axis force :" + rec.lFX + "\n";
                * actualState += "y-axis force :" + rec.lFY + "\n";
                * actualState += "z-axis force :" + rec.lFZ + "\n";
                * actualState += "x-axis torque :" + rec.lFRx + "\n";
                * actualState += "y-axis torque :" + rec.lFRy + "\n";
                * actualState += "z-axis torque :" + rec.lFRz + "\n";
                * actualState += "extra axes forces 1 :" + rec.rglFSlider[0] + "\n";
                * actualState += "extra axes forces 2 :" + rec.rglFSlider[1] + "\n";
                */

                int shifterTipe = LogitechGSDK.LogiGetShifterMode(0);
                string shifterString = "";
                if (shifterTipe == 1) shifterString = "Gated";
                else if (shifterTipe == 0) shifterString = "Sequential";
                else shifterString = "Unknown";
                actualState += "\nSHIFTER MODE:" + shifterString;




                // FORCES AND EFFECTS 
                activeForces = "Active forces and effects :\n";

                //Spring Force -> S
                if (Input.GetKeyUp(KeyCode.S))
                {
                    if (LogitechGSDK.LogiIsPlaying(0, LogitechGSDK.LOGI_FORCE_SPRING))
                    {
                        LogitechGSDK.LogiStopSpringForce(0);
                        activeForceAndEffect[0] = "";
                    }
                    else
                    {
                        LogitechGSDK.LogiPlaySpringForce(0, 0, 50, 50);
                        activeForceAndEffect[0] = "Spring Force\n ";
                    }
                }

                //Constant Force -> C
                if (Input.GetKeyUp(KeyCode.C))
                {
                    if (LogitechGSDK.LogiIsPlaying(0, LogitechGSDK.LOGI_FORCE_CONSTANT))
                    {
                        LogitechGSDK.LogiStopConstantForce(0);
                        activeForceAndEffect[1] = "";
                    }
                    else
                    {
                        LogitechGSDK.LogiPlayConstantForce(0, 50);
                        activeForceAndEffect[1] = "Constant Force\n ";
                    }
                }

                //Damper Force -> D
                if (Input.GetKeyUp(KeyCode.D))
                {
                    if (LogitechGSDK.LogiIsPlaying(0, LogitechGSDK.LOGI_FORCE_DAMPER))
                    {
                        LogitechGSDK.LogiStopDamperForce(0);
                        activeForceAndEffect[2] = "";
                    }
                    else
                    {
                        LogitechGSDK.LogiPlayDamperForce(0, 50);
                        activeForceAndEffect[2] = "Damper Force\n ";
                    }
                }

                //Side Collision Force -> left or right arrow
                if (Input.GetKeyUp(KeyCode.LeftArrow))
                {
                    LogitechGSDK.LogiPlaySideCollisionForce(0, 100);
                }
                if (Input.GetKey(KeyCode.RightArrow))
                {
                    LogitechGSDK.LogiPlaySideCollisionForce(0, -100);
                }

                //Front Collision Force -> up arrow
                if (Input.GetKeyUp(KeyCode.UpArrow))
                {
                    LogitechGSDK.LogiPlayFrontalCollisionForce(0, 100);
                }

                //Dirt Road Effect-> I
                if (Input.GetKeyUp(KeyCode.I))
                {
                    ToggleDirtRoadEffect();
                }

                //Bumpy Road Effect-> B
                if (Input.GetKeyUp(KeyCode.B))
                {
                    ToggleBumpyRoadEffect();
                }

                //Slippery Road Effect-> L
                if (Input.GetKeyUp(KeyCode.L))
                {
                    ToggleSlipperyRoadEffect();
                }

                //Surface Effect-> U
                if (Input.GetKeyUp(KeyCode.U))
                {
                    ToggleSurfaceEffect();
                }

                //Car Airborne -> A
                if (Input.GetKeyUp(KeyCode.A))
                {
                    ToggleAirborneEffect();
                }

                //Soft Stop Force -> O
                if (Input.GetKeyUp(KeyCode.O))
                {
                    ToggleSoftStopEffect();
                }

                //Set preferred controller properties -> PageUp
                if (Input.GetKeyUp(KeyCode.J))
                {
                    //Setting example values
                    // public bool forceEnable;
                    // public int overallGain;
                    // public int springGain;
                    // public int damperGain;
                    // public bool defaultSpringEnabled;
                    // public int defaultSpringGain;
                    // public bool combinePedals;
                    // public int wheelRange;
                    // public bool gameSettingsEnabled;
                    // public bool allowGameSettings;
                    properties.forceEnable = true;
                    properties.overallGain = 80;
                    properties.springGain = 80;
                    properties.damperGain = 80;
                    properties.defaultSpringEnabled = true;
                    properties.defaultSpringGain = 80;
                    properties.combinePedals = false;
                    properties.wheelRange = 90;
                    properties.gameSettingsEnabled = true;
                    properties.allowGameSettings = true;
                    // LogitechGSDK.LogiControllerPropertiesData existingProperties = new();
                    // LogitechGSDK.LogiGetCurrentControllerProperties(0, ref existingProperties);

                    // Debug.Log(existingProperties.defaultSpringEnabled);

                    // existingProperties.defaultSpringEnabled = true;
                    // existingProperties.defaultSpringGain = 80;
                    // existingProperties.allowGameSettings = true;
                    // existingProperties.gameSettingsEnabled = false;
                    // // LogitechGSDK.LogiSet
                    // if (LogitechGSDK.LogiSetPreferredControllerProperties(existingProperties))
                    // {
                    //     Debug.Log("Properties set");
                    // }
                    // else
                    // {
                    //     Debug.Log("DO NOT REDEEM. DO NOT REDEEM THE CARD.");
                    // }

                }

                //Play leds -> P
                if (Input.GetKeyUp(KeyCode.P))
                {
                    LogitechGSDK.LogiPlayLeds(0, 20, 20, 20);
                }

                for (int i = 0; i < 9; i++)
                {
                    activeForces += activeForceAndEffect[i];
                }

                if (changeLights >= 5)
                {
                    LogitechGSDK.LogiPlayLeds(0, vehicleController.currentEngineRPM, vehicleController.firstLightOn, vehicleController.redLine);
                    changeLights = 0;
                }
                else
                {
                    changeLights++;
                }

            }
            else if (!LogitechGSDK.LogiIsConnected(0))
            {
                actualState = "PLEASE PLUG IN A STEERING WHEEL OR A FORCE FEEDBACK CONTROLLER";
            }
            else
            {
                actualState = "THIS WINDOW NEEDS TO BE IN FOREGROUND IN ORDER FOR THE SDK TO WORK PROPERLY";
            }
        }
    }

    private void ApplyForceFeedback()
    {
        WheelHit hit1;
        WheelHit hit2;
        vehicleController.frontLeftWheel.WheelCollider.GetGroundHit(out hit1);
        vehicleController.frontRightWheel.WheelCollider.GetGroundHit(out hit2);
        TerrainInfo terrainInfo1 = null;
        TerrainInfo terrainInfo2 = null;
        if (vehicleController.frontLeftWheel.WheelCollider.isGrounded)
        {
            terrainInfo1 = hit1.collider.GetComponent<TerrainInfo>();
        }
        if (vehicleController.frontRightWheel.WheelCollider.isGrounded)
        {
            terrainInfo2 = hit2.collider.GetComponent<TerrainInfo>();
        }

        bool vibration = false;
        float vibrationFrequency = 0f;
        float vibrationIntensity = 0f;

        if (terrainInfo1 != null)
        {
            vibration = terrainInfo1.vibration;
            vibrationFrequency = terrainInfo1.vibrationFrequency;
            vibrationIntensity = terrainInfo1.vibrationIntensity;
        }
        if (terrainInfo2 != null)
        {
            if (terrainInfo2.vibration)
            {
                vibration = terrainInfo2.vibration;
                if (terrainInfo2.vibrationFrequency > vibrationFrequency) vibrationFrequency = terrainInfo2.vibrationFrequency;
                if(terrainInfo2.vibrationIntensity > vibrationIntensity) vibrationIntensity = terrainInfo2.vibrationIntensity;
            }
        }

        if (vibration)
        {
            int intensity = Mathf.Clamp((int)(vehicleController.GetSpeed() * vibrationIntensity), 0, 40); // Scale intensity with speed
            float frequency = Mathf.Clamp(vehicleController.GetSpeed() / 5f * vibrationFrequency, 1f, 50f);  // Scale frequency with speed

            SimulateVibration(intensity, frequency);
        }
        else
        {
            StopVibration();
        }


        // Apply centering force
        float slipForce = CalculateSlipForce();
        float centeringForce = centeringForceMultiplier * (vehicleController.GetSpeed() / vehicleController.maxSpeed) * 2.5f / Math.Max(slipForce / 3, 1);
        LogitechGSDK.LogiPlaySpringForce(0, 0, Mathf.Clamp(Mathf.Abs((int)centeringForce), 20, 100), 100);

        LogitechGSDK.LogiPlayDamperForce(0, (int)slipForce);
    }

    private void SimulateVibration(int intensity, float frequency)
    {
        // Calculate the interval between force toggles
        float interval = 1f / (frequency * 2f); // Half-period for toggling force

        // Update the timer
        vibrationTimer += Time.deltaTime;

        if (vibrationTimer >= interval)
        {
            vibrationTimer = 0f; // Reset timer
            vibrationState = !vibrationState; // Toggle vibration state

            // Apply force in alternating directions
            int forceMagnitude = vibrationState ? intensity : -intensity;
            LogitechGSDK.LogiPlayConstantForce(0, forceMagnitude);
        }
    }

    private void StopVibration()
    {
        LogitechGSDK.LogiStopConstantForce(0);
    }

    private float CalculateSlipForce()
    {
        float totalSlip = 0f;

        // Calculate slip from each wheel
        totalSlip += GetWheelSlip(vehicleController.frontLeftWheel.WheelCollider);
        totalSlip += GetWheelSlip(vehicleController.frontRightWheel.WheelCollider);

        // Average slip and apply multiplier
        return (totalSlip / 2f) * slipForceMultiplier;
    }

    private float GetWheelSlip(WheelCollider wheel)
    {
        WheelHit hit;
        if (wheel.isGrounded && wheel.GetGroundHit(out hit))
        {
            return Mathf.Abs(hit.sidewaysSlip);
        }
        return 0f;
    }

    public bool IsConnected()
    {
        return LogitechGSDK.LogiIsConnected(0);
    }

    public float GetSteeringAngle()
    {
        return steeringAngle;
    }

    public void ToggleBumpyRoadEffect()
    {
        if (LogitechGSDK.LogiIsPlaying(0, LogitechGSDK.LOGI_FORCE_BUMPY_ROAD))
        {
            LogitechGSDK.LogiStopBumpyRoadEffect(0);
            activeForceAndEffect[4] = "";
        }
        else
        {
            LogitechGSDK.LogiPlayBumpyRoadEffect(0, 50);
            activeForceAndEffect[4] = "Bumpy Road Effect\n";
        }
    }

    public void ToggleDirtRoadEffect()
    {
        if (LogitechGSDK.LogiIsPlaying(0, LogitechGSDK.LOGI_FORCE_DIRT_ROAD))
        {
            LogitechGSDK.LogiStopDirtRoadEffect(0);
            activeForceAndEffect[3] = "";
        }
        else
        {
            LogitechGSDK.LogiPlayDirtRoadEffect(0, 50);
            activeForceAndEffect[3] = "Dirt Road Effect\n ";
        }
    }

    public void ToggleAirborneEffect()
    {
        if (LogitechGSDK.LogiIsPlaying(0, LogitechGSDK.LOGI_FORCE_CAR_AIRBORNE))
        {
            LogitechGSDK.LogiStopCarAirborne(0);
            activeForceAndEffect[7] = "";
        }
        else
        {
            LogitechGSDK.LogiPlayCarAirborne(0);
            activeForceAndEffect[7] = "Car Airborne\n ";
        }
    }

    public void ToggleSoftStopEffect()
    {
        if (LogitechGSDK.LogiIsPlaying(0, LogitechGSDK.LOGI_FORCE_SOFTSTOP))
        {
            LogitechGSDK.LogiStopSoftstopForce(0);
            activeForceAndEffect[8] = "";
        }
        else
        {
            LogitechGSDK.LogiPlaySoftstopForce(0, 40);
            activeForceAndEffect[8] = "Soft Stop Force\n";
        }
    }

    public void ToggleSlipperyRoadEffect()
    {
        if (LogitechGSDK.LogiIsPlaying(0, LogitechGSDK.LOGI_FORCE_SLIPPERY_ROAD))
        {
            LogitechGSDK.LogiStopSlipperyRoadEffect(0);
            activeForceAndEffect[5] = "";
        }
        else
        {
            LogitechGSDK.LogiPlaySlipperyRoadEffect(0, 50);
            activeForceAndEffect[5] = "Slippery Road Effect\n ";
        }
    }

    public void ToggleSurfaceEffect()
    {
        if (LogitechGSDK.LogiIsPlaying(0, LogitechGSDK.LOGI_FORCE_SURFACE_EFFECT))
        {
            LogitechGSDK.LogiStopSurfaceEffect(0);
            activeForceAndEffect[6] = "";
        }
        else
        {
            LogitechGSDK.LogiPlaySurfaceEffect(0, LogitechGSDK.LOGI_PERIODICTYPE_SQUARE, 50, 1000);
            activeForceAndEffect[6] = "Surface Effect\n";
        }
    }

}
