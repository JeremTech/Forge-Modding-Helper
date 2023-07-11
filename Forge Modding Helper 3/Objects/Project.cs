﻿using FMH.Workspace.Data;
using Forge_Modding_Helper_3.Files.Workspace_Data;
using Forge_Modding_Helper_3.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Forge_Modding_Helper_3.Objects
{
    /// <summary>
    /// Workspace project data and function
    /// </summary>
    public class Project
    {
        /// <summary>
        /// Mod infos data
        /// </summary>
        public ModInfos ModData { get; set; }

        /// <summary>
        /// Mod versions history data
        /// </summary>
        public ModVersionsHistory ModVersionsHistoryData { get; set; }

        /// <summary>
        /// Project directory
        /// </summary>
        public string ProjectDirectory { get; set; }

        /// <summary>
        /// Textures list of the project
        /// </summary>
        public List<string> TexturesList { get; set; }

        /// <summary>
        /// Blockstates files list of the project
        /// </summary>
        public List<string> BlockstatesList { get; set; }

        /// <summary>
        /// Models files list of the project
        /// </summary>
        public List<string> ModelsList { get; set; }

        /// <summary>
        /// Java files list of the project
        /// </summary>
        public List<string> JavaFilesList { get; set; }

        /// <summary>
        /// Number of code lines
        /// </summary>
        public int CodeLinesCount { get; set; }

        /// <summary>
        /// Basic constructor
        /// </summary>
        public Project(string directory)
        {
            this.ProjectDirectory = directory;
            this.ModData = new ModInfos();
            this.ModVersionsHistoryData = new ModVersionsHistory(directory);
        }

        /// <summary>
        /// Constructor with modData
        /// </summary>
        public Project(string directory, ModInfos data)
        {
            this.ProjectDirectory = directory;
            this.ModData = data;
            this.ModVersionsHistoryData = new ModVersionsHistory(directory);
        }

        /// <summary>
        /// Constructor with modData and mod history data
        /// </summary>
        public Project(string directory, ModInfos data, ModVersionsHistory modVersionsHistoryData)
        {
            this.ProjectDirectory = directory;
            this.ModData = data;
            this.ModVersionsHistoryData = modVersionsHistoryData;
        }

        /// <summary>
        /// Read information from the build.gradle file
        /// </summary>
        public async Task ScanBuildGradle()
        {
            // Run it async
            await Task.Factory.StartNew(() =>
            {
                string buildGradle = File.ReadAllText(Path.Combine(this.ProjectDirectory, "build.gradle"));
                this.ModData.ModVersion = buildGradle.getBetween("version = '", "'");
                this.ModData.ModGroup = buildGradle.getBetween("group = '", "'");
                this.ModData.ModAPIVersion = buildGradle.getBetween("minecraft 'net.minecraftforge:forge:", "'");
                this.ModData.ModMinecraftVersion = this.ModData.ModAPIVersion.getBetween("", "-");

                // Mappings
                switch(buildGradle.getBetween("mappings channel: '", "'"))
                {
                    // Case 'snapshot' or 'stable', this is MCP mappings
                    case "snapshot":
                        this.ModData.ModMappingsVersion = buildGradle.getBetween("mappings channel: 'snapshot', version: '", "'") + " (MCP)";
                        break;
                    case "stable":
                        this.ModData.ModMappingsVersion = buildGradle.getBetween("mappings channel: 'stable', version: '", "'") + " (MCP)";
                        break;

                    // Case official, this is mojang mappings
                    case "official":
                        this.ModData.ModMappingsVersion = buildGradle.getBetween("mappings channel: 'official', version: '", "'") + " (Mojang)";
                        break;
                }
            });
        }

        /// <summary>
        /// Read information from the mod.toml file
        /// </summary>
        public async Task ScanModToml()
        {
            // Run it async
            await Task.Factory.StartNew(() =>
            {
                string modToml = File.ReadAllText(Path.Combine(this.ProjectDirectory, "src\\main\\resources\\META-INF\\mods.toml"));
                this.ModData.ModLicense = modToml.getBetween("license=\"", "\"");
                this.ModData.ModID = modToml.getBetween("modId=\"", "\"");
                this.ModData.ModName = modToml.getBetween("displayName=\"", "\"");
                this.ModData.ModDescription = modToml.getBetween("description='''", "'''").Trim();
                this.ModData.ModLogo = modToml.getBetween("logoFile=\"", "\"");
                this.ModData.ModCredits = modToml.getBetween("credits=\"", "\"");
                this.ModData.ModAuthors = modToml.getBetween("authors=\"", "\"");
                this.ModData.ModWebsite = modToml.getBetween("displayURL=\"", "\"");
                this.ModData.ModUpdateJSONURL = modToml.getBetween("updateJSONURL=\"", "\"");
                this.ModData.ModIssueTracker = modToml.getBetween("issueTrackerURL=\"", "\"");
            });
        }

        /// <summary>
        /// Retrieve all textures files
        /// </summary>
        public async Task ScanTextures()
        {
            // Run it async
            await Task.Factory.StartNew(() =>
            {
                TexturesList = DirectoryUtils.DeepFileListing(Path.Combine(this.ProjectDirectory, "src\\main\\resources\\assets", this.ModData.ModID, "textures"));
            });
        }

        /// <summary>
        /// Retrieve all textures files
        /// </summary>
        public async Task ScanBlockstates()
        {
            // Run it async
            await Task.Factory.StartNew(() =>
            {
                BlockstatesList = DirectoryUtils.DeepFileListing(Path.Combine(this.ProjectDirectory, "src\\main\\resources\\assets", this.ModData.ModID, "blockstates"));
            });
        }

        /// <summary>
        /// Retrieve all models files
        /// </summary>
        public async Task ScanModels()
        {
            // Run it async
            await Task.Factory.StartNew(() =>
            {
                ModelsList = DirectoryUtils.DeepFileListing(Path.Combine(this.ProjectDirectory, "src\\main\\resources\\assets", this.ModData.ModID, "models"));
            });
        }

        /// <summary>
        /// Retrieve all java files
        /// </summary>
        public async Task ScanJavaFiles()
        {
            // Run it async
            await Task.Factory.StartNew(() =>
            {
                JavaFilesList = DirectoryUtils.DeepFileListing(Path.Combine(this.ProjectDirectory, "src\\main\\java"));
            });
        }

        /// <summary>
        /// Count the number of code lines of the project
        /// </summary>
        /// <returns></returns>
        public async Task CountCodeLines()
        {
            // Run it async
            await Task.Factory.StartNew(() =>
            {
                CodeLinesCount = DirectoryUtils.CountCodeLines(Path.Combine(this.ProjectDirectory, "src\\main\\java"));
            });
        }

        /// <summary>
        /// Write mod data files in 'fmh' directory
        /// </summary>
        public async Task WriteModData()
        {
            // Run it async
            await Task.Factory.StartNew(() =>
            {
                string jsonContent = "";

                try
                {
                    // Writing mod infos
                    jsonContent = JsonConvert.SerializeObject(ModData, Formatting.Indented);
                    File.WriteAllText(Path.Combine(this.ProjectDirectory, "fmh", "mod_infos.json"), jsonContent);

                    // Writing textures list
                    jsonContent = JsonConvert.SerializeObject(this.TexturesList, Formatting.Indented);
                    File.WriteAllText(Path.Combine(this.ProjectDirectory, "fmh", "textures_list.json"), jsonContent);

                    // Writing blockstates list
                    jsonContent = JsonConvert.SerializeObject(this.BlockstatesList, Formatting.Indented);
                    File.WriteAllText(Path.Combine(this.ProjectDirectory, "fmh", "blockstates_list.json"), jsonContent);

                    // Writing models list
                    jsonContent = JsonConvert.SerializeObject(this.ModelsList, Formatting.Indented);
                    File.WriteAllText(Path.Combine(this.ProjectDirectory, "fmh", "models_list.json"), jsonContent);

                    // Writing java files list
                    jsonContent = JsonConvert.SerializeObject(this.JavaFilesList, Formatting.Indented);
                    File.WriteAllText(Path.Combine(this.ProjectDirectory, "fmh", "java_files_list.json"), jsonContent);
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            });
        }

        /// <summary>
        /// Write project file in 'fmh' directory
        /// </summary>
        public void WriteProjectFile()
        {
            if(File.Exists(this.ProjectDirectory + "\\src\\main\\resources\\logo.png"))
            {
                File.WriteAllText(Path.Combine(this.ProjectDirectory, "fmh", "project.fmh"), JsonConvert.SerializeObject(new ProjectFile(this.ModData.ModName, this.ModData.ModDescription, this.ProjectDirectory + "\\src\\main\\resources\\logo.png", "Forge", this.ModData.ModAPIVersion, this.ModData.ModMinecraftVersion, this.ProjectDirectory), Formatting.Indented));
            }
            else
            {
                File.WriteAllText(Path.Combine(this.ProjectDirectory, "fmh", "project.fmh"), JsonConvert.SerializeObject(new ProjectFile(this.ModData.ModName, this.ModData.ModDescription, new Uri("/Forge Modding Helper 3;component/Resources/Pictures/forge_logo.png", UriKind.Relative).ToString(), "Forge", this.ModData.ModAPIVersion, this.ModData.ModMinecraftVersion, this.ProjectDirectory), Formatting.Indented));
            }
        }
    }
}
