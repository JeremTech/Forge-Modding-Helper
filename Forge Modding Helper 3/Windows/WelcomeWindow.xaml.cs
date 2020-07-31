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
using Forge_Modding_Helper_3.Files;
using Forge_Modding_Helper_3.Objects;
using Forge_Modding_Helper_3.Utils;
using Microsoft.VisualBasic.FileIO;

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

            UITextTranslator.UpdateComponentsTranslations(this.main_grid);
        }

        #region Buttons
        /// <summary>
        /// Function called when the user click on the "new" button
        /// </summary>
        private void new_mod_button_Click(object sender, RoutedEventArgs e)
        {
            // Display the Assistant Creator
            AssistantCreator creator = new AssistantCreator();
            creator.ShowDialog();
        }

        /// <summary>
        /// Function called when the user click on the "open" button
        /// </summary>
        private void open_mod_button_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(this.selectedProjectPath))
            {
                new ProjectScanWindow(this.selectedProjectPath).Show();
                this.Close();
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
                    Workspace workspace = listbox_recent_workspaces.SelectedItem as Workspace;
                    FileSystem.DeleteDirectory(this.selectedProjectPath, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin, UICancelOption.DoNothing);
                    RecentWorkspaces.RecentWorkspacesList.Remove(workspace);
                    RecentWorkspaces.WriteDataFile();
                    listbox_recent_workspaces.ItemsSource = null;
                    listbox_recent_workspaces.ItemsSource = RecentWorkspaces.RecentWorkspacesList;
                }
            }
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
                Workspace workspace = listbox_recent_workspaces.SelectedItem as Workspace;
                this.selectedProjectPath = workspace != null ? workspace.path : "";
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
    }
}
