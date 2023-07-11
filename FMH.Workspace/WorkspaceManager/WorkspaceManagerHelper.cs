using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMH.Workspace.Data;

namespace FMH.Workspace.WorkspaceManager
{
    public static class WorkspaceManagerHelper
    {
        /// <summary>
        /// Return the corresponding workspace manager for the specified Minecraft version
        /// </summary>
        /// <param name="mcVersion">Targeted minecraft version</param>
        /// <param name="workspacePath">Workspace path</param>
        /// <returns>Corresponding workspace manager, <c>null</c> if not found</returns>
        public static IWorkspaceManager GetWorkspaceManager(string mcVersion, string workspacePath)
        {
            IWorkspaceManager workspaceManager;

            switch(mcVersion)
            {
                case "1.20":
                case "1.20.1":
                    workspaceManager = new WorkspaceManagerV2();
                    break;

                default:
                    workspaceManager = new WorkspaceManagerV1();
                    break;
            }

            // Set default values
            workspaceManager.WorkspaceProperties = new WorkspaceProperties()
            {
                WorkspacePath = workspacePath
            };
            workspaceManager.ModProperties = new ModProperties();
            workspaceManager.ModVersionsHistory = new ModVersionsHistory(workspacePath);

            return workspaceManager;
        }
    }
}
