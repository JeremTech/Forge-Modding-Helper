using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FMH.Core.Utils.Software
{
    public static class SoftwareDataManager
    {
        /// <summary>
        /// Return global software data directory path
        /// </summary>
        /// <returns>Global software data directory path</returns>
        public static string GetSoftwareDataDirectory()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "JeremTech", "Forge Modding Helper");
        }

        /// <summary>
        /// Return current version data directory path
        /// </summary>
        /// <returns>Current version data directory path</returns>
        public static string GetCurrentVersionDataDirectory()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "JeremTech", "Forge Modding Helper", App.GetApplicationVersionCompact());
        }
    }
}
