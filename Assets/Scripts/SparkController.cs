using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SparkController : MonoBehaviour
{
    public GameObject sparkPrefab;

    private void OnCollisionEnter(Collision other)
    {
        LogitechGSDK.LogiPlayFrontalCollisionForce(0, 1);
        var go = Instantiate(sparkPrefab, gameObject.transform);
        go.transform.position = other.contacts[0].point;
    }
}