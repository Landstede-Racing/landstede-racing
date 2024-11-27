using UnityEngine;

public class DamageController : PartController
{
    public GameObject[] DamagableParts;
    private Dictionary<DamagablePart, int> damages = new Dictionary<DamagablePart, int>();
    
    void Start()
    {
        foreach (GameObject part in DamagableParts)
        {
            DamagablePart damagablePart = part.GetComponent<DamagablePart>();
            damages.Add(damagablePart, 0);
        }
    }

    public int GetDamage(DamagablePart part)
    {
        return damages[part];
    }

    public void SetDamageablePart(DamagablePart part, int damage)
    {
        damages[part] = damage;
    }
}
