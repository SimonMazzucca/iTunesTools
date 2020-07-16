using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace PlaylistManager.Utilities
{
    public class SettingsFileAccess : ISettingsFileAccess
    {

        private const string SETTINGS_FILE = "Settings.json";

        public string Load()
        {
            string settingsJson = GetResourceFileContent(SETTINGS_FILE);
            return settingsJson;
        }

        private string GetResourceFileContent(string filename)
        {
            Assembly assembly = Assembly.GetAssembly(typeof(Settings));

            string[] resources = assembly.GetManifestResourceNames();
            string resource = resources.FirstOrDefault(r => r.EndsWith("." + filename));
            string toReturn = "";

            if (string.IsNullOrEmpty(resource))
            {
                string message = string.Format("Missing resource: {0} (Ensure BuildAction is set to 'Embedded Resource')", filename);
                throw new Exception(message);
            }
            else
            {
                using (Stream stream = assembly.GetManifestResourceStream(resource))
                using (StreamReader reader = new StreamReader(stream))
                {
                    toReturn = reader.ReadToEnd();
                }

                return toReturn;
            }
        }

    }
}