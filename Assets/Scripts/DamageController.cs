using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEditor.SearchService;
using UnityEngine.SceneManagement;
using UnityEditor;

public class DamageController : MonoBehaviour
{
    public GameObject textObject;
    public TextMeshProUGUI text;
    public DamagablePart[] damagableParts;
    private Dictionary<DamagablePart, int> damages = new();

    void Start()
    {
        damagableParts = GetComponentsInChildren<DamagablePart>();
        text = textObject.GetComponent<TextMeshProUGUI>();

        foreach (DamagablePart part in damagableParts)
        {
            damages.Add(part, 0);
        }
    }

    void Update()
    {
        text.SetText("Yipeeee");
        Debug.Log("DamageController: ");
        
    }

    public int GetDamage(DamagablePart part)
    {
        return damages[part];
    }

    public void SetDamage(DamagablePart part, int damage)
    {
        damages[part] = damage;
    }
}
