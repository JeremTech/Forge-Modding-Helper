using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forge_Modding_Helper_3.Utils
{
    // This class contains global application's infos
    public class AppInfos
    {
        // Define if the current version is stable or not
        private static bool isStable = false;

        // If "isStable" is set to false, then the current version is the pre-release with the number "pre_release_number"
        private static int pre_release_number = 2;

        /// <summary>
        /// Allow to get the current version of the software
        /// </summary>
        /// <returns>Formatted string with the version and, if needed, the pre-release number</returns>
        public static String GetApplicationVersionString()
        {
            if(isStable)
                return "v" + System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString();
            
            return "v" + System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString() + " - " + "Pre-Release " + pre_release_number;
        }
    }
}
