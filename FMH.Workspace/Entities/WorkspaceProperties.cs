using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMH.Workspace.Entities
{
    public class WorkspaceProperties
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
        /// Modding API
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
        /// Software version of the last workspace's opening
        /// </summary>
        public string SoftwareVersion { get; set; }

        /// <summary>
        /// Last project's openning date
        /// </summary>
        public DateTime LastOpened { get; set; }

        /// <summary>
        /// Workspace path
        /// </summary>
        public string WorkspacePath { get; set; }
    }
}
