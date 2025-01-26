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
    public static readonly Upgrade GearBoxLubrication = new() {
        location = Location.Gearbox,
        name = "Gear Box Lubrication",
        description = "Remind engineers to lubricate the gearbox before every race.",
        cost = 750,
        category = UpgradeCategory.PowerUnit,
        stats = new() {
            { Stats.engineHP, 10 }
        }
    };

    public static readonly Upgrade TurboBoost = new() {
        location = Location.TurboCharger,
        name = "Turbo Boost",
        description = "Install a turbo boost for those 'Fast and Furious' moments.",
        cost = 1500,
        category = UpgradeCategory.PowerUnit,
        stats = new() {
            { Stats.engineHP, 50 }
        }
    };

    public static readonly Upgrade FeatherWeight = new() {
        location = Location.Body,
        name = "Feather Weight",
        description = "Replace heavy parts with feathers. Just kidding, but it's really light!",
        cost = 1200,
        category = UpgradeCategory.Chassis,
        stats = new() {
            { Stats.weight, -20 }
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
            yield return GearBoxLubrication;
            yield return TurboBoost;
            yield return FeatherWeight;
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