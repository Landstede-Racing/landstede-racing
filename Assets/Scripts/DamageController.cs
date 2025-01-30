using System.Collections.Generic;
using UnityEngine;

public class DamageController : MonoBehaviour
{
    public DamagablePart[] damagableParts;
    private readonly Dictionary<DamagablePart, int> damages = new();

    public int GetDamage(DamagablePart part)
    {
        return damages[part];
    }
}