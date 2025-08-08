using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(ConstantForce))]
public class AeroSurface : MonoBehaviour
{
    private readonly float MAX_RAYCAST_DISTANCE = 50f;

    [SerializeField] private float surfaceWidth = 1f;
    [SerializeField] private float surfaceHeight = 1f;
    [SerializeField] private float angle = 0f;
    [SerializeField] private ParentTransform parentTransform;

    [Header("Aerodynamic Coefficients")]
    [SerializeField] private float CLGain = 1f;
    [SerializeField]
    private AnimationCurve angleToCLCurve = new(
        new Keyframe(-10, -0.2f),
        new Keyframe(0, 0f),
        new Keyframe(2, 0f),
        new Keyframe(5, 0f),
        new Keyframe(8, 0f),
        new Keyframe(10, 0.2f)
    );
    [SerializeField]
    private AnimationCurve heightToCLCurve = new(
        new Keyframe(0f, 1f),
        new Keyframe(1f, 1f)
    );
    [SerializeField] private float CDGain = 1f;
    [SerializeField]
    private AnimationCurve angleToCDCurve = new(
        new Keyframe(-10, 1f),
        new Keyframe(-2, 0.48f),
        new Keyframe(2, 0.488f),
        new Keyframe(3, 0.49f),
        new Keyframe(4, 0.50f),
        new Keyframe(5, 0.51f),
        new Keyframe(8, 0.59f),
        new Keyframe(10, 0.68f),
        new Keyframe(12, 0.69f),
        new Keyframe(14, 0.71f),
        new Keyframe(16, 0.73f),
        new Keyframe(20, 0.75f),
        new Keyframe(24, 0.94f),
        new Keyframe(26, 0.96f),
        new Keyframe(30, 1f)
    );
    [SerializeField]
    private AnimationCurve heightToCDCurve = new(
        new Keyframe(0f, 1f),
        new Keyframe(1f, 1f)
    );

    [Header("Overrides")]
    [SerializeField] private float speedOverride;

    private Rigidbody rb;
    private ConstantForce constantForce;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        constantForce = GetComponent<ConstantForce>();
    }

    private void FixedUpdate()
    {
        if (parentTransform.transform != null)
        {
            switch (parentTransform.axis)
            {
                case ParentTransform.Axis.X:
                    angle = parentTransform.transform.localEulerAngles.x;
                    break;
                case ParentTransform.Axis.Y:
                    angle = parentTransform.transform.localEulerAngles.y;
                    break;
                case ParentTransform.Axis.Z:
                    angle = parentTransform.transform.localEulerAngles.z;
                    break;
                default: break;
            }
        }

        float speed = speedOverride != 0 ? speedOverride : Vector3.Dot(rb.linearVelocity, transform.forward);
        float referenceArea = surfaceWidth * surfaceHeight;
        float airDensity = 1.225f;
        float heightFromGround = GetGroundHeight();
        float liftCoefficient = angleToCLCurve.Evaluate(angle) * heightToCLCurve.Evaluate(heightFromGround) * CLGain;
        float dragCoefficient = angleToCDCurve.Evaluate(angle) * heightToCDCurve.Evaluate(heightFromGround) * CDGain;
        float liftForce = -liftCoefficient * (0.5f * airDensity * (speed * speed) * referenceArea);
        float dragForce = 0.5f * -dragCoefficient * airDensity * (speed * speed) * referenceArea;

        constantForce.relativeForce = new(0, liftForce, dragForce);

        Debug.Log(gameObject.name + ": " + constantForce.relativeForce);


#if UNITY_EDITOR
        DrawArrow.ForDebug(transform.position, transform.up * liftForce, Color.green);
        DrawArrow.ForDebug(transform.position, transform.forward * dragForce, Color.red);
#endif
    }

    private float GetGroundHeight()
    {
        RaycastHit hit = new();
        bool foundGround = false;
        RaycastHit[] hitArray = Physics.RaycastAll(transform.position, Vector3.down, MAX_RAYCAST_DISTANCE);
        foreach (RaycastHit possibleHit in hitArray)
        {
            if (possibleHit.transform.CompareTag("Ground"))
            {
                hit = possibleHit;
                foundGround = true;
                break;
            }
        }
        
        if (foundGround)
        {
            float heightFromGround = hit.distance;
#if UNITY_EDITOR
            Debug.DrawRay(transform.position, Vector3.down * heightFromGround, Color.yellow);
#endif
            return heightFromGround;
        }

        return float.MaxValue;
    }

    private void OnDrawGizmos()
    {
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation * Quaternion.Euler(90 + angle, 0, 0), Vector3.one);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(Vector3.zero, new(surfaceWidth, surfaceHeight));
    }

    [Serializable]
    private class ParentTransform
    {
        public Transform transform;
        public Axis axis;

        public enum Axis
        {
            X,
            Y,
            Z
        }
    }
}