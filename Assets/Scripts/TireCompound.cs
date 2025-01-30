using UnityEngine;

public static class TireCompounds
{
    public static readonly TireCompound Soft = new(1.2f, 1.0f, new Color(1.0f, 0.0f, 0.0f));
    public static readonly TireCompound Medium = new(1.0f, 0.6f, new Color(1f, 0.7f, 0.0f));
    public static readonly TireCompound Hard = new(0.8f, 0.4f, new Color(0.0f, 0.0f, 0.0f));
}

public class TireCompound
{
    public Color color;
    public float grip;
    public float wearRate;

    public TireCompound(float grip, float wearRate, Color color)
    {
        this.grip = grip;
        this.wearRate = wearRate;
        this.color = color;
    }
}