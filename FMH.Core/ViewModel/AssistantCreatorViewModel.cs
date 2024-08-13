using FMH.Core.Model;
using FMH.Core.View.AssistantCreator;
using FMH.Workspace.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace FMH.Core.ViewModel
{
    public class AssistantCreatorViewModel : ViewModelBase
    {
        private List<Page> AssistantPages { get; set; }
        private int CurrentPageNumber { get; set; }

        #region Properties
        private NewWorkspace _newWorkspaceData;
        /// <summary>
        /// Creation workspace data
        /// </summary>
        public NewWorkspace NewWorkspaceData
        {
            get
            {
                return _newWorkspaceData;
            }
            set
            {
                _newWorkspaceData = value;
                OnPropertyChanged();
            }
        }

        private Page _currentAssistantPage;
        /// <summary>
        /// Current assistant page
        /// </summary>
        public Page CurrentAssistantPage
        {
            get
            {
                return _currentAssistantPage;
            }
            set
            {
                _currentAssistantPage = value;
                OnPropertyChanged();
            }
        }
        #endregion

        public AssistantCreatorViewModel() 
        {
            InitializeDefaultValues();
        }

        /// <summary>
        /// Initialize default values at window opening
        /// </summary>
        private void InitializeDefaultValues()
        {
            NewWorkspaceData = new NewWorkspace()
            {
                ModAPI = ModAPIType.Forge
            };

            AssistantPages = new List<Page>()
            {
                new AssistantCreatorFirstPageView(this),
            };

            CurrentPageNumber = 0;
            CurrentAssistantPage = AssistantPages.ElementAt(CurrentPageNumber);
        }

    }
}
