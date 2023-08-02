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
using FMH.Core.Utils.Software;
using FMH.Core.Utils.UI;

namespace FMH.Core.UI.Dialogs
{
    /// <summary>
    /// Logique d'interaction pour UpgradeSettingsDialog.xaml
    /// </summary>
    public partial class UpgradeSettingsDialog : Window
    {
        private List<string> _previousVersionsOptions;

        public UpgradeSettingsDialog()
        {
            InitializeComponent();

            // Data
            _previousVersionsOptions = new List<string>();

            // Events
            Loaded += UpgradeSettingsDialog_Loaded;
            OkButton.Click += OkButton_Click;
        }

        #region Events
        private void UpgradeSettingsDialog_Loaded(object sender, RoutedEventArgs e)
        {
            // Load previous versions options            
            var previousVersionsDataDirectories = Directory.EnumerateDirectories(SoftwareDataManager.GetSoftwareDataDirectory());
            var legacyDataFiles = Directory.EnumerateFiles(SoftwareDataManager.GetSoftwareDataDirectory());

            if(legacyDataFiles.Any()) 
            {
                _previousVersionsOptions.Add("Settings from a version before 3.3.0.0");
            }

            if(previousVersionsDataDirectories.Any())
            {
                _previousVersionsOptions.AddRange(previousVersionsDataDirectories.Where(dir => !string.Equals(dir, SoftwareDataManager.GetCurrentVersionDataDirectory()))
                                                                                 .Select(dir => Path.GetFileName(dir)));
            }

            this.previousVersionComboBox.ItemsSource = _previousVersionsOptions;
            this.previousVersionComboBox.SelectedIndex = 0;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            // No importation
            if (radioButtonOptionNone.IsChecked.HasValue && radioButtonOptionNone.IsChecked.Value)
            {
                this.Close();
                return;
            }

            // No selection
            if (string.IsNullOrEmpty((string)this.previousVersionComboBox.SelectedItem))
                return;

            // Previous settings directory path
            string previousDirectoryPath;
            if (string.Equals((string)this.previousVersionComboBox.SelectedItem, "Settings from a version before 3.3.0.0"))
                previousDirectoryPath = Path.Combine(SoftwareDataManager.GetSoftwareDataDirectory());
            else
                previousDirectoryPath = Path.Combine(SoftwareDataManager.GetSoftwareDataDirectory(), (string)this.previousVersionComboBox.SelectedItem);

            // Import settings
            SoftwareVersionUpgrader.ImportOldSettings(previousDirectoryPath);
            this.Close();
        }
        #endregion
    }
}
