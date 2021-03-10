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
        /// Basic constructor
        /// </summary>
        public Project(string directory)
        {
            this.ProjectDirectory = directory;
            this.ModData = new ModInfos();
        }

        /// <summary>
        /// Constructor with modData
        /// </summary>
        public Project(string directory, ModInfos data)
        {
            this.ProjectDirectory = directory;
            this.ModData = data;
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
                this.ModData.ModMappingsVersion = buildGradle.getBetween("mappings channel: 'snapshot', version: '", "'");
                this.ModData.ModVersion = buildGradle.getBetween("version = '", "'");
                this.ModData.ModGroup = buildGradle.getBetween("group = '", "'");
                this.ModData.ModForgeVersion = buildGradle.getBetween("minecraft 'net.minecraftforge:forge:", "'");
                this.ModData.ModMinecraftVersion = this.ModData.ModForgeVersion.getBetween("", "-");
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
    }
}
