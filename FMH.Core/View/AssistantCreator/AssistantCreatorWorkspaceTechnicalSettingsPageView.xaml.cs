using FMH.Core.Utils;
using FMH.Core.Utils.UI;
using FMH.Core.ViewModel;
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

namespace FMH.Core.View.AssistantCreator
{
    /// <summary>
    /// Logique d'interaction pour AssistantCreatorWorkspaceTechnicalSettingsPageView.xaml
    /// </summary>
    public partial class AssistantCreatorWorkspaceTechnicalSettingsPageView : UserControl, IComponentDisplayed, IComponentValidated
    {
        public AssistantCreatorWorkspaceTechnicalSettingsPageView()
        {
            // Set control events
            this.Initialized += AssistantCreatorWorkspaceTechnicalSettingsPageView_Initialized;

            InitializeComponent();
        }

        #region Events
        private void AssistantCreatorWorkspaceTechnicalSettingsPageView_Initialized(object? sender, EventArgs e)
        {
            UITextTranslator.UpdateComponentsTranslations(MainGrid);
        }
        #endregion

        #region Interfaces implementations
        /// <inheritdoc/>
        public void OnComponentDisplayed(params object[] args)
        {
            // Retrieve parent DataContext from arguments
            if (args.Any() && args[0] is AssistantCreatorViewModel viewModelDataContext)
            {
                // Create a proposition for the modid
                if (string.IsNullOrEmpty(viewModelDataContext.NewWorkspaceData.ModId))
                    viewModelDataContext.NewWorkspaceData.ModId = StringUtils.CreateModIDFromModName(viewModelDataContext.NewWorkspaceData.ModName);

                // Create a proposition for the modgroup
                if (string.IsNullOrEmpty(viewModelDataContext.NewWorkspaceData.ModGroup))
                    viewModelDataContext.NewWorkspaceData.ModGroup = StringUtils.CreateModGroupFromModIDAndAuthor(viewModelDataContext.NewWorkspaceData.ModId, viewModelDataContext.NewWorkspaceData.ModAuthors);
            }
        }

        /// <inheritdoc/>
        public bool ValidateData()
        {
            var result = true;

            // Mod id
            if (string.IsNullOrWhiteSpace(ModIdTextBox.InputText))
            {
                ModIdTextBox.UpdateInputStatus(InputStatus.Error);
                result = false;
            }

            // Mod group
            if (string.IsNullOrWhiteSpace(ModGroupTextBox.InputText))
            {
                ModGroupTextBox.UpdateInputStatus(InputStatus.Error);
                result = false;
            }

            // Mod version
            if (string.IsNullOrWhiteSpace(ModVersionTextBox.InputText))
            {
                ModVersionTextBox.UpdateInputStatus(InputStatus.Error);
                result = false;
            }

            return result;
        }
        #endregion
    }
}
