using Forge_Modding_Helper_3.Objects;
using System;
using System.Collections.Generic;
using System.IO;

namespace Forge_Modding_Helper_3
{
    // This class allow to configure build.gradle file
    public class BuildGradle
    {
        private ModInfos ModInfos = new ModInfos();
        private string folder = "";

        /// <summary>
        /// Constructor 
        /// </summary>
        /// <param name="mod_infos">Dictionnary with all mod infos</param>
        /// <param name="generation_foler">Output folder</param>
        public BuildGradle(ModInfos _ModInfos, string generation_foler)
        {
            this.ModInfos = _ModInfos;
            this.folder = generation_foler;
        }

        /// <summary>
        /// Generate the build.gradle file
        /// </summary>
        public void generateFile()
        {
            string[] lines = File.ReadAllLines(this.folder + @"\build.gradle");
            string[] output = new string[lines.Length];
            bool isDataSectionReaded = false;

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];

                if (line.Contains("version ="))
                {
                    string str = StringUtils.getBetween(line, "version = '", "'");
                    output[i] = line.Replace(str, this.ModInfos.ModVersion);
                }
                else if (line.Contains("group ="))
                {
                    string str = StringUtils.getBetween(line, "group = '", "'");
                    output[i] = line.Replace(str, this.ModInfos.ModGroup);
                }
                else if (line.Contains("archivesBaseName ="))
                {
                    string str = StringUtils.getBetween(line, "archivesBaseName = '", "'");
                    output[i] = line.Replace(str, this.ModInfos.ModID);
                }
                else if(line.Contains("Specification-Title"))
                {
                    string str = StringUtils.getBetween(line, "\"", ",").Replace(" ", "");
                    str = StringUtils.getBetween(str, "Specification-Title\":\"", "\"");
                    output[i] = line.Replace(str, this.ModInfos.ModID);
                }
                else if (line.Contains("Specification-Vendor"))
                {
                    string str = StringUtils.getBetween(line, "\"", ",").Replace(" ", "");
                    str = StringUtils.getBetween(str, "Specification-Vendor\":\"", "\"");
                    output[i] = line.Replace(str, this.ModInfos.ModAuthors);
                }
                else if (line.Contains("Implementation-Vendor"))
                {
                    string str = StringUtils.getBetween(line, "\"", ",").Replace(" ", "");
                    str = StringUtils.getBetween(str, "Implementation-Vendor\":\"", "\"");
                    output[i] = line.Replace(str, this.ModInfos.ModAuthors);
                }
                else if (line.Contains("data {"))
                {
                    isDataSectionReaded = true;
                    output[i] = line;
                }
                else if (line.Contains("examplemod {") && isDataSectionReaded)
                {
                    output[i] = line.Replace("examplemod", this.ModInfos.ModID);
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
