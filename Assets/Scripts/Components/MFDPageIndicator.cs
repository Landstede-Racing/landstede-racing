using UnityEngine;
using UnityEngine.UI;

public class MFDPageIndicator : MonoBehaviour
{
    public Graphic selectedIndicator;
    public Graphic unselectedIndicator;
    
    public Color warningColor;
    public Color criticalColor;

    public bool selected;
    public bool warning;
    public bool critical;

    void Update()
    {
        selectedIndicator.gameObject.SetActive(selected);
        unselectedIndicator.gameObject.SetActive(!selected);

        if (warning)
        {
            selectedIndicator.color = warningColor;
            unselectedIndicator.color = warningColor;
        }
        else if (critical)
        {
            selectedIndicator.color = criticalColor;
            unselectedIndicator.color = criticalColor;
        }
        else
        {
            selectedIndicator.color = Color.white;
            unselectedIndicator.color = Color.white;
        }
    }   
}