using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DamagablePart : MonoBehaviour
{
    public float maxDamage = 100;
    public float currentDamage;
    public float damageMultiplier = 0.001f;
    public bool shouldDestroy = false;
    public Part part;

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
