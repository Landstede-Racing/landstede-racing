using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.VectorGraphics;
using UnityEngine;

public enum Page
{
    Damage,
    Temps
}

public class MfdController : NetworkBehaviour
{
    public GameObject optionsMfd;
    public GameObject pitMfd;
    public GameObject heatMfd;
    public GameObject damageMfd;
    public DamageablePart[] damageableParts;
    public GameObject car;
    public int currentPage;
    private GameObject activePage;
    public GameObject[] pages;
    public GameObject pagesIndicator;
    public GameObject mainBackground;
    public GameObject pagesBackground;
    private Vector3 pagesBackgroundStartPosition;

    private Dictionary<Location, float> locationDamageMap = new();
    private Dictionary<MFDPart, Location> mfdPartMapDamage = new();
    private Dictionary<GameObject, DamageablePart> mfdPartMapHeat = new();
    private bool wasReleased;
    private int selectedStepper = 0;

    private bool init;
    private bool initStarted;

    public override void OnNetworkSpawn()
    {
        if(IsOwner)
        {
            EventService.PartDamaged += OnPartDamaged;
        }

        base.OnNetworkSpawn();
    }

    public override void OnNetworkDespawn()
    {
        if(IsOwner)
        {
            EventService.PartDamaged -= OnPartDamaged;
        }

        base.OnNetworkDespawn();
    }

    IEnumerator Init()
    {
        if (!init && !initStarted)
        {
            initStarted = true;
            yield return new WaitForFixedUpdate();
            damageableParts = car.GetComponentsInChildren<DamageablePart>();
            pagesBackgroundStartPosition = pagesBackground.transform.position;
            for (int i = 0; i < damageMfd.transform.childCount; i++)
            {
                GameObject mfdpart = damageMfd.transform.GetChild(i).gameObject;
                MFDPart part = mfdpart.GetComponent<MFDPart>();
                
                if(!part.ShouldUpdate()) continue;

                IEnumerable<Location> locations = Locations.Values.Where((l) => l.name == mfdpart.name);

                if(locations.Count() <= 0)
                {
                    if(DebugManager.Instance.ShouldDebugCar())
                        CustomLogger.Log("No Location found for " + mfdpart.name);
                    continue;
                }

                if(DebugManager.Instance.ShouldDebugCar())
                        CustomLogger.Log("Found Location for " + mfdpart.name);

                Location location = locations.First();

                if (location != null)
                {
                    mfdPartMapDamage.Add(part, location);
                    // #54E341
                    mfdpart.GetComponent<SVGImage>().color = new(84, 227, 65);
                }
                else
                {
                    if(DebugManager.Instance.ShouldDebugCar())
                        CustomLogger.Log("No Location found for " + mfdpart.name);
                }   
            }

            for (int i = 0; i < heatMfd.transform.childCount; i++)
            {
                GameObject mfdpart = heatMfd.transform.GetChild(i).gameObject;
                DamageablePart damageablePart = Array.Find(damageableParts, dmgblePart => dmgblePart.part.name.Replace(" ", "") == mfdpart.name);

                if (damageablePart != null)
                {
                    if (mfdpart.transform.Find("Wheel-Temperature-Outside") == null)
                    {
                        if(DebugManager.Instance.ShouldDebugCar())
                            CustomLogger.Log("No Wheel-Temperature-Outside found for " + mfdpart.name);
                        continue;
                    }
                    mfdPartMapHeat.Add(mfdpart.transform.Find("Wheel-Temperature-Outside").gameObject, damageablePart);
                }
                else
                {
                    CustomLogger.Log("No DamageablePart found for " + mfdpart.name);
                }
            }

            var compoundStepper = pitMfd.transform.GetChild(0).GetComponent<MFDStepper>();
            compoundStepper.options = TireCompounds.Values.Select(compound => compound.name).ToArray();
            UpdateUI();
            init = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!init && !initStarted)
        {
            StartCoroutine(Init());
        }
        else
        {
            MFDPageIndicator damagePageIndicator = pagesIndicator.transform.GetChild(2).GetComponent<MFDPageIndicator>();
            MFDPageIndicator tempsPageIndicator = pagesIndicator.transform.GetChild(3).GetComponent<MFDPageIndicator>();

            foreach (KeyValuePair<MFDPart, Location> entry in mfdPartMapDamage)
            {
                GameObject mfdPart = entry.Key.gameObject;
                Location location = entry.Value;

                var percentages = locationDamageMap.Where((l) => l.Key == location);

                var damagePercentage = 0f;

                if(percentages.Count() > 0)
                    damagePercentage = percentages.First().Value;

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

                if (!damagePageIndicator.warning && !damagePageIndicator.critical && damagePercentage > 50) damagePageIndicator.warning = true;
                if (!damagePageIndicator.critical && damagePercentage > 75)
                {
                    damagePageIndicator.warning = false;
                    damagePageIndicator.critical = true;
                }
            }

            foreach (KeyValuePair<GameObject, DamageablePart> entry in mfdPartMapHeat)
            {
                GameObject mfdpart = entry.Key;
                DamageablePart damageablePart = entry.Value;

                int heatPercentage = (int)((int)damageablePart.temperature / damageablePart.optimalTemperature * 50);

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

                if (!tempsPageIndicator.warning && !tempsPageIndicator.critical && heatPercentage > 50) tempsPageIndicator.warning = true;
                if (!tempsPageIndicator.critical && heatPercentage > 75)
                {
                    tempsPageIndicator.warning = false;
                    tempsPageIndicator.critical = true;
                }
            }

            if (activePage != null)
            {
                var page = pages[currentPage - 1].GetComponent<MFDPage>();
                if (page.mfdSteppers.Length > 0)
                {
                    MFDStepper[] mfdSteppers = page.mfdSteppers;
                    MFDStepper stepper = mfdSteppers[selectedStepper];
                    switch (SettingsController.DeviceController)
                    {
                        case 1:
                            if (Input.GetKeyDown(KeyCode.LeftArrow))
                            {
                                if (wasReleased) stepper.Previous();
                                wasReleased = false;
                            }
                            else if (Input.GetKeyDown(KeyCode.RightArrow))
                            {
                                if (wasReleased) stepper.Next();
                                wasReleased = false;
                            }
                            else if (Input.GetKeyDown(KeyCode.UpArrow))
                            {
                                if (wasReleased) SelectStepper(selectedStepper - 1);
                                wasReleased = false;
                            }
                            else if (Input.GetKeyDown(KeyCode.DownArrow))
                            {
                                if (wasReleased) SelectStepper(selectedStepper + 1);
                                wasReleased = false;
                            }
                            else wasReleased = true;
                            break;
                        case 2:
                            if (LogitechGSDK.LogiIsConnected(0))
                            {
                                switch (LogitechGSDK.LogiGetStateUnity(0).rgdwPOV[0])
                                {
                                    case 27000:
                                        if (wasReleased) stepper.Previous();
                                        wasReleased = false;
                                        break;
                                    case 9000:
                                        if (wasReleased) stepper.Next();
                                        wasReleased = false;
                                        break;
                                    case 0:
                                        if (wasReleased) SelectStepper(selectedStepper - 1);
                                        wasReleased = false;
                                        break;
                                    case 18000:
                                        if (wasReleased) SelectStepper(selectedStepper + 1);
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
            }

            if (Input.GetKeyDown(KeyCode.B)) NextPage();
        }
    }

    private void SelectStepper(int index)
    {
        if (activePage == null) return;
        MFDStepper[] mfdSteppers = activePage.GetComponent<MFDPage>().mfdSteppers;
        if (index < 0) index = mfdSteppers.Length - 1;
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
            activePage = pages[currentPage - 1].gameObject;
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
        selectedStepper = 0;

        UpdateUI();
    }

    void OnPartDamaged(Location location, float maxDamage, float currentDamage)
    {
        if(DebugManager.Instance.ShouldDebugCar())
            CustomLogger.Log($"OnPartDamaged called for {location.name}, with maxDamage {maxDamage} and currentDamage {currentDamage}");
        
        locationDamageMap[location] = (int)((int)currentDamage / maxDamage * 100);
    }
}
