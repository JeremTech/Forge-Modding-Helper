using System;
using System.Threading.Tasks;
using System.Windows;
using Forge_Modding_Helper_3.Files;
using Forge_Modding_Helper_3.Objects;
using Forge_Modding_Helper_3.Utils;

namespace Forge_Modding_Helper_3.Windows
{
    public partial class ProjectScanWindow : Window
    {
        // Project path
        private string path = "";

        // Show project window after scan
        private bool showProjectWindow = true;

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
                RecentWorkspaces.AddRecentWorkspace(new Workspace(App.CurrentProjectData.ModData.ModName, App.CurrentProjectData.ModData.ModVersion, path, App.CurrentProjectData.ModData.ModDescription, DateTime.Now));
                new ProjectExplorer().Show();
            }

            this.Close();
        }    
    }
}
