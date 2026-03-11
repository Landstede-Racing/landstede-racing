using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering;

public class WheelControl : NetworkBehaviour
{
    public Transform wheelModel;

    public Part part;
    public DamageablePart damageablePart;
    public TireCompound tireCompound;

    public WheelFrictionCurve defaultForwardFriction;
    public WheelFrictionCurve defaultSidewaysFriction;

    public GameObject track;

    [HideInInspector] public WheelCollider WheelCollider;

    public float currentTireWear;
    public float maxTireWear = 250;
    [SerializeField] private float wearMultiplier = 0.001f;
    [SerializeField] private float slipWearMultiplier = 0.01f;

    // Create properties for the CarControl script
    // (You should enable/disable these via the 
    // Editor Inspector window)
    public bool steerable;
    public bool motorized;

    Vector3 position;
    Quaternion rotation;

    private WeatherController weatherController; // Add this field

    // Start is called before the first frame update
    private void Start()
    {
        WheelCollider = GetComponent<WheelCollider>();
        defaultForwardFriction = WheelCollider.forwardFriction;
        defaultSidewaysFriction = WheelCollider.sidewaysFriction;
        weatherController = FindFirstObjectByType<WeatherController>();
    }

    // Update is called once per frame

    void Update()
    {
        // Get the Wheel collider's world pose values and
        // use them to set the wheel model's position and rotation
        WheelCollider.GetWorldPose(out position, out rotation);
        wheelModel.transform.position = position;
        wheelModel.transform.rotation = rotation;

        wheelModel.GetComponent<MeshRenderer>().materials[1].SetFloat("_Wear", currentTireWear / maxTireWear);

        // var matsCopy = wheelModel.GetComponent<MeshRenderer>().materials;
        // matsCopy[1].SetFloat("_Wear", damageablePart.currentDamage / damageablePart.maxDamage);
        // wheelModel.GetComponent<MeshRenderer>().materials = matsCopy;
    }

    void FixedUpdate()
    {
        // Damage from driving (get force from ground hit, and calculate damage using that)
        if (WheelCollider.isGrounded)
        {
            WheelCollider.GetGroundHit(out WheelHit hit);
            TerrainInfo hitTerrain = hit.collider.GetComponent<TerrainInfo>();

            HandleWheelDamage(hit, hitTerrain);
            // HandleWheelTemperature(hit);

            if (hitTerrain != null)
            {
                HandleWheelFriction(hitTerrain);
            }
        }
    }

    // public void HandleWheelTemperature(WheelHit hit)
    // {
    //     damageablePart.temperature += (hit.force - 1400) * damageablePart.temperatureMultiplier;

    //     if (damageablePart.temperature > damageablePart.optimalTemperature + 10)
    //     {
    //         damageablePart.currentDamage += (damageablePart.temperature - damageablePart.optimalTemperature) * damageablePart.damageMultiplier * 10000000;
    //     }

    //     if (damageablePart.temperature > 0f)
    //     {
    //         damageablePart.temperature -= damageablePart.temperature * damageablePart.coollingRate * damageablePart.temperatureMultiplier;
    //     }
    // }

    public void HandleWheelDamage(WheelHit hit, TerrainInfo hitTerrain)
    {
        if (hitTerrain != null && (hit.sidewaysSlip > 1 || hit.forwardSlip > 1) && hit.force > 1400)
        {
            var newWear = currentTireWear;
            if(hit.force > 1400)
            {
                newWear += (hit.force - 1400) * wearMultiplier * hitTerrain.damageMultiplier * tireCompound.wearRate;   
            }
            newWear += Math.Max(hit.sidewaysSlip, hit.forwardSlip) * slipWearMultiplier * hitTerrain.damageMultiplier * tireCompound.wearRate;
            SetWear(newWear);
        }

        if(hit.force > 3000)
        {
            damageablePart.currentDamage += (hit.force - 1400) * damageablePart.damageMultiplier * 10;

            if (damageablePart.currentDamage >= damageablePart.maxDamage)
            {
                CustomLogger.Log("Here it will fly to the moon");
            }
        }
    }

    public void HandleWheelFriction(TerrainInfo hitTerrain)
    {
        WheelFrictionCurve newForwardFriction = defaultForwardFriction;
        WheelFrictionCurve newSidewaysFriction = defaultSidewaysFriction;

        var wear = 1 - currentTireWear / maxTireWear * 0.25f;

        newForwardFriction.stiffness *= hitTerrain.gripMultiplier;
        newForwardFriction.stiffness *= tireCompound.grip * wear;
        newSidewaysFriction.stiffness *= hitTerrain.gripMultiplier;
        newSidewaysFriction.stiffness *= tireCompound.grip * wear;

        if (weatherController != null && weatherController.isRaining)
        {
            // CustomLogger.Log("It's raining from weatherController and is now changed in the wheelControl!!! YIPPPYYYYY");
            float rainTime = weatherController.GetRainTimer();

            // Adjust the friction based on the rainTime
            newForwardFriction.stiffness *= Mathf.Lerp(1.0f, 0.75f, rainTime); //reduced by 25% after 1 min
            newSidewaysFriction.stiffness *= Mathf.Lerp(1.0f, 0.55f, rainTime); //reduced by 45% after 1 min
        }

        WheelCollider.forwardFriction = newForwardFriction;
        WheelCollider.sidewaysFriction = newSidewaysFriction;
    }

    public void SetTireCompound(TireCompound tireCompound)
    {
        this.tireCompound = tireCompound;
        wheelModel.GetComponent<MeshRenderer>().materials[0].SetColor("_Tire_Color", tireCompound.color);
        damageablePart.currentDamage = 0;
        damageablePart.temperature = 0; //TODO: Set to default temperature
        SetWear(0);
    }

    private void SetWear(float newWear)
    {
        currentTireWear = Math.Min(newWear, maxTireWear);
        
        if(part.location != null)
        {
            EventService.InvokePartDamaged(part.location, maxTireWear, currentTireWear);
        }
    }
}