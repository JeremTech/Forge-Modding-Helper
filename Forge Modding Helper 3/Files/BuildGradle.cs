using System;
using System.Collections.Generic;
using System.IO;

namespace Forge_Modding_Helper_3
{
    // This class allow to configure build.gradle file
    public class BuildGradle
    {
        private Dictionary<string, string> mod_infos = new Dictionary<string, string> { };
        private string folder = "";

        /// <summary>
        /// Constructor 
        /// </summary>
        /// <param name="mod_infos">Dictionnary with all mod infos</param>
        /// <param name="generation_foler">Output folder</param>
        public BuildGradle(Dictionary<string, string> mod_infos, string generation_foler)
        {
            // Checking correct values
            foreach(KeyValuePair<string, string> entry in mod_infos)
            {
                if(entry.Value.isTextValid())
                {
                    this.mod_infos.Add(entry.Key, entry.Value);
                }
            }

            this.folder = generation_foler;
        }

        /// <summary>
        /// Generate the build.gradle file
        /// </summary>
        public void generateFile()
        {
            string[] lines = File.ReadAllLines(this.folder + @"\build.gradle");
            string[] output = new string[lines.Length];

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];

                if (line.Contains("version ="))
                {
                    output[i] = line.Replace("1.0", this.mod_infos["mod_version"]);
                }
                else if (line.Contains("group ="))
                {
                    output[i] = line.Replace("com.yourname.modid", this.mod_infos["mod_group"]);
                }
                else if (line.Contains("archivesBaseName ="))
                {
                    output[i] = line.Replace("modid", this.mod_infos["mod_id"]);
                }
                else if(line.Contains("Specification-Title"))
                {
                    output[i] = line.Replace("examplemod", this.mod_infos["mod_id"]);
                }
                else if (line.Contains("Specification-Vendor"))
                {
                    output[i] = line.Replace("examplemodsareus", this.mod_infos["mod_authors"]);
                }
                else if (line.Contains("Implementation-Vendor"))
                {
                    output[i] = line.Replace("examplemodsareus", this.mod_infos["mod_authors"]);
                }
                else
                {
                    output[i] = line;
                }
            }

            File.Delete(this.folder + @"\build.gradle");
            File.AppendAllLines(this.folder + @"\build.gradle", output);
        }
    }
}
