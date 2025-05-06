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
using FMH.Core.Utils.Software;

namespace FMH.Core
{
    public partial class App : Application
    {
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
        public static string FormatedVersionString => SoftwareVersionUtils.GetApplicationVersionString();

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
