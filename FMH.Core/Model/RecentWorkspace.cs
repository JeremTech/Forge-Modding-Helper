using FMH.Workspace.Data;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System;

namespace FMH.Core.Model
{
    /// <summary>
    /// Recent workspace data
    /// </summary>
    public sealed class RecentWorkspace
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
    }
}
