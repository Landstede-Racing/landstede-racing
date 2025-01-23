using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UpgradeButton : MonoBehaviour
{
    public Upgrade upgrade;
    private Button button;
    public UpgradeMenuController upgradeMenuController;
    private TMP_Text buttonText;
    private bool init = false;
    private void Start()
    {
        if(upgrade != null) {
            button = GetComponent<Button>();
            buttonText = GetComponentInChildren<TMP_Text>();
            buttonText.text = upgrade.name;
            button.onClick.AddListener(OnButtonClicked);
        }
    }

    void Update()
    {
        if(!init && upgrade != null) {
            init = true;
            button = GetComponent<Button>();
            buttonText = GetComponentInChildren<TMP_Text>();
            buttonText.text = upgrade.name;
            button.onClick.AddListener(OnButtonClicked);
        }
    }

    private void OnButtonClicked()
    {
        Debug.Log("Button clicked: " + upgrade.name);
        // TODO: Upgrade buy logic
        upgradeMenuController.SelectUpgrade(upgrade);
    }
}