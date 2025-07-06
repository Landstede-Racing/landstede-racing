using System;
using UnityEngine;

[RequireComponent(typeof(MeshCollider))]
public class DamagablePart : MonoBehaviour
{
    public float maxDamage = 100;
    public float currentDamage;
    public float damageMultiplier = 0.001f;

    public float temperature;
    public float optimalTemperature;
    public float temperatureMultiplier = 0.001f;
    public float coollingRate = 0.001f;

    public bool shouldDestroy = false;
    public Part part;

    void Start()
    {
        // if (part.name == "Rear Left Wheel" || part.name == "Rear Right Wheel" || part.name == "Front Left Wheel" || part.name == "Front Right Wheel")
        // {
        //     TireCompound tireCompound = gameObject.GetComponent<WheelControl>().tireCompound;
        //     optimalTemperature = tireCompound.optimalTemperature;
        // }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (currentDamage < maxDamage)
        {
            currentDamage += (float)collision.impulse.magnitude * damageMultiplier;
        }
        else if (shouldDestroy) DestroyPart();

    }
    void DestroyPart()
    {
        Debug.Log("Part: " + part.name + " is destroyed");
        Destroy(gameObject);
    }
}
