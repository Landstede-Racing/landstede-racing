using UnityEngine;

public class PartController
{
    // <String, GameObject> String is the name of the part, GameObject is the part itself
    public Dictionary<Location, Part> parts = new Dictionary<Location, Part>();
    void Start()
    {

    }
    public Part GetPart(location)
    {
        return parts[location];
    }
    public Location getLocation(part)
    {
        return parts[part];
    }
}
