using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using Forge_Modding_Helper_3.Files;
using Forge_Modding_Helper_3.Files.Software;
using Forge_Modding_Helper_3.Objects;
using Forge_Modding_Helper_3.Utils;

namespace Forge_Modding_Helper_3.Windows
{
    public partial class ProjectScanWindow : Window
    {
        // Show project window after scan
        private bool showProjectWindow = true;

        public ProjectScanWindow(string projectPath, bool showProjectWindow = true)
        {
            InitializeComponent();
            App.CurrentProjectData = new Project(projectPath);
            this.showProjectWindow = showProjectWindow;

            // Loadings translations
            UITextTranslator.LoadTranslationFile(OptionsFile.GetCurrentLanguage());
            UITextTranslator.UpdateComponentsTranslations(this.main_grid);
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Check if fmh folder exist in workspace directory
            if (!Directory.Exists(Path.Combine(App.CurrentProjectData.ProjectDirectory, "fmh"))) 
                Directory.CreateDirectory(Path.Combine(App.CurrentProjectData.ProjectDirectory, "fmh"));

            // Check if fmh/versions folder exist in workspace directory
            if (!Directory.Exists(Path.Combine(App.CurrentProjectData.ProjectDirectory, @"fmh\versions")))
                Directory.CreateDirectory(Path.Combine(App.CurrentProjectData.ProjectDirectory, @"fmh\versions"));

            // Launch and wait mod infos scanning tasks
            await Task.WhenAll(new Task[] { App.CurrentProjectData.ScanBuildGradle(), App.CurrentProjectData.ScanModToml(), App.CurrentProjectData.ModVersionsHistoryData.ReadData() });

            // Launch and wait all others scanning tasks
            await Task.WhenAll(new Task[] { App.CurrentProjectData.ScanTextures(), App.CurrentProjectData.ScanBlockstates(), App.CurrentProjectData.ScanModels(), App.CurrentProjectData.ScanJavaFiles() });

            // Launch and wait code lines counting task
            if(OptionsFile.GetCountCodeLinesAtProjectOpeningOption())
                await Task.WhenAll(new Task[] { App.CurrentProjectData.CountCodeLines() });

            // Writing mod data
            await App.CurrentProjectData.WriteModData();
            App.CurrentProjectData.WriteProjectFile();

            // Once scan finished
            if (this.showProjectWindow)
            {
                LastWorkspaces.AddRecentWorkspace(new WorkspaceEntry(App.CurrentProjectData.ProjectDirectory, DateTime.Now));
                new ProjectExplorer().Show();
            }

            this.Close();
        }    
    }
}
