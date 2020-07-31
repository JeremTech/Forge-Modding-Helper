using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Forge_Modding_Helper_3.Files;
using Forge_Modding_Helper_3.Objects;
using Forge_Modding_Helper_3.Utils;
using Newtonsoft.Json;
using Path = System.IO.Path;

namespace Forge_Modding_Helper_3.Windows
{
    public partial class ProjectScanWindow : Window
    {
        // Project path
        private string path = "";

        // Show project window after scan
        private bool showProjectWindow = true;

        // Mod infos storage
        private Dictionary<string, string> modInfos = new Dictionary<string, string>
        {
            {"mod_name", ""},
            {"mod_authors", ""},
            {"mod_version", ""},
            {"mod_description", ""},
            {"mod_id", ""},
            {"mod_group", ""},
            {"mod_logo", ""},
            {"mod_credits", ""},
            {"display_url", ""},
            {"issue_tracker", ""},
            {"update_json", ""},
            {"minecraft_version", ""},
            {"forge_version", ""},
            {"mappings_version", ""}
        };

        // Blockstates list storage
        private List<String> blockstatesList = new List<string>();

        // Textures list storage
        private List<String> texturesList = new List<string>();

        // Models list storage
        private List<String> modelsList = new List<string>();

        // java file list storage
        private List<String> javaFileList = new List<string>();

        public ProjectScanWindow(string path, bool showProjectWindow = true)
        {
            InitializeComponent();
            this.path = path;
            this.showProjectWindow = showProjectWindow;

            // Loadings translations
            UITextTranslator.LoadTranslationFile("french");
            UITextTranslator.UpdateComponentsTranslations(this.main_grid);

            this.main_status_label.Content = UITextTranslator.getTranslation("project_scan.initialization");
            this.secondary_status_label.Content = UITextTranslator.getTranslation("project_scan.initialization");
        }

        private void Window_Initialized(object sender, EventArgs e)
        {

        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await Task.Factory.StartNew(ScanProject);

            // Once scan finished
            if (this.showProjectWindow)
            {
                RecentWorkspaces.AddRecentWorkspace(new Workspace(modInfos["mod_name"], modInfos["minecraft_version"], path, modInfos["mod_description"], DateTime.Now));
                new WorkspaceManager(this.path).Show();
            }

            this.Close();
        }

        private void updateMainStatus(string statusTranslateKey, int statusPercentage)
        {
            this.main_status_label.Dispatcher.Invoke((MethodInvoker) delegate { main_status_label.Content = UITextTranslator.getTranslation(statusTranslateKey); });
            this.main_progress_bar.Dispatcher.Invoke((MethodInvoker) delegate { main_progress_bar.Value = statusPercentage; });
        }

        private void updateSecondaryStatus(string statusTranslateKey, int statusPercentage)
        {
            this.secondary_status_label.Dispatcher.Invoke((MethodInvoker)delegate { secondary_status_label.Content = UITextTranslator.getTranslation(statusTranslateKey); });
            this.secondary_progressbar.Dispatcher.Invoke((MethodInvoker)delegate { secondary_progressbar.Value = statusPercentage; });
        }

        public void ScanProject()
        {
            // fmh directory creation
            Directory.CreateDirectory(Path.Combine(path, "fmh"));

            #region Scan mod infos
            updateMainStatus("project_scan.mod_info", 0);

            // Scan build.gradle
            updateSecondaryStatus("project_scan.mod_info.build_gradle", 0);

            string buildGradle = File.ReadAllText(Path.Combine(path, "build.gradle"));
            modInfos["mappings_version"] = buildGradle.getBetween("mappings channel: 'snapshot', version: '", "'");
            updateSecondaryStatus("project_scan.mod_info.build_gradle", 20);
            modInfos["mod_version"] = buildGradle.getBetween("version = '", "'");
            updateSecondaryStatus("project_scan.mod_info.build_gradle", 40);
            modInfos["mod_group"] = buildGradle.getBetween("group = '", "'");
            updateSecondaryStatus("project_scan.mod_info.build_gradle", 60);
            modInfos["forge_version"] = buildGradle.getBetween("minecraft 'net.minecraftforge:forge:", "'");
            updateSecondaryStatus("project_scan.mod_info.build_gradle", 80);
            modInfos["minecraft_version"] = modInfos["forge_version"].getBetween("", "-");
            updateSecondaryStatus("project_scan.mod_info.build_gradle", 100);

            // Scan mods.toml
            updateSecondaryStatus("project_scan.mod_info.mods_toml", 0);

            string modToml = File.ReadAllText(Path.Combine(path, "src\\main\\resources\\META-INF\\mods.toml"));
            modInfos["mod_id"] = modToml.getBetween("modId=\"", "\"");
            updateSecondaryStatus("project_scan.mod_info.mods_toml", 11);
            modInfos["mod_name"] = modToml.getBetween("displayName=\"", "\"");
            updateSecondaryStatus("project_scan.mod_info.mods_toml", 22);
            modInfos["mod_description"] = modToml.getBetween("description='''", "'''").Replace(Environment.NewLine, " ").Trim();
            updateSecondaryStatus("project_scan.mod_info.mods_toml", 33);
            modInfos["mod_logo"] = modToml.getBetween("logoFile=\"", "\"");
            updateSecondaryStatus("project_scan.mod_info.mods_toml", 44);
            modInfos["mod_credits"] = modToml.getBetween("credits=\"", "\"");
            updateSecondaryStatus("project_scan.mod_info.mods_toml", 55);
            modInfos["mod_authors"] = modToml.getBetween("authors=\"", "\"");
            updateSecondaryStatus("project_scan.mod_info.mods_toml", 66);
            modInfos["display_url"] = modToml.getBetween("displayURL=\"", "\"");
            updateSecondaryStatus("project_scan.mod_info.mods_toml", 77);
            modInfos["update_json"] = modToml.getBetween("updateJSONURL=\"", "\"");
            updateSecondaryStatus("project_scan.mod_info.mods_toml", 88);
            modInfos["issue_tracker"] = modToml.getBetween("issueTrackerURL=\"", "\"");
            updateSecondaryStatus("project_scan.mod_info.mods_toml", 100);

            updateSecondaryStatus("project_scan.mod_info.writing_file", 0);
            WriteModInfos();
            updateSecondaryStatus("project_scan.mod_info.writing_file", 100);
            #endregion

            #region Scan textures
            updateMainStatus("project_scan.textures", 25);

            // Scan textures directory
            DirectoryFileListing((Path.Combine(path, "src\\main\\resources\\assets", modInfos["mod_id"], "textures")));

            updateSecondaryStatus("project_scan.textures.writing_file", 0);
            WriteTexturesList();
            updateSecondaryStatus("project_scan.textures.writing_file", 100);
            #endregion

            #region Scan blockstates
            updateMainStatus("project_scan.blockstates", 50);

            // Scan blockstates directory
            DirectoryFileListing((Path.Combine(path, "src\\main\\resources\\assets", modInfos["mod_id"], "blockstates")));

            updateSecondaryStatus("project_scan.blockstates.writing_file", 0);
            WriteBlockstatesList();
            updateSecondaryStatus("project_scan.blockstates.writing_file", 100);
            #endregion

            #region Scan models
            updateMainStatus("project_scan.models", 75);

            // Scan models directory
            DirectoryFileListing(Path.Combine(path, "src\\main\\resources\\assets", modInfos["mod_id"], "models"));

            updateSecondaryStatus("project_scan.models.writing_file", 0);
            WriteModelsList();
            updateSecondaryStatus("project_scan.models.writing_file", 100);
            #endregion

            #region Scan java files
            updateMainStatus("project_scan.java_files", 95);

            // Scan java directory
            DirectoryFileListing(Path.Combine(path, "src\\main\\java"));

            updateSecondaryStatus("project_scan.java_files.writing_file", 0);
            WriteJavaFilesList();
            updateSecondaryStatus("project_scan.java_files.writing_file", 100);
            #endregion
        }

        public void WriteModInfos()
        {
            string jsonContent = JsonConvert.SerializeObject(modInfos, Formatting.Indented);
            File.WriteAllText(Path.Combine(path, "fmh", "mod_infos.json"), jsonContent);
        }

        public void WriteBlockstatesList()
        {
            string jsonContent = JsonConvert.SerializeObject(blockstatesList, Formatting.Indented);
            File.WriteAllText(Path.Combine(path, "fmh", "blockstates_list.json"), jsonContent);
        }

        public void WriteTexturesList()
        {
            string jsonContent = JsonConvert.SerializeObject(texturesList, Formatting.Indented);
            File.WriteAllText(Path.Combine(path, "fmh", "textures_list.json"), jsonContent);
        }

        public void WriteModelsList()
        {
            string jsonContent = JsonConvert.SerializeObject(modelsList, Formatting.Indented);
            File.WriteAllText(Path.Combine(path, "fmh", "models_list.json"), jsonContent);
        }

        public void WriteJavaFilesList()
        {
            string jsonContent = JsonConvert.SerializeObject(javaFileList, Formatting.Indented);
            File.WriteAllText(Path.Combine(path, "fmh", "java_files_list.json"), jsonContent);
        }

        public void DirectoryFileListing(string path)
        {
            try
            {
                foreach (string f in Directory.GetFiles(path))
                {
                    updateSecondaryStatus(Path.GetFileName(f), 0);
                    if(path.Contains("textures"))
                        texturesList.Add(f);
                    else if (path.Contains("blockstates"))
                        blockstatesList.Add(f);
                    else if(path.Contains("models"))
                        modelsList.Add(f);
                    else if (path.Contains("java"))
                        javaFileList.Add(f);
                }

                foreach (string d in Directory.GetDirectories(path))
                {
                    DirectoryFileListing(d);
                }
            }
            catch (System.Exception excpt)
            {
                Console.WriteLine(excpt.Message);
            }
        }
    }
}
