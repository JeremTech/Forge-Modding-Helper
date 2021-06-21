using Forge_Modding_Helper_3.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forge_Modding_Helper_3.Files.Workspace_Data
{
    public class ProjectFile
    {
        /// <summary>
        /// Mod name
        /// </summary>
        public string ModName { get; set; }
        
        /// <summary>
        /// Mod description
        /// </summary>
        public string ModDescription { get; set; }

        /// <summary>
        /// Mod logo
        /// </summary>
        public string ModLogo { get; set; }

        /// <summary>
        /// Mod API
        /// </summary>
        public string ModAPI { get; set; }

        /// <summary>
        /// API Version
        /// </summary>
        public string APIVersion { get; set; }

        /// <summary>
        /// Minecraft version
        /// </summary>
        public string MCVersion { get; set; }

        /// <summary>
        /// Software version of the last project's opening
        /// </summary>
        public string SoftwareVersion { get; set; }

        /// <summary>
        /// Last project's update date
        /// </summary>
        public DateTime LastUpdate { get; set; }

        /// <summary>
        /// Project path
        /// </summary>
        public string ProjectPath { get; set; }

        /// <summary>
        /// ProjectFile constructor
        /// </summary>
        /// <param name="_modName">Mod name</param>
        /// <param name="_modDesc">Mod description</param>
        /// <param name="_modLogo">Mod logo path or internal resource</param>
        /// <param name="_modAPI">Mod API name</param>
        /// <param name="_APIVersion">Mod API version</param>
        /// <param name="_MCVersion">Minecraft Version</param>
        public ProjectFile(string _modName, string _modDesc, string _modLogo, string _modAPI, string _APIVersion, string _MCVersion, string _projectPath)
        {
            this.ModName = _modName;
            this.ModDescription = _modDesc;
            this.ModLogo = _modLogo;
            this.ModAPI = _modAPI;
            this.APIVersion = _APIVersion;
            this.MCVersion = _MCVersion;
            this.SoftwareVersion = AppInfos.GetApplicationVersionCompact();
            this.LastUpdate = DateTime.Now;
            this.ProjectPath = _projectPath;
        }
    }
}
