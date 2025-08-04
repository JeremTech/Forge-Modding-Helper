using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FMH.Core.Utils.Software
{
    public static class SoftwareVersionUtils
    {
        // Define if the current version is a preview
        private static bool IsPreview = true;

        // If "IsPreview" is set to true, then the current version is the preview with this number
        private static int PreviewNumber = 2;

        /// <summary>
        /// Allow to get the current version of the software
        /// </summary>
        /// <returns>Formatted string with the version and, if needed, the preview number</returns>
        public static string GetApplicationVersionString()
        {
            var assemblyVersion = Assembly.GetEntryAssembly()?.GetName()?.Version;
            if (assemblyVersion == null) return "Unknown";

            // If the version is a preview, we display the version number and the preview number
            if (IsPreview) return "v" + assemblyVersion.ToString() + " - " + "Preview " + PreviewNumber;

            // Else we display only the version number
            return "v" + assemblyVersion.ToString();
        }

        /// <summary>
        /// Allow to get the current version of the software
        /// </summary>
        /// <returns>String with the compacted version</returns>
        public static string GetApplicationVersionCompact()
        {
            var assemblyVersion = Assembly.GetEntryAssembly()?.GetName()?.Version;
            if (assemblyVersion == null) return "N/A";

            // If the version is a preview, we display the version number and the preview number
            if (IsPreview) return assemblyVersion.ToString() + "-" + "PRE" + PreviewNumber;

            // Else we display only the version number
            return assemblyVersion.ToString();
        }

    }
}
