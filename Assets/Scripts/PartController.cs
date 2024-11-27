using UnityEngine;

public class PartController
{
    // <Location, GameObject> Location is the location of the part, GameObject is the part itself
    public Dictionary<Location, Part> parts = new Dictionary<Location, Part>();
    public void SetPart(Location location, Part part)
    {
        if (parts.ContainsKey(location))
        {
            parts.Remove(location);
        }

        parts.Add(location, part);
    }
    public Part GetPart(Location location)
    {
        return parts[location];
    }
    public Location getLocation(GameObject part)
    {
        return parts.FirstOrDefault(x => x.Value == part).Key;
    }
}
