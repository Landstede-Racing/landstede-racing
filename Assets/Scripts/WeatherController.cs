using UnityEngine;

public class WeatherController : MonoBehaviour
{
    //TODO: Implement in settings or somthing or a randomiser for weather

    //TODO2: Make the car handle differently in different weather conditions

    public GameObject rainFallSystem;
    void Start()
    {
        rainFallSystem.SetActive(false); //Sets the rain system to inactive
                                         // rainFallSystem.SetActive(true);

    }

    // Update is called once per frame
    void Update()
    {

    }
}
