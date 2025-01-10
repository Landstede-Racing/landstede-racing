using System;
using UnityEngine;
using UnityEngine.Rendering;

public class WheelControl : MonoBehaviour
{
    public Transform wheelModel;

    public Part part;
    public DamagablePart damagablePart;

    [HideInInspector] public WheelCollider WheelCollider;

    // Create properties for the CarControl script
    // (You should enable/disable these via the 
    // Editor Inspector window)
    public bool steerable;
    public bool motorized;

    Vector3 position;
    Quaternion rotation;

    // Start is called before the first frame update
    private void Start()
    {
        WheelCollider = GetComponent<WheelCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        // Get the Wheel collider's world pose values and
        // use them to set the wheel model's position and rotation
        WheelCollider.GetWorldPose(out position, out rotation);
        wheelModel.transform.position = position;
        wheelModel.transform.rotation = rotation;

        // 
        // Calculations for the damage percentage :)
        // 
        // Debug.Log(Math.Floor(damagablePart.currentDamage / damagablePart.maxDamage * 100));
    }

    void FixedUpdate() {
        // Damage from driving (get force from ground hit, and calculate damage using that)
        if (WheelCollider.isGrounded)
        {
            WheelCollider.GetGroundHit(out WheelHit hit);

            if (damagablePart.currentDamage < damagablePart.maxDamage && hit.force > 1400)
            {
                if (hit.collider.CompareTag("Ground"))
                {
                    damagablePart.currentDamage += (hit.force - 1400) * damagablePart.damageMultiplier;

                    if (damagablePart.currentDamage >= damagablePart.maxDamage)
                    {
                        Debug.Log("Here it will break in a less horrible way than the others");
                    }
                }
                else if (hit.collider.CompareTag("Curb"))
                {
                    damagablePart.currentDamage += (hit.force - 1400) * damagablePart.damageMultiplier * 3;

                    if (damagablePart.currentDamage >= damagablePart.maxDamage)
                    {
                        Debug.Log("Here it will fly to china");
                    }
                }
                else if (hit.collider.CompareTag("Wall"))
                {
                    damagablePart.currentDamage += (hit.force - 1400) * damagablePart.damageMultiplier * 10;

                    if (damagablePart.currentDamage >= damagablePart.maxDamage)
                    {
                        Debug.Log("Here it will fly to the moon");
                    }
                }
            }
        }
    }
}