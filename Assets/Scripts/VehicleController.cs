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

    public float engineHP;
    public float maxEngineRPM;
    public float[] gearRatios;

    public TMP_Text gearText;
    public TMP_Text gearTextWheel;
    public TMP_Text speedText;
    public TMP_Text rpmText;
    public TMP_Text rpmTextWheel;
    public Transform backWing;

    Animator animator;
    WheelControl[] wheels;
    Rigidbody rigidBody;

    private int currentGear = 1; //Bc: R = 0 and N = 1
    private int maxGear = 9;

    public void Start()
    {
        wheels = GetComponentsInChildren<WheelControl>();

        rigidBody = GetComponent<Rigidbody>();

        animator = GetComponent<Animator>();

        // Adjust center of mass vertically, to help prevent the car from rolling
        rigidBody.centerOfMass += Vector3.up * centreOfGravityOffset;
    }

    public void FixedUpdate()
    {
        if (isEngineRunning == 0) StartCoroutine(GetComponent<EngineAudio>().StartEngine());
        ApplyMotor();
        ApplySteering();
        ApplyBrake();
        ApplyDownForce();

        // Change gear and speed texts
        if (gear == 0)
        {
            gearText.text = "R";
            gearTextWheel.text = "R";
        }
        else if (gear == 1)
        {
            gearText.text = "N";
            gearTextWheel.text = "N";
        }
        else
        {
            gearText.text = (gear - 1).ToString();
            gearTextWheel.text = (gear - 1).ToString();
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

    private void ApplyDownForce()
    {
        // TODO: Change values to newton instead of kg
        float leftFront = CalculateDownForce(maxFrontDownForce);
        leftFrontWing.relativeForce = new(0, -leftFront, 0);

        float rightFront = CalculateDownForce(maxFrontDownForce);
        rightFrontWing.relativeForce = new(0, -rightFront, 0);

        float rear = CalculateDownForce(maxRearDownForce);
        rearWing.relativeForce = new(0, -rear, 0);

        float diff = CalculateDownForce(maxDiffuserDownForce);
        diffuser.relativeForce = new(0, -diff, 0);
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
                rpms.Add((int)wheel.WheelCollider.rpm);
            }
            else
            {
                frontRpms.Add((int)wheel.WheelCollider.rpm);
            }
        }

        float averageWheelRPM = Math.Abs((rpms[0] + rpms[1]) / 2);
        wheelRPM = Math.Abs(averageWheelRPM * gearRatios[gear] * differentialRatio);
    }

    // Calculate wheel torque from engine RPM
    float CalculateMotorTorque()
    {
        float torque = 0;
        
        currentEngineRPM = Mathf.Lerp(currentEngineRPM, Mathf.Max(idleRPM - 100, wheelRPM), Time.deltaTime * 3f);
        string rpmTextValue = $"<size=120%><align=right>{(int)currentEngineRPM}</align></size>\n<align=right><size=50%>RPM</size></align>";
        rpmText.text = rpmTextValue;
        rpmTextWheel.text = rpmTextValue;


        torque = hpToRPMCurve.Evaluate((currentEngineRPM - 4500) / (redLine - 4500)) * engineHP / currentEngineRPM * gearRatios[gear] * differentialRatio * 5252f;
        
        return torque;
    }

    float CalculateBrakingTorque()
    {
        float torque = brakingCurve.Evaluate(GetSpeed() / maxSpeed) * brakeTorque;

        return torque;
    }


    // Apply steering angles
    private void ApplySteering()
    {
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

        return currentEngineRPM / redLine;
    }

    public float GetSpeed()
    {
        return Math.Abs(Vector3.Dot(transform.forward, rigidBody.linearVelocity) * 3.6f);
    }

    public void SetSteeringAngle(float steeringAngle)
    {
        this.steeringAngle = steeringAngle;
    }

    public void SetGas(float gas)
    {
        this.gas = gas;
    }

    public void SetBrake(float brake)
    {
        this.brake = brake;
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

    void OnCollisionEnter(Collision other)
    {
        float x = transform.InverseTransformPoint(other.gameObject.transform.position).x;

        float force = other.impulse.magnitude / 50;

        if(x < 0) {
            LogitechGSDK.LogiPlaySideCollisionForce(0, (int)-force);
        } else if(x > 0) {
            LogitechGSDK.LogiPlaySideCollisionForce(0, (int)force);
        } else {
            LogitechGSDK.LogiPlayFrontalCollisionForce(0, (int)force);
        }
    }
}