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
using System.Diagnostics;

namespace FMH.Core
{
    public partial class App : Application
    {
        // Define if the current version is stable or notS
        private static bool isStable = false;

        // If "isStable" is set to false, then the current version is the pre-release with the number "pre_release_number"
        private static int pre_release_number = 1;

        // List of Minecraft versions supported by Forge Modding Helper
        private static List<string> supportedMcVersions = new List<string>() 
        { 
            "1.19", "1.19.1", "1.19.2", "1.19.3", "1.19.4",
            "1.20", "1.20.1", "1.20.2", "1.20.4", "1.20.6", 
            "1.21", "1.21.1", "1.21.3", "1.21.4" 
        };

        /// <summary>
        /// Formated version string
        /// </summary>
        /// <remarks>Used for bindings</remarks>
        public static string FormatedVersionString => GetApplicationVersionString();

        /// <summary>
        /// Allow to get the current version of the software
        /// </summary>
        /// <returns>Formatted string with the version and, if needed, the pre-release number</returns>
        public static string GetApplicationVersionString()
        {
            var assemblyVersion = Assembly.GetEntryAssembly()?.GetName()?.Version;
            if(assemblyVersion == null) return "Unknown";

            // If the version is stable, we display only the version number
            if (isStable) return "v" + assemblyVersion.ToString();

            // Else we display the version number and the pre-release number
            return "v" + assemblyVersion.ToString() + " - " + "Pre-Release " + pre_release_number;
        }

        /// <summary>
        /// Allow to get the current version of the software
        /// </summary>
        /// <returns>String with the compacted version</returns>
        public static string GetApplicationVersionCompact()
        {
            var assemblyVersion = Assembly.GetEntryAssembly()?.GetName()?.Version;
            if (assemblyVersion == null) return "N/A";

            // If the version is stable, we display only the version number
            if (isStable) return assemblyVersion.ToString();

            // Else we display the version number and the pre-release number
            return assemblyVersion.ToString() + "-" + "PRE" + pre_release_number;
        }

        /// <summary>
        /// Allow to get the application's data directory in the user's APPDATA
        /// </summary>
        /// <returns>Application's data directory</returns>
        public static string GetApplicationDataDirectory()
        {
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "JeremTech", "Forge Modding Helper");

            // Creating folder if not exist 
            Directory.CreateDirectory(path);
            return path;
        }

        /// <summary>
        /// Return all supported Minecraft versions by Forge Modding Helper
        /// </summary>
        /// <returns>List of all supported Minecraft versions</returns>
        public static List<string> GetSupportedMinecraftVersions()
        {
            return supportedMcVersions;
        }

        /// <summary>
        /// Load theme from file
        /// </summary>
        /// <param name="fileName">File name of theme (without extension)</param>
        public static void LoadThemeFile(string fileName)
        {
            // Get application executing directory
            var applicationExecutingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (string.IsNullOrEmpty(applicationExecutingDirectory))
                return;

            // Check if the file exists
            var themeFilePath = Path.Combine(applicationExecutingDirectory, "Themes", fileName + ".json");
            if(!File.Exists(themeFilePath))
                return;

            // Retrieve file content
            var themeFileContent = File.ReadAllText(themeFilePath);
            if(string.IsNullOrEmpty(themeFileContent))
                return;

            // Deserialize theme file
            ThemeFile? themeData = JsonConvert.DeserializeObject<ThemeFile>(themeFileContent);
            if(themeData == null)
                return;

            // Load colors
            Application.Current.Resources["PrimaryBackgroundColor"] = new BrushConverter().ConvertFromString(themeData.PrimaryBackgroundColor) as Brush;
            Application.Current.Resources["SecondaryBackgroundColor"] = new BrushConverter().ConvertFromString(themeData.SecondaryBackgroundColor) as Brush;
            Application.Current.Resources["InputsBackgroundColor"] = new BrushConverter().ConvertFromString(themeData.InputsBackgroundColor) as Brush;
            Application.Current.Resources["FontColorPrimary"] = new BrushConverter().ConvertFromString(themeData.FontColorPrimary) as Brush;
            Application.Current.Resources["FontColorSecondary"] = new BrushConverter().ConvertFromString(themeData.FontColorSecondary) as Brush;
            Application.Current.Resources["BorderColor"] = new BrushConverter().ConvertFromString(themeData.BorderColor) as Brush;
        }
    }
}
