using System;
using System.Linq;
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

    public GameObject[] trackGrounds;
    private float matDefaultSmoothess;

    void Start()
    {
        rainFallSystem.SetActive(true); //Sets the rain system to inactive
                                        // rainFallSystem.SetActive(true);
        if(trackGrounds[0] != null && trackGrounds[0].TryGetComponent<MeshRenderer>(out MeshRenderer trackRenderer)) {
            matDefaultSmoothess = trackRenderer.materials[0].GetFloat("_Smoothness");
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (rainFallSystem.activeSelf == true)
        {
            isRaining = true;
        }

        if (trackGrounds.Length > 0)
        {
            foreach (GameObject track in trackGrounds) {
                Renderer trackRenderer = track.GetComponent<MeshRenderer>();
                if (trackRenderer != null)
                {
                    Debug.Log("Track renderer found");
                    foreach (Material mat in trackRenderer.materials)
                    {
                        if(mat.shader == Shader.Find("Shader Graphs/AsphaltShader")) {
                            mat.SetFloat("_Smoothness", matDefaultSmoothess + GetRainTimer());
                        }
                    }
                }
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
