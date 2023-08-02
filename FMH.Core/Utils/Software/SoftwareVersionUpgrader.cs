using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMH.Core.Utils.Software
{
    public class SoftwareVersionUpgrader
    {
        /// <summary>
        /// Import old setting from a previous version
        /// </summary>
        /// <param name="folderPath">Previous version settings folder</param>
        public static void ImportOldSettings(string folderPath)
        {
            if (!Directory.Exists(folderPath))
                return;

            // Creating current version data directory
            Directory.CreateDirectory(SoftwareDataManager.GetCurrentVersionDataDirectory());

            // Copy files
            File.Copy(Path.Combine(folderPath, "options.json"), Path.Combine(SoftwareDataManager.GetCurrentVersionDataDirectory(), "options.json"));
        }

        /// <summary>
        /// Check if a previous version data exist
        /// </summary>
        /// <returns><c>true</c> if preivous version data exist, <c>false</c> else</returns>
        public static bool ExistPreviousVersionData()
        {
            // Check old data files created before Forge Modding Helper 3.3.0.0-PRE2
            if (File.Exists(Path.Combine(SoftwareDataManager.GetSoftwareDataDirectory(), "options.json"))
                || File.Exists(Path.Combine(SoftwareDataManager.GetSoftwareDataDirectory(), "workspaces.json")))
                return true;

            // Check old data directories
            var softwareDataDirectories = Directory.EnumerateDirectories(SoftwareDataManager.GetSoftwareDataDirectory());

            if (softwareDataDirectories.Any()
                && !(softwareDataDirectories.Count() == 1 && ExistCurrentVersionData()))
                return true;

            return false;
        }

        /// <summary>
        /// Check if current version data directory exist
        /// </summary>
        /// <returns><c>true</c> if current version data directory exist, <c>false</c> else</returns>
        public static bool ExistCurrentVersionData()
        {
            return Directory.Exists(SoftwareDataManager.GetCurrentVersionDataDirectory());
        }
    }
}
