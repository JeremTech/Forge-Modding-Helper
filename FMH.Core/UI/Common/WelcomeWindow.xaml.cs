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
using FMH.Core.Files.Software;
using FMH.Core.Objects;
using FMH.Core.UI.Forge;
using FMH.Core.Utils;
using FMH.Core.Utils.UI;
using Microsoft.VisualBasic.FileIO;
using MessageBox = System.Windows.MessageBox;

namespace FMH.Core.UI.Common
{
    /// <summary>
    /// Logique d'interaction pour WelcomeWindow.xaml
    /// </summary>
    public partial class WelcomeWindow : Window
    {
        // Selected project path
        private WorkspaceEntry _selectedWorkspace;

        public WelcomeWindow()
        {
            InitializeComponent();

            this.Loaded += WelcomeWindow_Loaded;
        }

        private void WelcomeWindow_Loaded(object sender, RoutedEventArgs e)
        {
            /// Update version label content
            version_label.Content = App.GetApplicationVersionString();

            // Loading translations
            UITextTranslator.UpdateComponentsTranslations(this.main_grid);
            this.Title = UITextTranslator.getTranslation("welcome.title");

            // Loading last workspaces
            RefreshLastProjectData();
        }

        #region Buttons
        /// <summary>
        /// Function called when the user click on the "new" button
        /// </summary>
        private void new_mod_button_Click(object sender, RoutedEventArgs e)
        {
            // Display the Assistant Creator
            ForgeAssistantCreator creator = new ForgeAssistantCreator();
            creator.Show();

            this.Close();
        }

        /// <summary>
        /// Function called when the user click on the "open" button
        /// </summary>
        private void open_mod_button_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedWorkspace != null)
            {
                if (Directory.Exists(_selectedWorkspace.WorkspacePath))
                {
                    new ForgeProjectExplorer(_selectedWorkspace.WorkspacePath).Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show(UITextTranslator.getTranslation("welcome.alert.open.error"), "Forge Modding Helper", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// Function called when the user click on the "delete" button
        /// </summary>
        private void delete_mod_button_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedWorkspace != null)
            {
                MessageBoxResult msgResult = MessageBox.Show(UITextTranslator.getTranslation("welcome.alert.delete"), "Forge Modding Helper", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (msgResult == MessageBoxResult.Yes)
                {
                    if (Directory.Exists(_selectedWorkspace.WorkspacePath))
                    {
                        FileSystem.DeleteDirectory(_selectedWorkspace.WorkspacePath, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin, UICancelOption.DoNothing);
                        RefreshLastProjectData();
                    }
                    else
                    {
                        MessageBox.Show(UITextTranslator.getTranslation("welcome.alert.delete.error"), "Forge Modding Helper", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        /// <summary>
        /// Function called when the user click on the "refresh" button
        /// </summary>
        private void refresh_mod_list_button_Click(object sender, RoutedEventArgs e)
        {
            RefreshLastProjectData();
        }

        private void import_mod_button_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.ShowNewFolderButton = false;
            dialog.ShowDialog();

            if (Directory.Exists(dialog.SelectedPath))
            {
                if (DirectoryUtils.CheckFolderIsForgeWorkspace(dialog.SelectedPath))
                {
                    string buildGradle = File.ReadAllText(Path.Combine(dialog.SelectedPath, "build.gradle"));
                    string forge_version = buildGradle.getBetween("minecraft 'net.minecraftforge:forge:", "'");
                    string minecraft_version = forge_version.getBetween("", "-");

                    if (!App.getSupportedMinecraftVersions().Contains(minecraft_version))
                    {
                        MessageBox.Show(UITextTranslator.getTranslation("welcome.alert.import.error.unsupported"), "Forge Modding Helper", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        //new ProjectScanWindow(dialog.SelectedPath).Show();
                        //this.Close();
                    }
                }
                else
                {
                    MessageBox.Show(UITextTranslator.getTranslation("welcome.alert.import.error.invalid"), "Forge Modding Helper", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void options_mod_button_Click(object sender, RoutedEventArgs e)
        {
            new OptionWindow().ShowDialog();

            UITextTranslator.LoadTranslationFile(OptionsFile.GetCurrentLanguage());
            UITextTranslator.UpdateComponentsTranslations(this);
            this.Title = UITextTranslator.getTranslation("welcome.title");
        }
        #endregion

        /// <summary>
        /// Function called when the project selection is changed
        /// </summary>
        private void listbox_recent_workspaces_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Verifying selection
            if (listbox_recent_workspaces.SelectedItem != null)
            {
                WorkspaceEntry workspace = listbox_recent_workspaces.SelectedItem as WorkspaceEntry;
                this._selectedWorkspace = workspace ?? null;
            }
            else
            {
                this._selectedWorkspace = null;
            }

            // Update project buttons
            if (_selectedWorkspace != null)
            {
                open_mod_button.IsEnabled = true;
                delete_mod_button.IsEnabled = true;
            }
            else
            {
                open_mod_button.IsEnabled = false;
                delete_mod_button.IsEnabled = false;
            }
        }

        /// <summary>
        /// Function called
        /// </summary>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.listbox_recent_workspaces.ItemsSource = null;
        }

        private void RefreshLastProjectData()
        {
            LastWorkspaces.RefreshData();
            this.listbox_recent_workspaces.ItemsSource = null;
            listbox_recent_workspaces.ItemsSource = LastWorkspaces.LastWorkspacesData;
        }
    }
}
