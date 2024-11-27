using UnityEngine;

public class DamageController
{
    public DamagablePart[] DamagableParts;
    private Dictionary<DamagablePart, int> damages = new Dictionary<DamagablePart, int>();

    void Start()
    {
        foreach (GameObject part in DamagableParts)
        {
            DamagablePart damagablePart = part.GetComponent<DamagablePart>();
            damages.Add(damagablePart, 0);
        }
    }

    public int GetDamage(GameObject DamagablePart)
    {
        return damages[part];
    }

    public void SetDamage(GameObject DamagablePart, int damage)
    {
        damages[part] = damage;
    }
}
