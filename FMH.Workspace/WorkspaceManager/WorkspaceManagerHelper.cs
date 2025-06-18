using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMH.Workspace.Data;
using FMH.Workspace.WorkspaceManager.Forge;
using Newtonsoft.Json;

namespace FMH.Workspace.WorkspaceManager
{
    public static class WorkspaceManagerHelper
    {
        /// <summary>
        /// Return the corresponding workspace manager
        /// </summary>
        /// <param name="workspacePath">Workspace path</param>
        /// <param name="mcVersion">Workspace Minecraft version</param>
        /// <param name="modAPI">Workspace modding API</param>
        /// <returns>Corresponding workspace manager</returns>
        /// <exception cref="NotSupportedException">No workspace manager founded for specified settings</exception>
        public static IWorkspaceManager GetWorkspaceManager(string workspacePath, string mcVersion, ModAPIType modAPI)
        {
            IWorkspaceManager workspaceManager;

            switch(modAPI)
            {
                case ModAPIType.Forge:
                    workspaceManager = GetForgeWorkspaceManager(workspacePath, mcVersion);
                    break;
                default:
                    throw new NotSupportedException($"Mod API '{modAPI}' is not supported.");
            }

            // Defined knowed workspace properties
            workspaceManager.WorkspaceProperties.WorkspacePath = workspacePath;
            workspaceManager.WorkspaceProperties.MCVersion = mcVersion;
            workspaceManager.WorkspaceProperties.ModAPI = modAPI;

            return workspaceManager;
        }

        /// <summary>
        /// Return the corresponding workspace manager for a Forge workspace
        /// </summary>
        /// <param name="mcVersion">Workspace Minecraft version</param>
        /// <param name="workspacePath">Workspace path</param>
        /// <returns>Corresponding Forge workspace manager</returns>
        private static IWorkspaceManager GetForgeWorkspaceManager(string workspacePath, string mcVersion)
        {
            switch (mcVersion)
            {
                case "1.20":
                case "1.20.1":
                case "1.20.2":
                case "1.20.4":
                case "1.20.6":
                case "1.21":
                case "1.21.1":
                case "1.21.3":
                case "1.21.4":
                case "1.21.5":
                    return new ForgeWorkspaceManagerV2(workspacePath);
                default:
                    return new ForgeWorkspaceManagerV1(workspacePath);
            }
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
