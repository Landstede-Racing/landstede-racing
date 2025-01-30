using System.Collections;
using UnityEngine;

public class EngineAudio : MonoBehaviour
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
    public float LimiterSound = 1f;
    public float LimiterFrequency = 3f;
    public float LimiterEngage = 0.8f;
    public bool isEngineRunning;

    public AudioSource startingSound;


    private VehicleController carController;

    private float revLimiter;

    // Start is called before the first frame update
    private void Start()
    {
        carController = GetComponent<VehicleController>();
        idleSound.volume = 0;
        runningSound.volume = 0;
        reverseSound.volume = 0;
    }

    // Update is called once per frame
    private void Update()
    {
        float speedSign = 0;
        if (carController)
        {
            speedSign = Mathf.Sign(carController.GetSpeedRatio());
            speedRatio = Mathf.Abs(carController.GetSpeedRatio());
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

    public IEnumerator StartEngine()
    {
        startingSound.Play();
        carController.isEngineRunning = 1;
        yield return new WaitForSeconds(0.6f);

        isEngineRunning = true;
        yield return new WaitForSeconds(0.4f);

        carController.isEngineRunning = 2;
    }
}