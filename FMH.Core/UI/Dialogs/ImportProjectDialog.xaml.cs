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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using FMH.Core.Files.Software;
using FMH.Core.Objects;
using FMH.Core.Provider;
using FMH.Core.Utils.UI;
using FMH.Workspace.Data;
using FMH.Workspace.WorkspaceManager;
using FontAwesome.WPF;
using Path = System.IO.Path;

namespace FMH.Core.UI.Dialogs
{
    public partial class ImportProjectDialog : Window
    {
        IWorkspaceManager _workspaceManager;

        public ImportProjectDialog()
        {
            InitializeComponent();

            // Events
            Loaded += ImportProjectDialog_Loaded;
            textBoxWorskpacePath.TextChanged += TextBoxWorskpacePath_TextChanged;
        }

        private async void UpdateWorkspaceValidityStatus()
        {
            if (string.IsNullOrWhiteSpace(comboBoxMcVersion.Text)
                || string.IsNullOrWhiteSpace(textBoxWorskpacePath.Text))
                return;

            // Set spinner
            statusImage.Visibility = Visibility.Visible;
            statusImage.Icon = FontAwesomeIcon.CircleOutlineNotch;
            statusImage.Spin = true;
            statusImage.SpinDuration = 5;
            statusImage.Foreground = (Brush)App.Current.FindResource("FontColorPrimary");
            statusTextBlock.Visibility = Visibility.Visible;
            statusTextBlock.Text = UITextTranslator.GetTranslation("dialog.import_project.checks_working");

            var workspaceValidity = await Task.Run<bool>(() => { return CheckWorkspaceValidity(); });

            // Stop spinner
            statusImage.Spin = false;
            statusImage.SpinDuration = 0;

            // Set result
            if (workspaceValidity)
            {
                statusImage.Icon = FontAwesomeIcon.Check;
                statusImage.Foreground = new SolidColorBrush(Colors.DarkGreen);
                statusTextBlock.Text = UITextTranslator.GetTranslation("dialog.import_project.checks_success");
                importButton.IsEnabled = true;
            }
            else
            {
                statusImage.Icon = FontAwesomeIcon.Times;
                statusImage.Foreground = new SolidColorBrush(Colors.Red);
                statusTextBlock.Text = UITextTranslator.GetTranslation("dialog.import_project.checks_failed");
                importButton.IsEnabled = false;
            }
        }

        private bool CheckWorkspaceValidity()
        {
            return Dispatcher.Invoke<bool>(() =>
            {
                if (string.IsNullOrEmpty(comboBoxMcVersion.Text)
                    || string.IsNullOrEmpty(textBoxWorskpacePath.Text)
                    || !Directory.Exists(textBoxWorskpacePath.Text))
                    return false;

                _workspaceManager = WorkspaceManagerHelper.GetWorkspaceManager(textBoxWorskpacePath.Text, comboBoxMcVersion.Text, ModAPIType.Forge);

                if (_workspaceManager == null)
                    return false;

                return _workspaceManager.CheckWorkspaceValidity(App.GetSupportedForgeMinecraftVersions());
            });
        }

        #region Events
        private void ImportProjectDialog_Loaded(object sender, RoutedEventArgs e)
        {
            UITextTranslator.UpdateComponentsTranslations(this.mainGrid);
            this.Title = UITextTranslator.GetTranslation("dialog.import_project.title");

            comboBoxMcVersion.ItemsSource = App.GetSupportedForgeMinecraftVersions().OrderByDescending(v => v);
            comboBoxMcVersion.SelectedIndex = 0;
        }

        private void TextBoxWorskpacePath_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateWorkspaceValidityStatus();
        }

        private void ComboBoxMcVersion_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateWorkspaceValidityStatus();
        }

        private void BrowseDirectoyButton_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            folderDialog.ShowNewFolderButton = false;
            folderDialog.ShowDialog();

            if(!string.IsNullOrEmpty(folderDialog.SelectedPath))
                textBoxWorskpacePath.Text = folderDialog.SelectedPath;
        }

        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            WorkspaceManagerHelper.WriteWorkspaceData(_workspaceManager);

            var recentWorkspace = new Model.RecentWorkspace() 
            { 
                WorkspacePath = _workspaceManager.WorkspaceProperties.WorkspacePath,
                LastUpdated = DateTime.Now,
                WorkspaceModAPI = _workspaceManager.WorkspaceProperties.ModAPI,
                WorkspaceMcVersion = _workspaceManager.WorkspaceProperties.MCVersion
            };

            RecentsWorkspacesProvider.AddRecentWorkspace(recentWorkspace);
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
        #endregion
    }
}
