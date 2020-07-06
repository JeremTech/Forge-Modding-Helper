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
        #endregion

        private void open_mod_button_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(this.selectedProjectPath))
            {
                new ProjectScanWindow(this.selectedProjectPath).Show();
                this.Close();
            }
        }

        private void listbox_recent_workspaces_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listbox_recent_workspaces.SelectedItem != null)
            {
                Workspace workspace = listbox_recent_workspaces.SelectedItem as Workspace;

                if (workspace != null)
                    this.selectedProjectPath = workspace.path;
                else
                    this.selectedProjectPath = "";

            }
            else
            {
                this.selectedProjectPath = "";
            }

            if (!string.IsNullOrWhiteSpace(this.selectedProjectPath))
                open_mod_button.IsEnabled = true;
            else
                open_mod_button.IsEnabled = false;
        }
    }
}
