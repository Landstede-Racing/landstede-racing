using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UpgradeController : MonoBehaviour
{
    public static List<Upgrade> GetAllUpgrades() {
        return Upgrade.Values.ToList();
    }  

    public static void UnlockUpgrade(Upgrade upgrade) {
        int unlockedUpgrades = PlayerPrefs.GetInt("UnlockedUpgrades", 0);
        PlayerPrefs.SetString("UnlockedUpgrade_" + unlockedUpgrades, upgrade.name);
        PlayerPrefs.SetInt("UnlockedUpgrades", unlockedUpgrades + 1);
    }

    // Get all upgrades for car
    public static List<Upgrade> GetUnlockedUpgrades() {
        int unlockedUpgrades = PlayerPrefs.GetInt("UnlockedUpgrades", 0);

        List<Upgrade> upgrades = new List<Upgrade>();

        for (int i = 0; i < unlockedUpgrades; i++)
        {
            upgrades.Add(Upgrade.GetUpgrade(PlayerPrefs.GetString("UnlockedUpgrade_" + i)));
        }

        foreach (Upgrade item in upgrades)
        {
            Debug.Log(item.name);
        }

        return upgrades;
    }

    public static bool IsUpgradeUnlocked(Upgrade upgrade) {
        List<Upgrade> upgrades = GetUnlockedUpgrades();
        return upgrades.Contains(upgrade);
    }

    public static void ApplyUpgrades(VehicleController vehicleController) {
        List<Upgrade> upgrades = GetUnlockedUpgrades();

        foreach (Upgrade upgrade in upgrades)
        {
            vehicleController.drsEnabled = true;
            foreach (KeyValuePair<Stats, float> pair in upgrade.stats)
            {
                Stats stat = pair.Key;
                float value = pair.Value;

                switch (stat)
                {
                    case Stats.engineHP:
                        vehicleController.engineHP += value;
                        break;
                    case Stats.frontDownForce:
                        vehicleController.maxFrontDownForce += value;
                        break;
                    case Stats.rearDownForce:
                        vehicleController.maxRearDownForce += value;
                        break;
                    case Stats.diffuserDownForce:
                        vehicleController.maxDiffuserDownForce += value;
                        break;
                    case Stats.brakeTorque:
                        vehicleController.brakeTorque += value;
                        break;
                    case Stats.maxERSCharge:
                        vehicleController.maxERSCharge += value;
                        break;
                    // case Stats.ERSDrain:
                    //     vehicleController.ERSDrain += value;
                    //     break;
                    // case Stats.ERSHP:
                    //     vehicleController.ERSHP += value;
                    //     break;
                    case Stats.ERSGenRate:
                        vehicleController.ERSGenerationRate += value;
                        break;
                    default:
                        break;
                }
            }
        }
    }
}