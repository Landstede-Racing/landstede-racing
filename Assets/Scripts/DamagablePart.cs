using System;
using UnityEngine;

// It's also a scriptable object since part also is a scriptable object
public class DamagablePart : MonoBehaviour
{
    public String name;
    public Location location;
    public int maxDamage;
    public int currentDamage;
    public Part part;
}
