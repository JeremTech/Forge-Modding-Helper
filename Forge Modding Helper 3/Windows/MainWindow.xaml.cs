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

            // Checking app folders
            updateLoadingStatut("Vérification des dossiers d'application...", 20);

            if (!Directory.Exists(Path.Combine(Environment.CurrentDirectory, "Data")))
                Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, "Data"));

            // Load last workspaces
            updateLoadingStatut("Récupération des espaces de travail récents...", 40);
            WelcomeWindow welcomeWindow = new WelcomeWindow();

            if (RecentWorkspaces.ReadDataFile())
            {
                welcomeWindow.label_no_workspace_found.Visibility = Visibility.Hidden;
                welcomeWindow.listbox_recent_workspaces.ItemsSource = RecentWorkspaces.RecentWorkspacesList;
            }
            else
            {
                welcomeWindow.label_no_workspace_found.Visibility = Visibility.Visible;
            }

            welcomeWindow.Show();
            this.Close();
        }

        public void updateLoadingStatut(string statut_text, int progress_value)
        {
            loading_label.Content = statut_text;
            loading_progressbar.Value = progress_value;
        }
    }
}
