using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FMH.Workspace.Data
{
    public class ModVersionsHistory
    {
        /// <summary>
        /// Version history list
        /// </summary>
        public List<ModVersionHistoryEntry> VersionHistory { get; set; }

        /// <summary>
        /// Workspace path
        /// </summary>
        private string _workspacePath { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="workspacePath">Workspace directory path</param>
        public ModVersionsHistory(string workspacePath) 
        { 
            this._workspacePath = workspacePath;
        }

        /// <summary>
        /// Read versions history
        /// </summary>
        public void ReadVersionsHistory()
        {
            var filePath = Path.Combine(_workspacePath, "fmh", "versions", "history.json");

            if (File.Exists(filePath))
            {
                try
                {
                    VersionHistory = JsonConvert.DeserializeObject<List<ModVersionHistoryEntry>>(File.ReadAllText(filePath));
                }
                catch
                {
                    VersionHistory = new List<ModVersionHistoryEntry>();
                }
            }
            else
            {
                VersionHistory = new List<ModVersionHistoryEntry>();
            }
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
            VersionHistory.Add(new ModVersionHistoryEntry()
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
            var item = VersionHistory.FirstOrDefault(v => v.ModVersion == modVersion);

            if (item != null)
                VersionHistory.Remove(item);
        }

        /// <summary>
        /// Write versions history data to file
        /// </summary>
        public void WriteData()
        {
            var filePath = Path.Combine(_workspacePath, "fmh", "versions", "history.json");
            File.WriteAllText(filePath, JsonConvert.SerializeObject(VersionHistory, Formatting.Indented));
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
        /// File name
        /// </summary>
        public string FileName { get; set; }
    }
}
