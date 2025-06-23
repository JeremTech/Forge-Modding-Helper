using FMH.Utils.Files;
using FMH.Workspace.Data;
using FMH.Workspace.Provider;
using Microsoft.VisualBasic.FileIO;
using StringExtensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FMH.Workspace.WorkspaceManager.Forge
{
    /// <summary>
    /// Workspace manager for mod developed for Minecraft 1.20 and older
    /// </summary>
    public class ForgeWorkspaceManagerV2 : IWorkspaceManager
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
        public ForgeWorkspaceManagerV2(string workspacePath)
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
            // No data to read for this file
            return true;
        }

        /// <summary>
        /// Read data from gradle.properties file
        /// </summary>
        /// <returns><c>true</c> if success, else <c>false</c></returns>
        public bool ReadGradleProperties()
        {
            string filePath = Path.Combine(WorkspaceProperties.WorkspacePath, "gradle.properties");
            string[] fileLines = File.ReadAllLines(filePath);

            try
            {
                foreach (var line in fileLines) 
                {
                    var infoLine = line.Split("=");

                    if (string.Equals(infoLine[0], "minecraft_version"))
                    {
                        ModProperties.ModMinecraftVersion = line.Split('=')[1]
                                                                     .Replace("[", string.Empty)
                                                                     .Replace("]", string.Empty)
                                                                     .Replace("(", string.Empty)
                                                                     .Replace(")", string.Empty)
                                                                     .Replace(",", string.Empty);
                        WorkspaceProperties.MCVersion = ModProperties.ModMinecraftVersion;
                    }

                    if (string.Equals(infoLine[0], "forge_version"))
                    {
                        ModProperties.ModAPIVersion = line.Split('=')[1]
                                                               .Replace("[", string.Empty)
                                                               .Replace("]", string.Empty)
                                                               .Replace("(", string.Empty)
                                                               .Replace(")", string.Empty)
                                                               .Replace(",", string.Empty);

                        WorkspaceProperties.APIVersion = ModProperties.ModAPIVersion;
                    }

                    if (string.Equals(infoLine[0], "mapping_version"))
                        ModProperties.ModMappingsVersion = line.Split('=')[1];

                    if (string.Equals(infoLine[0], "mod_id"))
                        ModProperties.ModID = line.Split('=')[1];

                    if (string.Equals(infoLine[0], "mod_name"))
                        ModProperties.ModName = line.Split('=')[1];

                    if (string.Equals(infoLine[0], "mod_license"))
                        ModProperties.ModLicense = line.Split('=')[1];

                    if (string.Equals(infoLine[0], "mod_version"))
                        ModProperties.ModVersion = line.Split('=')[1];

                    if (string.Equals(infoLine[0], "mod_group_id"))
                        ModProperties.ModGroup = line.Split('=')[1];

                    if (string.Equals(infoLine[0], "mod_authors"))
                        ModProperties.ModAuthors = line.Split('=')[1];

                    if (string.Equals(infoLine[0], "mod_description"))
                        ModProperties.ModDescription = line.Split('=')[1];
                }

                WorkspaceProperties.ModAPI = ModAPIType.Forge;

                return true;
            }
            catch
            {
                return false;
            }
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
                ModProperties.ModIssueTracker = fileContent.Between("issueTrackerURL=\"", "\"", StringComparison.CurrentCulture);
                ModProperties.ModUpdateJSONURL = fileContent.Between("updateJSONURL=\"", "\"", StringComparison.CurrentCulture);
                ModProperties.ModWebsite = fileContent.Between("displayURL=\"", "\"", StringComparison.CurrentCulture);
                ModProperties.ModLogo = fileContent.Between("logoFile=\"", "\"", StringComparison.CurrentCulture);
                ModProperties.ModCredits = fileContent.Between("credits=\"", "\"", StringComparison.CurrentCulture);
                ModProperties.ModAuthors = fileContent.Between("authors=\"", "\"", StringComparison.CurrentCulture);

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
        /// <returns><c>true</c> if success, else <c>false</c></returns>
        public void WriteBuildGradle()
        {
            // No data to write for this file
            return;
        }

        /// <summary>
        /// Write gradle.properties file
        /// </summary>
        public void WriteGradleProperties()
        {
            StringBuilder outputText = new StringBuilder();
            string filePath = Path.Combine(WorkspaceProperties.WorkspacePath, "gradle.properties");
            string forgeVersionShort = ModProperties.ModAPIVersion;
            string forgeVersionRange = forgeVersionShort.Split('.')[0];

            // Default memory used for gradle commands
            outputText.AppendLine("org.gradle.jvmargs=-Xmx3G");
            outputText.AppendLine("org.gradle.daemon=false");
            outputText.AppendLine();

            // Environment Properties
            outputText.AppendLine(string.Format("minecraft_version={0}", ModProperties.ModMinecraftVersion));
            outputText.AppendLine(string.Format("minecraft_version_range=[{0}]", ModProperties.ModMinecraftVersion));
            outputText.AppendLine(string.Format("forge_version={0}", forgeVersionShort));
            outputText.AppendLine(string.Format("forge_version_range=[{0},)", forgeVersionRange));
            outputText.AppendLine(string.Format("loader_version_range=[{0},)", forgeVersionRange));
            outputText.AppendLine(string.Format("mapping_channel={0}", "official"));
            outputText.AppendLine(string.Format("mapping_version={0}", ModProperties.ModMinecraftVersion));
            outputText.AppendLine(string.Format("mod_id={0}", ModProperties.ModID));
            outputText.AppendLine(string.Format("mod_name={0}", ModProperties.ModName));
            outputText.AppendLine(string.Format("mod_license={0}", ModProperties.ModLicense));
            outputText.AppendLine(string.Format("mod_version={0}", ModProperties.ModVersion));
            outputText.AppendLine(string.Format("mod_group_id={0}", ModProperties.ModGroup));
            outputText.AppendLine(string.Format("mod_authors={0}", ModProperties.ModAuthors));
            outputText.AppendLine(string.Format("mod_description={0}", ModProperties.ModDescription));

            // Replace gradle.properties file
            File.WriteAllText(filePath, outputText.ToString());
        }

        /// <summary>
        /// Write mod.toml file
        /// </summary>
        public void WriteModToml()
        {
            StringBuilder outputText = new StringBuilder();
            string filePath = Path.Combine(WorkspaceProperties.WorkspacePath, @"src\main\resources\META-INF\mods.toml");

            // General section
            outputText.AppendLine("modLoader=\"javafml\"");
            outputText.AppendLine("loaderVersion=\"${loader_version_range}\"");
            outputText.AppendLine("license=\"${mod_license}\"");

            if (!string.IsNullOrEmpty(ModProperties.ModIssueTracker))
                outputText.AppendLine(string.Format("issueTrackerURL=\"{0}\"", ModProperties.ModIssueTracker));

            // Mod section
            outputText.AppendLine();
            outputText.AppendLine("[[mods]]");
            outputText.AppendLine("modId=\"${mod_id}\"");
            outputText.AppendLine("version=\"${mod_version}\"");
            outputText.AppendLine("displayName=\"${mod_name}\"");

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

            outputText.AppendLine("description='''${mod_description}'''");

            // Dependencies section
            outputText.AppendLine();
            outputText.AppendLine("[[dependencies.${mod_id}]]");
            outputText.AppendLine("\tmodId=\"forge\"");
            outputText.AppendLine("\tmandatory=true");
            outputText.AppendLine("\tversionRange=\"${forge_version_range}\"");
            outputText.AppendLine("\tordering=\"NONE\"");
            outputText.AppendLine("\tside=\"BOTH\"");
            outputText.AppendLine();
            outputText.AppendLine("[[dependencies.${mod_id}]]");
            outputText.AppendLine("\tmodId=\"minecraft\"");
            outputText.AppendLine("\tmandatory=true");
            outputText.AppendLine("\tversionRange=\"${minecraft_version_range}\"");
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
                || !File.Exists(Path.Combine(WorkspaceProperties.WorkspacePath, "gradle.properties")))
                return false;

            // Read files
            if (!ReadModToml() 
                || !ReadGradleProperties())
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
            var mdkLink = MDKProvider.GetMinecraftForgeMDKLink(WorkspaceProperties.APIVersion);
            var outputFilePath = Path.Combine(WorkspaceProperties.WorkspacePath, "mdk.zip");

            await FileUtils.DownloadFileAsync(mdkLink, outputFilePath, progressReportingObject);
        }

        /// <inheritdoc/>
        public async Task ExtractMDK(IProgress<double> progressReportingObject)
        {
            var mdkZipPath = Path.Combine(WorkspaceProperties.WorkspacePath, "mdk.zip");
            var mdkExtractPath = WorkspaceProperties.WorkspacePath;

            // Uncompress the MDK zip file
            await FileUtils.UncompressFileAsync(mdkZipPath, mdkExtractPath, progressReportingObject);

            // Delete MDK zip file permanently
            FileSystem.DeleteFile(mdkZipPath, UIOption.OnlyErrorDialogs, RecycleOption.DeletePermanently);
        }
    }
}
