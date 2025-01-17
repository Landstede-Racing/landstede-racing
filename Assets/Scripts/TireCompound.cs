using UnityEngine;

public static class TireCompounds
{
    public static readonly TireCompound Soft = new(1.2f, 1.0f);
    public static readonly TireCompound Medium = new(1.0f, 0.6f);
    public static readonly TireCompound Hard = new(0.8f, 0.4f);
}

public class TireCompound {
    public float grip;
    public float wearRate;

    public TireCompound(float grip, float wearRate) {
        this.grip = grip;
        this.wearRate = wearRate;
    }
}