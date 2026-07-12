using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DebugMenu
{
    public class Settings
    {
        public bool enabled = true;
        public string activateMouseKey = "f1";

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
