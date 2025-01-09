using UnityEngine;
using System.Collections.Generic;

public class DamageController : MonoBehaviour
{
    public DamagablePart[] damagableParts;
    private Dictionary<DamagablePart, int> damages = new();

    public int GetDamage(DamagablePart part)
    {
        return damages[part];
    }
}
