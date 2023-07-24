using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMH.Workspace.Data;
using Newtonsoft.Json;

namespace FMH.Workspace.WorkspaceManager
{
    public static class WorkspaceManagerHelper
    {
        /// <summary>
        /// Return the corresponding workspace manager
        /// </summary>
        /// <param name="workspacePath">Workspace path</param>
        /// <returns>Corresponding workspace manager, <c>null</c> if not found</returns>
        public static IWorkspaceManager GetWorkspaceManager(string workspacePath)
        {
            IWorkspaceManager workspaceManager;
            WorkspaceProperties workspaceProperties = ReadProjectFile(workspacePath);
            workspaceProperties.WorkspacePath = workspacePath;

            switch (workspaceProperties.MCVersion)
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
            workspaceManager.WorkspaceProperties = workspaceProperties;
            workspaceManager.ModProperties = new ModProperties();
            workspaceManager.SourceCodeProperties = new SourceCodeProperties(workspacePath);
            workspaceManager.AssetsProperties = new AssetsProperties(workspacePath);
            workspaceManager.ModVersionsHistory = new ModVersionsHistory(workspacePath);

            return workspaceManager;
        }

        /// <summary>
        /// Return the corresponding workspace manager for the specified Minecraft version
        /// </summary>
        /// <param name="mcVersion">Targeted minecraft version</param>
        /// <param name="workspacePath">Workspace path</param>
        /// <returns>Corresponding workspace manager, <c>null</c> if not found</returns>
        public static IWorkspaceManager GetWorkspaceManager(string mcVersion, string workspacePath)
        {
            IWorkspaceManager workspaceManager;

            switch (mcVersion)
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
                WorkspacePath = workspacePath,
                MCVersion = mcVersion
            };
            workspaceManager.ModProperties = new ModProperties();
            workspaceManager.SourceCodeProperties = new SourceCodeProperties(workspacePath);
            workspaceManager.AssetsProperties = new AssetsProperties(workspacePath);
            workspaceManager.ModVersionsHistory = new ModVersionsHistory(workspacePath);

            return workspaceManager;
        }

        /// <summary>
        /// Write workspace data files
        /// </summary>
        /// <param name="workspaceData">Workspace data</param>
        public static void WriteWorkspaceData(IWorkspaceManager workspaceData)
        {
            try
            {
                string fmhDirectoryPath = Path.Combine(workspaceData.WorkspaceProperties.WorkspacePath, "fmh");
                string jsonContent = string.Empty;

                // Creating 'fmh' folder
                if (!Directory.Exists(fmhDirectoryPath))
                    Directory.CreateDirectory(fmhDirectoryPath);

                // Writing mod infos
                jsonContent = JsonConvert.SerializeObject(workspaceData.ModProperties, Formatting.Indented);
                File.WriteAllText(Path.Combine(fmhDirectoryPath, "mod_infos.json"), jsonContent);

                // Writing textures list
                jsonContent = JsonConvert.SerializeObject(workspaceData.AssetsProperties.TexturesFiles, Formatting.Indented);
                File.WriteAllText(Path.Combine(fmhDirectoryPath, "textures_list.json"), jsonContent);

                // Writing blockstates list
                jsonContent = JsonConvert.SerializeObject(workspaceData.AssetsProperties.BlockstatesFiles, Formatting.Indented);
                File.WriteAllText(Path.Combine(fmhDirectoryPath, "blockstates_list.json"), jsonContent);

                // Writing models list
                jsonContent = JsonConvert.SerializeObject(workspaceData.AssetsProperties.ModelsFiles, Formatting.Indented);
                File.WriteAllText(Path.Combine(fmhDirectoryPath, "models_list.json"), jsonContent);

                // Writing java files list
                jsonContent = JsonConvert.SerializeObject(workspaceData.SourceCodeProperties.JavaFiles, Formatting.Indented);
                File.WriteAllText(Path.Combine(fmhDirectoryPath, "java_files_list.json"), jsonContent);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static void WriteProjectFile(WorkspaceProperties workspaceProperties)
        {
            try
            {
                string fmhDirectoryPath = Path.Combine(workspaceProperties.WorkspacePath, "fmh");
                string fmhProjectFilePath = Path.Combine(fmhDirectoryPath, "project.fmh");

                if (!Directory.Exists(fmhDirectoryPath))
                    Directory.CreateDirectory(fmhDirectoryPath);

                File.WriteAllText(fmhProjectFilePath, JsonConvert.SerializeObject(workspaceProperties, Formatting.Indented));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static WorkspaceProperties ReadProjectFile(string workspacePath)
        {
            try
            {
                string fmhDirectoryPath = Path.Combine(workspacePath, "fmh");
                string fmhProjectFilePath = Path.Combine(fmhDirectoryPath, "project.fmh");

                if (!Directory.Exists(fmhDirectoryPath) || !File.Exists(fmhProjectFilePath))
                    return null;

                return JsonConvert.DeserializeObject<WorkspaceProperties>(File.ReadAllText(fmhProjectFilePath));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }

        }
    }
}
