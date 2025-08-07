using UnityEngine;

public class AeroSurface : MonoBehaviour
{
    private readonly float MAX_RAYCAST_DISTANCE = 50f;

    [SerializeField] private float surfaceWidth = 1f;
    [SerializeField] private float surfaceHeight = 1f;
    [SerializeField] private float angle = 0f;
    [SerializeField] private AnimationCurve angleToCLCurve = new(
        new Keyframe(-10, -0.1f),
        new Keyframe(0, 0.250f),
        new Keyframe(1, 0.284f),
        new Keyframe(2, 0.317f),
        new Keyframe(3, 0.350f),
        new Keyframe(4, 0.383f),
        new Keyframe(5, 0.416f),
        new Keyframe(6, 0.448f),
        new Keyframe(7, 0.481f),
        new Keyframe(8, 0.514f),
        new Keyframe(9, 0.547f),
        new Keyframe(10, 0.580f),
        new Keyframe(11, 0.613f),
        new Keyframe(12, 0.646f),
        new Keyframe(13, 0.678f),
        new Keyframe(14, 0.711f),
        new Keyframe(15, 0.744f),
        new Keyframe(16, 0.777f),
        new Keyframe(17, 0.810f),
        new Keyframe(18, 0.831f),
        new Keyframe(19, 0.851f),
        new Keyframe(20, 0.873f),
        new Keyframe(21, 0.880f),
        new Keyframe(22, 0.886f),
        new Keyframe(23, 0.893f),
        new Keyframe(24, 0.899f),
        new Keyframe(25, 0.873f)
    );
    [SerializeField] private AnimationCurve heightToCLCurve = new(
        new Keyframe(0f, 0f),
        new Keyframe(0.010f, 1.14f),
        new Keyframe(0.040f, 1.12f),
        new Keyframe(0.050f, 1.07f),
        new Keyframe(0.060f, 1.02f),
        new Keyframe(0.070f, 1.00f),
        new Keyframe(0.080f, 0.99f),
        new Keyframe(0.090f, 0.98f),
        new Keyframe(0.120f, 0.95f)
    );
    [SerializeField] private AnimationCurve angleToCDCurve = new(
        new Keyframe(-10, 0.200f),
        new Keyframe(0, 0.020f),
        new Keyframe(1, 0.018f),
        new Keyframe(2, 0.022f),
        new Keyframe(3, 0.023f),
        new Keyframe(4, 0.028f),
        new Keyframe(5, 0.030f),
        new Keyframe(6, 0.032f),
        new Keyframe(7, 0.033f),
        new Keyframe(8, 0.035f),
        new Keyframe(9, 0.038f),
        new Keyframe(10, 0.041f),
        new Keyframe(11, 0.045f),
        new Keyframe(12, 0.048f),
        new Keyframe(13, 0.051f),
        new Keyframe(14, 0.053f),
        new Keyframe(15, 0.056f),
        new Keyframe(16, 0.057f),
        new Keyframe(17, 0.059f),
        new Keyframe(18, 0.068f),
        new Keyframe(19, 0.087f),
        new Keyframe(20, 0.096f),
        new Keyframe(21, 0.105f),
        new Keyframe(22, 0.114f),
        new Keyframe(23, 0.123f),
        new Keyframe(24, 0.132f),
        new Keyframe(25, 0.150f)
    );
    [SerializeField] private AnimationCurve heightToCDCurve = new(
        new Keyframe(0f, 0.970f),
        new Keyframe(0.005f, 0.990f),
        new Keyframe(0.010f, 0.994f),
        new Keyframe(0.020f, 0.998f),
        new Keyframe(0.030f, 1.000f),
        new Keyframe(0.040f, 1.002f),
        new Keyframe(0.050f, 1.004f),
        new Keyframe(0.060f, 1.006f)
    );

    [Header("Overrides")]
    [SerializeField] private float speedOverride;

    private void FixedUpdate()
    {
        float speed = speedOverride != 0 ? speedOverride : 0; // Replace with rigidbody velocity
        float referenceArea = surfaceWidth * surfaceHeight;
        float airDensity = 1.225f;
        float heightFromGround = GetGroundHeight();
        float liftCoefficient = angleToCLCurve.Evaluate(angle) * heightToCLCurve.Evaluate(heightFromGround);
        float dragCoefficient = angleToCDCurve.Evaluate(angle) * heightToCDCurve.Evaluate(heightFromGround);
        float liftForce = -liftCoefficient * (0.5f * airDensity * (speed * speed) * referenceArea);
        float dragForce = 0.5f * dragCoefficient * airDensity * (speed * speed) * referenceArea;

        DrawArrow.ForDebug(transform.position, transform.up * liftForce, Color.green);
        DrawArrow.ForDebug(transform.position, -transform.forward * dragForce, Color.red);
    }

    private float GetGroundHeight()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, MAX_RAYCAST_DISTANCE))
        {
            float heightFromGround = hit.distance;
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
}