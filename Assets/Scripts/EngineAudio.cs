using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class EngineAudio : NetworkBehaviour
{
    public AudioSource runningSound;
    public float runningMaxVolume;
    public float runningMaxPitch;
    public AudioSource reverseSound;
    public float reverseMaxVolume;
    public float reverseMaxPitch;
    public AudioSource idleSound;
    public float idleMaxVolume;
    public float speedRatio;
    public float speedSign;
    public float LimiterSound = 1f;
    public float LimiterFrequency = 3f;
    public float LimiterEngage = 0.8f;
    public bool isEngineRunning;

    public AudioSource startingSound;


    private VehicleController carController;

    private float revLimiter;

    private NetworkVariable<float> m_SpeedRatio = new(0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<float> m_SpeedSign = new(0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<bool> m_IsEngineRunning = new(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public override void OnNetworkSpawn()
    {
        m_SpeedRatio.OnValueChanged += SpeedRatioChanged;
        m_SpeedSign.OnValueChanged += SpeedSignChanged;
        m_IsEngineRunning.OnValueChanged += IsEngineRunningChanged;
        base.OnNetworkSpawn();
    }

    public override void OnNetworkDespawn()
    {
        m_SpeedRatio.OnValueChanged -= SpeedRatioChanged;
        m_SpeedSign.OnValueChanged -= SpeedSignChanged;
        m_IsEngineRunning.OnValueChanged -= IsEngineRunningChanged;
        base.OnNetworkDespawn();
    }


    // Start is called before the first frame update

    private void Start()
    {
        carController = GetComponent<VehicleController>();
        idleSound.volume = 0;
        runningSound.volume = 0;
        reverseSound.volume = 0;
    }

    private void FixedUpdate()
    {
        if (carController && IsOwner)
        {
            m_SpeedSign.Value = Mathf.Sign(carController.GetSpeedRatio());
            m_SpeedRatio.Value = Mathf.Abs(carController.GetSpeedRatio());
        }

        if (speedRatio > LimiterEngage)
            revLimiter = (Mathf.Sin(Time.time * LimiterFrequency) + 1f) * LimiterSound * (speedRatio - LimiterEngage);
        if (isEngineRunning)
        {
            idleSound.volume = Mathf.Lerp(0.1f, idleMaxVolume, speedRatio);
            if (speedSign > 0)
            {
                reverseSound.volume = 0;
                runningSound.volume = Mathf.Lerp(0.3f, runningMaxVolume, speedRatio);
                runningSound.pitch = Mathf.Lerp(0.6f, runningMaxPitch, speedRatio);
            }
            else
            {
                runningSound.volume = 0;
                reverseSound.volume = Mathf.Lerp(0f, reverseMaxVolume, speedRatio);
                reverseSound.pitch = Mathf.Lerp(0.4f, reverseMaxPitch, speedRatio);
            }
        }
        else
        {
            idleSound.volume = 0;
            runningSound.volume = 0;
        }
    }

    public void StartEngine()
    {
        isEngineRunning = true;
        m_IsEngineRunning.Value = true;
        carController.isEngineRunning = 2;
        carController.m_IsEngineRunning.Value = 2;
    }

    private void SpeedRatioChanged(float oldValue, float newValue)
    {
        speedRatio = newValue;
    }

    private void SpeedSignChanged(float oldValue, float newValue)
    {
        speedSign = newValue;
    }

    private void IsEngineRunningChanged(bool oldValue, bool newValue)
    {
        isEngineRunning = newValue;
    }
}