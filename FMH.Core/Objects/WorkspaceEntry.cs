using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMH.Workspace.Data;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.IO;

namespace FMH.Core.Objects
{
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
            switch (this.WorkspaceModAPI)
            {
                case ModAPIType.Forge:
                    this.WorkspaceLogoPath = new Uri("/FMH.Resources;component/Pictures/forge_logo.png", UriKind.Relative).ToString();
                    break;

                default:
                    this.WorkspaceLogoPath = new Uri("/FMH.Resources;component/Pictures/icon.png", UriKind.Relative).ToString();
                    break;
            }
        }
    }
}
