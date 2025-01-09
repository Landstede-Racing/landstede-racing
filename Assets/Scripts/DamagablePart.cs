using System;
using UnityEngine;

[RequireComponent(typeof(MeshCollider))]
public class DamagablePart : MonoBehaviour
{
    public float maxDamage = 100;
    public float currentDamage;
    public float damageMultiplier = 0.001f;
    public Part part;

    void OnCollisionEnter(Collision collision)
    {
        if (currentDamage < maxDamage)
        {
            currentDamage += (float)collision.impulse.magnitude * damageMultiplier;
        }
        else
        {
            Debug.Log("Part: " + part.name + " is destroyed");
            Destroy(gameObject);
        }
    }
}
