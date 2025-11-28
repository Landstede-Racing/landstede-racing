using System.Linq;
using UnityEngine;

public class Part : MonoBehaviour
{
    public string name;
    public Location location;
    public string description;

    private void Start() {
        if(location == null)
        {
            var locations = Locations.Values.Where((l) => l.name == name.Replace(" ", ""));
            if(locations.Count() > 0)
                location = locations.First();
        }
    }
}