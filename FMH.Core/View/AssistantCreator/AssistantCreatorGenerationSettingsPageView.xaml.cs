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
using System.Windows.Navigation;
using System.Windows.Shapes;
using CommunityToolkit.Mvvm.Input;
using FMH.Core.Utils;
using FMH.Core.Utils.UI;

namespace FMH.Core.View.AssistantCreator
{
    public partial class AssistantCreatorGenerationSettingsPageView : UserControl, IComponentValidated
    {
        #region Commands
        /// <summary>
        /// Next command
        /// </summary>
        private ICommand _browseOutputDirectoryCommand;
        public ICommand BrowseOutputDirectoryCommand
        {
            get
            {
                return _browseOutputDirectoryCommand;
            }
            set
            {
                _browseOutputDirectoryCommand = value;
            }
        }
        #endregion

        public AssistantCreatorGenerationSettingsPageView()
        {
            InitializeComponent();
            this.BrowseOutputDirectoryCommand = new RelayCommand(BrowseOutputDirectory);
        }

        public bool ValidateData()
        {
            if(string.IsNullOrWhiteSpace(GenerationPathTextBox.InputText))
            {
                GenerationPathTextBox.UpdateInputStatus(InputStatus.Error);
                return false;
            }

            return true;
        }

        public void BrowseOutputDirectory()
        {
            // Allow user to select workspace output directory
            using (var fbd = new System.Windows.Forms.FolderBrowserDialog())
            {
                // Display FolderDialog
                System.Windows.Forms.DialogResult result = fbd.ShowDialog();

                // Check the user selection
                if (!string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    this.GenerationPathTextBox.InputText = fbd.SelectedPath;
                }
            }
        }
    }
}
