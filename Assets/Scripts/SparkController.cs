using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SparkController : MonoBehaviour
{
    private VehicleController vehicleController;
    public GameObject sparkPrefab;

    void OnCollisionEnter(Collision other)
    {
        LogitechGSDK.LogiPlayFrontalCollisionForce(0, 1);
        GameObject go = GameObject.Instantiate(sparkPrefab, gameObject.transform);
        go.transform.position = other.contacts[0].point;
    }
}