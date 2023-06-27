﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FMH.Workspace.Entities;
using StringExtensions;

namespace FMH.Workspace.WorkspaceManager
{
    /// <summary>
    /// Workspace manager for mod developed for Minecraft before 1.20 version
    /// </summary>
    public class WorkspaceManagerV1 : IWorkspaceManager
    {
        public WorkspaceProperties WorkspaceProperties { get; set; }
        public ModProperties ModProperties { get; set; }

        /// <summary>
        /// Read data from build.gralde file
        /// </summary>
        public void ReadBuildGradle()
        {
            string filePath = Path.Combine(WorkspaceProperties.WorkspacePath, "build.gradle");
            string fileContent = File.ReadAllText(filePath);

            this.ModProperties.ModVersion = fileContent.Between("version = '", "'", StringComparison.CurrentCulture);
            this.ModProperties.ModGroup = fileContent.Between("group = '", "'", StringComparison.CurrentCulture);
            this.ModProperties.ModAPIVersion = fileContent.Between("minecraft 'net.minecraftforge:forge:", "'", StringComparison.CurrentCulture);
            this.ModProperties.ModMinecraftVersion = this.ModProperties.ModAPIVersion.Between("", "-", StringComparison.CurrentCulture);

            // Mappings
            switch (fileContent.Between("mappings channel: '", "'", StringComparison.CurrentCulture))
            {
                // Case 'snapshot' or 'stable', this is MCP mappings
                case "snapshot":
                    this.ModProperties.ModMappingsVersion = fileContent.Between("mappings channel: 'snapshot', version: '", "'", StringComparison.CurrentCulture) + " (MCP)";
                    break;
                case "stable":
                    this.ModProperties.ModMappingsVersion = fileContent.Between("mappings channel: 'stable', version: '", "'", StringComparison.CurrentCulture) + " (MCP)";
                    break;

                // Case official, this is mojang mappings
                case "official":
                    this.ModProperties.ModMappingsVersion = fileContent.Between("mappings channel: 'official', version: '", "'", StringComparison.CurrentCulture) + " (Mojang)";
                    break;
            }
        }

        /// <summary>
        /// Read data from gradle.properties file
        /// </summary>
        public void ReadGradleProperties()
        {
            // No data to read for this file
            return;
        }

        /// <summary>
        /// Read data from mod.toml file
        /// </summary>
        public void ReadModToml()
        {
            string filePath = Path.Combine(WorkspaceProperties.WorkspacePath, @"src\main\resources\META-INF\mods.toml");
            string fileContent = File.ReadAllText(filePath);

            this.ModProperties.ModLicense = fileContent.Between("license=\"", "\"", StringComparison.CurrentCulture);
            this.ModProperties.ModID = fileContent.Between("modId=\"", "\"", StringComparison.CurrentCulture);
            this.ModProperties.ModName = fileContent.Between("displayName=\"", "\"", StringComparison.CurrentCulture);
            this.ModProperties.ModDescription = fileContent.Between("description='''", "'''", StringComparison.CurrentCulture).Trim();
            this.ModProperties.ModLogo = fileContent.Between("logoFile=\"", "\"", StringComparison.CurrentCulture);
            this.ModProperties.ModCredits = fileContent.Between("credits=\"", "\"", StringComparison.CurrentCulture);
            this.ModProperties.ModAuthors = fileContent.Between("authors=\"", "\"", StringComparison.CurrentCulture);
            this.ModProperties.ModWebsite = fileContent.Between("displayURL=\"", "\"", StringComparison.CurrentCulture);
            this.ModProperties.ModUpdateJSONURL = fileContent.Between("updateJSONURL=\"", "\"", StringComparison.CurrentCulture);
            this.ModProperties.ModIssueTracker = fileContent.Between("issueTrackerURL=\"", "\"", StringComparison.CurrentCulture);
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
    }
}
