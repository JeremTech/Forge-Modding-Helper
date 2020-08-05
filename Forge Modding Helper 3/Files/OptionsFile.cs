using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Forge_Modding_Helper_3.Objects;
using Forge_Modding_Helper_3.Utils;
using Newtonsoft.Json;

namespace Forge_Modding_Helper_3.Files
{
    public class OptionsFile
    {
        // Data-file path
        private static string FilePath = Path.Combine(AppInfos.getApplicationDataDirectory(), "options.json");
        // Data object
        private static OptionsData data = new OptionsData("Français");

        // Selected language
        private static string language = "Français";

        public static string getCurrentLanguage()
        {
            return language;
        }

        public static void setCurrentLanguage(string lang)
        {
            language = lang;
            data.language = lang;
        }

        /// <summary>
        /// Read the data file 
        /// </summary>
        /// <returns><code>true</code> if success, <code>false</code> if fail</returns>
        public static bool ReadDataFile()
        {
            bool success = false;

            if (File.Exists(FilePath))
            {
                string jsonContent = File.ReadAllText(FilePath);
                OptionsData jsonContentFormatted = JsonConvert.DeserializeObject<OptionsData>(jsonContent);

                if (jsonContentFormatted.language != null)
                {
                    data = jsonContentFormatted;
                    language = jsonContentFormatted.language;
                    success = true;
                }
            }

            return success;
        }

        /// <summary>
        /// Write the data file 
        /// </summary>
        /// <returns><code>true</code> if success, <code>false</code> if fail</returns>
        public static bool WriteDataFile()
        {
            bool success = false;

            if (language != "")
            {
                OptionsData data = new OptionsData(language);
                string jsonContent = JsonConvert.SerializeObject(data, Formatting.Indented);
                File.WriteAllText(FilePath, jsonContent);

                success = true;
            }

            return success;
        }
    }

    public class OptionsData
    {
        public string language { get; set; }

        public OptionsData(string language)
        {
            this.language = language;
        }
    }
}
