using TMPro;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.UI;

public enum MFDStepperType
{
    PERCENTAGE,
    INT,
    OPTIONS,
    BOOL
}

public enum MFDStepperAction
{
    BRAKE_BIAS,
    DIFFERENTIAL,
    ERS_DEPLOY,
    NEXT_COMPOUND,
    REPAIR_WING,
}

public class MFDStepper : MonoBehaviour
{
    public MFDStepperType stepperType;
    public MFDStepperAction action;
    public string[] options;
    public int intMax;
    public int value;
    [SerializeField] private SVGImage leftArrow;
    [SerializeField] private SVGImage rightArrow;
    [SerializeField] private Image background;
    [SerializeField] private TMP_Text optionText;
    [SerializeField] private VehicleController vehicleController;
    public bool selected;

    [SerializeField] private Color arrowColor = new(1, 1, 1, 1);
    [SerializeField] private Color arrowDisabledColor = new(1, 1, 1, 0.4f);
    [SerializeField] private Color selectedColor = new(0.31f, 0.31f, 0.31f, 0.7f);
    [SerializeField] private Color unselectedColor = new(0.25f, 0.25f, 0.25f, 0.7f);

    private void Start() {
        value = vehicleController.frontLeftWheel.tireCompound.index;
        SetValue(value);
    }

    void FixedUpdate()
    {
        switch (action)
        {
            case MFDStepperAction.BRAKE_BIAS:
                UpdateValue(Mathf.RoundToInt(vehicleController.GetBrakeBias() * 100));
                break;
            // case MFDStepperAction.DIFFERENTIAL:
            //     value = vehicleController.GetDifferential();
            //     break;
            case MFDStepperAction.ERS_DEPLOY:
                UpdateValue(vehicleController.GetERSMode());
                break;
            case MFDStepperAction.NEXT_COMPOUND:
                UpdateValue(vehicleController.nextCompound.index);
                break;
            case MFDStepperAction.REPAIR_WING:
                UpdateValue(vehicleController.replaceWing == true ? 1 : 0);
                break;
            default:
                break;
        }
    }

    void UpdateValue(int newValue) {
        if(newValue != value) {
            SetValue(newValue);
        }
    }

    void UpdateUI() {
        background.color = selected ? selectedColor : unselectedColor;
        switch (stepperType)
        {
            case MFDStepperType.PERCENTAGE:
                optionText.text = value + "%";
                leftArrow.GetComponent<SVGImage>().color = value > 0 ? arrowColor : arrowDisabledColor;
                rightArrow.GetComponent<SVGImage>().color = value < 100 ? arrowColor : arrowDisabledColor;
                break;
            case MFDStepperType.INT:
                optionText.text = value.ToString();
                leftArrow.GetComponent<SVGImage>().color = value > 0 ? arrowColor : arrowDisabledColor;
                rightArrow.GetComponent<SVGImage>().color = value < intMax ? arrowColor : arrowDisabledColor;
                break;
            case MFDStepperType.OPTIONS:
                optionText.text = options[value];
                leftArrow.GetComponent<SVGImage>().color = value > 0 ? arrowColor : arrowDisabledColor;
                rightArrow.GetComponent<SVGImage>().color = value < options.Length - 1 ? arrowColor : arrowDisabledColor;
                break;
            case MFDStepperType.BOOL:
                var actualValue = value == 1;
                optionText.text = actualValue ? "Yes" : "No";
                leftArrow.GetComponent<SVGImage>().color = actualValue ? arrowColor : arrowDisabledColor;
                rightArrow.GetComponent<SVGImage>().color = !actualValue ? arrowColor : arrowDisabledColor;
                break;

            default:
                break;
        }
    }

    public void SetSelected(bool value) {
        selected = value;
        UpdateUI();
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

        switch (action)
        {
            case MFDStepperAction.BRAKE_BIAS:
                vehicleController.SetBrakeBias(option / 100f);
                Debug.Log(vehicleController.GetBrakeBias());
                break;
            // case MFDStepperAction.DIFFERENTIAL:
            //     vehicleController.SetDifferential(option);
            //     break;
            case MFDStepperAction.ERS_DEPLOY:
                vehicleController.SetERSMode(option);
                break;
            case MFDStepperAction.NEXT_COMPOUND:
                vehicleController.nextCompound = TireCompounds.Values[option];
                vehicleController.changeTires = option != vehicleController.frontLeftWheel.tireCompound.index;
                break;
            case MFDStepperAction.REPAIR_WING:
                vehicleController.replaceWing = option == 1;
                break;
            default:
                break;
        }

        value = option;
        UpdateUI();
    }
}