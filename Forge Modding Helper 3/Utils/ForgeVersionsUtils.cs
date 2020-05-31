using Newtonsoft.Json.Linq;
using System.Web.Script.Serialization;

namespace Forge_Modding_Helper_3
{
    // This class allow to read forge version
    public class ForgeVersionsUtils
    {
        // Variables according to the forge versions json content
        public string homepage { get; set; }
        public JObject promos { get; set; }

        /// <summary>
        /// Get the latest version of forge of a specific minecraft version
        /// </summary>
        /// <param name="mcVersion">Minecraft version</param>
        /// <returns></returns>
        public string getLatestForgeVersion(string mcVersion)
        {
            // Check if promos is not null
            if (this.promos != null)
            {
                JToken getversion = this.promos[mcVersion + "-latest"];
                return getversion.ToString();
            }

            return "";
        }

        /// <summary>
        /// Get the recommended version of forge of a specific minecraft version
        /// </summary>
        /// <param name="mcVersion">Minecraft version</param>
        /// <returns></returns>
        public string getRecommendedForgeVersion(string mcVersion)
        {
            // Check if promos is not null
            if (this.promos != null && !string.IsNullOrWhiteSpace(mcVersion))
            {
                // Check if a recommended version is available
                if (this.promos[mcVersion + "-recommended"] != null)
                {
                    JToken getversion = this.promos[mcVersion + "-recommended"];

                    if (!string.IsNullOrWhiteSpace(getversion.ToString()))
                    {
                        return getversion.ToString();
                    }
                }
            }

            return "";
        }
    }
}
