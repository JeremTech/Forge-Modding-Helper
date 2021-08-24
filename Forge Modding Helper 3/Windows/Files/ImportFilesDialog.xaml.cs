using Forge_Modding_Helper_3.Files;
using Forge_Modding_Helper_3.Utils;
using System;
using System.Collections.Generic;
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

namespace Forge_Modding_Helper_3.Windows.Files
{
    /// <summary>
    /// Logique d'interaction pour ImportFileDialog.xaml
    /// </summary>
    public partial class ImportFileDialog : Window
    {
        public ImportFileDialog(string[] filesToImport, string[] subFolders)
        {
            InitializeComponent();

            // Load translations
            UITextTranslator.LoadTranslationFile(OptionsFile.getCurrentLanguage());
            UITextTranslator.UpdateComponentsTranslations(this.MainGrid);
            this.Title = UITextTranslator.getTranslation("dialog.import.title");

            // Creating line for each files
            foreach (string file in filesToImport)
            {
                FilesListView.Items.Add(new FileEntry(true, System.IO.Path.GetFileName(file), subFolders));
            }
        }

        public ImportFileDialog(string[] filesToImport)
        {
            InitializeComponent();

            // Load translations
            UITextTranslator.LoadTranslationFile(OptionsFile.getCurrentLanguage());
            UITextTranslator.UpdateComponentsTranslations(this.MainGrid);
            this.Title = UITextTranslator.getTranslation("dialog.import.title");

            // Creating line for each files
            foreach (string file in filesToImport)
            {
                FilesListView.Items.Add(new FileEntry(true, System.IO.Path.GetFileName(file), new string[]{ UITextTranslator.getTranslation("dialog.import.root") }));
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

            public FileEntry(bool _ImportStatut, string _OriginalName, string[] _SubFoldersList)
            {
                this.ImportStatut = _ImportStatut;
                this.OriginalName = _OriginalName;
                this.FinalName = _OriginalName;
                this.SubFoldersList = _SubFoldersList;
                this.SubFolderDestination = SubFoldersList[0];
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
