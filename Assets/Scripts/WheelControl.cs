using UnityEngine;

public class WheelControl : MonoBehaviour
{
    public Transform wheelModel;
    private DamagablePart damagablePart;

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
        damagablePart = GetComponent<DamagablePart>();
    }

    // Update is called once per frame
    void Update()
    {
        // Get the Wheel collider's world pose values and
        // use them to set the wheel model's position and rotation
        WheelCollider.GetWorldPose(out position, out rotation);
        wheelModel.transform.position = position;
        wheelModel.transform.rotation = rotation;

        // Damage from driving (get force from ground hit, and calculate damage using that)
        if(WheelCollider.isGrounded) {
            WheelCollider.GetGroundHit(out WheelHit hit);

            // damagablePart.currentDamage += (int)collision.impulse.magnitude;
            // Debug.Log("-----------------------------");
            // Debug.Log("Part: " + damagablePart.part.name);
            // Debug.Log("currentDamage: " + damagablePart.currentDamage);
            // Debug.Log("velocity: " + collision.impulse.magnitude);
            // Debug.Log("-----------------------------");
            
            if (damagablePart.currentDamage >= damagablePart.maxDamage)
            {
                Debug.Log("Part: " + damagablePart.part.name + " is destroyed");
                Destroy(gameObject);
            }
        }
    }
}