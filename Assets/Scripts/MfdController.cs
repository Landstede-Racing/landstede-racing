using System;
using System.Collections.Generic;
using Unity.VectorGraphics;
using UnityEngine;

public class MfdController : MonoBehaviour
{
    public GameObject heatMfd;
    public GameObject damageMfd;
    public DamagablePart[] damageableParts;
    private Dictionary<GameObject, DamagablePart> mfdPartMapDamage = new();
    private Dictionary<GameObject, DamagablePart> mfdPartMapHeat = new();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < damageMfd.transform.childCount; i++)
        {
            GameObject mfdpart = damageMfd.transform.GetChild(i).gameObject;
            DamagablePart damagablePart = Array.Find(damageableParts, dmgblePart => dmgblePart.part.name.Replace(" ", "") == mfdpart.name);

            if (damagablePart != null)
            {
                mfdPartMapDamage.Add(mfdpart, damagablePart);
            }
            else
            {
                Debug.Log("No DamagablePart found for " + mfdpart.name);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach (KeyValuePair<GameObject, DamagablePart> entry in mfdPartMapDamage)
        {
            GameObject mfdPart = entry.Key;
            DamagablePart damagablePart = entry.Value;

            int damagePercentage = (int)((int)damagablePart.currentDamage / damagablePart.maxDamage * 100);

            var gradient = new Gradient();
            var colorKeys = new GradientColorKey[3];

            ColorUtility.TryParseHtmlString("#E34141", out Color color1);
            colorKeys[0] = new GradientColorKey(color1, 100);

            ColorUtility.TryParseHtmlString("#FF9D35", out Color color2);
            colorKeys[1] = new GradientColorKey(color2, 50);

            ColorUtility.TryParseHtmlString("#54E341", out Color color3);
            colorKeys[2] = new GradientColorKey(color3, 0);

            gradient.SetKeys(colorKeys, new GradientAlphaKey[0]);
            mfdPart.GetComponent<SVGImage>().color = gradient.Evaluate(damagePercentage / 100f);
        }
    }
}
