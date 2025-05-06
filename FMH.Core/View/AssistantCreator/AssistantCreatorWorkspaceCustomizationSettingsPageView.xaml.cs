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
using System.Windows.Navigation;
using System.Windows.Shapes;
using FMH.Core.Utils.UI;
using FMH.Core.ViewModel;
using FMH.Workspace.WorkspaceManager;
using Microsoft.VisualBasic.FileIO;

namespace FMH.Core.View.AssistantCreator
{
    public partial class AssistantCreatorWorkspaceCustomizationSettingsPageView : UserControl, IComponentDisplayed
    {
        private AssistantCreatorViewModel? _viewModelDataContext;

        public AssistantCreatorWorkspaceCustomizationSettingsPageView()
        {
            // Set control events
            this.Initialized += AssistantCreatorWorkspaceCustomizationSettingsPageView_Initialized;

            InitializeComponent();
        }

        #region Events
        private void AssistantCreatorWorkspaceCustomizationSettingsPageView_Initialized(object? sender, EventArgs e)
        {
            UITextTranslator.UpdateComponentsTranslations(MainGrid);
        }

        private void ModLogoDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if(_viewModelDataContext != null)
                _viewModelDataContext.NewWorkspaceData.ModLogoSourcePath = string.Empty;
        }

        private void ModLogoBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            // Create and configure FileDialog
            Microsoft.Win32.OpenFileDialog fileDialog = new Microsoft.Win32.OpenFileDialog();
            fileDialog.RestoreDirectory = true;
            fileDialog.Title = UITextTranslator.GetTranslation("project_explorer.mod_settings.choose_logo_file");
            fileDialog.DefaultExt = "png";
            fileDialog.Filter = UITextTranslator.GetTranslation("project_explorer.mod_settings.filter_logo_file") + " (*.png)|*.png";
            fileDialog.CheckFileExists = true;
            fileDialog.CheckPathExists = true;
            fileDialog.Multiselect = false;
            // Display the FileDialog
            fileDialog.ShowDialog();

            // Check the user selection
            if (!string.IsNullOrWhiteSpace(fileDialog.FileName) && _viewModelDataContext != null)
                _viewModelDataContext.NewWorkspaceData.ModLogoSourcePath = fileDialog.FileName;

        }
        #endregion

        #region Interfaces implementations
        public void OnComponentDisplayed(params object[] args)
        {
            // Retrieve parent DataContext from arguments
            if (args.Any() && args[0] is AssistantCreatorViewModel)
                _viewModelDataContext = args[0] as AssistantCreatorViewModel;
        }
        #endregion
    }
}
