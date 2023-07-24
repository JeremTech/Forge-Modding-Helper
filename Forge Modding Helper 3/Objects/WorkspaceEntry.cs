using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using FMH.Workspace.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Forge_Modding_Helper_3.Objects
{
    // This class represent a workspace in last workspace file and last workspace list on Welcome window
    public class WorkspaceEntry
    {
        [JsonProperty("path")]
        public string WorkspacePath { get; set; }

        [JsonProperty("lastUpdated")]
        public DateTime LastUpdated { get; set; }

        [JsonProperty("modAPI")]
        [JsonConverter(typeof(StringEnumConverter))]
        public ModAPIType WorkspaceModAPI { get; set; }

        [JsonProperty("mcVersion")]
        public string WorkspaceMcVersion { get; set; }

        [JsonIgnore]
        public string WorkspaceName { get; set; }

        [JsonIgnore]
        public string WorkspaceLogoPath { get; set; }

        // Constructor
        public WorkspaceEntry(string _path, DateTime _lastUpdated, ModAPIType modAPI, string mcVersion)
        {
            this.LastUpdated = _lastUpdated;
            this.WorkspacePath = _path;
            this.WorkspaceName = Path.GetFileName(this.WorkspacePath);
            this.WorkspaceMcVersion = mcVersion;
            this.WorkspaceModAPI = modAPI;

            // Setup workspace logo
            SetupWorkspaceLogo();
        }

        private void SetupWorkspaceLogo()
        {
            switch(this.WorkspaceModAPI) 
            {
                case ModAPIType.Forge:
                    this.WorkspaceLogoPath = new Uri("/Forge Modding Helper 3;component/Resources/Pictures/forge_logo.png", UriKind.Relative).ToString();
                    break;

                default:
                    this.WorkspaceLogoPath = new Uri("/Forge Modding Helper 3;component/Resources/Pictures/icon.png", UriKind.Relative).ToString();
                    break;
            }
        }
    }
}
