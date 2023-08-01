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
using System.Windows.Shapes;
using FMH.Core.Files.Software;
using FMH.Core.Utils.UI;

namespace FMH.Core.UI.Dialogs
{
    /// <summary>
    /// Logique d'interaction pour ImportFilesDialog.xaml
    /// </summary>
    public partial class ImportFilesDialog : Window
    {
        private string _workspacePath;
        private string _modId;

        public ImportFilesDialog(string[] filesToImport, string assetsFolder, string workspacePath, string modId)
        {
            InitializeComponent();

            // Set properties
            _workspacePath = workspacePath;
            _modId = modId;

            // Load translations
            UITextTranslator.LoadTranslationFile(OptionsFile.GetCurrentLanguage());
            UITextTranslator.UpdateComponentsTranslations(this.MainGrid);
            this.Title = UITextTranslator.getTranslation("dialog.import.title");

            // Get Subdirectories names
            int i = 0;
            string[] subfoldersNames = new string[Directory.GetDirectories(System.IO.Path.Combine(_workspacePath, "src\\main\\resources\\assets", _modId, assetsFolder)).Length + 1];
            foreach (string folder in Directory.GetDirectories(System.IO.Path.Combine(_workspacePath, "src\\main\\resources\\assets", _modId, assetsFolder)))
            {
                subfoldersNames[i] = System.IO.Path.GetFileName(folder);
                i++;
            }

            // Add root choice
            subfoldersNames[i] = UITextTranslator.getTranslation("dialog.import.root");

            // Creating line for each files
            foreach (string file in filesToImport)
            {
                FilesListView.Items.Add(new FileEntry(true, System.IO.Path.GetFileName(file), subfoldersNames, file, assetsFolder));
            }
        }

        /// <summary>
        /// Represent a file entry in the listview
        /// </summary>
        class FileEntry
        {
            public bool ImportStatut { get; set; }
            public string OriginalName { get; set; }
            public string FinalName { get; set; }
            public string[] SubFoldersList { get; set; }
            public string SubFolderDestination { get; set; }
            public string OriginalFilePath { get; set; }
            public string AssetsFolder { get; set; }

            public FileEntry(bool _ImportStatut, string _OriginalName, string[] _SubFoldersList, string _FilePath, string _AssetsFolder)
            {
                this.ImportStatut = _ImportStatut;
                this.OriginalName = _OriginalName;
                this.FinalName = _OriginalName;
                this.SubFoldersList = _SubFoldersList;
                this.SubFolderDestination = SubFoldersList[0];
                this.OriginalFilePath = _FilePath;
                this.AssetsFolder = _AssetsFolder;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            // For each entry
            foreach (FileEntry element in FilesListView.Items)
            {
                // If importation statut is checked
                if (element.ImportStatut == true)
                {
                    // If selected folder is not root
                    if (element.SubFolderDestination != UITextTranslator.getTranslation("dialog.import.root"))
                    {
                        File.Copy(element.OriginalFilePath, System.IO.Path.Combine(_workspacePath, "src\\main\\resources\\assets", _modId, element.AssetsFolder, element.SubFolderDestination, element.FinalName));
                    }
                    else
                    {
                        File.Copy(element.OriginalFilePath, System.IO.Path.Combine(_workspacePath, "src\\main\\resources\\assets", _modId, element.AssetsFolder, element.FinalName));
                    }
                }
            }

            this.Close();
        }
    }
}
