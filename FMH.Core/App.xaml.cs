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
using FMH.Workspace.Data;

namespace FMH.Core
{
    public partial class App : Application
    {
        // List of Forge Minecraft versions supported by Forge Modding Helper
        private static List<string> _supportedForgeMcVersions = new List<string>() 
        { 
            "1.20", "1.20.1", "1.20.2", "1.20.4", "1.20.6", 
            "1.21", "1.21.1", "1.21.3", "1.21.4", "1.21.5", "1.21.8", "1.21.10", "1.21.11"
        };

        // List of NeoForge Minecraft versions supported by Forge Modding Helper
        private static List<string> _supportedNeoForgeMcVersions = new List<string>()
        {
            "1.20.2", "1.20.4", "1.20.6",
            "1.21", "1.21.1", "1.21.3", "1.21.4", "1.21.5", "1.21.8", "1.21.10", "1.21.11"
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
            return _supportedForgeMcVersions;
        }

        /// <summary>
        /// Return all supported NeoForge Minecraft version by Forge Modding Helper
        /// </summary>
        /// <returns>List of all supported NeoForge Minecraft versions</returns>
        public static List<string> GetSupportedNeoForgeMinecraftVersions()
        {
            return _supportedNeoForgeMcVersions;
        }

        /// <summary>
        /// Return all supported Minecraft versions by Forge Modding Helper for a specific ModAPI type
        /// </summary>
        /// <param name="modAPIType">ModAPI type</param>
        /// <returns>List of all supported Minecraft versions for the specified modding API</returns>
        public static List<string> GetSupportedMinecraftVersions(ModAPIType modAPIType)
        {
            return modAPIType switch
            {
                ModAPIType.Forge => GetSupportedForgeMinecraftVersions(),
                ModAPIType.NeoForge => GetSupportedNeoForgeMinecraftVersions(),
                _ => new List<string>()
            };
        }
    }
}
