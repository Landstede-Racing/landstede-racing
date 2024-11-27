using System;
using System.Collections;
using System.Collections.Generic;
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

    public AnimationCurve downForceCurve;
    public float currentDownForce;
    public ConstantForce downForce;
    public float maxDownForce;

    public float engineHP;
    public float maxEngineRPM;
    public float[] gearRatios;

    public TMP_Text gearText;
    public TMP_Text speedText;
    public TMP_Text rpmText;

    public Transform backWing;

    WheelControl[] wheels;
    Rigidbody rigidBody;

    public void Start()
    {
        wheels = GetComponentsInChildren<WheelControl>();

        rigidBody = GetComponent<Rigidbody>();

        // Adjust center of mass vertically, to help prevent the car from rolling
        rigidBody.centerOfMass += Vector3.up * centreOfGravityOffset;


    }

    public void Update()
    {
        if (isEngineRunning == 0) StartCoroutine(GetComponent<EngineAudio>().StartEngine());
        ApplyMotor();
        ApplySteering();
        ApplyBrake();


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
        speedText.text = (int)(Vector3.Dot(transform.forward, rigidBody.linearVelocity) * 3.6) + " KM/U";


        // Change linear drag (!!TEMPORARY, TO BE CHANGED!!)
        if (drsEnabled)
        {
            rigidBody.linearDamping = 0.07f;
            Vector3 backWingRotation = backWing.eulerAngles;
            backWingRotation.x = -11;
            backWing.eulerAngles = backWingRotation;
        }
        else
        {
            rigidBody.linearDamping = 0.1f;
            Vector3 backWingRotation = backWing.eulerAngles;
            backWingRotation.x = 20;
            backWing.eulerAngles = backWingRotation;
        }

        // Calculate current downforce from current speed divided by maxSpeed, times maxDownForce, and clamp it.
        currentDownForce = Math.Clamp(downForceCurve.Evaluate((float)(Vector3.Dot(transform.forward, rigidBody.linearVelocity) * 3.6 / maxSpeed)) * maxDownForce, 0f, maxDownForce);

        // Apply the calculated downforce to the Constant Force component on the car.
        downForce.relativeForce = new(0, currentDownForce, 0);
    }


    // Apply motor forces to wheels
    private void ApplyMotor()
    {
        List<int> rpms = new();

        foreach (var wheel in wheels)
        {
            if (wheel.motorized)
            {
                if (gearState == GearState.Running)
                {
                    currentTorque = CalculateTorque();
                    wheel.WheelCollider.motorTorque = currentTorque * gas;
                }
                // Debug.Log("Back RPM: " + wheel.WheelCollider.rpm);
                rpms.Add((int)wheel.WheelCollider.rpm);
            }
            else
            {
                // Debug.Log("Front RPM: " + wheel.WheelCollider.rpm);
            }
        }

        float averageWheelRPM = Math.Abs((rpms[0] + rpms[1]) / 2);
        wheelRPM = Math.Abs(averageWheelRPM * gearRatios[gear] * differentialRatio);
    }

    // Calculate wheel torque from engine RPM
    float CalculateTorque()
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
        rpmText.text = currentEngineRPM + " RPM";
        torque = hpToRPMCurve.Evaluate(currentEngineRPM / redLine) * engineHP / currentEngineRPM * gearRatios[gear] * differentialRatio * 5252f;
        // if (isEngineRunning > 0)
        // {
        //     currentEngineRPM = Mathf.Lerp(currentEngineRPM, Mathf.Max(idleRPM, redLine * gas) + UnityEngine.Random.Range(-50, 50), Time.deltaTime);
        // }
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
            wheel.WheelCollider.brakeTorque = brake * brakeTorque;
        }
    }

    // Coroutine for gear changing
    public IEnumerator ChangeGear(int gearChange)
    {
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

    // Speed ratio for engine audio
    public float GetSpeedRatio()
    {
        var gasA = Mathf.Clamp(Mathf.Abs(gas), 0.5f, 1f);
        // return currentEngineRPM * gasA / redLine;
        return currentEngineRPM / redLine;
    }

    public void SetSteeringAngle(float steeringAngle)
    {
        this.steeringAngle = steeringAngle;
        // Debug.Log($"SetSteeringAngle called with value: {steeringAngle} yippy!!");
    }

    public void SetGas(float gas)
    {
        this.gas = gas;
        // Debug.Log($"SetGas called with value: {gas} yippy");
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
    }

    public bool GetDRS()
    {
        return drsEnabled;
    }
}