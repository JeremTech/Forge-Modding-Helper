using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMH.Core.Objects;
using FMH.Core.Utils;
using FMH.Workspace.Data;
using Newtonsoft.Json;

namespace FMH.Core.Files.Software
{
    public class LastWorkspaces
    {
        /// <summary>
        /// List of last workspaces paths
        /// </summary>
        public static List<WorkspaceEntry> LastWorkspacesData = new List<WorkspaceEntry>();

        /// <summary>
        /// List of last workspaces properties
        /// </summary>
        public static List<WorkspaceProperties> LastWorkspacesProjectFile = new List<WorkspaceProperties>();

        /// <summary>
        /// File path
        /// </summary>
        private static string FilePath = Path.Combine(App.getApplicationDataDirectory(), "workspaces.json");

        /// <summary>
        /// Add a workspace to the last workspaces list. If it already exist, it's updated
        /// </summary>
        /// <param name="workspace">Workspace to add</param>
        public static void AddRecentWorkspace(WorkspaceEntry workspace)
        {
            int lenght = LastWorkspacesData.Count;
            WorkspaceEntry[] workspaces = new WorkspaceEntry[lenght];
            LastWorkspacesData.CopyTo(workspaces, 0);

            foreach (WorkspaceEntry work in workspaces)
            {
                if (work.WorkspacePath == workspace.WorkspacePath)
                {
                    LastWorkspacesData.Remove(work);
                    break;
                }
            }

            LastWorkspacesData.Add(workspace);
            WriteData();
        }

        /// <summary>
        /// Read file
        /// </summary>
        public static void ReadData()
        {
            // Check if file exist
            if (File.Exists(FilePath))
            {
                List<WorkspaceEntry> jsonData = JsonConvert.DeserializeObject<List<WorkspaceEntry>>(File.ReadAllText(FilePath));

                if (jsonData.Count > 0)
                {
                    LastWorkspacesData = jsonData.OrderByDescending(element => element.LastUpdated).ToList();
                }
            }
        }

        /// <summary>
        /// Write file
        /// </summary>
        public static void WriteData()
        {
            // If there are last workspaces
            if (LastWorkspacesData.Count > 0)
            {
                File.WriteAllText(FilePath, JsonConvert.SerializeObject(LastWorkspacesData.OrderByDescending(element => element.LastUpdated), Formatting.Indented));
            }
        }

        /// <summary>
        /// Refresh recents workspaces list
        /// Remove unreachable workspaces
        /// </summary>
        public static void RefreshData()
        {
            // Read data file
            ReadData();

            List<WorkspaceEntry> workspaces = new List<WorkspaceEntry>();

            foreach (WorkspaceEntry workspace in LastWorkspacesData)
            {
                // Check directory validity and if workspace is not already registered
                if (Directory.Exists(workspace.WorkspacePath) && DirectoryUtils.CheckFolderIsForgeWorkspace(workspace.WorkspacePath) && !workspaces.Exists(w => w.WorkspacePath == workspace.WorkspacePath))
                {
                    workspaces.Add(new WorkspaceEntry(workspace.WorkspacePath, workspace.LastUpdated, workspace.WorkspaceModAPI, workspace.WorkspaceMcVersion));
                }
            }

            // Overwrite recents workspaces
            LastWorkspacesData.Clear();
            LastWorkspacesData.AddRange(workspaces);

            // Write new data file
            WriteData();
        }
    }
}
