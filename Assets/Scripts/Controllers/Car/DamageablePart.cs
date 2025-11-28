using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(MeshCollider))]
public class DamageablePart : NetworkBehaviour
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
        var newDamage = currentDamage + (float)collision.impulse.magnitude * damageMultiplier;
        currentDamage = Math.Min(newDamage, maxDamage);
        EventService.InvokePartDamaged(part.location, maxDamage, currentDamage);
        if (currentDamage < maxDamage)
        {
            if (subParts != null && subParts.Count > 0)
            {
                float damagePercent = currentDamage / maxDamage;
                int subPartsToDestroy = Mathf.FloorToInt(damagePercent * subParts.Count);

                for (int i = 0; i < subPartsToDestroy; i++)
                {
                    if(i < subParts.Count)
                    {
                        DestroySubPart(subParts[i], collision);   
                    }
                }
            }
        }
        else if (shouldDestroy) DestroyPart(collision);
    }

    void DestroySubPart(GameObject go, Collision collision)
    {
        if (go.activeInHierarchy)
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
            go.SetActive(false);
        }
    }

    void DestroyPart(Collision collision)
    {
        if(DebugManager.Instance.ShouldDebugCar())
            CustomLogger.Log("Part: " + part.name + " is destroyed");
        subParts.ForEach((part) =>
        {
            DestroySubPart(part, collision);
        });
        DestroySubPart(gameObject, collision);
        DestroyPartRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    void DestroyPartRpc()
    {
        subParts.ForEach((part) =>
        {
            part.SetActive(false);
        });
        gameObject.SetActive(false);
    }

    public void RepairPart()
    {
        currentDamage = 0;
        gameObject.SetActive(true);
        subParts.ForEach((part) =>
        {
            part.SetActive(true);
        });
        RepairPartRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    void RepairPartRpc()
    {
        gameObject.SetActive(true);
        subParts.ForEach((part) =>
        {
            part.SetActive(true);
        });
    }
}
