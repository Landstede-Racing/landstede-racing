using System.Collections.Generic;
using Unity.VectorGraphics;
using UnityEngine;

public class ControllerButton {
    public int button = 0;
    public string imageName;

    public Sprite GetImage() {
        return Resources.Load<Sprite>("Textures/Buttons/" + imageName);
    }
}

public class ControllerButtons
{
    public static ControllerButton CrossButton = new() { button = 0, imageName = "Cross Button" };
    public static ControllerButton SquareButton = new() { button = 1, imageName = "Square Button" };
    public static ControllerButton CircleButton = new() { button = 2, imageName = "Circle Button" };
    public static ControllerButton TriangleButton = new() { button = 3, imageName = "Triangle Button" };
    public static ControllerButton RightPaddle = new() { button = 4, imageName = "Right Paddle" };
    public static ControllerButton LeftPaddle = new() { button = 5, imageName = "Left Paddle" };
    public static ControllerButton R2Button = new() { button = 6, imageName = "R2" };
    public static ControllerButton L2Button = new() { button = 7, imageName = "L2" };
    public static ControllerButton R3Button = new() { button = 10, imageName = "R3" };
    public static ControllerButton L3Button = new() { button = 11, imageName = "L3" };
    public static ControllerButton PlusButton = new() { button = 19, imageName = "Plus Button" };
    public static ControllerButton MinusButton = new() { button = 20, imageName = "Minus Button" };
    public static ControllerButton OptionsButton = new() { button = 9, imageName = "Options" };

    public static IEnumerable<ControllerButton> Values
    {
        get
        {
            yield return CrossButton;
            yield return SquareButton;
            yield return CircleButton;
            yield return TriangleButton;
            yield return RightPaddle;
            yield return LeftPaddle;
            yield return R2Button;
            yield return L2Button;
            yield return R3Button;
            yield return L3Button;
            yield return PlusButton;
            yield return MinusButton;
            yield return OptionsButton;
        }
    }

    public static ControllerButton GetButton(int buttonNumber)
    {
        foreach (ControllerButton button in Values)
        {
            if (button.button == buttonNumber)
            {
                return button;
            }
        }
        return null;
    }
}