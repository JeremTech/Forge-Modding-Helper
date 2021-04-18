using System.IO;
using Forge_Modding_Helper_3.Utils;
using Newtonsoft.Json;

namespace Forge_Modding_Helper_3.Files
{
    public class OptionsFile
    {
        // Data-file path
        private static string FilePath = Path.Combine(AppInfos.getApplicationDataDirectory(), "options.json");
        // Data object
        private static OptionsData data = new OptionsData("en_us");

        // Selected language
        private static string language = "en_us";

        // Selected theme
        private static string theme = "Dark";

        /// <summary>
        /// Get current language
        /// </summary>
        /// <returns>Current language file name without extension</returns>
        public static string getCurrentLanguage()
        {
            return language;
        }

        /// <summary>
        /// Set current language
        /// </summary>
        /// <returns>Set language file name (without extension)</returns>
        public static void setCurrentLanguage(string lang)
        {
            language = lang;
            data.language = lang;
        }

        /// <summary>
        /// Get current theme
        /// </summary>
        /// <returns>Current theme file name without extension</returns>
        public static string GetCurrentTheme()
        {
            return theme;
        }

        /// <summary>
        /// Set current theme
        /// </summary>
        /// <returns>Set theme file name (without extension)</returns>
        public static void SetCurrentTheme(string themeIn)
        {
            theme = themeIn;
            data.theme = themeIn;
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

                if (jsonContentFormatted.theme != null)
                {
                    theme = jsonContentFormatted.theme;
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
                OptionsData data = new OptionsData(language, theme);
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

        public string theme { get; set; }

        public OptionsData()
        {
            this.language = language;
        }

        public OptionsData(string language)
        {
            this.language = language;
        }

        public OptionsData(string language, string theme)
        {
            this.language = language;
            this.theme = theme;
        }
    }
}
