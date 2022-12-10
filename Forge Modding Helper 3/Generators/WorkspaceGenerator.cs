using Forge_Modding_Helper_3.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Forge_Modding_Helper_3.Generators
{
    /// <summary>
    /// Parent class for generators
    /// </summary>
    public class WorkspaceGenerator
    {
        /// <summary>
        /// Get generator for a specified version
        /// </summary>
        /// <param name="mcVersion">Targeted minecraft version</param>
        /// <returns>Generator or null if it not exist</returns>
        public static WorkspaceGenerator GetGenerator(string mcVersion)
        {
            switch(mcVersion)
            {
                case "1.15.2":
                    return new Generator115();

                case "1.16.3":
                case "1.16.4":
                case "1.16.5":
                    return new Generator116();

                case "1.17.1":
                    return new Generator117();

                case "1.18.1":
                case "1.18.2":
                    return new Generator118();

                case "1.19":
                case "1.19.1":
                case "1.19.2":
                case "1.19.3":
                    return new Generator119();
            }

            return null;
        }

        /// <summary>
        /// Generate build.gradle file
        /// Default implementation of this function
        /// </summary>
        public void GenerateBuildGradle()
        {
            string[] lines = File.ReadAllLines(App.CurrentProjectData.ProjectDirectory + @"\build.gradle");
            string[] output = new string[lines.Length];
            bool isDataSectionReaded = false;

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];

                if (line.Contains("version ="))
                {
                    string str = StringUtils.getBetween(line, "version = '", "'");
                    output[i] = line.Replace(str, App.CurrentProjectData.ModData.ModVersion);
                }
                else if (line.Contains("group ="))
                {
                    string str = StringUtils.getBetween(line, "group = '", "'");
                    output[i] = line.Replace(str, App.CurrentProjectData.ModData.ModGroup);
                }
                else if (line.Contains("archivesBaseName ="))
                {
                    string str = StringUtils.getBetween(line, "archivesBaseName = '", "'");
                    output[i] = line.Replace(str, App.CurrentProjectData.ModData.ModID);
                }
                else if (line.Contains("Specification-Title"))
                {
                    string str = line.Replace(" ", "");
                    str = StringUtils.getBetween(str, "Specification-Title\":\"", "\"");
                    output[i] = "                " + line.Replace(" ", "").Replace(":", " : ").Replace(str, App.CurrentProjectData.ModData.ModID);
                }
                else if (line.Contains("Specification-Vendor"))
                {
                    string str = line.Replace(" ", "");
                    str = StringUtils.getBetween(str, "Specification-Vendor\":\"", "\"");
                    output[i] = "                " + line.Replace(" ", ""). Replace(":", " : ").Replace(str, App.CurrentProjectData.ModData.ModAuthors);
                }
                else if (line.Contains("Implementation-Vendor"))
                {
                    string str = line.Replace(" ", "");
                    str = StringUtils.getBetween(str, "Implementation-Vendor\":\"", "\"");
                    output[i] = "                " + line.Replace(" ", "").Replace(":", " : ").Replace(str, App.CurrentProjectData.ModData.ModAuthors);
                }
                else if (line.Contains("data {"))
                {
                    isDataSectionReaded = true;
                    output[i] = line;
                }
                else if (line.Contains("examplemod {") && isDataSectionReaded)
                {
                    output[i] = line.Replace("examplemod", App.CurrentProjectData.ModData.ModID);
                }
                else
                {
                    output[i] = line;
                }
            }

            File.Delete(App.CurrentProjectData.ProjectDirectory + @"\build.gradle");
            File.AppendAllLines(App.CurrentProjectData.ProjectDirectory + @"\build.gradle", output);
        }

        /// <summary>
        /// Generate mod.toml file
        /// Default implementation of this function
        /// </summary>
        public void GenerateModToml()
        {
            // modloader
            string modToml = "modLoader=\"javafml\"";
            // loaderVersion
            modToml += Environment.NewLine + "loaderVersion=\"[" + App.CurrentProjectData.ModData.ModAPIVersion.Split('.')[0] + ",)\"";
            // modId
            modToml += Environment.NewLine + "license=\"" + App.CurrentProjectData.ModData.ModLicense + "\"";
            // issueTrackerURL
            if (!string.IsNullOrEmpty(App.CurrentProjectData.ModData.ModIssueTracker))
                modToml += Environment.NewLine + "issueTrackerURL=\"" + App.CurrentProjectData.ModData.ModIssueTracker + "\"";
            // logoFile
            modToml += Environment.NewLine + "logoFile=\"logo.png\"";

            // Mod section declaration
            modToml += Environment.NewLine + Environment.NewLine + "[[mods]]";

            // modId
            modToml += Environment.NewLine + "modId=\"" + App.CurrentProjectData.ModData.ModID + "\"";
            // version
            modToml += Environment.NewLine + "version=\"${ file.jarVersion}\"";
            // displayName
            modToml += Environment.NewLine + "displayName=\"" + App.CurrentProjectData.ModData.ModName + "\"";
            // updateJSONURL
            if (!string.IsNullOrEmpty(App.CurrentProjectData.ModData.ModUpdateJSONURL))
                modToml += Environment.NewLine + "updateJSONURL=\"" + App.CurrentProjectData.ModData.ModUpdateJSONURL + "\"";
            // displayURL
            if (!string.IsNullOrEmpty(App.CurrentProjectData.ModData.ModWebsite))
                modToml += Environment.NewLine + "displayURL=\"" + App.CurrentProjectData.ModData.ModWebsite + "\"";
            // credits
            if (!string.IsNullOrEmpty(App.CurrentProjectData.ModData.ModCredits))
                modToml += Environment.NewLine + "credits=\"" + App.CurrentProjectData.ModData.ModCredits + "\"";
            // authors
            if (!string.IsNullOrEmpty(App.CurrentProjectData.ModData.ModAuthors))
                modToml += Environment.NewLine + "authors=\"" + App.CurrentProjectData.ModData.ModAuthors + "\"";
            // description
            if (!string.IsNullOrEmpty(App.CurrentProjectData.ModData.ModDescription))
                modToml += Environment.NewLine + "description='''" + Environment.NewLine + App.CurrentProjectData.ModData.ModDescription + Environment.NewLine + "'''";

            // Dependencies section
            modToml += Environment.NewLine + Environment.NewLine;

            modToml += @"[[dependencies." + App.CurrentProjectData.ModData.ModID + @"]]
modId=""forge""
mandatory = true
versionRange = ""[" + App.CurrentProjectData.ModData.ModAPIVersion.Split('.')[0] + @",)"" 
ordering = ""NONE""
side = ""BOTH""

[[dependencies." + App.CurrentProjectData.ModData.ModID + @"]]
modId = ""minecraft""
mandatory = true
versionRange = ""[" + App.CurrentProjectData.ModData.ModMinecraftVersion + @"]""
ordering = ""NONE""
side = ""BOTH""";

            File.Delete(App.CurrentProjectData.ProjectDirectory + @"\src\main\resources\META-INF\mods.toml");
            File.WriteAllText(App.CurrentProjectData.ProjectDirectory + @"\src\main\resources\META-INF\mods.toml", modToml);
        }
    }
}
