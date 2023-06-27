using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMH.Workspace.Entities;

namespace FMH.Workspace.WorkspaceManager
{
    public class WorkspaceManagerHelper
    {
        /// <summary>
        /// Return the corresponding workspace manager for the specified Minecraft version
        /// </summary>
        /// <param name="mcVersion">Targeted minecraft version</param>
        /// <returns>Corresponding workspace manager, <c>null</c> if not found</returns>
        public IWorkspaceManager GetWorkspaceManager(string mcVersion)
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
            workspaceManager.WorkspaceProperties = new WorkspaceProperties();
            workspaceManager.ModProperties = new ModProperties();

            return workspaceManager;
        }
    }
}
