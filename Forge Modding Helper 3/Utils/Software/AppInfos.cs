using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forge_Modding_Helper_3.Utils
{
    // This class contains global application's infos
    public class AppInfos
    {
        // Define if the current version is stable or not
        private static bool isStable = true;

        // If "isStable" is set to false, then the current version is the pre-release with the number "pre_release_number"
        private static int pre_release_number = 0;

        // List of Minecraft versions supported by Forge Modding Helper
        private static List<String> supportedMcVersions = new List<string>() { "1.15.2", "1.16.3", "1.16.4", "1.16.5", "1.17.1", "1.18.1" };

        /// <summary>
        /// Allow to get the current version of the software
        /// </summary>
        /// <returns>Formatted string with the version and, if needed, the pre-release number</returns>
        public static String GetApplicationVersionString()
        {
            // If the version is stable, we display only the version number
            if(isStable) return "v" + System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString();

            // Else we display the version number and the pre-release number
            return "v" + System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString() + " - " + "Pre-Release " + pre_release_number;
        }

        /// <summary>
        /// Allow to get the current version of the software
        /// </summary>
        /// <returns>String with the compacted version</returns>
        public static string GetApplicationVersionCompact()
        {
            // If the version is stable, we display only the version number
            if (isStable) return System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString();

            // Else we display the version number and the pre-release number
            return System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString() + "-" + "PRE" + pre_release_number;
        }

        /// <summary>
        /// Allow to get the application's data directory in the user's APPDATA
        /// </summary>
        /// <returns>Application's data directory</returns>
        public static String getApplicationDataDirectory()
        {
            // Creating folder if not exist 
            Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "JeremTech", "Forge Modding Helper"));

            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "JeremTech", "Forge Modding Helper");
        }

        public static List<String> getSupportedMinecraftVersions()
        {
            return supportedMcVersions;
        }
    }
}
