using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DamagablePart : MonoBehaviour
{
    public int maxDamage = 250;
    public int currentDamage;
    public Part part;

    void OnCollisionEnter(Collision collision)
    { 
        currentDamage += (int)collision.relativeVelocity.magnitude;
        Debug.Log("-----------------------------");
        Debug.Log("Part: " + part.name);
        Debug.Log("currentDamage: " + currentDamage);
        Debug.Log("velocity: " + collision.relativeVelocity.magnitude);
        Debug.Log("-----------------------------");
        
        if (currentDamage >= maxDamage)
        {
            Debug.Log("Part: " + part.name + " is destroyed");
            Destroy(gameObject);
        }
    }
}
