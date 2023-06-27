using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMH.Workspace.Entities
{
    public class ModProperties
    {
        /// <summary>
        /// Name of the mod
        /// </summary>
        public string ModName { get; set; }

        /// <summary>
        /// Authors of the mod
        /// </summary>
        public string ModAuthors { get; set; }

        /// <summary>
        /// Version of the mod
        /// </summary>
        public string ModVersion { get; set; }

        /// <summary>
        /// License of the mod
        /// </summary>
        public string ModLicense { get; set; }

        /// <summary>
        /// Description of the mod
        /// </summary>
        public string ModDescription { get; set; }

        /// <summary>
        /// ModID of the mod
        /// </summary>
        public string ModID { get; set; }

        /// <summary>
        /// ModGroup of the mod
        /// </summary>
        public string ModGroup { get; set; }

        /// <summary>
        /// Name of the logo file of the mod
        /// </summary>
        public string ModLogo { get; set; }

        /// <summary>
        /// Credits of the mod
        /// </summary>
        public string ModCredits { get; set; }

        /// <summary>
        /// Website of the mod
        /// </summary>
        public string ModWebsite { get; set; }

        /// <summary>
        /// Issues tracker of the mod
        /// </summary>
        public string ModIssueTracker { get; set; }

        /// <summary>
        /// Json update URL of the mod
        /// </summary>
        public string ModUpdateJSONURL { get; set; }

        /// <summary>
        /// Minecraft version of the mod
        /// </summary>
        public string ModMinecraftVersion { get; set; }

        /// <summary>
        /// Forge version of the mod
        /// </summary>
        public string ModAPIVersion { get; set; }

        /// <summary>
        /// Mappings version of the mod
        /// </summary>
        public string ModMappingsVersion { get; set; }
    }
}
