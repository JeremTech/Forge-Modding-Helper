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
        public static IEnumerable<RecentWorkspace> GetRecentsWorkspaces() 
        {
            try
            {
                var historyFileContent = File.ReadAllText(_filePath);
                var historyFileDeserialized = JsonConvert.DeserializeObject<List<RecentWorkspace>>(historyFileContent);

                if (historyFileDeserialized != null)
                {
                    CompleteRecentWorkspaceData(historyFileDeserialized);
                    return historyFileDeserialized.OrderByDescending(w => w.LastUpdated);
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

        /// <summary>
        /// Add a recent workspace to list
        /// </summary>
        /// <param name="recentWorkspace">Recent workspace to add</param>
        public static void AddRecentWorkspace(RecentWorkspace recentWorkspace)
        {
            var currentRecentsWorkspacesList = GetRecentsWorkspaces().ToList();
            currentRecentsWorkspacesList.Add(recentWorkspace);
            
            WriteRecentsWorkspaces(currentRecentsWorkspacesList);
        }

        /// <summary>
        /// Delete a recent workspace from list
        /// </summary>
        /// <param name="recentWorkspace">Recent workspace to remove</param>
        public static void RemoveRecentWorkspace(RecentWorkspace recentWorkspace)
        {
            var currentRecentsWorkspacesList = GetRecentsWorkspaces().ToList();
            currentRecentsWorkspacesList.RemoveAll(w => string.Equals(w.WorkspacePath, recentWorkspace.WorkspacePath));

            WriteRecentsWorkspaces(currentRecentsWorkspacesList);
        }

        /// <summary>
        /// Update modification date of a recent workspace
        /// </summary>
        /// <param name="recentWorkspace">Recent workspace to update</param>
        /// <param name="newDateTime">New modification date to apply to the recent workspace</param>
        public static void UpdateRecentWorkspaceModificationDate(RecentWorkspace recentWorkspace, DateTime newDateTime)
        {
            var currentRecentsWorkspacesList = GetRecentsWorkspaces().ToList();
            
            // Remove current recent workspace's data
            currentRecentsWorkspacesList.RemoveAll(w => string.Equals(w.WorkspacePath, recentWorkspace.WorkspacePath));

            // Update date
            recentWorkspace.LastUpdated = newDateTime;

            // Add to list and write new data
            currentRecentsWorkspacesList.Add(recentWorkspace);
            WriteRecentsWorkspaces(currentRecentsWorkspacesList);
        }

        /// <summary>
        /// Write recents worskpaces list to disk
        /// </summary>
        /// <param name="recentWorkspacesList">Recents workspaces list to write</param>
        private static void WriteRecentsWorkspaces(IEnumerable<RecentWorkspace> recentWorkspacesList)
        {
            try
            {
                var historyFileSerialized = JsonConvert.SerializeObject(recentWorkspacesList.OrderByDescending(w => w.LastUpdated));
                File.WriteAllText(_filePath, historyFileSerialized);
            }
            catch { }
        }
    }
}
