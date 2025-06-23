using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.IO.Compression;
using System.IO.Pipes;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
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
using FMH.Core.Model;
using FMH.Core.Provider;
using FMH.Core.Utils;
using FMH.Core.Utils.UI;
using FMH.Core.ViewModel;
using FMH.Workspace.Data;
using FMH.Workspace.WorkspaceManager;
using McVersionsLib.Forge;
using Microsoft.VisualBasic.FileIO;
using Path = System.IO.Path;

namespace FMH.Core.View.AssistantCreator
{
    public partial class AssistantCreatorGenerationProcessPageView : UserControl, IComponentDisplayed, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private AssistantCreatorViewModel parentViewModel;
        private IWorkspaceManager workspaceManager;

        #region Properties
        private string _statusLabelText { get; set; }
        /// <summary>
        /// Status label text
        /// </summary>
        public string StatusLabelText
        {
            get
            {
                return _statusLabelText;
            }
            set
            {
                _statusLabelText = value;
                OnPropertyChanged();
            }
        }


        private Visibility _progressBarVisibility { get; set; }
        /// <summary>
        /// Progress bar visibility
        /// </summary>
        public Visibility ProgressBarVisibility
        {
            get
            {
                return _progressBarVisibility;
            }
            set
            {
                _progressBarVisibility = value;
                OnPropertyChanged();
            }
        }

        private double _progressBarValue { get; set; }
        /// <summary>
        /// Progress bar value
        /// </summary>
        public double ProgressBarValue
        {
            get
            {
                return _progressBarValue;
            }
            set
            {
                _progressBarValue = value;
                OnPropertyChanged();
            }
        }
        #endregion

        public AssistantCreatorGenerationProcessPageView()
        {
            InitializeComponent();

            // Initialize properties
            StatusLabelText = UITextTranslator.GetTranslation("assistant_creator.generation.label.status.initializing");
        }

        private async void DoWorkspaceGeneration()
        {
            try
            {
                // Create WorkspaceManager
                await ConfigureWorkspaceManager();

                // Download the MDK
                await DownloadWorkspaceMDK();

                // Generate folders
                await GenerateWorkspaceFolders();

                // Generate files
                await GenerateWorskpaceFiles();

                // Displayed next page
                await Dispatcher.InvokeAsync(() => parentViewModel.Next());

            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Concat(UITextTranslator.GetTranslation("assistant_creator.generation.error.message"), "\n", ex.Message), UITextTranslator.GetTranslation("assistant_creator.generation.error.title"), MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #region Download and uncompress MDK
        private async Task DownloadWorkspaceMDK()
        {
            var progressObject = new Progress<double>(value => ProgressBarValue = value);

            // Download MDK
            StatusLabelText = UITextTranslator.GetTranslation("assistant_creator.generation.label.status.downloading_mdk");
            ProgressBarValue = 0;
            ProgressBarVisibility = Visibility.Visible;
            await workspaceManager.DownloadMDK(progressObject);

            // Uncompress MDK
            StatusLabelText = UITextTranslator.GetTranslation("assistant_creator.generation.label.status.uncompressing_mdk");
            ProgressBarValue = 0;
            await workspaceManager.ExtractMDK(progressObject);
            
            ProgressBarVisibility = Visibility.Hidden;
        }
        #endregion

        #region Configure workspace
        private async Task ConfigureWorkspaceManager()
        {
            await Task.Run(() => 
            { 
                workspaceManager = WorkspaceManagerHelper.GetWorkspaceManager(parentViewModel.NewWorkspaceData.WorkspaceFolderPath, parentViewModel.NewWorkspaceData.ModAPIVersion.MinecraftVersion, parentViewModel.NewWorkspaceData.ModAPI);
                if (workspaceManager == null)
                    throw new Exception("Unable to create workspace manager for Minecraft Forge " + parentViewModel.NewWorkspaceData.ModAPIVersion.MinecraftVersion);

                workspaceManager.WorkspaceProperties = new WorkspaceProperties()
                {
                    WorkspacePath = parentViewModel.NewWorkspaceData.WorkspaceFolderPath,
                    MCVersion = parentViewModel.NewWorkspaceData.ModAPIVersion.MinecraftVersion,
                    ModId = parentViewModel.NewWorkspaceData.ModId,
                    ModAPI = parentViewModel.NewWorkspaceData.ModAPI,
                    APIVersion = parentViewModel.NewWorkspaceData.ModAPIVersion.APIVersion
                };

                workspaceManager.ModProperties = new ModProperties()
                {
                    ModID = parentViewModel.NewWorkspaceData.ModId,
                    ModName = parentViewModel.NewWorkspaceData.ModName,
                    ModVersion = parentViewModel.NewWorkspaceData.ModVersion,
                    ModDescription = parentViewModel.NewWorkspaceData.ModDescription,
                    ModAuthors = parentViewModel.NewWorkspaceData.ModAuthors,
                    ModWebsite = parentViewModel.NewWorkspaceData.ModWebsite,
                    ModLicense = parentViewModel.NewWorkspaceData.ModLicense,
                    ModAPIVersion = parentViewModel.NewWorkspaceData.ModAPIVersion.APIVersion,
                    ModCredits = parentViewModel.NewWorkspaceData.ModCredits,
                    ModGroup = parentViewModel.NewWorkspaceData.ModGroup,
                    ModIssueTracker = parentViewModel.NewWorkspaceData.ModIssueTracker,
                    ModLogo = string.IsNullOrEmpty(parentViewModel.NewWorkspaceData.ModLogoSourcePath) ? "logo.png" : string.Empty,
                    ModMinecraftVersion = parentViewModel.NewWorkspaceData.ModAPIVersion.MinecraftVersion,
                    ModUpdateJSONURL = parentViewModel.NewWorkspaceData.UpdateJsonFileLink
                };

                workspaceManager.AssetsProperties = new AssetsProperties(parentViewModel.NewWorkspaceData.WorkspaceFolderPath);
                workspaceManager.AssetsProperties.SetModId(parentViewModel.NewWorkspaceData.ModId);

                workspaceManager.SourceCodeProperties = new SourceCodeProperties(parentViewModel.NewWorkspaceData.WorkspaceFolderPath);
            });
        }

        private async Task GenerateWorkspaceFolders()
        {
            await Task.Run(() =>
            {
                // Generate assets folders
                StatusLabelText = UITextTranslator.GetTranslation("assistant_creator.generation.label.status.assets_folders");
                workspaceManager.AssetsProperties.GenerateAssetsFolders();
                Thread.Sleep(1000);

                // Generate source code folders
                StatusLabelText = UITextTranslator.GetTranslation("assistant_creator.generation.label.status.source_code_folders");
                workspaceManager.SourceCodeProperties.GenerateSourceCodeFolders(parentViewModel.NewWorkspaceData.ModGroup);
                Thread.Sleep(1000);
            });
        }

        private async Task GenerateWorskpaceFiles()
        {
            await Task.Run(() =>
            {
                // Configuring build.gradle
                StatusLabelText = UITextTranslator.GetTranslation("assistant_creator.generation.label.status.build_gradle");
                workspaceManager.WriteBuildGradle();
                Thread.Sleep(1000);

                // Configuring gradle.properties
                StatusLabelText = UITextTranslator.GetTranslation("assistant_creator.generation.label.status.gradle_properties");
                workspaceManager.WriteGradleProperties();
                Thread.Sleep(1000);

                // Configuring mod.toml
                StatusLabelText = UITextTranslator.GetTranslation("assistant_creator.generation.label.status.mod_toml");
                workspaceManager.WriteModToml();
                Thread.Sleep(1000);

                // Copying mod logo
                if (!string.IsNullOrEmpty(parentViewModel.NewWorkspaceData.ModLogoSourcePath)
                    && File.Exists(parentViewModel.NewWorkspaceData.ModLogoSourcePath)
                    && !File.Exists(parentViewModel.NewWorkspaceData.WorkspaceFolderPath + @"\src\main\resources\logo.png"))
                {
                    StatusLabelText = UITextTranslator.GetTranslation("assistant_creator.generation.label.status.mod_logo");
                    File.Copy(parentViewModel.NewWorkspaceData.ModLogoSourcePath, parentViewModel.NewWorkspaceData.WorkspaceFolderPath + @"\src\main\resources\logo.png");
                    Thread.Sleep(1000);
                }

                // Generate FMH project file
                StatusLabelText = UITextTranslator.GetTranslation("assistant_creator.generation.label.status.project_file");
                WorkspaceManagerHelper.WriteProjectFile(workspaceManager.WorkspaceProperties);
                Thread.Sleep(1000);

                // Write workspace data
                StatusLabelText = UITextTranslator.GetTranslation("assistant_creator.generation.label.status.workspace_data");
                WorkspaceManagerHelper.WriteWorkspaceData(workspaceManager);
                Thread.Sleep(1000);

                // Add workspace to recents workspaces
                var recentWorkspaceData = new RecentWorkspace()
                {
                    WorkspacePath = workspaceManager.WorkspaceProperties.WorkspacePath,
                    LastUpdated = DateTime.Now,
                    WorkspaceModAPI = workspaceManager.WorkspaceProperties.ModAPI,
                    WorkspaceMcVersion = workspaceManager.WorkspaceProperties.MCVersion,
                    WorkspaceName = Path.GetFileName(workspaceManager.WorkspaceProperties.WorkspacePath)
                };
                RecentsWorkspacesProvider.AddRecentWorkspace(recentWorkspaceData);
            });
        }
        #endregion

        #region Interfaces implementations
        /// <inheritdoc/>
        public void OnComponentDisplayed(params object[] args)
        {
            // Retrieve parent DataContext from arguments and start the workspace generation process
            if (args.Any() && args[0] is AssistantCreatorViewModel viewModelDataContext)
            {
                parentViewModel = viewModelDataContext;
                Task.Run(() => DoWorkspaceGeneration()).Wait();
            }
        }

        public void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
