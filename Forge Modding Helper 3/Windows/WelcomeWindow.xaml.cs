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
using Forge_Modding_Helper_3.Files;
using Forge_Modding_Helper_3.Files.Software;
using Forge_Modding_Helper_3.Files.Workspace_Data;
using Forge_Modding_Helper_3.Objects;
using Forge_Modding_Helper_3.Utils;
using Microsoft.VisualBasic.FileIO;
using MessageBox = System.Windows.MessageBox;

namespace Forge_Modding_Helper_3.Windows
{
    /// <summary>
    /// Logique d'interaction pour WelcomeWindow.xaml
    /// </summary>
    public partial class WelcomeWindow : Window
    {
        // Selected project path
        private string selectedProjectPath = "";

        public WelcomeWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Function called at the window initialization
        /// </summary>
        private void Window_Initialized(object sender, EventArgs e)
        {
            version_label.Content = AppInfos.GetApplicationVersionString();

            // Loading translations
            UITextTranslator.UpdateComponentsTranslations(this.main_grid);
            this.Title = UITextTranslator.getTranslation("welcome.title");
        }

        #region Buttons
        /// <summary>
        /// Function called when the user click on the "new" button
        /// </summary>
        private void new_mod_button_Click(object sender, RoutedEventArgs e)
        {
            // Display the Assistant Creator
            AssistantCreator creator = new AssistantCreator();
            creator.Show();

            this.Close();
        }

        /// <summary>
        /// Function called when the user click on the "open" button
        /// </summary>
        private void open_mod_button_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(this.selectedProjectPath))
            {
                if (Directory.Exists(this.selectedProjectPath))
                {
                    new ProjectScanWindow(this.selectedProjectPath).Show();
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
            if (!string.IsNullOrWhiteSpace(this.selectedProjectPath))
            {
                MessageBoxResult msgResult = MessageBox.Show(UITextTranslator.getTranslation("welcome.alert.delete"), "Forge Modding Helper", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (msgResult == MessageBoxResult.Yes)
                {
                    if (Directory.Exists(this.selectedProjectPath))
                    {
                        FileSystem.DeleteDirectory(this.selectedProjectPath, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin, UICancelOption.DoNothing);
                        LastWorkspaces.RefreshData();
                        listbox_recent_workspaces.ItemsSource = null;
                        listbox_recent_workspaces.ItemsSource = LastWorkspaces.LastWorkspacesProjectFile;
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
            LastWorkspaces.RefreshData();
            listbox_recent_workspaces.ItemsSource = null;
            listbox_recent_workspaces.ItemsSource = LastWorkspaces.LastWorkspacesProjectFile;
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
                    string buildGradle = File.ReadAllText(System.IO.Path.Combine(dialog.SelectedPath, "build.gradle"));
                    string forge_version = buildGradle.getBetween("minecraft 'net.minecraftforge:forge:", "'");
                    string minecraft_version = forge_version.getBetween("", "-");

                    if (!AppInfos.getSupportedMinecraftVersions().Contains(minecraft_version))
                    {
                        MessageBox.Show(UITextTranslator.getTranslation("welcome.alert.import.error.unsupported"), "Forge Modding Helper", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        new ProjectScanWindow(dialog.SelectedPath).Show();
                        this.Close();
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

            UITextTranslator.LoadTranslationFile(OptionsFile.getCurrentLanguage());
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
                ProjectFile workspace = listbox_recent_workspaces.SelectedItem as ProjectFile;
                this.selectedProjectPath = workspace != null ? workspace.ProjectPath : "";
            }
            else
            {
                this.selectedProjectPath = "";
            }

            // Update project buttons
            if (!string.IsNullOrWhiteSpace(this.selectedProjectPath))
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
    }
}
