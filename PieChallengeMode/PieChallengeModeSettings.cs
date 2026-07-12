using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PieChallengeMode
{
    public class Settings
    {
        public bool enabled = false;
        public bool alwaysOn = true;
        public bool debugLog = false;
        public int minTotalObjectives = 2;
        public int maxTotalObjectives = 5;

        public Dictionary<string, float> baseTimePerShipType = new Dictionary<string, float>
        {
            { "Javelin_Industrial_Refueling_Lrg_ModuleConstructionAsset", 120 },
            { "Javelin_Industrial_Cargo_Lrg_ModuleConstructionAsset", 120 },
            { "Mistral_PATROLPrototype_ModuleConstructionAsset", 90 },
            { "Mistral_TUGPrototype_ModuleConstructionAsset", 90 },
            { "Mistral_CARGOPrototype_ModuleConstructionAsset", 90 },
            { "Mackerel_Civilian_Transit_ModuleConstructionAsset", 60 },
            { "Mackerel_Science_Shuttle_ModuleConstructionAsset", 90 },
            { "Mackerel_Industrial_Cargo_ModuleConstructionAsset", 60 },
            { "Mackerel_Civilian_Cargo_ModuleConstructionAsset", 60 },
            { "Gecko_Industrial_Cargo_ModuleConstructionAsset", 150 },
            { "Gecko_Commercial_Transit_ModuleConstructionAsset", 150 },
            { "Gecko_Science_Stargazer_ModuleConstructionAsset", 150 },
            { "Gecko_Industrial_Salvage_ModuleConstructionAsset", 150 },
        };

        public Dictionary<string, ObjectCollectionParameters> collectionObjects = new Dictionary<string, ObjectCollectionParameters>
        {
            { "INFO_AirlockConsole", new ObjectCollectionParameters(30, 30) },
            { "INFO_AirlockInterior", new ObjectCollectionParameters(60, 60) },
            { "INFO_Amplifier", new ObjectCollectionParameters(30, 60) },
            { "INFO_Antenna", new ObjectCollectionParameters(0, 10) },
            { "INFO_Antenna_LongRange", new ObjectCollectionParameters(0, 10) },
            { "INFO_AtmosphereRegulator", new ObjectCollectionParameters(20, 20) },
            { "INFO_CargoContainer", new ObjectCollectionParameters(30, 30) },
            { "INFO_CargoHatch", new ObjectCollectionParameters(5, 5) },
            { "INFO_Chair_Flight", new ObjectCollectionParameters(2, 2) },
            { "INFO_Chair_Wall", new ObjectCollectionParameters(2, 2) },
            { "INFO_CommDish", new ObjectCollectionParameters(2, 2) },
            { "INFO_Crate_Hard", new ObjectCollectionParameters(30, 30) },
            { "INFO_Crate_Shipping", new ObjectCollectionParameters(30, 30) },
            { "INFO_Crate_Soft", new ObjectCollectionParameters(30, 30) },
            { "INFO_CryoTank", new ObjectCollectionParameters(30, 30) },
            { "INFO_Forklift_New", new ObjectCollectionParameters(30, 30) },
            { "INFO_FuelTank", new ObjectCollectionParameters(30, 30) },
            { "INFO_Hoist", new ObjectCollectionParameters(60, 60) },
            { "INFO_HydroponicsCabinet", new ObjectCollectionParameters(20, 20) },
            { "INFO_Nacell", new ObjectCollectionParameters(60, 60) },
            { "INFO_NacellDouble", new ObjectCollectionParameters(60, 60) },
            { "INFO_PowerGenerator", new ObjectCollectionParameters(30, 60) },
            { "INFO_PowerJunctionBox", new ObjectCollectionParameters(20, 20) },
            { "INFO_Powercell", new ObjectCollectionParameters(20, 20) },
            { "INFO_Prop_Fuse", new ObjectCollectionParameters(30, 60) },
            { "INFO_Reactor_Class_I", new ObjectCollectionParameters(45, 45) },
            { "INFO_Reactor_Class_II", new ObjectCollectionParameters(90, 90) },
            { "INFO_SensorTower", new ObjectCollectionParameters(15, 15) },
            { "INFO_SensorsArray", new ObjectCollectionParameters(15, 15) },
            { "INFO_Sign", new ObjectCollectionParameters(30, 60) },
            { "INFO_SolarBattery", new ObjectCollectionParameters(15, 15) },
            { "INFO_StorageBin", new ObjectCollectionParameters(30, 30) },
            { "INFO_Table", new ObjectCollectionParameters(30, 60) },
            { "INFO_Terminal_Standard", new ObjectCollectionParameters(15, 15) },
            { "INFO_ThermalRegulatorHeatSink", new ObjectCollectionParameters(20, 20) },
            { "INFO_Thruster_Class_I", new ObjectCollectionParameters(30, 30) },
            { "INFO_Thruster_Class_II", new ObjectCollectionParameters(60, 60) },
            { "INFO_Thruster_Class_X_Fuel_Injector", new ObjectCollectionParameters(30, 60) },
            { "INFO_Thruster_Class_X_Fuel_Injector_CoverPlate", new ObjectCollectionParameters(30, 60) },
            { "INFO_Thruster_Engine", new ObjectCollectionParameters(30, 60) },
        };

        public List<string> invalidCollectionObjects = new List<string> { "INFO_Light" };

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

        public class ObjectCollectionParameters
        {
            public float minPercent = 1;
            public float maxPercent = 1;
            public float minPercentTime;
            public float maxPercentTime;

            public ObjectCollectionParameters() { }

            public ObjectCollectionParameters(float minPercentTime, float maxPercentTime)
            {
                this.minPercentTime = minPercentTime;
                this.maxPercentTime = maxPercentTime;
            }
        }
    }
}
