using UnityEngine;

public static class TireCompounds
{
    public static readonly TireCompound Soft = new(1.2f, 1.0f, new Color(1.0f, 0.0f, 0.0f), 90);
    public static readonly TireCompound Medium = new(1.0f, 0.6f, new Color(1f, 0.7f, 0.0f), 90);
    public static readonly TireCompound Hard = new(0.8f, 0.4f, new Color(0.0f, 0.0f, 0.0f), 85);
    public static readonly TireCompound Intermediate = new(2f, 0.4f, new Color(0.0f, 0.64f, 0.17f), 85);
    public static readonly TireCompound FullWet = new(4f, 0.4f, new Color(0.14f, 0.38f, 0.68f), 85);
}

public class TireCompound
{
    public float grip;
    public float wearRate;
    public Color color;
    public float optimalTemperature;

    public TireCompound(float grip, float wearRate, Color color, float optimalTemperature)
    {
        this.grip = grip;
        this.wearRate = wearRate;
        this.color = color;
        this.optimalTemperature = optimalTemperature;
    }
}