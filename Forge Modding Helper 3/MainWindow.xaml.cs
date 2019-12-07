using System;
using System.IO;
using System.Windows;

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

            // Checking current folder
            updateLoadingStatut("Vérification du dossier courant...", 20);
            bool checkFiles = checkFolder();

            if(!checkFiles)
            {
                new AssistantCreator().Show();
                this.Close();
            }
            else
            {

            }
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
