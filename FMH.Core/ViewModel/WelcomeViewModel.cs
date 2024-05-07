using CommunityToolkit.Mvvm.Input;
using FMH.Core.Files.Software;
using FMH.Core.Model;
using FMH.Core.Provider;
using FMH.Core.UI.Common;
using FMH.Core.UI.Dialogs;
using FMH.Core.UI.Forge;
using FMH.Core.Utils.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using MessageBox = System.Windows.MessageBox;

namespace FMH.Core.ViewModel
{
    public class WelcomeViewModel : ViewModelBase
    {
        #region Properties
        private ObservableCollection<RecentWorkspace> _recentsWorkspaces;
        /// <summary>
        /// Recents workspaces data
        /// </summary>
        public ObservableCollection<RecentWorkspace> RecentsWorkspaces 
        {  
            get {  return _recentsWorkspaces; }
            set
            {
                _recentsWorkspaces = value;
                OnPropertyChanged(nameof(RecentsWorkspaces));
            }
        }
        #endregion

        #region Commands
        /// <summary>
        /// Open settings command
        /// </summary>
        private ICommand _openSettingsCommand;
        public ICommand OpenSettingsCommand
        {
            get
            {
                return _openSettingsCommand;
            }
            set
            {
                _openSettingsCommand = value;
                OnPropertyChanged(nameof(OpenSettingsCommand));
            }
        }

        /// <summary>
        /// Create project command
        /// </summary>
        private ICommand _createProjectCommand;
        public ICommand CreateProjectCommand
        {
            get
            {
                return _createProjectCommand;
            }
            set
            {
                _createProjectCommand = value;
                OnPropertyChanged(nameof(CreateProjectCommand));
            }
        }

        /// <summary>
        /// Import project command
        /// </summary>
        private ICommand _importProjectCommand;
        public ICommand ImportProjectCommand
        {
            get
            {
                return _importProjectCommand;
            }
            set
            {
                _importProjectCommand = value;
                OnPropertyChanged(nameof(ImportProjectCommand));
            }
        }

        /// <summary>
        /// Open project command
        /// </summary>
        private ICommand _openProjectCommand;
        public ICommand OpenProjectCommand
        {
            get
            {
                return _openProjectCommand;
            }
            set
            {
                _openProjectCommand = value;
                OnPropertyChanged(nameof(OpenProjectCommand));
            }
        }
        #endregion

        #region Actions
        /// <summary>
        /// Close parent window function
        /// </summary>
        /// <remarks>This action must be defined in the parent window constructor</remarks>
        public Action CloseParentWindow;
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public WelcomeViewModel()
        {
            LoadRecentsWorkspacesList();
            this.OpenSettingsCommand = new RelayCommand(OpenSettings);
            this.CreateProjectCommand = new RelayCommand(CreateProject);
            this.ImportProjectCommand = new RelayCommand(ImportProject);
            this.OpenProjectCommand = new RelayCommand<RecentWorkspace>(OpenProject);
        }

        #region Data management
        /// <summary>
        /// Load recents workspaces list
        /// </summary>
        private void LoadRecentsWorkspacesList()
        {
            this.RecentsWorkspaces = new ObservableCollection<RecentWorkspace>(RecentsWorkspacesProvider.GetRecentWorkspaces());
        }
        #endregion

        #region Commands functions
        /// <summary>
        /// Open settings command function
        /// </summary>
        private void OpenSettings()
        {
            new OptionWindow().ShowDialog();
        }

        /// <summary>
        /// Create project command function
        /// </summary>
        private void CreateProject()
        {
            // Display the Assistant Creator
            ForgeAssistantCreator assistantCreator = new ForgeAssistantCreator();
            assistantCreator.ShowDialog();

            if (assistantCreator.DialogResult.HasValue && assistantCreator.DialogResult.Value)
                this.CloseParentWindow();
        }

        /// <summary>
        /// Import project command function
        /// </summary>
        private void ImportProject()
        {
            var importProjectDialog = new ImportProjectDialog();
            importProjectDialog.ShowDialog();

            if (importProjectDialog.DialogResult.HasValue && importProjectDialog.DialogResult.Value)
            {
                var lastWorkspace = LastWorkspaces.LastWorkspacesData.OrderByDescending(w => w.LastUpdated).FirstOrDefault();
                if (lastWorkspace != null)
                {
                    new ForgeProjectExplorer(lastWorkspace.WorkspacePath).Show();
                    this.CloseParentWindow();
                }
            }
        }

        /// <summary>
        /// Open project command function
        /// </summary>
        private void OpenProject(RecentWorkspace workspace)
        {
            if (workspace != null)
            {
                if (Directory.Exists(workspace.WorkspacePath))
                {
                    new ForgeProjectExplorer(workspace.WorkspacePath).Show();
                    this.CloseParentWindow();
                }
                else
                    MessageBox.Show(UITextTranslator.getTranslation("welcome.alert.open.error"), "Forge Modding Helper", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion
    }
}
