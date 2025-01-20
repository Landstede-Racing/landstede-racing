using NUnit.Framework;
using UnityEngine;

public class WeatherController : MonoBehaviour
{

    //TODO: Implement in settings or somthing or a randomiser for weather

    //TODO2: Make the car handle differently in different weather conditions

    public GameObject rainFallSystem;
    public bool isRaining = false;

    void Start()
    {
        rainFallSystem.SetActive(true); //Sets the rain system to inactive
                                        // rainFallSystem.SetActive(true);

    }

    // Update is called once per frame
    void Update()
    {

        if (rainFallSystem == true)
        {
            isRaining = true;
            Debug.Log("It's raining");
        }

    }
}
