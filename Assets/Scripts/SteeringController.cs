using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringController : MonoBehaviour
{
    public LogitechSteeringWheel steeringWheel;
    public GameObject coob;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Quaternion rot = coob.transform.rotation;
        rot.z = steeringWheel.GetSteeringAngle();
        coob.transform.rotation = rot;
    }
}
