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
        // List of Forge Minecraft versions supported by Forge Modding Helper
        private static List<string> supportedForgeMcVersions = new List<string>() 
        { 
            "1.19", "1.19.1", "1.19.2", "1.19.3", "1.19.4",
            "1.20", "1.20.1", "1.20.2", "1.20.4", "1.20.6", 
            "1.21", "1.21.1", "1.21.3", "1.21.4", "1.21.5"
        };

        /// <summary>
        /// Formated version string
        /// </summary>
        /// <remarks>Used for bindings</remarks>
        public static string FormatedVersionString => SoftwareVersionUtils.GetApplicationVersionString();

        /// <summary>
        /// Return all supported Forge Minecraft versions by Forge Modding Helper
        /// </summary>
        /// <returns>List of all supported Forge Minecraft versions</returns>
        public static List<string> GetSupportedForgeMinecraftVersions()
        {
            return supportedForgeMcVersions;
        }
    }
}
