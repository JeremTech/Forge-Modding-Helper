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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using FMH.Core.Files.Software;
using FMH.Core.UI.Dialogs;
using FMH.Core.Utils.Software;
using FMH.Core.Utils.UI;

namespace FMH.Core.UI.Common
{
    /// <summary>
    /// Logique d'interaction pour StartupWindow.xaml
    /// </summary>
    public partial class StartupWindow : Window
    {
        public StartupWindow()
        {
            // Initialization
            InitializeComponent();
            
            // Events
            Loaded += StartupWindow_Loaded;
        }

        private void StartupWindow_Loaded(object sender, RoutedEventArgs e)
        {
            version_label.Content = App.GetApplicationVersionString();

            // Previous version settings importations
            if (SoftwareVersionUpgrader.ExistPreviousVersionData()
                && !SoftwareVersionUpgrader.ExistCurrentVersionData())
            {
                var settingsImportationWindow = new UpgradeSettingsDialog();
                settingsImportationWindow.Owner = this;
                settingsImportationWindow.ShowDialog();
            }

            // Checking files
            if (!File.Exists(Path.Combine(SoftwareDataManager.GetCurrentVersionDataDirectory(), "options.json")))
                OptionsFile.WriteDataFile();

            // Load options
            OptionsFile.ReadDataFile();

            // Loadings translations
            UITextTranslator.LoadTranslationFile(OptionsFile.GetCurrentLanguage());
            UITextTranslator.UpdateComponentsTranslations(this);
            updateLoadingStatut(UITextTranslator.getTranslation("loading.label.loading"), 0);

            // Loading theme
            App.LoadThemeFile(OptionsFile.GetCurrentTheme());

            // Load last workspaces
            updateLoadingStatut(UITextTranslator.getTranslation("loading.retrieving_workspaces"), 40);
            WelcomeWindow welcomeWindow = new WelcomeWindow();

            // Update welcome UI depending on the presence of recent projects or not
            LastWorkspaces.ReadData();
            if (LastWorkspaces.LastWorkspacesData.Count > 0)
            {
                welcomeWindow.label_no_workspace_found.Visibility = Visibility.Hidden;
                welcomeWindow.listbox_recent_workspaces.ItemsSource = LastWorkspaces.LastWorkspacesProjectFile;
            }
            else
            {
                welcomeWindow.label_no_workspace_found.Visibility = Visibility.Visible;
            }

            // Windows management
            welcomeWindow.Show();
            this.Close();
        }

        /// <summary>
        /// Update the loading status
        /// </summary>
        /// <param name="statut_text">Text to display over the progressBar</param>
        /// <param name="progress_value">New progressBar value</param>
        public void updateLoadingStatut(string statut_text, int progress_value)
        {
            loading_label.Content = statut_text;
            loading_progressbar.Value = progress_value;
        }
    }
}
