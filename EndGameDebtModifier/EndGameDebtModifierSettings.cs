using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EndGameDebtModifier
{
    public class Settings
    {
        public bool enabled = true;
        public bool modifyDebt = true;
        public bool returnWeaver = false;
        public bool debugLog = false;

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
    }
}
