using System;
using UnityEngine;
using UnityEngine.Rendering;

public class WheelControl : MonoBehaviour
{
    public Transform wheelModel;

    public Part part;
    public DamagablePart damagablePart;
    public TireCompound tireCompound;

    public WheelFrictionCurve defaultForwardFriction;
    public WheelFrictionCurve defaultSidewaysFriction;

    public GameObject track;

    [HideInInspector] public WheelCollider WheelCollider;

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
    }

    // Update is called once per frame

    void Update()

    {
        weatherController = FindObjectOfType<WeatherController>();
        // Get the Wheel collider's world pose values and
        // use them to set the wheel model's position and rotation
        WheelCollider.GetWorldPose(out position, out rotation);
        wheelModel.transform.position = position;
        wheelModel.transform.rotation = rotation;

        wheelModel.GetComponent<MeshRenderer>().materials[1].SetFloat("_Wear", damagablePart.currentDamage / damagablePart.maxDamage);

        // var matsCopy = wheelModel.GetComponent<MeshRenderer>().materials;
        // matsCopy[1].SetFloat("_Wear", damagablePart.currentDamage / damagablePart.maxDamage);
        // wheelModel.GetComponent<MeshRenderer>().materials = matsCopy;

        // 
        // Calculations for the damage percentage :)
        // 
        // Debug.Log(Math.Floor(damagablePart.currentDamage / damagablePart.maxDamage * 100));
    }

    void FixedUpdate()
    {
        // Damage from driving (get force from ground hit, and calculate damage using that)
        if (WheelCollider.isGrounded)
        {
            WheelCollider.GetGroundHit(out WheelHit hit);
            TerrainInfo hitTerrain = hit.collider.GetComponent<TerrainInfo>();

            if (damagablePart.currentDamage < damagablePart.maxDamage && hit.force > 1400)
            {
                if (hit.collider.CompareTag("Wall"))
                {
                    damagablePart.currentDamage += (hit.force - 1400) * damagablePart.damageMultiplier * 10;

                    if (damagablePart.currentDamage >= damagablePart.maxDamage)
                    {
                        Debug.Log("Here it will fly to the moon");
                    }
                }
                else if (hitTerrain != null)
                {
                    damagablePart.currentDamage += (hit.force - 1400) * damagablePart.damageMultiplier * hitTerrain.damageMultiplier * tireCompound.wearRate;

                    if (damagablePart.currentDamage >= damagablePart.maxDamage)
                    {
                        Debug.Log("Here it will break in a less horrible way than the others");
                    }
                }
            }




            if (hitTerrain != null)
            {
                WheelFrictionCurve newForwardFriction = defaultForwardFriction;
                WheelFrictionCurve newSidewaysFriction = defaultSidewaysFriction;

                if (weatherController != null && weatherController.isRaining)
                {
                    // Debug.Log("It's raining from weatherController and is now changed in the wheelControl!!! YIPPPYYYYY");
                    float rainTime = weatherController.GetRainTimer();

                    // Adjust the friction based on the rainTime
                    newForwardFriction.stiffness *= Mathf.Lerp(1.0f, 0.75f, rainTime); //reduced by 25% after 1 min
                    newSidewaysFriction.stiffness *= Mathf.Lerp(1.0f, 0.55f, rainTime); //reduced by 45% after 1 min
                }
                else
                {
                    newForwardFriction.stiffness *= hitTerrain.gripMultiplier;
                    newForwardFriction.stiffness *= tireCompound.grip;
                    newSidewaysFriction.stiffness *= hitTerrain.gripMultiplier;
                    newSidewaysFriction.stiffness *= tireCompound.grip;
                }

                WheelCollider.forwardFriction = newForwardFriction;
                WheelCollider.sidewaysFriction = newSidewaysFriction;
            }
        }
    }

    public void SetTireCompound(TireCompound tireCompound)
    {
        this.tireCompound = tireCompound;
        wheelModel.GetComponent<MeshRenderer>().materials[0].SetColor("_Tire_Color", tireCompound.color);
    }
}