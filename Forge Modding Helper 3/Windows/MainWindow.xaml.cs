using System;
using System.IO;
using System.Windows;
using Forge_Modding_Helper_3.Files;
using Forge_Modding_Helper_3.Utils;
using Forge_Modding_Helper_3.Windows;

namespace Forge_Modding_Helper_3
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            // Initialization
            InitializeComponent();
            version_label.Content = AppInfos.GetApplicationVersionString();

            // Checking files
            if(!File.Exists(Path.Combine(AppInfos.getApplicationDataDirectory(), "options.json")))
                OptionsFile.WriteDataFile();

            // Load options
            OptionsFile.ReadDataFile();

            // Loadings translations
            UITextTranslator.LoadTranslationFile(OptionsFile.getCurrentLanguage());
            UITextTranslator.UpdateComponentsTranslations(this);

            // Checking app folders
            updateLoadingStatut("Vérification des dossiers d'application...", 20);

            // Load last workspaces
            updateLoadingStatut("Récupération des espaces de travail récents...", 40);
            WelcomeWindow welcomeWindow = new WelcomeWindow();

            // Update welcome UI depending on the presence of recent projects or not
            if (RecentWorkspaces.ReadDataFile())
            {
                welcomeWindow.label_no_workspace_found.Visibility = Visibility.Hidden;
                welcomeWindow.listbox_recent_workspaces.ItemsSource = RecentWorkspaces.RecentWorkspacesList;
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
