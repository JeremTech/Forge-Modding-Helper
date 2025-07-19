using CommunityToolkit.Mvvm.Input;
using FMH.Core.Model;
using FMH.Core.Provider;
using FMH.Core.Utils.UI;
using FMH.Workspace.Data;
using FMH.Workspace.WorkspaceManager;
using FontAwesome.WPF;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace FMH.Core.ViewModel
{
    public class ImportProjectViewModel : ViewModelBase
    {
        #region Properties
        /// <summary>
        /// Mod API type to import
        /// </summary>
        private ModAPIType _modAPIType;
        public ModAPIType ModAPIType
        {
            get { return _modAPIType; }
            set
            {
                _modAPIType = value;
                OnPropertyChanged();
                OnPropertyChanged("IsImportButtonEnabled");
                OnPropertyChanged("IsMinecraftVersionsComboBoxEnabled");
                LoadAvailableMinecraftVersions();
                AnalyzeProject();
            }
        }

        /// <summary>
        /// Available Minecraft versions for the selected Mod API type
        /// </summary>
        private string _projectDirectoryPath;
        public string ProjectDirectoryPath
        {
            get { return _projectDirectoryPath; }
            set
            {
                _projectDirectoryPath = value;
                OnPropertyChanged();
                OnPropertyChanged("IsImportButtonEnabled");
                AnalyzeProject();
            }
        }

        /// <summary>
        /// Available Minecraft versions for the selected Mod API type
        /// </summary>
        private string _projectMinecraftVersion;
        public string ProjectMinecraftVersion
        {
            get { return _projectMinecraftVersion; }
            set
            {
                _projectMinecraftVersion = value;
                OnPropertyChanged();
                OnPropertyChanged("IsImportButtonEnabled");
                AnalyzeProject();
            }
        }

        /// <summary>
        /// Available Minecraft versions for the selected Mod API type
        /// </summary>
        private ObservableCollection<string> _availableMinecraftVersions;
        public ObservableCollection<string> AvailableMinecraftVersions
        {
            get { return _availableMinecraftVersions; }
            set
            {
                _availableMinecraftVersions = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets a value indicating whether the Minecraft versions combobox is enabled
        /// </summary>
        public bool IsMinecraftVersionsComboBoxEnabled
        {
            get
            {
                return ModAPIType != ModAPIType.None;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the import button is enabled
        /// </summary>
        public bool IsImportButtonEnabled
        {
            get 
            {
                return ModAPIType != ModAPIType.None
                       && !string.IsNullOrEmpty(ProjectMinecraftVersion)
                       && !string.IsNullOrEmpty(ProjectDirectoryPath)
                       && IsProjectCompatible;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the project is compatible with the selected settings
        /// </summary>
        private bool _isProjectCompatible;
        private bool IsProjectCompatible
        {
            get { return _isProjectCompatible; }
            set
            {
                _isProjectCompatible = value;
                OnPropertyChanged();
                OnPropertyChanged("IsImportButtonEnabled");
            }
        }


        /// <summary>
        /// Gets or sets the text displayed on the import button.
        /// </summary>
        private string _importButtonText;
        public string ImportButtonText
        {
            get { return _importButtonText; }
            set
            {
                _importButtonText = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the icon displayed on the import button.
        /// </summary>
        private FontAwesomeIcon _importButtonIcon;
        public FontAwesomeIcon ImportButtonIcon
        {
            get { return _importButtonIcon; }
            set
            {
                _importButtonIcon = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Dialog result
        /// </summary>
        private bool _dialogResult { get; set; }
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

        /// <summary>
        /// Workspace manager used to import the project
        /// </summary>
        private IWorkspaceManager _workspaceManager;

        /// <summary>
        /// Gets or sets the action to be executed when the parent window is closed
        /// </summary>
        public Action? CloseWindowAction { get; set; }
        #endregion

        #region Commands
        /// <summary>
        /// Browse project directory command
        /// </summary>
        private ICommand _browseProjectDirectoryCommand;
        public ICommand BrowseProjectDirectoryCommand
        {
            get
            {
                return _browseProjectDirectoryCommand;
            }
            set
            {
                _browseProjectDirectoryCommand = value;
                OnPropertyChanged();
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
                OnPropertyChanged();
            }
        }
        #endregion

        public ImportProjectViewModel()
        {
            // Initialize default values
            DialogResult = false;
            ModAPIType = ModAPIType.None;
            UpdateImportButton(UITextTranslator.GetTranslation("project_importation.button.import.waiting"), FontAwesomeIcon.None);
            AvailableMinecraftVersions = new ObservableCollection<string>();

            // Initialize commands
            BrowseProjectDirectoryCommand = new RelayCommand(BrowseProjectDirectory);
            ImportProjectCommand = new RelayCommand(ImportProject);
        }

        #region Data management
        private void LoadAvailableMinecraftVersions()
        {
            if (AvailableMinecraftVersions == null)
                return;

            AvailableMinecraftVersions.Clear();

            switch (ModAPIType)
            {
                case ModAPIType.NeoForge:
                    AvailableMinecraftVersions = new ObservableCollection<string>(App.GetSupportedNeoForgeMinecraftVersions());
                    break;
                case ModAPIType.Forge:
                    AvailableMinecraftVersions = new ObservableCollection<string>(App.GetSupportedForgeMinecraftVersions());
                    break;
            }
        }
        
        /// <summary>
        /// Analyzes the current project to determine its compatibility with the selected settings
        /// </summary>
        private async void AnalyzeProject()
        {
            if(ModAPIType == ModAPIType.None
                || string.IsNullOrEmpty(ProjectDirectoryPath) 
                || string.IsNullOrEmpty(ProjectMinecraftVersion))
                return;

            // Retrieve the workspace manager for the specified project settings
            _workspaceManager = WorkspaceManagerHelper.GetWorkspaceManager(ProjectDirectoryPath, ProjectMinecraftVersion, ModAPIType);
            if (_workspaceManager == null)
            {
                IsProjectCompatible = false;
                UpdateImportButton(UITextTranslator.GetTranslation("project_importation.button.import.incompatible"), FontAwesomeIcon.Times);
                return;
            }

            // Analyze the project to check its compatibility
            UpdateImportButton(UITextTranslator.GetTranslation("project_importation.button.import.analyzing"), FontAwesomeIcon.Hourglass);
            IsProjectCompatible = await new TaskFactory().StartNew(() => _workspaceManager.CheckWorkspaceValidity(App.GetSupportedMinecraftVersions(ModAPIType)));

            // Update the import button based on the project compatibility
            if (IsProjectCompatible)
                UpdateImportButton(UITextTranslator.GetTranslation("project_importation.button.import.compatible"), FontAwesomeIcon.Check);
            else
                UpdateImportButton(UITextTranslator.GetTranslation("project_importation.button.import.incompatible"), FontAwesomeIcon.Times);
        }

        /// <summary>
        /// Updates the text and icon displayed on the import button.
        /// </summary>
        /// <param name="text">The text to display on the import button</param>
        /// <param name="icon">The icon to display on the import button</param>
        private void UpdateImportButton(string text, FontAwesomeIcon icon)
        {
            ImportButtonText = text;
            ImportButtonIcon = icon;
        }
        #endregion

        #region Commands functions
        private void BrowseProjectDirectory()
        {
            // Allow user to select workspace output directory
            using (var fbd = new System.Windows.Forms.FolderBrowserDialog())
            {
                // Display FolderDialog
                System.Windows.Forms.DialogResult result = fbd.ShowDialog();

                // Check the user selection
                if (!string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    ProjectDirectoryPath = fbd.SelectedPath;
                }
            }
        }

        private void ImportProject()
        {
            // Generate project data files
            WorkspaceManagerHelper.WriteWorkspaceData(_workspaceManager);

            // Add the workspace to the recents workspaces
            var recentWorkspace = new RecentWorkspace()
            {
                WorkspacePath = _workspaceManager.WorkspaceProperties.WorkspacePath,
                LastUpdated = DateTime.Now,
                WorkspaceModAPI = _workspaceManager.WorkspaceProperties.ModAPI,
                WorkspaceMcVersion = _workspaceManager.WorkspaceProperties.MCVersion
            };
            RecentsWorkspacesProvider.AddRecentWorkspace(recentWorkspace);

            // Close the importation window
            DialogResult = true;
            CloseWindowAction?.Invoke();
        }
        #endregion
    }
}
