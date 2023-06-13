using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forge_Modding_Helper_3.Generators
{
    public class Generator120 : WorkspaceGenerator
    {
        public Generator120() { }

        public override void GenerateBuildGradle()
        {
            // All data is now stored in gradle.properties
            return;
        }

        public override void GenerateModToml()
        {
            var outputText = new StringBuilder();
            var filePath = Path.Combine(App.CurrentProjectData.ProjectDirectory, @"src\main\resources\META-INF\mods.toml");

            // General section
            outputText.AppendLine("modLoader=\"javafml\"");
            outputText.AppendLine("loaderVersion=\"${loader_version_range}\"");
            outputText.AppendLine("license=\"${mod_license}\"");

            if(!string.IsNullOrEmpty(App.CurrentProjectData.ModData.ModIssueTracker))
                outputText.AppendLine(string.Format("issueTrackerURL=\"{0}\"", App.CurrentProjectData.ModData.ModIssueTracker));

            // Mod section
            outputText.AppendLine();
            outputText.AppendLine("[[mods]]");
            outputText.AppendLine("modId=\"${mod_id}\"");
            outputText.AppendLine("version=\"${mod_version}\"");
            outputText.AppendLine("displayName=\"${mod_name}\"");

            if (!string.IsNullOrEmpty(App.CurrentProjectData.ModData.ModUpdateJSONURL))
                outputText.AppendLine(string.Format("updateJSONURL=\"{0}\"", App.CurrentProjectData.ModData.ModUpdateJSONURL));

            if (!string.IsNullOrEmpty(App.CurrentProjectData.ModData.ModWebsite))
                outputText.AppendLine(string.Format("displayURL=\"{0}\"", App.CurrentProjectData.ModData.ModWebsite));

            if (!string.IsNullOrEmpty(App.CurrentProjectData.ModData.ModLogo))
                outputText.AppendLine("logoFile=\"logo.png\"");

            if (!string.IsNullOrEmpty(App.CurrentProjectData.ModData.ModCredits))
                outputText.AppendLine(string.Format("credits=\"{0}\"", App.CurrentProjectData.ModData.ModCredits));

            if (!string.IsNullOrEmpty(App.CurrentProjectData.ModData.ModAuthors))
                outputText.AppendLine(string.Format("authors=\"{0}\"", App.CurrentProjectData.ModData.ModAuthors));

            outputText.AppendLine("description='''${mod_description}'''");

            // Dependencies section
            outputText.AppendLine();
            outputText.AppendLine("[[dependencies.${mod_id}]]");
            outputText.AppendLine("\tmodId=\"forge\"");
            outputText.AppendLine("\tmandatory=true");
            outputText.AppendLine("\tversionRange=\"${forge_version_range}\"");
            outputText.AppendLine("\tordering=\"NONE\"");
            outputText.AppendLine("\tside=\"BOTH\"");
            outputText.AppendLine();
            outputText.AppendLine("[[dependencies.${mod_id}]]");
            outputText.AppendLine("\tmodId=\"minecraft\"");
            outputText.AppendLine("\tmandatory=true");
            outputText.AppendLine("\tversionRange=\"${minecraft_version_range}\"");
            outputText.AppendLine("\tordering=\"NONE\"");
            outputText.AppendLine("\tside=\"BOTH\"");

            File.WriteAllText(filePath, outputText.ToString());
        }

        public override void GenerateGradleProperties()
        {
            var outputText = new StringBuilder();
            var filePath = Path.Combine(App.CurrentProjectData.ProjectDirectory, "gradle.properties");
            var forgeVersionShort = App.CurrentProjectData.ModData.ModAPIVersion.Split('-')[1];
            var forgeVersionRange = forgeVersionShort.Split('.')[0];

            // Default memory used for gradle commands
            outputText.AppendLine("org.gradle.jvmargs=-Xmx3G");
            outputText.AppendLine("org.gradle.daemon=false");

            // Environment Properties
            outputText.AppendLine(string.Format("minecraft_version={0}", App.CurrentProjectData.ModData.ModMinecraftVersion));
            outputText.AppendLine(string.Format("minecraft_version_range=[{0}]", App.CurrentProjectData.ModData.ModMinecraftVersion));
            outputText.AppendLine(string.Format("forge_version={0}", forgeVersionShort));
            outputText.AppendLine(string.Format("forge_version_range=[{0},)", forgeVersionRange));
            outputText.AppendLine(string.Format("loader_version_range=[{0},)", forgeVersionRange));
            outputText.AppendLine(string.Format("mapping_channel={0}", "official"));
            outputText.AppendLine(string.Format("mapping_version={0}", App.CurrentProjectData.ModData.ModMinecraftVersion));
            outputText.AppendLine(string.Format("mod_id={0}", App.CurrentProjectData.ModData.ModID));
            outputText.AppendLine(string.Format("mod_name={0}", App.CurrentProjectData.ModData.ModName));
            outputText.AppendLine(string.Format("mod_license={0}", App.CurrentProjectData.ModData.ModLicense));
            outputText.AppendLine(string.Format("mod_version={0}", App.CurrentProjectData.ModData.ModVersion));
            outputText.AppendLine(string.Format("mod_group_id={0}", App.CurrentProjectData.ModData.ModGroup));
            outputText.AppendLine(string.Format("mod_authors={0}", App.CurrentProjectData.ModData.ModAuthors));
            outputText.AppendLine(string.Format("mod_description={0}", App.CurrentProjectData.ModData.ModDescription));

            File.WriteAllText(filePath, outputText.ToString());
        }
    }
}
