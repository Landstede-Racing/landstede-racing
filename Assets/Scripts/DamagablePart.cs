using System;
using UnityEngine;

[RequireComponent(typeof(MeshCollider))]
public class DamagablePart : MonoBehaviour
{
    public int maxDamage = 250;
    public int currentDamage;
    public Part part;

    void OnCollisionEnter(Collision collision)
    { 
        currentDamage += (int)collision.impulse.magnitude;
        Debug.Log("-----------------------------");
        Debug.Log("Part: " + part.name);
        Debug.Log("currentDamage: " + currentDamage);
        Debug.Log("velocity: " + collision.impulse.magnitude);
        Debug.Log("-----------------------------");
        
        if (currentDamage >= maxDamage)
        {
            Debug.Log("Part: " + part.name + " is destroyed");
            Destroy(gameObject);
        }
    }
}
