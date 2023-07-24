using Forge_Modding_Helper_3.Files;
using Forge_Modding_Helper_3.Objects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Forge_Modding_Helper_3
{
    /// <summary>
    /// Global app values
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Load theme from file
        /// </summary>
        /// <param name="fileName">File name of theme (without extension)</param>
        public static void LoadThemeFile(string fileName)
        {
            // Retrieve file content
            ThemeFile themeData = JsonConvert.DeserializeObject<ThemeFile>(File.ReadAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Resources", "Themes", fileName + ".json")));

            // Load colors
            Application.Current.Resources["PrimaryBackgroundColor"] = (Brush) new BrushConverter().ConvertFromString(themeData.PrimaryBackgroundColor);
            Application.Current.Resources["SecondaryBackgroundColor"] = (Brush)new BrushConverter().ConvertFromString(themeData.SecondaryBackgroundColor);
            Application.Current.Resources["InputsBackgroundColor"] = (Brush)new BrushConverter().ConvertFromString(themeData.InputsBackgroundColor);
            Application.Current.Resources["FontColorPrimary"] = (Brush)new BrushConverter().ConvertFromString(themeData.FontColorPrimary);
            Application.Current.Resources["FontColorSecondary"] = (Brush)new BrushConverter().ConvertFromString(themeData.FontColorSecondary);
            Application.Current.Resources["BorderColor"] = (Brush)new BrushConverter().ConvertFromString(themeData.BorderColor);
        }
    }
}
