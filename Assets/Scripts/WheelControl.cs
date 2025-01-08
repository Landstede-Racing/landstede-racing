using UnityEngine;
using UnityEngine.Rendering;

public class WheelControl : MonoBehaviour
{
    public Transform wheelModel;

    public Part part;
    public int maxDamage = 250;
    public int currentDamage;


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

        // Damage from driving (get force from ground hit, and calculate damage using that)
        if (WheelCollider.isGrounded)
        {
            WheelCollider.GetGroundHit(out WheelHit hit);

            int force = (int)hit.force;
            if (force > 0)
            {
                Debug.Log("-----------------------------");
                Debug.Log("Part: " + part.name);
                Debug.Log("currentDamage: " + currentDamage);
                Debug.Log("force: " + force);
                Debug.Log("-----------------------------");
            }

            if (currentDamage >= maxDamage)
            {
                Debug.Log("Part: " + part.name + " is destroyed");
                Destroy(gameObject);
            }
        }
    }
}