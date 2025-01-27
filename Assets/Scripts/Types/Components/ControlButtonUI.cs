using TMPro;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.UI;

public class ControlButtonUI : MonoBehaviour
{
    public SVGImage image;
    public TMP_Text text;
    public ControllerButton button;
    public Control control;

    void Update()
    {
        UpdateUI();
    }

    void UpdateUI() {
        if(control != null) {
            text.text = control.controlName;
            if(button != null) {
                if(control.controlNumber >= 0) {
                    image.sprite = button.GetImage();
                } else {
                    image.sprite = null;
                    image.color = new Color(0, 0, 0, 0);
                }
            }
        }
    }

    public void SetControl()
    {
        FindFirstObjectByType<SettingsController>().StartListeningForInput(control.controlNumber);
    }
}