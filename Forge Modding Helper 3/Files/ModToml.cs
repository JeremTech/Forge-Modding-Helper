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
        private Dictionary<string, string> mod_infos = new Dictionary<string, string> { };
        private string folder = "";

        /// <summary>
        /// Constructor 
        /// </summary>
        /// <param name="mod_infos">Dictionnary with all mod infos</param>
        /// <param name="generation_foler">Output folder</param>
        public ModToml(Dictionary<string, string> mod_infos, string generation_foler)
        {
            this.mod_infos = mod_infos;
            this.folder = generation_foler;
        }

        public void generateFile()
        {
            // modloader
            string modToml = "modLoader=\"javafml\"";
            // loaderVersion
            modToml += Environment.NewLine + "loaderVersion=\"[" + this.mod_infos["forge_version"].Split('.')[0] + ",)\"";
            // modId
            modToml += Environment.NewLine + "license=\"" + this.mod_infos["mod_license"] + "\"";
            // issueTrackerURL
            if (!string.IsNullOrEmpty(this.mod_infos["issue_tracker"]))
                modToml += Environment.NewLine + "issueTrackerURL=\"" + this.mod_infos["issue_tracker"] + "\"";
            // logoFile
            modToml += Environment.NewLine + "logoFile=\"logo.png\"";

            // Mod section declaration
            modToml += Environment.NewLine + Environment.NewLine + "[[mods]]";

            // modId
            modToml += Environment.NewLine + "modId=\"" + this.mod_infos["mod_id"] + "\"";
            // version
            modToml += Environment.NewLine + "version=\"${ file.jarVersion}\"";
            // displayName
            modToml += Environment.NewLine + "displayName=\"" + this.mod_infos["mod_name"] + "\"";
            // updateJSONURL
            if (!string.IsNullOrEmpty(this.mod_infos["update_json"]))
                modToml += Environment.NewLine + "updateJSONURL=\"" + this.mod_infos["update_json"] + "\"";
            // displayURL
            if (!string.IsNullOrEmpty(this.mod_infos["display_url"]))
                modToml += Environment.NewLine + "displayURL=\"" + this.mod_infos["display_url"] + "\"";
            // credits
            if (!string.IsNullOrEmpty(this.mod_infos["mod_credits"]))
                modToml += Environment.NewLine + "credits=\"" + this.mod_infos["mod_credits"] + "\"";
            // authors
            if (!string.IsNullOrEmpty(this.mod_infos["mod_authors"]))
                modToml += Environment.NewLine + "authors=\"" + this.mod_infos["mod_authors"] + "\"";
            // description
            if (!string.IsNullOrEmpty(this.mod_infos["mod_description"]))
                modToml += Environment.NewLine + "description='''" + Environment.NewLine + this.mod_infos["mod_description"] + Environment.NewLine + "'''";

            // Dependencies section
            modToml += Environment.NewLine + Environment.NewLine;

            modToml += @"[[dependencies." + this.mod_infos["mod_id"] + @"]]
modId=""forge""
mandatory = true
versionRange = ""[" + this.mod_infos["forge_version"].Split('.')[0] + @",)"" 
ordering = ""NONE""
side = ""BOTH""

[[dependencies." + this.mod_infos["mod_id"] + @"]]
modId = ""minecraft""
mandatory = true
versionRange = ""[" + this.mod_infos["minecraft_version"] + @"]""
ordering = ""NONE""
side = ""BOTH""";

            File.Delete(this.folder + @"\src\main\resources\META-INF\mods.toml");
            File.WriteAllText(this.folder + @"\src\main\resources\META-INF\mods.toml", modToml);
        }
    }
}
