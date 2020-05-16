using System;
using System.IO;
using System.Windows;
using Forge_Modding_Helper_3.Files;
using Forge_Modding_Helper_3.Windows;

namespace Forge_Modding_Helper_3
{
    public partial class MainWindow : Window
    {
        private string execution_folder = Environment.CurrentDirectory;

        public MainWindow()
        {
            // Initialization
            InitializeComponent();
            version_label.Content = "v" + System.Reflection.Assembly.GetEntryAssembly().GetName().Version;

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
                welcomeWindow.listbox_recent_workspaces.ItemsSource = RecentWorkspaces.GetRecentWorkspaces();
            }
            else
            {
                welcomeWindow.label_no_workspace_found.Visibility = Visibility.Visible;
            }

            welcomeWindow.Show();
            new AssistantCreator().Show();
            this.Close();
        }

        public void updateLoadingStatut(string statut_text, int progress_value)
        {
            loading_label.Content = statut_text;
            loading_progressbar.Value = progress_value;
        }

        public bool checkFolder()
        {
            bool isForgeWorkspace = true;
            String[] filesToCheck = { @"build.gradle" };
            String[] foldersToCheck = { @"src", @"src\main", @"src\main\resources", @"src\main\java", @"src\main\resources\assets", @"src\main\resources\META-INF" };

            int id = 0;
            while (isForgeWorkspace && id < filesToCheck.Length)
            {
                updateLoadingStatut("Vérification du fichier : " + filesToCheck[id], (int) loading_progressbar.Value + 3);
                isForgeWorkspace = File.Exists(Path.Combine(execution_folder, filesToCheck[id]));
                id++;
            }

            id = 0;
            while (isForgeWorkspace && id < foldersToCheck.Length)
            {
                updateLoadingStatut("Vérification du dossier : " + foldersToCheck[id], (int)loading_progressbar.Value + 3);
                isForgeWorkspace = Directory.Exists(Path.Combine(execution_folder, foldersToCheck[id]));
                id++;
            }

            return isForgeWorkspace;
        }
    }
}
