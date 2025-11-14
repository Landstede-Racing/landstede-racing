using System.Collections.Generic;
using UnityEngine;

public class DamageController : MonoBehaviour
{
    public DamageablePart[] damageableParts;
    private readonly Dictionary<DamageablePart, int> damages = new();

    public int GetDamage(DamageablePart part)
    {
        return damages[part];
    }
}