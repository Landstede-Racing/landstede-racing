using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeMenuController : MonoBehaviour
{
    private static ToggleGroup toggleGroup;
    private UpgradeCategory currentCategory;
    private List<Upgrade> upgrades;
    public static ScrollRect scrollRect;
    public GameObject upgradeButtonPrefab;
    void Start()
    {
        scrollRect = GetComponentInChildren<ScrollRect>();
        toggleGroup = GetComponentInChildren<ToggleGroup>();
        Debug.Log(toggleGroup.ActiveToggles().FirstOrDefault().name);
        UpdateUpgrades();
    }

    void Update()
    {
        Toggle activeToggle = toggleGroup.ActiveToggles().FirstOrDefault();
        if (activeToggle != null)
        {
            UpgradeCategory category = activeToggle.GetComponent<UpgradeToggle>().upgradeCategory;
            if (category != currentCategory)
            {
                currentCategory = category;
                Debug.Log("Category changed to: " + category);
                UpdateUpgrades();
            }
        }
    }

    public void UpdateUpgrades() {
        List<Upgrade> newUpgrades = Upgrade.GetUpgrades(currentCategory);
        List<Upgrade> unlockedUpgrades = UpgradeController.GetUnlockedUpgrades();
        newUpgrades.ForEach(upgrade => {
            if(unlockedUpgrades.Contains(upgrade)) {
                upgrade.unlocked = true;
            }
        });
        upgrades = newUpgrades;
        UpdateUI();
    }

    public void UpdateUI() {
        if(scrollRect.content.childCount > 0) {
            for (int i = 0; i < scrollRect.content.childCount; i++)
            {
                Destroy(scrollRect.content.GetChild(i).gameObject);
            }
        }

        upgrades.ForEach(upgrade => {
            GameObject upgradeButton = Instantiate(upgradeButtonPrefab, scrollRect.content);
            upgradeButton.GetComponent<UpgradeButton>().upgrade = upgrade;
        });
    }
}