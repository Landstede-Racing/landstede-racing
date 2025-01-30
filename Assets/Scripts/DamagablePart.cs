using UnityEngine;

[RequireComponent(typeof(MeshCollider))]
public class DamagablePart : MonoBehaviour
{
    public float maxDamage = 100;
    public float currentDamage;
    public float damageMultiplier = 0.001f;
    public bool shouldDestroy;
    public Part part;

    private void OnCollisionEnter(Collision collision)
    {
        if (currentDamage < maxDamage)
            currentDamage += collision.impulse.magnitude * damageMultiplier;
        else if (shouldDestroy) DestroyPart();
    }

    private void DestroyPart()
    {
        Debug.Log("Part: " + part.name + " is destroyed");
        Destroy(gameObject);
    }
}