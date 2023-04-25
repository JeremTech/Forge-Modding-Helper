using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Forge_Modding_Helper_3.Files.Workspace
{
    public class ModVersionsHistoryFile
    {
        private List<ModVersionHistoryEntry> _versionsHistoryData {  get; set; }

        /// <summary>
        /// Read versions history file of the current project
        /// </summary>
        public async Task ReadData()
        {
            // Run it async
            await Task.Factory.StartNew(() =>
            {
                var versionsHistoryFilePath = Path.Combine(App.CurrentProjectData.ProjectDirectory, "fmh", "versions", "history.json");

                if (File.Exists(versionsHistoryFilePath))
                {
                    try
                    {
                        _versionsHistoryData = JsonConvert.DeserializeObject<List<ModVersionHistoryEntry>>(File.ReadAllText(versionsHistoryFilePath));
                    }
                    catch
                    {
                        _versionsHistoryData = new List<ModVersionHistoryEntry>();
                    }
                }
                else
                {
                    _versionsHistoryData = new List<ModVersionHistoryEntry>();
                }
            });
        }

        /// <summary>
        /// Add a version to the versions history file.<br/>
        /// You must use ReadData() before use this function, and WriteData() once you have added your versions
        /// </summary>
        /// <param name="modVersion">Mod version</param>
        /// <param name="mcVersion">Minecraft version</param>
        /// <param name="creationDateTime">Version creation DateTime</param>
        /// <param name="fileName">Output file name</param>
        public void AddVersionToHistory(string modVersion, string mcVersion, DateTime creationDateTime, string fileName) 
        {
            _versionsHistoryData.Add(new ModVersionHistoryEntry()
            {
                ModVersion = modVersion,
                MinecraftVersion = mcVersion,
                VersionDateTime = creationDateTime,
                FileName = fileName
            });
        }

        /// <summary>
        /// Remove a version from the versions history file.<br/>
        /// You must use ReadData() before use this function, and WriteData() once you have deleted your versions
        /// </summary>
        /// <param name="modVersion">Mod version</param>
        public void RemoveVersionFromHistory(string modVersion)
        {
            var item = _versionsHistoryData.FirstOrDefault(v => v.ModVersion == modVersion);

            if(item != null)
                _versionsHistoryData.Remove(item);
        }


        /// <summary>
        /// Read and return versions history from the versions history file
        /// </summary>
        /// <returns>History of versions</returns>
        public async Task<List<ModVersionHistoryEntry>> GetModVersionsHistory()
        {
            await ReadData();
            
            var dataResult = new List<ModVersionHistoryEntry>();
            foreach (var entry in _versionsHistoryData)
            {
                dataResult.Add(new ModVersionHistoryEntry()
                {
                    ModVersion = entry.ModVersion,
                    MinecraftVersion = entry.MinecraftVersion,
                    VersionDateTime = entry.VersionDateTime,
                    FileName = entry.FileName
                });
            }

            return dataResult;
        }

        /// <summary>
        /// Write versions history data to file
        /// </summary>
        public async Task WriteData()
        {
            // Run it async
            await Task.Factory.StartNew(() =>
            {
                var versionsHistoryFilePath = Path.Combine(App.CurrentProjectData.ProjectDirectory, "fmh", "versions", "history.json");
                File.WriteAllText(versionsHistoryFilePath, JsonConvert.SerializeObject(_versionsHistoryData, Formatting.Indented));
            });
        }
    }

    public class ModVersionHistoryEntry
    {
        /// <summary>
        /// Mod version
        /// </summary>
        public string ModVersion { get; set; }

        /// <summary>
        /// Minecraft version
        /// </summary>
        public string MinecraftVersion { get; set; }

        /// <summary>
        /// Exportation DateTime
        /// </summary>
        public DateTime VersionDateTime { get; set; }

        /// <summary>
        /// Mod version file name
        /// </summary>
        public string FileName { get; set; }
    }
}
