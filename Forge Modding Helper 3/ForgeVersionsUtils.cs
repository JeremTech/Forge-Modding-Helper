using Newtonsoft.Json.Linq;
using System.Web.Script.Serialization;

namespace Forge_Modding_Helper_3
{
    class ForgeVersionsUtils
    {
        public string homepage { get; set; }
        public JObject promos { get; set; }

        public string getLatestForgeVersion(string mcVersion)
        {
            if (this.promos != null)
            {
                JToken getversion = this.promos[mcVersion + "-latest"];
                return getversion.ToString();
            }

            return "";
        }

        public string getRecommendedForgeVersion(string mcVersion)
        {
            if (this.promos != null && !string.IsNullOrWhiteSpace(mcVersion))
            {
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
