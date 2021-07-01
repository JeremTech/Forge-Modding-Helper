using Forge_Modding_Helper_3.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forge_Modding_Helper_3
{
    // This class allow to generate mod.toml file
    public class ModToml
    {
        private ModInfos ModInfos = new ModInfos();
        private string folder = "";

        /// <summary>
        /// Constructor 
        /// </summary>
        /// <param name="mod_infos">Dictionnary with all mod infos</param>
        /// <param name="generation_foler">Output folder</param>
        public ModToml(ModInfos _ModInfos, string generation_foler)
        {
            this.ModInfos = _ModInfos;
            this.folder = generation_foler;
        }

        public void generateFile()
        {
            // modloader
            string modToml = "modLoader=\"javafml\"";
            // loaderVersion
            modToml += Environment.NewLine + "loaderVersion=\"[" + this.ModInfos.ModAPIVersion.Split('.')[0] + ",)\"";
            // modId
            modToml += Environment.NewLine + "license=\"" + this.ModInfos.ModLicense + "\"";
            // issueTrackerURL
            if (!string.IsNullOrEmpty(this.ModInfos.ModIssueTracker))
                modToml += Environment.NewLine + "issueTrackerURL=\"" + this.ModInfos.ModIssueTracker + "\"";
            // logoFile
            modToml += Environment.NewLine + "logoFile=\"logo.png\"";

            // Mod section declaration
            modToml += Environment.NewLine + Environment.NewLine + "[[mods]]";

            // modId
            modToml += Environment.NewLine + "modId=\"" + this.ModInfos.ModID + "\"";
            // version
            modToml += Environment.NewLine + "version=\"${ file.jarVersion}\"";
            // displayName
            modToml += Environment.NewLine + "displayName=\"" + this.ModInfos.ModName + "\"";
            // updateJSONURL
            if (!string.IsNullOrEmpty(this.ModInfos.ModUpdateJSONURL))
                modToml += Environment.NewLine + "updateJSONURL=\"" + this.ModInfos.ModUpdateJSONURL + "\"";
            // displayURL
            if (!string.IsNullOrEmpty(this.ModInfos.ModWebsite))
                modToml += Environment.NewLine + "displayURL=\"" + this.ModInfos.ModWebsite + "\"";
            // credits
            if (!string.IsNullOrEmpty(this.ModInfos.ModCredits))
                modToml += Environment.NewLine + "credits=\"" + this.ModInfos.ModCredits + "\"";
            // authors
            if (!string.IsNullOrEmpty(this.ModInfos.ModAuthors))
                modToml += Environment.NewLine + "authors=\"" + this.ModInfos.ModAuthors + "\"";
            // description
            if (!string.IsNullOrEmpty(this.ModInfos.ModDescription))
                modToml += Environment.NewLine + "description='''" + Environment.NewLine + this.ModInfos.ModDescription + Environment.NewLine + "'''";

            // Dependencies section
            modToml += Environment.NewLine + Environment.NewLine;

            modToml += @"[[dependencies." + this.ModInfos.ModID + @"]]
modId=""forge""
mandatory = true
versionRange = ""[" + this.ModInfos.ModAPIVersion.Split('.')[0] + @",)"" 
ordering = ""NONE""
side = ""BOTH""

[[dependencies." + this.ModInfos.ModID + @"]]
modId = ""minecraft""
mandatory = true
versionRange = ""[" + this.ModInfos.ModMinecraftVersion + @"]""
ordering = ""NONE""
side = ""BOTH""";

            File.Delete(this.folder + @"\src\main\resources\META-INF\mods.toml");
            File.WriteAllText(this.folder + @"\src\main\resources\META-INF\mods.toml", modToml);
        }
    }
}
