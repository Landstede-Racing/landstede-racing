using System.Collections.Generic;
using UnityEngine;

public enum UpgradeCategory {
    PowerUnit,
    Aerodynamics,
    Chassis,
    Durability,
}

public class Upgrade
{
    public static readonly Upgrade TestEngineUpgrade = new() {
        location = Location.ICE,
        name = "Test Engine Upgrade",
        description = "This is a test engine upgrade",
        cost = 1000,
        category = UpgradeCategory.PowerUnit,
        stats = new() {
            { Stats.engineHP, 10 },
            { Stats.maxERSCharge, 10 },
            { Stats.ERSGenRate, 10 },
        }
    };

    public Location location;
    public string name;
    public string description;
    public int cost;
    public Dictionary<Stats, float> stats = new();
    public UpgradeCategory category;
    public bool unlocked = false;

    public static IEnumerable<Upgrade> Values
    {
        get
        {
            yield return TestEngineUpgrade;
        }
    }

    public static Upgrade GetUpgrade(Location location)
    {
        foreach (var upgrade in Values)
        {
            if (upgrade.location == location)
            {
                return upgrade;
            }
        }

        return null;
    }

    public static Upgrade GetUpgrade(string name)
    {
        foreach (var upgrade in Values)
        {
            if (upgrade.name == name)
            {
                return upgrade;
            }
        }

        return null;
    }

    public static List<Upgrade> GetUpgrades(UpgradeCategory category) {
        List<Upgrade> upgrades = new();
        foreach (var upgrade in Values)
        {
            if (upgrade.category == category)
            {
                upgrades.Add(upgrade);
            }
        }

        return upgrades;
    }
}