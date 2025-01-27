using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UpgradeButton : MonoBehaviour
{
    public Upgrade upgrade;
    private Button button;
    public UpgradeMenuController upgradeMenuController;
    public TMP_Text nameText;
    public TMP_Text costText;
    public Color affordableColor;
    public Color unaffordableColor;
    private bool init = false;
    private void Start()
    {
        if(upgrade != null) {
            Init();
        }
    }

    void Update()
    {
        if(upgrade != null) {
            if(!init) Init();
            button.interactable = !upgrade.unlocked;
        }
    }

    void Init() {
        init = true;
        button = GetComponent<Button>();
        nameText.text = upgrade.name;
        if(upgrade.unlocked) {
            costText.text = "Unlocked";
            costText.color = Color.white;
        } else {
            costText.text = upgrade.cost.ToString();
            costText.color = upgrade.cost <= PlayerPrefs.GetInt("Points", 0) ? affordableColor : unaffordableColor;
        }
        button.onClick.AddListener(OnButtonClicked);
    }

    private void OnButtonClicked()
    {
        upgradeMenuController.SelectUpgrade(upgrade);
    }
}