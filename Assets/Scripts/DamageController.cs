using UnityEngine;
using System.Collections.Generic;

public class DamageController
{
    public DamagablePart[] DamagableParts;
    private Dictionary<DamagablePart, int> damages = new Dictionary<DamagablePart, int>();

    void Start()
    {
        foreach (DamagablePart part in DamagableParts)
        {
            damages.Add(part, 0);
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
