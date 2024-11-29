using UnityEngine;
using System.Collections.Generic;

public class Part : ScriptableObject
{
    public string name;
    public GameObject gameObject;
    public Location location;
    public Dictionary<Stat, float> influences = new Dictionary<Stat, float>();
    public string description;
}
