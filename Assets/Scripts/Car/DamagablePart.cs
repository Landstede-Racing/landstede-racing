using System;
using System.Collections.Generic;
using Unity.Netcode;
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
    public List<GameObject> subParts;
    public GameObject flyingPartPrefab;

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
            if (subParts != null && subParts.Count > 0)
            {
                float damagePercent = currentDamage / maxDamage;
                int subPartsToDestroy = Mathf.FloorToInt(damagePercent * subParts.Count);

                for (int i = 0; i < subPartsToDestroy; i++)
                {
                    if (subParts[i] != null)
                    {
                        DestroySubPart(subParts[i], collision);
                        subParts[i] = null;
                    }
                }
            }
        }
        else if (shouldDestroy) DestroyPart();
    }

    void DestroySubPart(GameObject go, Collision collision)
    {
        var flyingPart = Instantiate(flyingPartPrefab);
        flyingPart.transform.parent = null;
        flyingPart.transform.SetPositionAndRotation(go.transform.position, go.transform.rotation);
        var meshCollider = flyingPart.GetComponent<MeshCollider>();
        meshCollider.sharedMesh = go.GetComponent<SkinnedMeshRenderer>().sharedMesh;
        flyingPart.GetComponent<MeshRenderer>().material = go.GetComponent<SkinnedMeshRenderer>().material;
        flyingPart.GetComponent<MeshFilter>().sharedMesh = go.GetComponent<SkinnedMeshRenderer>().sharedMesh;
        var rb = flyingPart.GetComponent<Rigidbody>();
        rb.linearVelocity = transform.forward * collision.relativeVelocity.magnitude;
        // rb.isKinematic = true;
        Destroy(go);
    }

    void DestroyPart()
    {
        Debug.Log("Part: " + part.name + " is destroyed");
        Destroy(gameObject);
        subParts.ForEach((part) =>
        {
            Destroy(part);
        });
        DestroyPartRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    void DestroyPartRpc()
    {
        Destroy(gameObject);
        subParts.ForEach((part) =>
        {
            Destroy(part);
        });
    }
}
