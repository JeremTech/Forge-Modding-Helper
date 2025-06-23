using FMH.Utils.Files;
using FMH.Workspace.Data;
using FMH.Workspace.Provider;
using Microsoft.VisualBasic.FileIO;
using StringExtensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FMH.Workspace.WorkspaceManager.NeoForge
{
    /// <summary>
    /// Workspace manager for mod developed for Minecraft before 1.20 version
    /// </summary>
    public class NeoForgeWorkspaceManagerV1 : IWorkspaceManager
    {
        /// <summary>
        /// Workspace properties
        /// </summary>
        public WorkspaceProperties WorkspaceProperties { get; set; }

        /// <summary>
        /// Mod properties
        /// </summary>
        public ModProperties ModProperties { get; set; }

        /// <summary>
        /// Mod version history
        /// </summary>
        public ModVersionsHistory ModVersionsHistory { get; set; }

        /// <summary>
        /// Assets properties
        /// </summary>
        public AssetsProperties AssetsProperties { get; set; }

        /// <summary>
        /// Source code properties
        /// </summary>
        public SourceCodeProperties SourceCodeProperties { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="workspacePath">Worskpace path</param>
        public NeoForgeWorkspaceManagerV1(string workspacePath)
        {
            WorkspaceProperties = new WorkspaceProperties();
            ModProperties = new ModProperties();
            ModVersionsHistory = new ModVersionsHistory(workspacePath);
            AssetsProperties = new AssetsProperties(workspacePath);
            SourceCodeProperties = new SourceCodeProperties(workspacePath);
        }

        /// <summary>
        /// Read data from build.gralde file
        /// </summary>
        /// <returns><c>true</c> if success, else <c>false</c></returns>
        public bool ReadBuildGradle()
        {
            string filePath = Path.Combine(WorkspaceProperties.WorkspacePath, "build.gradle");
            string fileContent = File.ReadAllText(filePath);

            try
            {
                ModProperties.ModVersion = fileContent.Between("version = '", "'", StringComparison.CurrentCulture);
                ModProperties.ModGroup = fileContent.Between("group = '", "'", StringComparison.CurrentCulture);
                ModProperties.ModAPIVersion = fileContent.Between("minecraft 'net.minecraftforge:forge:", "'", StringComparison.CurrentCulture);
                WorkspaceProperties.MCVersion = ModProperties.ModAPIVersion;
                ModProperties.ModMinecraftVersion = ModProperties.ModAPIVersion.Between("", "-", StringComparison.CurrentCulture);
                WorkspaceProperties.MCVersion = ModProperties.ModMinecraftVersion;

                // Mappings
                switch (fileContent.Between("mappings channel: '", "'", StringComparison.CurrentCulture))
                {
                    // Case 'snapshot' or 'stable', this is MCP mappings
                    case "snapshot":
                        ModProperties.ModMappingsVersion = fileContent.Between("mappings channel: 'snapshot', version: '", "'", StringComparison.CurrentCulture) + " (MCP)";
                        break;
                    case "stable":
                        ModProperties.ModMappingsVersion = fileContent.Between("mappings channel: 'stable', version: '", "'", StringComparison.CurrentCulture) + " (MCP)";
                        break;

                    // Case official, this is mojang mappings
                    case "official":
                        ModProperties.ModMappingsVersion = fileContent.Between("mappings channel: 'official', version: '", "'", StringComparison.CurrentCulture) + " (Mojang)";
                        break;
                }

                WorkspaceProperties.ModAPI = ModAPIType.NeoForge;

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Read data from gradle.properties file
        /// </summly>
        /// <returns><c>true</c> if success, else <c>false</c></returns>
        public bool ReadGradleProperties()
        {
            // No data to read for this file
            return true;
        }

        /// <summary>
        /// Read data from mod.toml file
        /// </summary>
        /// <returns><c>true</c> if success, else <c>false</c></returns>
        public bool ReadModToml()
        {
            string filePath = Path.Combine(WorkspaceProperties.WorkspacePath, @"src\main\resources\META-INF\mods.toml");
            string fileContent = File.ReadAllText(filePath);

            try
            {
                ModProperties.ModLicense = fileContent.Between("license=\"", "\"", StringComparison.CurrentCulture);
                ModProperties.ModID = fileContent.Between("modId=\"", "\"", StringComparison.CurrentCulture);
                ModProperties.ModName = fileContent.Between("displayName=\"", "\"", StringComparison.CurrentCulture);
                ModProperties.ModDescription = fileContent.Between("description='''", "'''", StringComparison.CurrentCulture)?.Trim();
                ModProperties.ModLogo = fileContent.Between("logoFile=\"", "\"", StringComparison.CurrentCulture);
                ModProperties.ModCredits = fileContent.Between("credits=\"", "\"", StringComparison.CurrentCulture);
                ModProperties.ModAuthors = fileContent.Between("authors=\"", "\"", StringComparison.CurrentCulture);
                ModProperties.ModWebsite = fileContent.Between("displayURL=\"", "\"", StringComparison.CurrentCulture);
                ModProperties.ModUpdateJSONURL = fileContent.Between("updateJSONURL=\"", "\"", StringComparison.CurrentCulture);
                ModProperties.ModIssueTracker = fileContent.Between("issueTrackerURL=\"", "\"", StringComparison.CurrentCulture);

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Write build.gradle file
        /// </summary>
        public void WriteBuildGradle()
        {
            string filePath = Path.Combine(WorkspaceProperties.WorkspacePath, "build.gradle");
            string[] lines = File.ReadAllLines(filePath);
            string[] output = new string[lines.Length];
            bool isDataSectionReaded = false;

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];

                if (line.Contains("version ="))
                {
                    string str = line.Between("version = '", "'", StringComparison.CurrentCulture);
                    output[i] = line.Replace(str, ModProperties.ModVersion);
                }
                else if (line.Contains("group ="))
                {
                    string str = line.Between("group = '", "'", StringComparison.CurrentCulture);
                    output[i] = line.Replace(str, ModProperties.ModGroup);
                }
                else if (line.Contains("archivesBaseName ="))
                {
                    string str = line.Between("archivesBaseName = '", "'", StringComparison.CurrentCulture);
                    output[i] = line.Replace(str, ModProperties.ModID);
                }
                else if (line.Contains("Specification-Title"))
                {
                    string str = line.Replace(" ", "");
                    str = str.Between("Specification-Title\":\"", "\"", StringComparison.CurrentCulture);
                    output[i] = "                " + line.Replace(" ", "").Replace(":", " : ").Replace(str, ModProperties.ModID);
                }
                else if (line.Contains("Specification-Vendor"))
                {
                    string str = line.Replace(" ", "");
                    str = str.Between("Specification-Vendor\":\"", "\"", StringComparison.CurrentCulture);
                    output[i] = "                " + line.Replace(" ", "").Replace(":", " : ").Replace(str, ModProperties.ModAuthors);
                }
                else if (line.Contains("Implementation-Vendor"))
                {
                    string str = line.Replace(" ", "");
                    str = str.Between("Implementation-Vendor\":\"", "\"", StringComparison.CurrentCulture);
                    output[i] = "                " + line.Replace(" ", "").Replace(":", " : ").Replace(str, ModProperties.ModAuthors);
                }
                else if (line.Contains("data {"))
                {
                    isDataSectionReaded = true;
                    output[i] = line;
                }
                else if (line.Contains("examplemod {") && isDataSectionReaded)
                {
                    output[i] = line.Replace("examplemod", ModProperties.ModID);
                }
                else
                {
                    output[i] = line;
                }
            }

            // Replace build.gradle file
            File.WriteAllLines(filePath, output);
        }

        /// <summary>
        /// Write gradle.properties file
        /// </summary>
        public void WriteGradleProperties()
        {
            // No data to write for this file
            return;
        }

        /// <summary>
        /// Write mod.toml file
        /// </summary>
        public void WriteModToml()
        {
            StringBuilder outputText = new StringBuilder();
            string filePath = Path.Combine(WorkspaceProperties.WorkspacePath, @"src\main\resources\META-INF\mods.toml");
            string forgeVersionShort = ModProperties.ModAPIVersion.Split('-')[1];
            string forgeVersionRange = forgeVersionShort.Split('.')[0];

            // General section
            outputText.AppendLine("modLoader=\"javafml\"");
            outputText.AppendLine(string.Format("loaderVersion=\"[{0},)\"", forgeVersionRange));
            outputText.AppendLine(string.Format("license=\"{0}\"", ModProperties.ModLicense));

            if (!string.IsNullOrEmpty(ModProperties.ModIssueTracker))
                outputText.AppendLine(string.Format("issueTrackerURL=\"{0}\"", ModProperties.ModIssueTracker));

            if (!string.IsNullOrEmpty(ModProperties.ModLogo))
                outputText.AppendLine("logoFile=\"logo.png\"");

            // Mod section
            outputText.AppendLine();
            outputText.AppendLine("[[mods]]");
            outputText.AppendLine(string.Format("modId=\"{0}\"", ModProperties.ModID));
            outputText.AppendLine("version=\"${file.jarVersion}\"");
            outputText.AppendLine(string.Format("displayName=\"{0}\"", ModProperties.ModName));

            if (!string.IsNullOrEmpty(ModProperties.ModUpdateJSONURL))
                outputText.AppendLine(string.Format("updateJSONURL=\"{0}\"", ModProperties.ModUpdateJSONURL));

            if (!string.IsNullOrEmpty(ModProperties.ModWebsite))
                outputText.AppendLine(string.Format("displayURL=\"{0}\"", ModProperties.ModWebsite));

            if (!string.IsNullOrEmpty(ModProperties.ModLogo))
                outputText.AppendLine("logoFile=\"logo.png\"");

            if (!string.IsNullOrEmpty(ModProperties.ModCredits))
                outputText.AppendLine(string.Format("credits=\"{0}\"", ModProperties.ModCredits));

            if (!string.IsNullOrEmpty(ModProperties.ModAuthors))
                outputText.AppendLine(string.Format("authors=\"{0}\"", ModProperties.ModAuthors));

            if(!string.IsNullOrEmpty(ModProperties.ModDescription))
                outputText.AppendLine("description='''")
                          .AppendLine(ModProperties.ModDescription.Trim())
                          .AppendLine("'''");

            // Dependencies section
            outputText.AppendLine();
            outputText.AppendLine(string.Format("[[dependencies.{0}]]", ModProperties.ModID));
            outputText.AppendLine("\tmodId=\"forge\"");
            outputText.AppendLine("\tmandatory=true");
            outputText.AppendLine(string.Format("\tversionRange=\"[{0},)\"", forgeVersionRange));
            outputText.AppendLine("\tordering=\"NONE\"");
            outputText.AppendLine("\tside=\"BOTH\"");
            outputText.AppendLine();
            outputText.AppendLine(string.Format("[[dependencies.{0}]]", ModProperties.ModID));
            outputText.AppendLine("\tmodId=\"minecraft\"");
            outputText.AppendLine("\tmandatory=true");
            outputText.AppendLine(string.Format("\tversionRange=\"[{0}]\"", ModProperties.ModMinecraftVersion));
            outputText.AppendLine("\tordering=\"NONE\"");
            outputText.AppendLine("\tside=\"BOTH\"");

            // Replace mod.toml file
            File.WriteAllText(filePath, outputText.ToString());
        }

        /// <summary>
        /// Check if the workspace is valid for Forge Modding Helper
        /// </summary>
        /// <param name="supportedMinecraftVersions">Supported minecraft versions</param>
        /// <returns><c>true</c> if this is a valid workspace, else <c>false</c></returns>
        public bool CheckWorkspaceValidity(List<string> supportedMinecraftVersions)
        {
            // Check configs files
            if (!File.Exists(Path.Combine(WorkspaceProperties.WorkspacePath, @"src\main\resources\META-INF\mods.toml"))
                || !File.Exists(Path.Combine(WorkspaceProperties.WorkspacePath, "build.gradle")))
                return false;

            // Read files
            if (!ReadModToml()
                || !ReadBuildGradle())
                return false;

            // Check mandatory data
            if (string.IsNullOrEmpty(ModProperties.ModID)
                || string.IsNullOrEmpty(ModProperties.ModGroup)
                || string.IsNullOrEmpty(ModProperties.ModMinecraftVersion)
                || string.IsNullOrEmpty(ModProperties.ModMappingsVersion)
                || string.IsNullOrEmpty(ModProperties.ModVersion)
                || string.IsNullOrEmpty(ModProperties.ModName)
                || string.IsNullOrEmpty(ModProperties.ModAPIVersion))
                return false;

            // Check Minecraft version
            if (!supportedMinecraftVersions.Contains(ModProperties.ModMinecraftVersion))
                return false;

            return true;   
        }

        /// <inheritdoc/>
        public async Task DownloadMDK(IProgress<double> progressReportingObject)
        {
            var mdkLink = MDKProvider.GetNeoForgeMDKLink(WorkspaceProperties.MCVersion);
            var outputFilePath = Path.Combine(WorkspaceProperties.WorkspacePath, "mdk.zip");

            await FileUtils.DownloadFileAsync(mdkLink, outputFilePath, progressReportingObject);
        }

        /// <inheritdoc/>
        public async Task ExtractMDK(IProgress<double> progressReportingObject)
        {
            var mdkZipPath = Path.Combine(WorkspaceProperties.WorkspacePath, "mdk.zip");
            var mdkExtractPath = WorkspaceProperties.WorkspacePath;

            // Uncompress the MDK zip file
            await FileUtils.UncompressSubFolderInFileAsync(mdkZipPath, mdkExtractPath, string.Format("MDK-{0}-NeoGradle-main", WorkspaceProperties.MCVersion), progressReportingObject);

            // Delete MDK zip file permanently
            FileSystem.DeleteFile(mdkZipPath, UIOption.OnlyErrorDialogs, RecycleOption.DeletePermanently);
        }
    }
}
