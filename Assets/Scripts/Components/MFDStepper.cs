using TMPro;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.UI;

public enum MFDStepperType
{
    PERCENTAGE,
    INT,
    OPTIONS
}

public class MFDStepper : MonoBehaviour
{
    public MFDStepperType stepperType;
    public string[] options;
    public int intMax;
    public int value;
    [SerializeField] private SVGImage leftArrow;
    [SerializeField] private SVGImage rightArrow;
    [SerializeField] private Image background;
    [SerializeField] private TMP_Text optionText;
    public bool selected;

    [SerializeField] private Color arrowColor = new(1, 1, 1, 1);
    [SerializeField] private Color arrowDisabledColor = new(1, 1, 1, 0.4f);
    [SerializeField] private Color selectedColor = new(0.31f, 0.31f, 0.31f, 0.7f);
    [SerializeField] private Color unselectedColor = new(0.25f, 0.25f, 0.25f, 0.7f);

    private void Awake() {
        UpdateUI();
    }

    void UpdateUI() {
        background.color = selected ? selectedColor : unselectedColor;
        if(stepperType == MFDStepperType.PERCENTAGE) {
            optionText.text = value + "%";
            leftArrow.GetComponent<SVGImage>().color = value > 0 ? arrowColor : arrowDisabledColor;
            rightArrow.GetComponent<SVGImage>().color = value < 100 ? arrowColor : arrowDisabledColor;
        } else if(stepperType == MFDStepperType.INT) {
            optionText.text = value.ToString();
            leftArrow.GetComponent<SVGImage>().color = value > 0 ? arrowColor : arrowDisabledColor;
            rightArrow.GetComponent<SVGImage>().color = value < intMax ? arrowColor : arrowDisabledColor;
        } else if(stepperType == MFDStepperType.OPTIONS) {
            optionText.text = options[value];
            leftArrow.GetComponent<SVGImage>().color = value > 0 ? arrowColor : arrowDisabledColor;
            rightArrow.GetComponent<SVGImage>().color = value < options.Length - 1 ? arrowColor : arrowDisabledColor;
        }
    }

    public void SetSelected(bool value) {
        selected = value;
        UpdateUI();
    }

    public void SelectNext() {
        int childIndex = transform.GetSiblingIndex();
        transform.parent.GetChild(childIndex < transform.parent.childCount - 1 ? childIndex + 1 : 0).GetComponent<MFDStepper>().SetSelected(true);
        SetSelected(false);
    }

    public void SelectPrevious() {
        int childIndex = transform.GetSiblingIndex();
        transform.parent.GetChild(childIndex > 0 ? childIndex - 1 : transform.parent.childCount - 1).GetComponent<MFDStepper>().SetSelected(true);
        SetSelected(false);
    }

    public void Next()
    {
        SetValue(value + 1);
    }

    public void Previous()
    {
        SetValue(value - 1);
    }

    public void SetValue(int option)
    {
        if (option < 0)
        {
            option = 0;
        }
        else if (stepperType == MFDStepperType.PERCENTAGE && option > 100)
        {
            option = 100;
        }
        else if (stepperType == MFDStepperType.INT && option > intMax)
        {
            option = intMax;
        }
        else if (stepperType == MFDStepperType.OPTIONS && option >= options.Length)
        {
            option = options.Length - 1;
        }

        value = option;
        UpdateUI();
    }
}