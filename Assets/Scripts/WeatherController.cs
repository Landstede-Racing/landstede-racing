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

    public GameObject track;

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
        }

        if (track != null)
        {
            Renderer trackRenderer = track.GetComponent<Renderer>();
            if (trackRenderer != null)
            {
                Debug.Log("Track renderer found");
                Material trackMaterial = trackRenderer.material;

                // //TODO: Needs to be an new transparent material that slowly fades in due to rain time, now just overwrites the material or smt
                // Material glossyOverlayMaterial = new Material(Shader.Find("Standard"));
                // glossyOverlayMaterial.SetFloat("_Glossiness", 1.0f);
                // glossyOverlayMaterial.SetFloat("_Metallic", 1.0f);
                // trackRenderer.materials = new Material[] { trackMaterial, glossyOverlayMaterial };
            }
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

    public float GetRainTimer()
    {
        return Math.Min(timeRaining / maxRainTime, 1);
    }
}
