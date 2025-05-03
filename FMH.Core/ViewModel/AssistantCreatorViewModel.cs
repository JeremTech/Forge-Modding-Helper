using CommunityToolkit.Mvvm.Input;
using FMH.Core.Model;
using FMH.Core.Provider;
using FMH.Core.UI.Common;
using FMH.Core.UI.Forge;
using FMH.Core.Utils.UI;
using FMH.Core.View.AssistantCreator;
using FMH.Workspace.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;

namespace FMH.Core.ViewModel
{
    public class AssistantCreatorViewModel : ViewModelBase
    {
        #region Properties
        private List<UserControl> AssistantPages { get; set; }
        public Action? CloseWindowAction { get; set; }


        public bool _dialogResult { get; set; }
        /// <summary>
        /// Dialog result
        /// </summary>
        public bool DialogResult
        {
            get
            {
                return _dialogResult;
            }
            set
            {
                _dialogResult = value;
                OnPropertyChanged();
            }
        }

        private int _currentPageNumber { get; set; }
        /// <summary>
        /// Current page number
        /// </summary>
        public int CurrentPageNumber
        {
            get
            {
                return _currentPageNumber;
            }
            set
            {
                _currentPageNumber = value;
                OnPropertyChanged();
            }
        }

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

        private UserControl _currentAssistantPage;
        /// <summary>
        /// Current assistant page
        /// </summary>
        public UserControl CurrentAssistantPage
        {
            get
            {
                return _currentAssistantPage;
            }
            set
            {
                _currentAssistantPage = value;
                OnPropertyChanged();
                OnPropertyChanged("PreviousButtonEnabled");
                OnPropertyChanged("NextButtonEnabled");
                OnPropertyChanged("NextButtonVisibility");
                OnPropertyChanged("FinishButtonVisibility");
            }
        }

        /// <summary>
        /// Previous button enable status
        /// </summary>
        public bool PreviousButtonEnabled
        {
            get
            {
                if(CurrentAssistantPage is AssistantCreatorApiTypeSelectionPageView
                   || CurrentAssistantPage is AssistantCreatorGenerationProcessPageView
                   || CurrentAssistantPage is AssistantCreatorFinishPageView)
                    return false;

                return true;
            }
        }

        /// <summary>
        /// Next button enable status
        /// </summary>
        public bool NextButtonEnabled
        {
            get
            {
                if (CurrentAssistantPage is AssistantCreatorGenerationProcessPageView
                    || CurrentAssistantPage is AssistantCreatorFinishPageView)
                    return false;

                return true;
            }
        }

        /// <summary>
        /// Next button visiblity
        /// </summary>
        public Visibility NextButtonVisibility
        {
            get
            {
                if (CurrentAssistantPage is AssistantCreatorFinishPageView)
                    return Visibility.Collapsed;

                return Visibility.Visible;
            }
        }

        /// <summary>
        /// Finish button visiblity
        /// </summary>
        public Visibility FinishButtonVisibility
        {
            get
            {
                if (CurrentAssistantPage is AssistantCreatorFinishPageView)
                    return Visibility.Visible;

                return Visibility.Collapsed;
            }
        }
        #endregion

        #region Commands
        /// <summary>
        /// Next command
        /// </summary>
        private ICommand _nextCommand;
        public ICommand NextCommand
        {
            get
            {
                return _nextCommand;
            }
            set
            {
                _nextCommand = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Previous command
        /// </summary>
        private ICommand _previousCommand;
        public ICommand PreviousCommand
        {
            get
            {
                return _previousCommand;
            }
            set
            {
                _previousCommand = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Finish command
        /// </summary>
        private ICommand _finishCommand;
        public ICommand FinishCommand
        {
            get
            {
                return _finishCommand;
            }
            set
            {
                _finishCommand = value;
                OnPropertyChanged();
            }
        }
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public AssistantCreatorViewModel() 
        {
            InitializeDefaultValues();
            this.NextCommand = new RelayCommand(Next);
            this.PreviousCommand = new RelayCommand(Previous);
            this.FinishCommand = new RelayCommand(Finish);
        }

        /// <summary>
        /// Initialize default values at window opening
        /// </summary>
        private void InitializeDefaultValues()
        {
            DialogResult = false;

            NewWorkspaceData = new NewWorkspace()
            {
                ModAPI = ModAPIType.Forge
            };

            AssistantPages = new List<UserControl>()
            {
                new AssistantCreatorApiTypeSelectionPageView(),
                new AssistantCreatorApiVersionSelectionPageView(),
                new AssistantCreatorWorkspaceBasicsSettingsPageView(),
                new AssistantCreatorWorkspaceTechnicalSettingsPageView(),
                new AssistantCreatorWorkspaceCustomizationSettingsPageView(),
                new AssistantCreatorGenerationSettingsPageView(),
                new AssistantCreatorGenerationProcessPageView(),
                new AssistantCreatorFinishPageView()
            };

            CurrentPageNumber = 0;
            CurrentAssistantPage = AssistantPages.ElementAt(CurrentPageNumber);
        }

        #region Commands functions
        /// <summary>
        /// Next command function
        /// </summary>
        internal void Next()
        {
            if(CurrentAssistantPage is IComponentValidated currentPage)
            {
                if (!currentPage.ValidateData())
                    return;
            }

            if (CurrentPageNumber + 1 == AssistantPages.Count)
                return;

            CurrentPageNumber++;
            CurrentAssistantPage = AssistantPages.ElementAt(CurrentPageNumber);

            if(CurrentAssistantPage is IComponentDisplayed newPage)
                newPage.OnComponentDisplayed(this);
        }

        /// <summary>
        /// Previous command function
        /// </summary>
        internal void Previous()
        {
            if (CurrentPageNumber == 0)
                return;

            CurrentPageNumber--;
            CurrentAssistantPage = AssistantPages.ElementAt(CurrentPageNumber);

            if (CurrentAssistantPage is IComponentDisplayed currentPage)
                currentPage.OnComponentDisplayed(this);
        }

        /// <summary>
        /// Finish command function
        /// </summary>
        internal void Finish()
        {
            // Open Project Explorer
            new ForgeProjectExplorer(NewWorkspaceData.WorkspaceFolderPath).Show();

            // Close Assistant Creator
            DialogResult = true;
            CloseWindowAction?.Invoke();
        }
        #endregion
    }
}
