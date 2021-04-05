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
            {"mod_license", ""},
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

        public ProjectScanWindow(string path, bool showProjectWindow = true)
        {
            InitializeComponent();
            this.path = path;
            this.showProjectWindow = showProjectWindow;

            // Loadings translations
            UITextTranslator.LoadTranslationFile(OptionsFile.getCurrentLanguage());
            UITextTranslator.UpdateComponentsTranslations(this.main_grid);
        }

        public ProjectScanWindow(string projectPath)
        {
            InitializeComponent();
            App.CurrentProjectData = new Project(projectPath);

            // Loadings translations
            UITextTranslator.LoadTranslationFile(OptionsFile.getCurrentLanguage());
            UITextTranslator.UpdateComponentsTranslations(this.main_grid);
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Launch and wait all scanning tasks
            await Task.WhenAll(new Task[] {App.CurrentProjectData.ScanBuildGradle(), App.CurrentProjectData.ScanModToml(), App.CurrentProjectData.ScanTextures(), App.CurrentProjectData.ScanBlockstates(), App.CurrentProjectData.ScanModels(), App.CurrentProjectData.ScanJavaFiles()});

            // Writing mod data
            await App.CurrentProjectData.WriteModData();

            // Once scan finished
            if (this.showProjectWindow)
            {
                RecentWorkspaces.AddRecentWorkspace(new Workspace(modInfos["mod_name"], modInfos["minecraft_version"], path, modInfos["mod_description"], DateTime.Now));
                new ProjectExplorer().Show();
            }

            this.Close();
        }    
    }
}
