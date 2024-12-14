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
using FMH.Core.Utils.UI;
using FMH.Core.ViewModel;

namespace FMH.Core.View.AssistantCreator
{

    public partial class AssistantCreatorWorkspaceBasicsSettingsPageView : UserControl, IComponentValidated
    {
        public AssistantCreatorWorkspaceBasicsSettingsPageView()
        {
            InitializeComponent();
        }

        /// <inheritdoc/>
        public bool ValidateData()
        {
            var result = true;

            // Mod name
            if(string.IsNullOrWhiteSpace(ModNameTextBox.InputText))
            {
                ModNameTextBox.UpdateInputStatus(InputStatus.Error);
                result = false;
            }

            // Mod authors
            if(string.IsNullOrWhiteSpace(ModAuthorsTextBox.InputText))
            {
                ModAuthorsTextBox.UpdateInputStatus(InputStatus.Error);
                result = false;
            }

            // Mod license
            if(string.IsNullOrWhiteSpace(ModLicenseTextBox.InputText))
            {
                ModLicenseTextBox.UpdateInputStatus(InputStatus.Error);
                result = false;
            }

            return result;
        }
    }
}
