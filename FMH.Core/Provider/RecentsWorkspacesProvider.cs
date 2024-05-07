using FMH.Core.Model;
using FMH.Core.Objects;
using FMH.Core.Utils.Software;
using FMH.Workspace.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMH.Core.Provider
{
    /// <summary>
    /// Provider for recents workspaces
    /// </summary>
    public static class RecentsWorkspacesProvider
    {
        /// <summary>
        /// Recents workspaces history file
        /// </summary>
        private static string _filePath = Path.Combine(SoftwareDataManager.GetCurrentVersionDataDirectory(), "workspaces.json");

        /// <summary>
        /// Get all recents workspaces
        /// </summary>
        /// <returns>List of recents workspaces</returns>
        public static IEnumerable<RecentWorkspace> GetRecentWorkspaces() 
        {
            try
            {
                var historyFileContent = File.ReadAllText(_filePath);
                var historyFileDeserialized = JsonConvert.DeserializeObject<List<RecentWorkspace>>(historyFileContent);

                if (historyFileDeserialized != null)
                {
                    CompleteRecentWorkspaceData(historyFileDeserialized);
                    return historyFileDeserialized?.OrderByDescending(w => w.LastUpdated);
                }

            }
            catch { }

            return new List<RecentWorkspace>();

        }

        /// <summary>
        /// Complete a list of recents workspaces data with additional infos
        /// </summary>
        /// <param name="recentWorkspacesList">Raw recents workspaces data</param>
        private static void CompleteRecentWorkspaceData(IEnumerable<RecentWorkspace> recentWorkspacesList)
        {
            foreach(var workspace in recentWorkspacesList)
            {
                // Set workspace name
                workspace.WorkspaceName = Path.GetFileName(workspace.WorkspacePath);

                // Set workspace logo
                switch (workspace.WorkspaceModAPI)
                {
                    case ModAPIType.Forge:
                        workspace.WorkspaceLogoPath = new Uri("/FMH.Resources;component/Pictures/forge_logo.png", UriKind.Relative).ToString();
                        break;

                    default:
                        workspace.WorkspaceLogoPath = new Uri("/FMH.Resources;component/Pictures/icon.png", UriKind.Relative).ToString();
                        break;
                }
            }
        }
    }
}
