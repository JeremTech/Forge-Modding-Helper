using Forge_Modding_Helper_3.Files.Workspace_Data;
using Forge_Modding_Helper_3.Objects;
using Forge_Modding_Helper_3.Utils;
using Forge_Modding_Helper_3.Windows;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forge_Modding_Helper_3.Files.Software
{
    /// <summary>
    /// Last workspaces storage
    /// </summary>
    public class LastWorkspaces
    {
        /// <summary>
        /// List of last workspaces paths
        /// </summary>
        public static List<Workspace> LastWorkspacesData = new List<Workspace>();

        /// <summary>
        /// List of last workspaces project file
        /// </summary>
        public static List<ProjectFile> LastWorkspacesProjectFile = new List<ProjectFile>();

        /// <summary>
        /// File path
        /// </summary>
        private static string FilePath = Path.Combine(AppInfos.getApplicationDataDirectory(), "workspaces.json");

        /// <summary>
        /// Add a workspace to the last workspaces list. If it already exist, it's updated
        /// </summary>
        /// <param name="workspace">Workspace to add</param>
        public static void AddRecentWorkspace(Workspace workspace)
        {
            int lenght = LastWorkspacesData.Count;
            Workspace[] workspaces = new Workspace[lenght];
            LastWorkspacesData.CopyTo(workspaces, 0);

            foreach (Workspace work in workspaces)
            {
                if (work.path == workspace.path)
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
            if(File.Exists(FilePath))
            {
                List<Workspace> jsonData = JsonConvert.DeserializeObject<List<Workspace>>(File.ReadAllText(FilePath));

                if (jsonData.Count > 0)
                {
                    LastWorkspacesData = jsonData.OrderByDescending(element => element.lastUpdated).ToList();
                }

                // Clear project files data
                LastWorkspacesProjectFile.Clear();

                // Read project files of each recents workspaces
                foreach(Workspace _workspace in LastWorkspacesData)
                {
                    // Create project file if project file doesn't exist (workspaces from previous version of FMH)
                    if(!File.Exists(Path.Combine(_workspace.path, "fmh\\project.fmh")) && DirectoryUtils.CheckFolderIsForgeWorkspace(_workspace.path))
                    {
                        new ProjectScanWindow(_workspace.path, false).ShowDialog();
                    }

                    // Check if project folder is always correct and if it's not a duplication
                    if (DirectoryUtils.CheckFolderIsForgeWorkspace(_workspace.path) && !LastWorkspacesProjectFile.Exists(wp => wp.ProjectPath == _workspace.path)) 
                    {
                        LastWorkspacesProjectFile.Add(JsonConvert.DeserializeObject<ProjectFile>(File.ReadAllText(Path.Combine(_workspace.path, "fmh\\project.fmh"))));
                    }
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
                File.WriteAllText(FilePath, JsonConvert.SerializeObject(LastWorkspacesData.OrderByDescending(element => element.lastUpdated), Formatting.Indented));
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

            List<Workspace> workspaces = new List<Workspace>();

            foreach (Workspace workspace in LastWorkspacesData)
            {
                // Check directory validity and if workspace is not already registered
                if (Directory.Exists(workspace.path) && DirectoryUtils.CheckFolderIsForgeWorkspace(workspace.path) && !workspaces.Exists(w => w.path == workspace.path))
                {
                    workspaces.Add(new Workspace(workspace.path, workspace.lastUpdated));
                }
            }

            // Overwrite recents workspaces
            LastWorkspacesData = workspaces;

            // Write new data file
            WriteData();
        }
    }
}
