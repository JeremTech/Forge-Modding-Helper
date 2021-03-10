using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forge_Modding_Helper_3.Objects
{
    public class ModInfos
    {
        #region Mod infos variables
        /// <summary>
        /// Name of the mod (mod.toml - displayName entry)
        /// </summary>
        public string ModName { get; set; }

        /// <summary>
        /// Authors of the mod (mod.toml - authors entry)
        /// </summary>
        public string ModAuthors { get; set; }

        /// <summary>
        /// Version of the mod (build.gradle - version entry)
        /// </summary>
        public string ModVersion { get; set; }

        /// <summary>
        /// License of the mod (mod.toml - license entry)
        /// </summary>
        public string ModLicense { get; set; }

        /// <summary>
        /// Description of the mod (mod.toml - description entry)
        /// </summary>
        public string ModDescription { get; set; }

        /// <summary>
        /// ModID of the mod (mod.toml - modId entry / build.gradle - archivesBaseName & Specification entries)
        /// </summary>
        public string ModID { get; set; }

        /// <summary>
        /// ModGroup of the mod (build.gradle - group entry)
        /// </summary>
        public string ModGroup { get; set; }

        /// <summary>
        /// Name of the logo file of the mod (mod.toml - logoFile entry)
        /// </summary>
        public string ModLogo { get; set; }

        /// <summary>
        /// Credits of the mod (mod.toml - credits entry)
        /// </summary>
        public string ModCredits { get; set; }

        /// <summary>
        /// Website of the mod (mod.toml - displayURL entry)
        /// </summary>
        public string ModWebsite { get; set; }

        /// <summary>
        /// Issues tracker of the mod (mod.toml - issueTrackerURL entry)
        /// </summary>
        public string ModIssueTracker { get; set; }

        /// <summary>
        /// Json update URL of the mod (mod.toml - updateJSONURL entry)
        /// </summary>
        public string ModUpdateJSONURL { get; set; }

        /// <summary>
        /// Minecraft version of the mod
        /// </summary>
        public string ModMinecraftVersion { get; set; }

        /// <summary>
        /// Forge version of the mod
        /// </summary>
        public string ModForgeVersion { get; set; }

        /// <summary>
        /// Mappings version of the mod
        /// </summary>
        public string ModMappingsVersion { get; set; }
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public ModInfos()
        {
            this.ModName = "";
            this.ModAuthors = "";
            this.ModVersion = "";
            this.ModLicense = "";
            this.ModDescription = "";
            this.ModID = "";
            this.ModGroup = "";
            this.ModLogo = "";
            this.ModCredits = "";
            this.ModWebsite = "";
            this.ModIssueTracker = "";
            this.ModUpdateJSONURL = "";
            this.ModMinecraftVersion = "";
            this.ModForgeVersion = "";
            this.ModMappingsVersion = "";
        }
    }
}
