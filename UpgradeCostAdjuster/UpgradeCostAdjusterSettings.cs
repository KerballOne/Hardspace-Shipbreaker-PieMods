using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpgradeCostAdjuster
{
    public class Settings
    {
        public bool enabled = false;
        public float globalLTCostMultiplier = 2;
        public float globalLTToCreditFactor = -1;
        public int globalRequiredLevelDefaultOverride = -1;
        public bool debugLogChanges = false;

        // All upgrades default to the same disabled/unset sentinel (prices -1, requiredLevel -1);
        // per-upgrade overrides go in settings.json, not here.
        public Dictionary<string, UpgradeCosts> upgradeCosts = defaultUpgradeIds.ToDictionary(id => id, id => new UpgradeCosts());

        public List<string> excludedBecauseBroken = new List<string>
        {
            "UnlockGrapplingHook_UpgradeAsset",
            "UnlockPush_UpgradeAsset",
            "ScannerMode_Structure_UpgradeAsset",
            "UnlockScalpel_UpgradeAsset",
            "TUTORIAL_O2Capacity_UpgradeAsset",
            "TUTORIAL_TethersAmount_UpgradeAsset",
            "TUTORIAL_FuelCap_UpgradeAsset",
        };

        public static Settings settings;

        public static void Load()
        {
            var settingsPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "settings.json");
            var defaults = JObject.FromObject(new Settings());
            var loaded = System.IO.File.Exists(settingsPath) ? JObject.Parse(System.IO.File.ReadAllText(settingsPath)) : new JObject();

            var addedFields = false;
            foreach (var property in defaults.Properties())
            {
                if (loaded.Property(property.Name) == null)
                {
                    loaded.Add(property.Name, property.Value);
                    addedFields = true;
                }
            }

            if (addedFields || !System.IO.File.Exists(settingsPath))
                System.IO.File.WriteAllText(settingsPath, loaded.ToString(Formatting.Indented));

            settings = loaded.ToObject<Settings>();
        }

        private static readonly string[] defaultUpgradeIds = new[]
        {
            "GrappleDurability1_UpgradeAsset", "GrappleDurability2_UpgradeAsset", "GrappleDurability3_UpgradeAsset",
            "GrappleDurabilityDrain1_UpgradeAsset", "GrappleDurabilityDrain2_UpgradeAsset", "GrappleDurabilityDrain3_UpgradeAsset",
            "GrappleRange1_UpgradeAsset", "GrappleRange2_UpgradeAsset", "GrappleRange3_UpgradeAsset",
            "GrappleRange4_UpgradeAsset", "GrappleRange5_UpgradeAsset", "GrappleStrength1_UpgradeAsset",
            "GrappleStrength2_UpgradeAsset", "GrappleStrength3_UpgradeAsset", "GrappleStrength4_UpgradeAsset",
            "GrappleStrength5_UpgradeAsset", "UnlockChargedPush_UpgradeAsset", "ChargedPushForce2_UpgradeAsset",
            "TethersAmount1_UpgradeAsset", "TethersAmount2_UpgradeAsset", "TethersAmount3_UpgradeAsset",
            "TethersLifetime1_UpgradeAsset", "TethersLifetime2_UpgradeAsset", "TethersLifetime3_UpgradeAsset",
            "TethersLifetime4_UpgradeAsset", "UnlockTethers_UpgradeAsset", "HelmetRechargeRate1_UpgradeAsset",
            "HelmetRechargeRate2_UpgradeAsset", "HelmetRechargeRate3_UpgradeAsset", "HelmetRechargeUnlock_UpgradeAsset",
            "HelmetTankCapacity1_UpgradeAsset", "HelmetTankCapacity2_UpgradeAsset", "HelmetTankCapacity3_UpgradeAsset",
            "HelmetTankCapacity4_UpgradeAsset", "HelmetTankCapacity5_UpgradeAsset", "ScannerDetail_Category_UpgradeAsset",
            "ScannerDetail_Condition_UpgradeAsset", "ScannerDetail_Hazards_UpgradeAsset", "ScannerDetail_Materials_UpgradeAsset",
            "ScannerDurability1_UpgradeAsset", "ScannerDurability2_UpgradeAsset", "ScannerDurability3_UpgradeAsset",
            "ScannerDurabilityDrain1_UpgradeAsset", "ScannerDurabilityDrain2_UpgradeAsset", "ScannerDurabilityDrain3_UpgradeAsset",
            "ScannerMode_Objects_UpgradeAsset", "ScannerMode_Systems_UpgradeAsset", "ScannerRange1_UpgradeAsset",
            "ScannerRange2_UpgradeAsset", "ScannerRange3_UpgradeAsset", "ScannerRange4_UpgradeAsset",
            "ScannerRange5_UpgradeAsset", "SuitCryoShield1_UpgradeAsset", "SuitCryoShield2_UpgradeAsset",
            "SuitCryoShield3_UpgradeAsset", "SuitCryoShield4_UpgradeAsset", "SuitCryoShield5_UpgradeAsset",
            "SuitDefence1_UpgradeAsset", "SuitDefence2_UpgradeAsset", "SuitDefence3_UpgradeAsset",
            "SuitDefence4_UpgradeAsset", "SuitDefence5_UpgradeAsset", "SuitDurability1_UpgradeAsset",
            "SuitDurability2_UpgradeAsset", "SuitDurability3_UpgradeAsset", "SuitElectricalShield1_UpgradeAsset",
            "SuitElectricalShield2_UpgradeAsset", "SuitElectricalShield3_UpgradeAsset", "SuitElectricalShield4_UpgradeAsset",
            "SuitElectricalShield5_UpgradeAsset", "SuitHeatShield1_UpgradeAsset", "SuitHeatShield2_UpgradeAsset",
            "SuitHeatShield3_UpgradeAsset", "SuitHeatShield4_UpgradeAsset", "SuitHeatShield5_UpgradeAsset",
            "ThrustersBraking1_UpgradeAsset", "ThrustersBraking2_UpgradeAsset", "ThrustersBraking3_UpgradeAsset",
            "ThrustersDurability1_UpgradeAsset", "ThrustersDurability2_UpgradeAsset", "ThrustersDurability3_UpgradeAsset",
            "ThrustersDurabilityDrain1_UpgradeAsset", "ThrustersDurabilityDrain2_UpgradeAsset", "ThrustersDurabilityDrain3_UpgradeAsset",
            "ThrustersFuelCap1_UpgradeAsset", "ThrustersFuelCap2_UpgradeAsset", "ThrustersFuelCap3_UpgradeAsset",
            "ThrustersSpeed1_UpgradeAsset", "ThrustersSpeed2_UpgradeAsset", "ThrustersSpeed3_UpgradeAsset",
            "CutterHeatCap1_UpgradeAsset", "CutterHeatCap2_UpgradeAsset", "CutterHeatCap3_UpgradeAsset",
            "SuitResynth1_UpgradeAsset", "SuitResynth2_UpgradeAsset", "SuitResynth3_UpgradeAsset",
            "BoneSaw_Range1_UpgradeAsset", "BoneSaw_Range2_UpgradeAsset", "BoneSaw_Range3_UpgradeAsset",
            "BoneSaw_Range4_UpgradeAsset", "CutterCooldown1_UpgradeAsset", "CutterCooldown2_UpgradeAsset",
            "CutterCooldown3_UpgradeAsset", "CutterDurability1_UpgradeAsset", "CutterDurability2_UpgradeAsset",
            "CutterDurability3_UpgradeAsset", "CutterDurabilityDrain1_UpgradeAsset", "CutterDurabilityDrain2_UpgradeAsset",
            "CutterDurabilityDrain3_UpgradeAsset", "Scalpel_Range1_UpgradeAsset", "Scalpel_Range2_UpgradeAsset",
            "Scalpel_Range3_UpgradeAsset", "Scalpel_Range4_UpgradeAsset", "Scalpel_Range5_UpgradeAsset",
            "UnlockBoneSaw_UpgradeAsset", "CutterPurchase_UpgradeAsset", "HelmetPurchase_UpgradeAsset",
            "GrapplePurchase_UpgradeAsset", "ScannerPurchase_UpgradeAsset", "SuitPurchase_UpgradeAsset",
            "ThrustersPurchase_UpgradeAsset", "ChargedPushForce1_UpgradeAsset", "DemoCharge_AutoDeploy_UpgradeAsset",
            "DemoCharge_Capacity1_UpgradeAsset", "DemoCharge_Capacity2_UpgradeAsset", "DemoCharge_Capacity3_UpgradeAsset",
            "DemoCharge_Capacity4_UpgradeAsset", "DemoCharge_Capacity5_UpgradeAsset", "DemoCharge_Cleanup1_UpgradeAsset",
            "DemoCharge_Cleanup2_UpgradeAsset", "DemoCharge_Cleanup3_UpgradeAsset", "DemoCharge_Disarming1_UpgradeAsset",
            "DemoCharge_Disarming2_UpgradeAsset", "DemoCharge_Disarming3_UpgradeAsset", "UnlockDemoCharge_UpgradeAsset",
            "DemoChargePurchase_UpgradeAsset", "DemoChargeDurabilityDrain1_UpgradeAsset", "DemoChargeDurabilityDrain2_UpgradeAsset",
            "DemoChargeDurabilityDrain3_UpgradeAsset", "DemoChargeDurabilityDrain4_UpgradeAsset", "DemoChargeDurabilityDrain5_UpgradeAsset",
            "CutterRental_UpgradeAsset", "CutterDurabilityDrain4_UpgradeAsset", "CutterDurabilityDrain5_UpgradeAsset",
            "BoneSaw_Range5_UpgradeAsset", "Scalpel_CutGrade_UpgradeAsset", "GrappleDurabilityDrain4_UpgradeAsset",
            "GrappleDurabilityDrain5_UpgradeAsset", "ChargedPushForce3_UpgradeAsset", "ScannerDurabilityDrain4_UpgradeAsset",
            "ScannerDurabilityDrain5_UpgradeAsset", "ThrustersDurabilityDrain4_UpgradeAsset", "ThrustersDurabilityDrain5_UpgradeAsset",
        };
    }

    public class UpgradeCosts
    {
        public Dictionary<string, float> prices = new Dictionary<string, float> { { "LT_CurrencyAsset", -1 }, { "Credits_CurrencyAsset", -1 } };
        public int requiredLevel = -1;
    }
}
