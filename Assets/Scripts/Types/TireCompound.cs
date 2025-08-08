using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class TireCompounds
{
    public static readonly TireCompound Soft = new("Soft", 0, 1.2f, 1.0f, new Color(1.0f, 0.0f, 0.0f), 90);
    public static readonly TireCompound Medium = new("Medium", 1, 1.0f, 0.6f, new Color(1f, 0.7f, 0.0f), 90);
    public static readonly TireCompound Hard = new("Hard", 2, 0.8f, 0.4f, new Color(1.0f, 1.0f, 1.0f), 85);
    public static readonly TireCompound Intermediate = new("Intermediate", 3, 2f, 0.4f, new Color(0.0f, 0.64f, 0.17f), 85);
    public static readonly TireCompound FullWet = new("Full Wet", 4, 4f, 0.4f, new Color(0.14f, 0.38f, 0.68f), 85);

    public static TireCompound[] Values = {
        Soft,
        Medium,
        Hard,
        Intermediate,
        FullWet
    };
}

public class TireCompound
{
    public string name;
    public int index;
    public float grip;
    public float wearRate;
    public Color color;
    public float optimalTemperature;

    public TireCompound(string name, int index, float grip, float wearRate, Color color, float optimalTemperature)
    {
        this.name = name;
        this.index = index;
        this.grip = grip;
        this.wearRate = wearRate;
        this.color = color;
        this.optimalTemperature = optimalTemperature;
    }
}