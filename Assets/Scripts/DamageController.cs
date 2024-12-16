using UnityEngine;
using System.Collections.Generic;

public class DamageController : MonoBehaviour
{
    public DamagablePart[] damagableParts;
    private Dictionary<DamagablePart, int> damages = new();

    void Start()
    {
        foreach (DamagablePart part in damagableParts)
        {
            Debug.Log("Part: " + part.name);
        }
    }

    public int GetDamage(DamagablePart part)
    {
        return damages[part];
    }

    public void SetDamage(DamagablePart part, int damage)
    {
        damages[part] = damage;
    }
}
