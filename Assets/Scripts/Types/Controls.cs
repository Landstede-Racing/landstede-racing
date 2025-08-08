using System.Collections.Generic;

public class Control
{
    public int controlNumber = 0;
    public int button = 0;
    public string controlName;
}

public class Controls 
{
    public static Control MfdButton = new() { controlNumber = 0, button = 2, controlName = "MFD" };
    public static Control NextGearButton = new() { controlNumber = 1, button = 4, controlName = "Next Gear" };
    public static Control PreviousGearButton = new() { controlNumber = 2, button = 5, controlName = "Previous Gear" };
    public static Control DrsButton = new() { controlNumber = 3, button = 7, controlName = "DRS" };
    public static Control PitLimiterButton = new() { controlNumber = 4, button = 6, controlName = "Pit Limiter" };
    public static Control NextCamButton = new() { controlNumber = 5, button = 10, controlName = "Next Camera" };
    public static Control ReverseCamButton = new() { controlNumber = 6, button = 11, controlName = "Reverse Camera" };
    public static Control NextErsModeButton = new() { controlNumber = 7, button = 19, controlName = "Next ERS Mode" };
    public static Control PreviousErsModeButton = new() { controlNumber = 8, button = 20, controlName = "Previous ERS Mode" };
    public static Control PauseButton = new() { controlNumber = 9, button = 9, controlName = "Pause" };

    public static IEnumerable<Control> Values
    {
        get
        {
            yield return MfdButton;
            yield return NextGearButton;
            yield return PreviousGearButton;
            yield return DrsButton;
            yield return PitLimiterButton;
            yield return NextCamButton;
            yield return ReverseCamButton;
            yield return NextErsModeButton;
            yield return PreviousErsModeButton;
            yield return PauseButton;
        }
    }

    public static void SetControl(int controlNumber, int button)
    {
        foreach (Control control in Values)
        {
            if (control.controlNumber == controlNumber)
            {
                control.button = button;
            }
        }
    }
}