using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;


public class Part : MonoBehaviour
{
    public string name;
    public Location location;
    public string description;

    void Start()
    {
        Physics.IgnoreCollision(GetComponent<Collider>(), GetComponentInParent<Collider>());

        String[] ignoreColliders = { "Plane.001", "Curbs.001", "Curbs.002", "Curbs.003" };
        foreach (string ignoreCollider in ignoreColliders)
        {
            Physics.IgnoreCollision(GetComponent<Collider>(), GameObject.Find(ignoreCollider).GetComponent<MeshCollider>());
        }

        WheelCollider[] wheelColliders = transform.parent.GetComponentsInChildren<WheelCollider>();
        foreach (WheelCollider wheelCollider in wheelColliders)
        {
            Physics.IgnoreCollision(GetComponent<Collider>(), wheelCollider.GetComponent<WheelCollider>());
            Physics.IgnoreCollision(GetComponent<Collider>(), wheelCollider.GetComponent<MeshCollider>());
        }

        MeshCollider[] meshColliders = transform.parent.GetComponentsInChildren<MeshCollider>();
        foreach (MeshCollider meshCollider in meshColliders)
        {
            Physics.IgnoreCollision(GetComponent<Collider>(), meshCollider);
        }
    }
}
