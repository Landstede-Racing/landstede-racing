using System;
using NUnit.Framework;
using UnityEngine;

public class WeatherController : MonoBehaviour
{

    //TODO: Implement in settings or somthing or a randomiser for weather

    //TODO2: Make the car handle differently in different weather conditions

    public GameObject rainFallSystem;
    public bool isRaining = false;
    public float timeRaining = 0;

    public float maxRainTime = 60;

    void Start()
    {
        rainFallSystem.SetActive(true); //Sets the rain system to inactive
                                        // rainFallSystem.SetActive(true);
                            

    }

    // Update is called once per frame
    void Update()
    {

        if (rainFallSystem.activeSelf == true)
        {
            isRaining = true;
            Debug.Log("It's raining");
        }

    }

    void FixedUpdate()
    {
        if (isRaining)
        {
            timeRaining += 0.02f;
            // Debug.Log("Time raining: " + timeRaining);
        }
    }

    public float GetRainTimer() {
        return Math.Min(timeRaining / maxRainTime, 1);
    }
}
