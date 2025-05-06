using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FMH.Core.Files.Software;
using Newtonsoft.Json;
using System.Windows.Media;
using System.IO;
using System.Windows;

namespace FMH.Core.Utils.Software
{
    public static class SoftwareThemeManager
    {
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
            if (!File.Exists(themeFilePath))
                return;

            // Retrieve file content
            var themeFileContent = File.ReadAllText(themeFilePath);
            if (string.IsNullOrEmpty(themeFileContent))
                return;

            // Deserialize theme file
            ThemeFile? themeData = JsonConvert.DeserializeObject<ThemeFile>(themeFileContent);
            if (themeData == null)
                return;

            // Load colors
            foreach(var resourceName in Application.Current.Resources.Keys)
            {
                var resourceNameString = resourceName.ToString();
                if (!string.IsNullOrEmpty(resourceNameString) && resourceNameString.Contains("color", StringComparison.OrdinalIgnoreCase))
                    LoadColor(themeData, resourceNameString);
            }
        }

        private static void LoadColor(ThemeFile? themeData, string resourceName)
        {
            string? themeValue = (string?) themeData?.GetType()?.GetProperty(resourceName)?.GetValue(themeData);
            if(!string.IsNullOrEmpty(themeValue))
            {
                var color = new BrushConverter().ConvertFromString(themeValue.ToString());
                if (color != null)
                    Application.Current.Resources[resourceName] = color;
            }
        }
    }
}
