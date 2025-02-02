using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VectorGraphics;
using UnityEngine;

public enum Page
{
    Damage,
    Temps
}

public class MfdController : MonoBehaviour
{
    public GameObject optionsMfd;
    public GameObject heatMfd;
    public GameObject damageMfd;
    public DamagablePart[] damageableParts;
    public GameObject car;
    public int currentPage;
    private GameObject activePage;
    public GameObject[] pages;
    public GameObject pagesIndicator;
    public GameObject mainBackground;
    public GameObject pagesBackground;
    private Vector3 pagesBackgroundStartPosition;

    private Dictionary<GameObject, DamagablePart> mfdPartMapDamage = new();
    private Dictionary<GameObject, DamagablePart> mfdPartMapHeat = new();
    [SerializeField] private MFDStepper[] mfdSteppers;
    private bool wasReleased;
    private int selectedStepper = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        damageableParts = car.GetComponentsInChildren<DamagablePart>();
        pagesBackgroundStartPosition = pagesBackground.transform.position;
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

        for (int i = 0; i < heatMfd.transform.childCount; i++)
        {
            GameObject mfdpart = heatMfd.transform.GetChild(i).gameObject;
            DamagablePart damagablePart = Array.Find(damageableParts, dmgblePart => dmgblePart.part.name.Replace(" ", "") == mfdpart.name);

            if (damagablePart != null)
            {
                if(mfdpart.transform.Find("Wheel-Temperature-Outside") == null)
                {
                    Debug.Log("No Wheel-Temperature-Outside found for " + mfdpart.name);
                    continue;
                }
                mfdPartMapHeat.Add(mfdpart.transform.Find("Wheel-Temperature-Outside").gameObject, damagablePart);
            }
            else
            {
                Debug.Log("No DamagablePart found for " + mfdpart.name);
            }
        }

        UpdateUI();
    }

    // Update is called once per frame
    void Update()
    {
        MFDPageIndicator damagePageIndicator = pagesIndicator.transform.GetChild(0).GetComponent<MFDPageIndicator>();
        MFDPageIndicator tempsPageIndicator = pagesIndicator.transform.GetChild(1).GetComponent<MFDPageIndicator>();

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

            if(!damagePageIndicator.warning && !damagePageIndicator.critical && damagePercentage > 50) damagePageIndicator.warning = true;
            if(!damagePageIndicator.critical && damagePercentage > 75) {
                damagePageIndicator.warning = false;
                damagePageIndicator.critical = true;
            }
        }

        foreach(KeyValuePair<GameObject, DamagablePart> entry in mfdPartMapHeat)
        {
            GameObject mfdpart = entry.Key;
            DamagablePart damagablePart = entry.Value;

            int heatPercentage = (int)((int)damagablePart.temperature / damagablePart.optimalTemperature * 50);

            var gradient = new Gradient();
            var colorKeys = new GradientColorKey[4];

            ColorUtility.TryParseHtmlString("#E34141", out Color color1);
            colorKeys[0] = new GradientColorKey(color1, 100);

            ColorUtility.TryParseHtmlString("#FF9D35", out Color color2);
            colorKeys[1] = new GradientColorKey(color2, 75);

            ColorUtility.TryParseHtmlString("#54E341", out Color color3);
            colorKeys[2] = new GradientColorKey(color3, 50);

            ColorUtility.TryParseHtmlString("#41DEE3", out Color color4);
            colorKeys[3] = new GradientColorKey(color4, 0);

            gradient.SetKeys(colorKeys, new GradientAlphaKey[0]);
            mfdpart.GetComponent<SVGImage>().color = gradient.Evaluate(heatPercentage / 100f);  
            
            if(!tempsPageIndicator.warning && !tempsPageIndicator.critical && heatPercentage > 50) tempsPageIndicator.warning = true;
            if(!tempsPageIndicator.critical && heatPercentage > 75) {
                tempsPageIndicator.warning = false;
                tempsPageIndicator.critical = true;
            }
        }

        if(optionsMfd.activeInHierarchy) {
            MFDStepper stepper = mfdSteppers[selectedStepper];
            switch (SettingsController.DeviceController)
            {
                case 1:
                    if (Input.GetKeyDown(KeyCode.LeftArrow)) {
                        if(wasReleased) stepper.Previous();
                        wasReleased = false;
                    }
                    else if (Input.GetKeyDown(KeyCode.RightArrow)) {
                        if(wasReleased) stepper.Next();
                        wasReleased = false;
                    }
                    else if (Input.GetKeyDown(KeyCode.UpArrow)) {
                        if(wasReleased) SelectStepper(selectedStepper - 1);
                        wasReleased = false;
                    }
                    else if (Input.GetKeyDown(KeyCode.DownArrow)) {
                        if(wasReleased) SelectStepper(selectedStepper + 1);
                        wasReleased = false;
                    }
                    else wasReleased = true;
                    break;
                case 2:
                    if(LogitechGSDK.LogiIsConnected(0)) {
                        switch (LogitechGSDK.LogiGetStateUnity(0).rgdwPOV[0])
                        {
                            case 27000:
                                if(wasReleased) stepper.Previous();
                                wasReleased = false;
                                break;
                            case 9000:
                                if(wasReleased) stepper.Next();
                                wasReleased = false;
                                break;
                            case 0:
                                if(wasReleased) SelectStepper(selectedStepper - 1);
                                wasReleased = false;
                                break;
                            case 18000:
                                if(wasReleased) SelectStepper(selectedStepper + 1);
                                wasReleased = false;
                                break;
                            default:
                                wasReleased = true;
                                break;
                        }
                    }
                    break;
            }
        }

        if(Input.GetKeyDown(KeyCode.B)) NextPage();
    }

    private void SelectStepper(int index)
    {
        if(index < 0) index = mfdSteppers.Length - 1;
        if(index >= mfdSteppers.Length) index = 0;
        mfdSteppers[selectedStepper].SetSelected(false);
        selectedStepper = index;
        mfdSteppers[selectedStepper].SetSelected(true);
    }

    public void UpdateIndicators() {
        foreach (Transform child in pagesIndicator.transform)
        {
            if (child.TryGetComponent<MFDPageIndicator>(out var indicator))
            {
                indicator.selected = false;
                indicator.warning = false;
                indicator.critical = false;
            }
        }

        if(currentPage > 0) {
            MFDPageIndicator currentPageIndicator = pagesIndicator.transform.GetChild(currentPage - 1).GetComponent<MFDPageIndicator>();
            currentPageIndicator.selected = true;
        }
    }

    public void UpdateUI() {
        foreach (GameObject page in pages)
        {
            page.SetActive(false);
        }

        if(currentPage > 0) {
            activePage = pages[currentPage - 1];
            activePage.SetActive(true);
        } else activePage = null;

        mainBackground.SetActive(activePage != null);
        pagesBackground.transform.position = pagesBackgroundStartPosition + new Vector3(0, activePage != null ? activePage.GetComponent<RectTransform>().rect.height : 0, 0);

        UpdateIndicators();
    }

    public void NextPage()
    {
        currentPage++;
        if(currentPage > pages.Length) currentPage = 0;

        UpdateUI();
    }
}
