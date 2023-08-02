using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;
using System.Windows.Media;
using FMH.Core.Files.Software;
using FMH.Core.UI.Common;

namespace FMH.Core
{
    public partial class App : Application
    {
        // Define if the current version is stable or notS
        private static bool isStable = false;

        // If "isStable" is set to false, then the current version is the pre-release with the number "pre_release_number"
        private static int pre_release_number = 1;

        // List of Minecraft versions supported by Forge Modding Helper
        private static List<String> supportedMcVersions = new List<string>() { "1.17.1", "1.18.1", "1.18.2", "1.19", "1.19.1", "1.19.2", "1.19.3", "1.19.4", "1.20", "1.20.1" };

        /// <summary>
        /// Allow to get the current version of the software
        /// </summary>
        /// <returns>Formatted string with the version and, if needed, the pre-release number</returns>
        public static String GetApplicationVersionString()
        {
            // If the version is stable, we display only the version number
            if (isStable) return "v" + Assembly.GetEntryAssembly().GetName().Version.ToString();

            // Else we display the version number and the pre-release number
            return "v" + Assembly.GetEntryAssembly().GetName().Version.ToString() + " - " + "Pre-Release " + pre_release_number;
        }

        /// <summary>
        /// Allow to get the current version of the software
        /// </summary>
        /// <returns>String with the compacted version</returns>
        public static string GetApplicationVersionCompact()
        {
            // If the version is stable, we display only the version number
            if (isStable) return Assembly.GetEntryAssembly().GetName().Version.ToString();

            // Else we display the version number and the pre-release number
            return Assembly.GetEntryAssembly().GetName().Version.ToString() + "-" + "PRE" + pre_release_number;
        }

        /// <summary>
        /// Allow to get the application's data directory in the user's APPDATA
        /// </summary>
        /// <returns>Application's data directory</returns>
        public static string GetApplicationDataDirectory()
        {
            // Creating folder if not exist 
            Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "JeremTech", "Forge Modding Helper"));

            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "JeremTech", "Forge Modding Helper");
        }

        public static List<String> GetSupportedMinecraftVersions()
        {
            return supportedMcVersions;
        }

        /// <summary>
        /// Load theme from file
        /// </summary>
        /// <param name="fileName">File name of theme (without extension)</param>
        public static void LoadThemeFile(string fileName)
        {
            // Retrieve file content
            ThemeFile themeData = JsonConvert.DeserializeObject<ThemeFile>(File.ReadAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Themes", fileName + ".json")));

            // Load colors
            Application.Current.Resources["PrimaryBackgroundColor"] = (Brush)new BrushConverter().ConvertFromString(themeData.PrimaryBackgroundColor);
            Application.Current.Resources["SecondaryBackgroundColor"] = (Brush)new BrushConverter().ConvertFromString(themeData.SecondaryBackgroundColor);
            Application.Current.Resources["InputsBackgroundColor"] = (Brush)new BrushConverter().ConvertFromString(themeData.InputsBackgroundColor);
            Application.Current.Resources["FontColorPrimary"] = (Brush)new BrushConverter().ConvertFromString(themeData.FontColorPrimary);
            Application.Current.Resources["FontColorSecondary"] = (Brush)new BrushConverter().ConvertFromString(themeData.FontColorSecondary);
            Application.Current.Resources["BorderColor"] = (Brush)new BrushConverter().ConvertFromString(themeData.BorderColor);
        }
    }
}
