using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FMH.Workspace.Data
{
    public class WorkspaceProperties
    {
        /// <summary>
        /// Modding API
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public ModAPIType ModAPI { get; set; }

        /// <summary>
        /// API Version
        /// </summary>
        public string APIVersion { get; set; }

        /// <summary>
        /// Minecraft version
        /// </summary>
        public string MCVersion { get; set; }

        /// <summary>
        /// ModId
        /// </summary>
        public string ModId { get; set; }

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
        [JsonIgnore]
        public string WorkspacePath { get; set; }
    }
}
