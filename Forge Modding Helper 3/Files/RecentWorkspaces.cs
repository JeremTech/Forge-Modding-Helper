using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using Forge_Modding_Helper_3.Objects;
using Forge_Modding_Helper_3.Utils;
using Newtonsoft.Json;

namespace Forge_Modding_Helper_3.Files
{
    // This class allwo to manage recent workspace
    public class RecentWorkspaces
    {
        // Recent Workspaces list
        public static List<Workspace> RecentWorkspacesList = new List<Workspace>();

        // Data-file path
        private static string FilePath = Path.Combine(AppInfos.getApplicationDataDirectory(), "workspaces.json");

        /// <summary>
        /// Read the data file 
        /// </summary>
        /// <returns><code>true</code> if success, <code>false</code> if fail</returns>
        public static bool ReadDataFile()
        {
            bool success = false;

            if (File.Exists(FilePath))
            {
                string jsonContent = File.ReadAllText(FilePath);
                List<Workspace> jsonContentFormatted = JsonConvert.DeserializeObject<List<Workspace>>(jsonContent);

                if (jsonContentFormatted.Count > 0)
                {
                    RecentWorkspacesList = jsonContentFormatted.OrderByDescending(element => element.last_updated).ToList();
                    success = true;
                }
            }

            return success;
        }

        /// <summary>
        /// Write the data file 
        /// </summary>
        /// <returns><code>true</code> if success, <code>false</code> if fail</returns>
        public static bool WriteDataFile()
        {
            bool success = false;

            if (RecentWorkspacesList.Count > 0)
            {
                string jsonContent = JsonConvert.SerializeObject(RecentWorkspacesList, Formatting.Indented);
                File.WriteAllText(FilePath, jsonContent);

                success = true;
            }

            return success;
        }
    }
}
