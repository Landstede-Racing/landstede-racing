using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public enum GearState
{
    Neutral,
    Running,
    CheckingChange,
    Changing
};

public class VehicleController : MonoBehaviour
{
    public LogitechSteeringWheel logitechSteering;
    public float steeringAngle = 0;
    public float gas = 0;
    public float brake = 0;
    public int gear = 0;
    public float firstLightOn;
    public float redLine;
    public float idleRPM;
    public float wheelRPM;

    public bool drsAvailable;
    public bool drsEnabled;

    public float currentTorque;
    public float brakeTorque = 2000;
    public float maxSpeed = 20;
    public float steeringRange = 30;
    public float steeringRangeAtMaxSpeed = 10;
    public float centreOfGravityOffset = -1f;
    public float currentEngineRPM;
    public float differentialRatio;
    public AnimationCurve hpToRPMCurve;
    private GearState gearState;
    public int isEngineRunning;
    public float changeGearTime = 0.1f;

    public AnimationCurve brakingCurve;
    public AnimationCurve downForceCurve;
    // public ConstantForce downForce;
    public float maxFrontDownForce;
    public float maxRearDownForce;
    public float maxDiffuserDownForce;
    public ConstantForce leftFrontWing;
    public ConstantForce rightFrontWing;
    public ConstantForce rearWing;
    public ConstantForce diffuser;

    [Header("Wheels")]
    public WheelControl frontLeftWheel;
    public WheelControl frontRightWheel;
    public WheelControl backLeftWheel;
    public WheelControl backRightWheel;

    [Header("Force Feedback Settings")]
    public float centeringForceMultiplier = 50f; // Strength of centering force
    public float slipForceMultiplier = 100f;    // Strength of slip feedback


    public float engineHP;
    public float maxEngineRPM;
    public float[] gearRatios;

    public TMP_Text gearText;
    public TMP_Text speedText;
    public TMP_Text rpmText;
    public Transform backWing;

    Animator animator;
    WheelControl[] wheels;
    Rigidbody rigidBody;

    private int currentGear = 1; //Bc: R = 0 and N = 1
    private int maxGear = 9;

    private float vibrationTimer = 0f;
    private bool vibrationState = false;

    public void Start()
    {
        wheels = GetComponentsInChildren<WheelControl>();

        rigidBody = GetComponent<Rigidbody>();

        animator = GetComponent<Animator>();

        // Adjust center of mass vertically, to help prevent the car from rolling
        rigidBody.centerOfMass += Vector3.up * centreOfGravityOffset;

        // rigidBody.linearVelocity = new(0, 0, -83.3333f);
        // drsEnabled = true;
    }

    public void Update()
    {
        if (isEngineRunning == 0) StartCoroutine(GetComponent<EngineAudio>().StartEngine());
        ApplyMotor();
        ApplySteering();
        ApplyBrake();
        ApplyDownForce();
        ApplyForceFeedback();


        // Log gas and brake inputs
        // Debug.Log("Gas: " + gas);
        // Debug.Log("Brake: " + brake);


        // Change gear and speed texts
        if (gear == 0)
        {
            gearText.text = "R";
        }
        else if (gear == 1)
        {
            gearText.text = "N";
        }
        else
        {
            gearText.text = (gear - 1).ToString();
        }

        speedText.text = $"<size=120%>{(int)(Vector3.Dot(transform.forward, rigidBody.linearVelocity) * 3.6)}</size>\n<size=50%>KM/U</size>";




        // Change linear drag (!!TEMPORARY, TO BE CHANGED!!)
        if (drsEnabled)
        {
            rigidBody.linearDamping = 0.07f;
        }
        else
        {
            rigidBody.linearDamping = 0.1f;
        }
    }

    private void ApplyForceFeedback()
    {
        List<float> forces = new();

        foreach (WheelControl item in wheels)
        {
            WheelHit hit;
            WheelCollider wheelCollider = item.WheelCollider;
            wheelCollider.GetGroundHit(out hit);
            if (item.steerable)
            {
                forces.Add(hit.force);
            }
        }

        WheelHit hit1;
        WheelHit hit2;
        frontLeftWheel.WheelCollider.GetGroundHit(out hit1);
        frontRightWheel.WheelCollider.GetGroundHit(out hit2);
        TerrainInfo terrainInfo1 = null;
        TerrainInfo terrainInfo2 = null;
        if (frontLeftWheel.WheelCollider.isGrounded)
        {
            terrainInfo1 = hit1.collider.GetComponent<TerrainInfo>();
        }
        if (frontRightWheel.WheelCollider.isGrounded)
        {
            terrainInfo2 = hit2.collider.GetComponent<TerrainInfo>();
        }

        bool vibration = false;
        float vibrationFrequency = 0f;

        if (terrainInfo1 != null)
        {
            vibration = terrainInfo1.vibration;
            vibrationFrequency = terrainInfo1.vibrationFrequency;
        }
        if (terrainInfo2 != null)
        {
            if (terrainInfo2.vibration)
            {
                vibration = terrainInfo2.vibration;
                if (terrainInfo2.vibrationFrequency > vibrationFrequency) vibrationFrequency = terrainInfo2.vibrationFrequency;
            }
        }

        Debug.Log("Forces average: " + forces.Average());

        if (vibration)
        {
            int intensity = Mathf.Clamp((int)(GetSpeed() * 3f), 0, 20); // Scale intensity with speed
            float frequency = Mathf.Clamp(GetSpeed() / 5f * vibrationFrequency, 1f, 50f);  // Scale frequency with speed

            SimulateVibration(intensity, frequency);
        }
        else
        {
            StopVibration();
        }


        // Apply centering force
        float centeringForce = -steeringAngle * centeringForceMultiplier;
        LogitechGSDK.LogiPlaySpringForce(0, 0, Mathf.Clamp(Mathf.Abs((int)centeringForce), 0, 100), 70);

        // Apply slip feedback based on WheelColliders
        float slipForce = CalculateSlipForce();
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
        totalSlip += GetWheelSlip(frontLeftWheel.WheelCollider);
        totalSlip += GetWheelSlip(frontRightWheel.WheelCollider);

        // Average slip and apply multiplier
        return (totalSlip / 2f) * slipForceMultiplier;
    }

    private float GetWheelSlip(WheelCollider wheel)
    {
        WheelHit hit;
        if (wheel.GetGroundHit(out hit))
        {
            return Mathf.Abs(hit.sidewaysSlip);
        }
        return 0f;
    }

    private void ApplyDownForce()
    {
        // TODO: Change values to newton instead of kg
        float leftFront = CalculateDownForce(maxFrontDownForce);
        leftFrontWing.relativeForce = new(0, -leftFront, 0);
        // Debug.Log("Left Front: " + leftFront);

        float rightFront = CalculateDownForce(maxFrontDownForce);
        rightFrontWing.relativeForce = new(0, -rightFront, 0);
        // Debug.Log("Right Front: " + rightFront);

        float rear = CalculateDownForce(maxRearDownForce);
        rearWing.relativeForce = new(0, -rear, 0);
        // Debug.Log("Rear: " + rear);

        float diff = CalculateDownForce(maxDiffuserDownForce);
        diffuser.relativeForce = new(0, -diff, 0);
        Debug.Log("Diffuser: " + diff);
    }

    private float CalculateDownForce(float max)
    {
        float force = Math.Clamp(downForceCurve.Evaluate((float)(Vector3.Dot(transform.forward, rigidBody.linearVelocity) * 3.6 / maxSpeed)) * max, 0f, max);
        return force;
    }


    // Apply motor forces to wheels
    private void ApplyMotor()
    {
        List<int> rpms = new();
        List<int> frontRpms = new();

        foreach (var wheel in wheels)
        {
            if (wheel.motorized)
            {
                if (gearState == GearState.Running)
                {
                    currentTorque = CalculateMotorTorque();
                    wheel.WheelCollider.motorTorque = currentTorque * gas;
                }
                // Debug.Log("Back RPM: " + wheel.WheelCollider.rpm);
                rpms.Add((int)wheel.WheelCollider.rpm);
            }
            else
            {
                // Debug.Log("Front RPM: " + wheel.WheelCollider.rpm);
                frontRpms.Add((int)wheel.WheelCollider.rpm);
            }
        }

        float averageWheelRPM = Math.Abs((rpms[0] + rpms[1]) / 2);
        wheelRPM = Math.Abs(averageWheelRPM * gearRatios[gear] * differentialRatio);
        Debug.Log("Front RPM: " + (frontRpms[0] + frontRpms[1]) / 2);
        Debug.Log("Back RPM: " + averageWheelRPM);
    }

    // Calculate wheel torque from engine RPM
    float CalculateMotorTorque()
    {
        float torque = 0;
        // if (gearState == GearState.Running && clutch > 0)
        // {
        //     if (RPM > increaseGearRPM)
        //     {
        //         StartCoroutine(ChangeGear(1));
        //     }
        //     else if (RPM < decreaseGearRPM)
        //     {
        //         StartCoroutine(ChangeGear(-1));
        //     }
        // }
        // wheelRPM = Mathf.Abs((colliders.RRWheel.rpm + colliders.RLWheel.rpm) / 2f) * gearRatios[gear] * differentialRatio;
        currentEngineRPM = Mathf.Lerp(currentEngineRPM, Mathf.Max(idleRPM - 100, wheelRPM), Time.deltaTime * 3f);
        rpmText.text = $"<size=120%><align=right>{(int)currentEngineRPM}</align></size>\n<align=right><size=50%>RPM</size></align>";
        torque = hpToRPMCurve.Evaluate((currentEngineRPM - 4500) / (redLine - 4500)) * engineHP / currentEngineRPM * gearRatios[gear] * differentialRatio * 5252f;
        // if (isEngineRunning > 0)
        // {
        //     currentEngineRPM = Mathf.Lerp(currentEngineRPM, Mathf.Max(idleRPM, redLine * gas) + UnityEngine.Random.Range(-50, 50), Time.deltaTime);
        // }
        return torque;
    }

    float CalculateBrakingTorque()
    {
        float torque = brakingCurve.Evaluate(GetSpeed() / maxSpeed) * brakeTorque;

        Debug.Log("Brake Torque: " + torque);

        return torque;
    }


    // Apply steering angles
    private void ApplySteering()
    {
        // Check whether the user input is in the same direction 
        // as the car's velocity
        // Calculate current speed in relation to the forward direction of the car
        // (this returns a negative number when traveling backwards)
        float forwardSpeed = Vector3.Dot(transform.forward, rigidBody.linearVelocity);


        // Calculate how close the car is to top speed
        // as a number from zero to one
        float speedFactor = Mathf.InverseLerp(0, maxSpeed / 3.6f, forwardSpeed);

        // …and to calculate how much to steer 
        // (the car steers more gently at top speed)
        float currentSteerRange = Mathf.Lerp(steeringRange, steeringRangeAtMaxSpeed, speedFactor);

        foreach (var wheel in wheels)
        {
            // Apply steering to Wheel colliders that have "Steerable" enabled
            if (wheel.steerable)
            {
                wheel.WheelCollider.steerAngle = steeringAngle * currentSteerRange;
            }
        }
        // TODO: Fix steering column rotation :')
        // Vector3 steeringColumnRotation = steeringColumn.eulerAngles;
        // steeringColumnRotation.z = Mathf.Lerp(0, 360, (steeringAngle + 1) / 2) - 180;
        // steeringColumn.eulerAngles = steeringColumnRotation;
    }

    // Apply brake
    private void ApplyBrake()
    {
        foreach (var wheel in wheels)
        {
            // TODO: Front Brake Bias
            wheel.WheelCollider.brakeTorque = brake * CalculateBrakingTorque();
        }
    }

    // Coroutine for gear changing
    public IEnumerator ChangeGear(int gearChange)
    {
        int newGear = currentGear + gearChange;
        if (newGear >= 0 && newGear <= maxGear) //Check if newGear isnt above 9 (Gear 8)
        {
            currentGear = newGear;
            gearState = GearState.CheckingChange;
            if (gear + gearChange >= 0)
            {
                gearState = GearState.Changing;
                yield return new WaitForSeconds(changeGearTime);
                gear += gearChange;
            }

            if (gearState != GearState.Neutral)
                gearState = GearState.Running;
        }
    }

    // Speed ratio for engine audio
    public float GetSpeedRatio()
    {
        var gasA = Mathf.Clamp(Mathf.Abs(gas), 0.5f, 1f);
        // return currentEngineRPM * gasA / redLine;
        return currentEngineRPM / redLine;
    }

    public float GetSpeed()
    {
        return Math.Abs(Vector3.Dot(transform.forward, rigidBody.linearVelocity) * 3.6f);
    }

    public void SetSteeringAngle(float steeringAngle)
    {
        this.steeringAngle = steeringAngle;
        // Debug.Log($"SetSteeringAngle called with value: {steeringAngle} yippy!!");
    }

    public void SetGas(float gas)
    {
        this.gas = gas;
        Debug.Log($"SetGas called with value: {gas} yippy");
    }

    public void SetBrake(float brake)
    {
        this.brake = brake;
        // Debug.Log($"Setbrake called with value: {brake} yippy!!!!!!!!");
    }

    public void SetGear(int gear)
    {
        this.gear = gear;
    }

    public int GetGear()
    {
        return gear;
    }

    public void ToggleDRS()
    {
        if (drsAvailable)
        {
            SetDRS(!drsEnabled);
        }
    }

    public void SetDRS(bool enabled)
    {
        drsEnabled = enabled;
        if (enabled)
        {
            animator.Play("DRS_On");
        }
        else
        {
            animator.Play("DRS_Off");
        }
    }

    public bool GetDRS()
    {
        return drsEnabled;
    }
}