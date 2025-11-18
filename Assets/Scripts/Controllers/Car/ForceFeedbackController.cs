using Unity.Netcode;
using UnityEngine;

public class ForceFeedbackController : NetworkBehaviour
{
    public VehicleController vehicleController;

    [Header("Self-Aligning Torque Settings")]
    [Tooltip("The speed (km/h) at which the SAT effect begins to apply.")]
    public float minSpeedForSat = 80.0f;

    [Tooltip("The speed (km/h) at which the SAT effect reaches its maximum strength.")]
    public float maxSpeedForSat = 300.0f;

    [Tooltip("The maximum stiffness (0-100) of the spring force at max speed (before slip reduction).")]
    [Range(0, 100)]
    public float maxStiffness = 90.0f;

    [Header("Tuning")]
    [Range(0, 2)]
    public float centeringForceMultiplier = 1.0f;
    [Range(0, 100)]
    public float slipForceMultiplier = 10.0f;
    public float understeerSlipThreshold = 0.8f; 
    public float terrainVibrationIntensityScale = 40f;
    public float terrainVibrationFrequencyScale = 1.0f;
    public int minDamper = 10;
    public int maxForceValue = 100;

    private float lastSteerAngle = 0f;
    private float lastUpdateTime = 0f;

    private string ffbDebugText = "";

    void OnEnable()
    {
        if(!IsOwner) return;
        lastSteerAngle = GetSteerAngleSafe();
        lastUpdateTime = Time.time;
    }

    void OnDisable()
    {
        if(!IsOwner) return;
        StopAllForces();
    }

    void Update()
    {
        if(!IsOwner) return;

        ApplyForceFeedback();
    }

    private void OnGUI() {
        if(DebugManager.Instance.ShouldDebugFFB())
        {
            GUI.TextArea(new Rect(200, 10, 300, 500), ffbDebugText);
        }
    }

    public void ApplyForceFeedback()
    {
        if(vehicleController == null) return;
        float currentSpeedKmh = vehicleController.GetSpeed();
        float speedFactor = vehicleController.maxSpeed > 0f
            ? Mathf.Clamp01(currentSpeedKmh / vehicleController.maxSpeed)
            : 0f;

        float steerAngle = GetSteerAngleSafe();

        float now = Time.time;
        float dt = Mathf.Max(1e-6f, now - lastUpdateTime);
        float steerVelocity = (steerAngle - lastSteerAngle) / dt;
        lastSteerAngle = steerAngle;
        lastUpdateTime = now;

        bool vibration = false;
        float terrainVibrationFrequency = 0f;
        float terrainVibrationIntensity = 0f;

        WheelHit hit;
        TerrainInfo tiLeft = null, tiRight = null;

        if(vehicleController.frontLeftWheel.WheelCollider.isGrounded &&
            vehicleController.frontLeftWheel.WheelCollider.GetGroundHit(out hit))
        {
            tiLeft = hit.collider ? hit.collider.GetComponent<TerrainInfo>() : null;
            if (tiLeft != null && tiLeft.vibration)
            {
                vibration = true;
                terrainVibrationFrequency = Mathf.Max(terrainVibrationFrequency, tiLeft.vibrationFrequency);
                terrainVibrationIntensity = Mathf.Max(terrainVibrationIntensity, tiLeft.vibrationIntensity);
            }
        }

        if (vehicleController.frontRightWheel.WheelCollider.isGrounded &&
            vehicleController.frontRightWheel.WheelCollider.GetGroundHit(out hit))
        {
            tiRight = hit.collider ? hit.collider.GetComponent<TerrainInfo>() : null;
            if (tiRight != null && tiRight.vibration)
            {
                vibration = true;
                terrainVibrationFrequency = Mathf.Max(terrainVibrationFrequency, tiRight.vibrationFrequency);
                terrainVibrationIntensity = Mathf.Max(terrainVibrationIntensity, tiRight.vibrationIntensity);
            }
        }

        float slipLeft = GetWheelSidewaysSlip(vehicleController.frontLeftWheel.WheelCollider);
        float slipRight = GetWheelSidewaysSlip(vehicleController.frontRightWheel.WheelCollider);
        float avgSlip = (slipLeft + slipRight) * 0.5f;

        float forwardSlipLeft = GetWheelForwardSlip(vehicleController.frontLeftWheel.WheelCollider);
        float forwardSlipRight = GetWheelForwardSlip(vehicleController.frontRightWheel.WheelCollider);
        float avgForwardSlip = (forwardSlipLeft + forwardSlipRight) * 0.5f;

        float springSpeedFactor = Mathf.InverseLerp(minSpeedForSat, maxSpeedForSat, currentSpeedKmh);

        float stiffness = springSpeedFactor * maxStiffness;

        if (avgForwardSlip > understeerSlipThreshold)
        {
            stiffness *= 0.1f; // Reduce stiffness by 90%
        }

        float slipReduction = avgSlip * slipForceMultiplier;
        stiffness = Mathf.Max(0, stiffness - slipReduction);

        stiffness *= centeringForceMultiplier;

        int coefficientPercentage = (int)Mathf.Clamp(stiffness, 0, 100);

        LogitechGSDK.LogiPlaySpringForce(0, 0, 100, coefficientPercentage);

        int damper = Mathf.Clamp((int)(Mathf.Abs(steerVelocity) * 2.5f), minDamper, maxForceValue);
        LogitechGSDK.LogiPlayDamperForce(0, damper);

        LogitechGSDK.LogiPlaySoftstopForce(0, 40);

        float slipRumbleIntensity = Mathf.Clamp(avgSlip * slipForceMultiplier * 5f, 0f, maxForceValue);
        float slipRumbleFrequency = Mathf.Lerp(8f, 40f, speedFactor); // low at low speed, higher at high speed

        float slipVibrationValue = 0f;
        if (slipRumbleIntensity > 1f)
        {
            slipVibrationValue = Mathf.Sin(Time.time * slipRumbleFrequency * 2f * Mathf.PI) * slipRumbleIntensity;
        }

        float terrainVibrationValue = 0f;
        if (vibration)
        {
            float vibIntensity = terrainVibrationIntensity * terrainVibrationIntensityScale * speedFactor;
            vibIntensity = Mathf.Clamp(vibIntensity, 0f, maxForceValue);

            float vibFrequency = Mathf.Max(0.5f, terrainVibrationFrequency * (1f + speedFactor * terrainVibrationFrequencyScale));

            if (vibIntensity > 1f)
            {
                terrainVibrationValue = Mathf.Sin(Time.time * vibFrequency * 2f * Mathf.PI) * vibIntensity;
            }
        }

        float combinedVibrationValue = slipVibrationValue + terrainVibrationValue;

        int finalVibrationForce = Mathf.Clamp((int)combinedVibrationValue, -maxForceValue, maxForceValue);
        LogitechGSDK.LogiPlayConstantForce(0, finalVibrationForce);

        if(DebugManager.Instance.ShouldDebugFFB())
        {
            ffbDebugText = $"FFB Debug\nSpeed: {currentSpeedKmh:F1}  SpeedFactor: {speedFactor:F2}\n" +
                           $"SteerAngle: {steerAngle:F2}  SteerVel: {steerVelocity:F2}\n\n" +
                           $"Spring \"Stiffness?\": {stiffness}\n" +
                           $"Coefficient Percentage: {coefficientPercentage}\n\n" +
                           $"Slip L:{slipLeft:F3} R:{slipRight:F3} Avg:{avgSlip:F3}\n" +
                           $"SlipRumbleIntensity: {slipRumbleIntensity:F1} SlipFreq: {slipRumbleFrequency:F1}\n" +
                           $"TerrainVib: {terrainVibrationIntensity:F2} TerrainFreq: {terrainVibrationFrequency:F2}\n\n" +
                           $"Damper: {damper}\n" +
                           $"Effects: Constant={LogitechGSDK.LogiIsPlaying(0, LogitechGSDK.LOGI_FORCE_CONSTANT)}\n" +
                           $"Spring={LogitechGSDK.LogiIsPlaying(0, LogitechGSDK.LOGI_FORCE_SPRING)}\n" +
                           $"Damper={LogitechGSDK.LogiIsPlaying(0, LogitechGSDK.LOGI_FORCE_DAMPER)}\n" +
                           $"SoftStop={LogitechGSDK.LogiIsPlaying(0, LogitechGSDK.LOGI_FORCE_SOFTSTOP)}\n";
        }
    }

    private void StopAllForces()
    {
        LogitechGSDK.LogiStopConstantForce(0);
        LogitechGSDK.LogiStopSpringForce(0);
        LogitechGSDK.LogiStopDamperForce(0);
        LogitechGSDK.LogiStopSoftstopForce(0);
    }

    private float GetSteerAngleSafe()
    {
        if (vehicleController == null) return 0f;

        return vehicleController.steeringAngle;
    }

    private float GetWheelForwardSlip(WheelCollider wheel)
    {
        if (wheel == null) return 0f;
        if (wheel.isGrounded && wheel.GetGroundHit(out WheelHit hit))
        {
            return Mathf.Abs(hit.forwardSlip);
        }
        return 0f;
    }

    private float GetWheelSidewaysSlip(WheelCollider wheel)
    {
        if (wheel == null) return 0f;
        WheelHit hit;
        if (wheel.isGrounded && wheel.GetGroundHit(out hit))
        {
            return Mathf.Abs(hit.sidewaysSlip);
        }
        return 0f;
    }
}