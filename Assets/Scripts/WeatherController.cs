using System;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class WeatherController : MonoBehaviour
{

    //TODO: Implement in settings or somthing or a randomiser for weather

    public GameObject rainFallSystem;
    public bool isRaining = false;
    public float timeRaining = 0;
    public VolumeProfile volumeProfile;

    public float maxRainTime = 60;

    public GameObject[] trackGrounds;
    private float matDefaultSmoothess;

    void Start()
    {
        rainFallSystem.SetActive(false); //Sets the rain system to inactive
                                         // rainFallSystem.SetActive(true);
        if (trackGrounds[0] != null && trackGrounds[0].TryGetComponent<MeshRenderer>(out MeshRenderer trackRenderer))
        {
            matDefaultSmoothess = trackRenderer.materials[0].GetFloat("_Smoothness");
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateClouds();

        isRaining = rainFallSystem.activeSelf;

        if (trackGrounds.Length > 0)
        {
            foreach (GameObject track in trackGrounds)
            {
                Renderer trackRenderer = track.GetComponent<MeshRenderer>();
                if (trackRenderer != null)
                {
                    // Debug.Log("Track renderer found");
                    foreach (Material mat in trackRenderer.materials)
                    {
                        if (mat.shader == Shader.Find("Shader Graphs/NewAsphaltShader"))
                        {
                            mat.SetFloat("_Smoothness", matDefaultSmoothess + GetRainTimer());
                        }
                    }
                }
            }
        }

    }

    public void UpdateClouds() {
        if(volumeProfile.TryGet<VolumetricClouds>(out var clouds)) {
            clouds.cloudPreset = isRaining ? VolumetricClouds.CloudPresets.Overcast : VolumetricClouds.CloudPresets.Sparse;
        }
        if(volumeProfile.TryGet<CloudLayer>(out var cloudLayer)) {
            CloudLayer.CloudMap layer = cloudLayer.layerA;
            layer.opacityA.value = isRaining ? 1 : 0;
            layer.opacityR.value = isRaining ? 0 : 1;
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
