using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeMenuController : MonoBehaviour
{
    public GameObject upgradeScreen;
    private static ToggleGroup toggleGroup;
    private UpgradeCategory currentCategory;
    private List<Upgrade> upgrades;
    public static ScrollRect scrollRect;
    private Upgrade selectedUpgrade;
    public TMP_Text upgradeName;
    public TMP_Text upgradeDescription;
    public TMP_Text upgradeCost;
    public Button buyButton;
    public Button closeButton;
    private int points;
    public TMP_Text pointsText;
    public GameObject upgradeButtonPrefab;
    void Start()
    {
        // Set points to 15000 for testing
        // PlayerPrefs.SetInt("Points", 15000);

        scrollRect = GetComponentInChildren<ScrollRect>();
        toggleGroup = GetComponentInChildren<ToggleGroup>();
        Debug.Log(toggleGroup.ActiveToggles().FirstOrDefault().name);
        closeButton.onClick.AddListener(CloseUpgradeScreen);
        buyButton.onClick.AddListener(BuyUpgrade);
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

    public void BuyUpgrade() {
        if(selectedUpgrade != null) {
            if(UpgradeController.BuyUpgrade(selectedUpgrade)) {
                UpdateUpgrades();
                CloseUpgradeScreen();
            }
        }
    }

    public void UpdateUI() {
        points = PlayerPrefs.GetInt("Points", 0);
        pointsText.text = points.ToString();
        if(scrollRect.content.childCount > 0) {
            for (int i = 0; i < scrollRect.content.childCount; i++)
            {
                Destroy(scrollRect.content.GetChild(i).gameObject);
            }
        }

        int j = 0;
        upgrades.ForEach(upgrade => {
            GameObject upgradeButton = Instantiate(upgradeButtonPrefab, scrollRect.content);
            UpgradeButton button = upgradeButton.GetComponent<UpgradeButton>();
            button.upgrade = upgrade;
            button.upgradeMenuController = this;
            RectTransform rt = upgradeButton.GetComponent<RectTransform>();
            Vector3 pos = rt.localPosition;
            pos.x += rt.rect.width * j;
            rt.localPosition = pos;
            j++;
        });

        upgradeScreen.SetActive(selectedUpgrade != null);
        if(selectedUpgrade != null) {
            upgradeName.text = selectedUpgrade.name;
            upgradeDescription.text = selectedUpgrade.description;
            upgradeCost.text = selectedUpgrade.cost.ToString();
        }
    }

    public void SelectUpgrade(Upgrade upgrade) {
        selectedUpgrade = upgrade;
        UpdateUI();
    }

    public void CloseUpgradeScreen() {
        SelectUpgrade(null);
    }
}