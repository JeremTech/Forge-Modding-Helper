using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using FMH.Core.Files.Software;
using FMH.Core.Objects;
using FMH.Core.UI.Common;
using FMH.Core.UI.Dialogs;
using FMH.Core.Utils.UI;
using FMH.Workspace.Data;
using FMH.Workspace.WorkspaceManager;
using FontAwesome.WPF;
using Microsoft.VisualBasic.FileIO;

using Application = System.Windows.Application;
using Button = System.Windows.Controls.Button;
using DataFormats = System.Windows.DataFormats;
using DragEventArgs = System.Windows.DragEventArgs;
using ListView = System.Windows.Controls.ListView;
using MessageBox = System.Windows.MessageBox;
using TextBox = System.Windows.Controls.TextBox;

namespace FMH.Core.UI.Forge
{
    /// <summary>
    /// Logique d'interaction pour ForgeProjectExplorer.xaml
    /// </summary>
    public partial class ForgeProjectExplorer : Window
    {
        /// <summary>
        /// Allow to know if window is closing or not
        /// </summary>
        private bool isClosing = false;

        // Cancellation token sources
        private CancellationTokenSource blockstatesTokenSource;
        private CancellationTokenSource modelsTokenSource;
        private CancellationTokenSource texturesTokenSource;

        // Worskpace Manager
        private IWorkspaceManager _workspaceManager;

        // Current section opened
        private string currentSectionOpenedTag;

        /// <summary>
        /// Constructor
        /// </summary>
        public ForgeProjectExplorer(string projectPath)
        {
            // Initialize UI
            InitializeComponent();

            // Load translations
            UITextTranslator.LoadTranslationFile(OptionsFile.GetCurrentLanguage());
            UITextTranslator.UpdateComponentsTranslations(this.MainGrid);
            this.Title = UITextTranslator.getTranslation("project_explorer.title");
            this.ModSettingsStatusLabel.Text = UITextTranslator.getTranslation("project_explorer.mod_settings.saved_modifications");
            this.ModSettingsStatusLabel.Foreground = (Brush)App.Current.FindResource("FontColorPrimary");

            // Initialize data
            currentSectionOpenedTag = "Home";

            // Initialize workspace manager
            _workspaceManager = WorkspaceManagerHelper.GetWorkspaceManager(projectPath);

            this.Loaded += ProjectExplorer_Loaded;
        }

        #region Loading project
        private void ProjectExplorer_Loaded(object sender, RoutedEventArgs e)
        {
            ReloadProject();
            RefreshInterfaceModInfos();
        }

        /// <summary>
        /// Reload current project data
        /// </summary>
        private async void ReloadProject()
        {
            // Showing loading dialog
            var loadingDialog = new LoadingDialog("Chargement des données...");
            loadingDialog.Owner = this;
            loadingDialog.Topmost = true;
            loadingDialog.Show();

            // Read data
            Task.WaitAll(Task.Run(() =>
            {
                // Workspace general data
                _workspaceManager.ReadBuildGradle();
                _workspaceManager.ReadGradleProperties();
                _workspaceManager.ReadModToml();
                _workspaceManager.ModVersionsHistory.ReadVersionsHistory();

                // Assets data
                _workspaceManager.AssetsProperties = new AssetsProperties(_workspaceManager.WorkspaceProperties.WorkspacePath);
                _workspaceManager.AssetsProperties.SetModId(_workspaceManager.ModProperties.ModID);
                _workspaceManager.AssetsProperties.RetrieveAllTexturesFiles();
                _workspaceManager.AssetsProperties.RetrieveAllModelsFiles();
                _workspaceManager.AssetsProperties.RetrieveAllBlockstatesFiles();

                // Source code data
                _workspaceManager.SourceCodeProperties = new SourceCodeProperties(_workspaceManager.WorkspaceProperties.WorkspacePath);
                _workspaceManager.SourceCodeProperties.RetrieveAllJavaFiles();
                _workspaceManager.SourceCodeProperties.CountCodeLines(true);
            }));

            // Showing main grid and closing loading dialog
            MainGrid.Visibility = Visibility.Visible;
            loadingDialog.Close();
        }
        #endregion

        #region Home Section
        /// <summary>
        /// Function called when open explorer button on home section is clicked
        /// </summary>
        private void HomeOpenExplorerButtonClick(object sender, RoutedEventArgs e)
        {
            Process.Start(_workspaceManager.WorkspaceProperties.WorkspacePath);
        }

        /// <summary>
        /// Function called when "Delete" is clicked in context menu of the versions history listview
        /// </summary>
        private async void HomeContextMenuVersionsHistory_Delete_Click(object sender, RoutedEventArgs e)
        {
            if (HomeModVersionsHistoryListView.SelectedItems.Count != 1)
                return;

            var selectedVersion = (ModVersionHistoryEntry)HomeModVersionsHistoryListView.SelectedItem;
            if (MessageBox.Show(string.Format(UITextTranslator.getTranslation("project_explorer.home.confirm_delete"), selectedVersion.ModVersion), "Forge Modding Helper", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                File.Delete(Path.Combine(_workspaceManager.WorkspaceProperties.WorkspacePath, "fmh", "versions", selectedVersion.FileName));

                _workspaceManager.ModVersionsHistory.RemoveVersionFromHistory(selectedVersion.ModVersion);
                await Task.Run(() => { _workspaceManager.ModVersionsHistory.WriteData(); });

                RefreshInterfaceModInfos();
            }
        }

        /// <summary>
        /// Function called when "Open location" is clicked in context menu of the versions history listview
        /// </summary>
        private void HomeContextMenuVersionsHistory_OpenLocation_Click(object sender, RoutedEventArgs e)
        {
            var selectedVersion = (ModVersionHistoryEntry)HomeModVersionsHistoryListView.SelectedItem;
            var filePath = Path.Combine(_workspaceManager.WorkspaceProperties.WorkspacePath, "fmh", "versions", selectedVersion.FileName);

            if (File.Exists(filePath))
            {
                Process.Start("explorer.exe", "/select, " + filePath);
            }
            else
            {
                MessageBox.Show(string.Format(UITextTranslator.getTranslation("project_explorer.home.version_not_found"), filePath), "Forge Modding Helper", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        #endregion

        #region ModSettings section
        /// <summary>
        /// Function called when text is changed in mod settings textboxes
        /// </summary>
        private void ModSettingsTextboxTextChanged(object sender, TextChangedEventArgs e)
        {
            var textBoxSender = sender as TextBox;

            if (textBoxSender != null && !textBoxSender.IsFocused)
                return;

            this.ModSettingsStatusLabel.Text = UITextTranslator.getTranslation("project_explorer.mod_settings.unsaved_modifications");
            this.ModSettingsStatusLabel.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF0000"));
        }

        /// <summary>
        /// Function called when mod settings mod logo browsing button is clicked
        /// </summary>
        private void ModSettingsModLogoButtonClick(object sender, RoutedEventArgs e)
        {
            // Create and configure FileDialog
            Microsoft.Win32.OpenFileDialog fileDialog = new Microsoft.Win32.OpenFileDialog();
            fileDialog.RestoreDirectory = true;
            fileDialog.Title = UITextTranslator.getTranslation("project_explorer.mod_settings.choose_logo_file");
            fileDialog.DefaultExt = "png";
            fileDialog.Filter = UITextTranslator.getTranslation("project_explorer.mod_settings.filter_logo_file") + " (*.png)|*.png";
            fileDialog.CheckFileExists = true;
            fileDialog.CheckPathExists = true;
            fileDialog.Multiselect = false;
            // Display the FileDialog
            fileDialog.ShowDialog();

            // Check the user selection
            if (!String.IsNullOrWhiteSpace(fileDialog.FileName))
            {
                // Deleting existing logo
                if (File.Exists(Path.Combine(_workspaceManager.WorkspaceProperties.WorkspacePath, @"src\main\resources\logo.png"))) FileSystem.DeleteFile(Path.Combine(_workspaceManager.WorkspaceProperties.WorkspacePath, @"src\main\resources\logo.png"), UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin, UICancelOption.DoNothing);
                // Copying new logo
                File.Copy(fileDialog.FileName, _workspaceManager.WorkspaceProperties.WorkspacePath + @"\src\main\resources\logo.png");
                // Update mod data 
                _workspaceManager.ModProperties.ModLogo = "logo.png";
            }

            // Refresh UI 
            RefreshInterfaceModInfos();
        }

        /// <summary>
        /// Function called when mod settings mod logo deleting button is clicked
        /// </summary>
        private void ModSettingsModLogoDeleteButtonClick(object sender, RoutedEventArgs e)
        {
            // Create and display confirmation message
            MessageBoxResult result = MessageBox.Show(UITextTranslator.getTranslation("project_explorer.mod_settings.alerte.delete_logo_message"), "Forge Modding Helper", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                _workspaceManager.ModProperties.ModLogo = "";
                FileSystem.DeleteFile(Path.Combine(_workspaceManager.WorkspaceProperties.WorkspacePath, @"src\main\resources\logo.png"), UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin, UICancelOption.DoNothing);
                RefreshInterfaceModInfos();
            }
        }

        /// <summary>
        /// Function called when save button mod settings is clicked
        /// </summary>
        private async void ModSettingsSaveButtonClick(object sender, RoutedEventArgs e)
        {
            // Save changes in ModData object
            _workspaceManager.ModProperties.ModName = this.ModSettingsModNameTextbox.Text;
            _workspaceManager.ModProperties.ModDescription = this.ModSettingsModDescriptionTextbox.Text;
            _workspaceManager.ModProperties.ModAuthors = this.ModSettingsModAuthorsTextbox.Text;
            _workspaceManager.ModProperties.ModLicense = this.ModSettingsModLicenseTextbox.Text;
            _workspaceManager.ModProperties.ModCredits = this.ModSettingsModCreditsTextbox.Text;
            _workspaceManager.ModProperties.ModWebsite = this.ModSettingsModWebsiteTextbox.Text;
            _workspaceManager.ModProperties.ModIssueTracker = this.ModSettingsModBugTrackerURLTextbox.Text;
            _workspaceManager.ModProperties.ModUpdateJSONURL = this.ModSettingsModUpdateJsonURLTextbox.Text;
            _workspaceManager.ModProperties.ModVersion = this.ModSettingsModVersionTextbox.Text;
            _workspaceManager.ModProperties.ModMinecraftVersion = this.ModSettingsMinecraftVersionTextbox.Text;
            _workspaceManager.ModProperties.ModAPIVersion = this.ModSettingsForgeVersionTextbox.Text;
            _workspaceManager.ModProperties.ModMappingsVersion = this.ModSettingsMappingsVersionTextbox.Text;
            _workspaceManager.ModProperties.ModID = this.ModSettingsModidTextbox.Text;
            _workspaceManager.ModProperties.ModGroup = this.ModSettingsModgroupTextbox.Text;

            // Update workspace's files
            await Task.Run(() =>
            {
                _workspaceManager.WriteBuildGradle();
                _workspaceManager.WriteModToml();
                _workspaceManager.WriteGradleProperties();
                WorkspaceManagerHelper.WriteWorkspaceData(_workspaceManager);
            });

            // Update UI
            this.ModSettingsStatusLabel.Text = UITextTranslator.getTranslation("project_explorer.mod_settings.saved_modifications");
            this.ModSettingsStatusLabel.Foreground = (Brush)App.Current.FindResource("FontColorPrimary");
            RefreshInterfaceModInfos();
        }
        #endregion

        #region Blockstates section
        /// <summary>
        /// Refresh blockstates listview
        /// </summary>
        /// <param name="filterText">Text filtering - Leave empty for no filter</param>
        private async Task RefreshBlockstatesListView(string filterText)
        {
            // Manage cancellation token
            if (blockstatesTokenSource != null) blockstatesTokenSource.Cancel();
            blockstatesTokenSource = new CancellationTokenSource();
            var cancellationToken = blockstatesTokenSource.Token;

            BlockstatesLoadingStackPanel.Visibility = Visibility.Visible;
            BlockstatesLoadingStatusTextblock.Text = UITextTranslator.getTranslation("project_explorer.blockstates.loading_files");

            // Run it async
            try
            {
                await Task.Run(() =>
                {
                    // Clear content
                    Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background, new Action(() => BlockstatesListView.Items.Clear()));
                    Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background, new Action(() => BlockstatesFileCountTextblock.Text = "0"));

                    // Foreach blockstates file
                    foreach (string fileIn in _workspaceManager.AssetsProperties.BlockstatesFiles)
                    {
                        // Check if cancellation have been requested
                        if (cancellationToken.IsCancellationRequested) return;

                        if (!string.IsNullOrWhiteSpace(filterText))
                        {
                            if (Path.GetFileName(fileIn).Contains(filterText))
                            {
                                Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background, new Action(() => BlockstatesListView.Items.Add(new FileEntry(fileIn, FontAwesomeIcon.FileCodeOutline))));
                                Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background, new Action(() => BlockstatesFileCountTextblock.Text = int.Parse(BlockstatesFileCountTextblock.Text) + 1 + ""));
                            }
                        }
                        else
                        {
                            Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background, new Action(() => BlockstatesListView.Items.Add(new FileEntry(fileIn, FontAwesomeIcon.FileCodeOutline))));
                            Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background, new Action(() => BlockstatesFileCountTextblock.Text = int.Parse(BlockstatesFileCountTextblock.Text) + 1 + ""));
                        }
                    }


                }, cancellationToken);
            }
            catch (OperationCanceledException)
            {

            }

            BlockstatesLoadingStackPanel.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Function called when files are dropped into blockstates listview
        /// </summary>
        private async void BlockstatesListViewDrop(object sender, DragEventArgs e)
        {
            // Check if this is files
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // Get all files
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                // Show importation dialog
                new ImportFilesDialog(files, "blockstates", _workspaceManager.WorkspaceProperties.WorkspacePath, _workspaceManager.ModProperties.ModID).ShowDialog();

                await Task.Run(() => { _workspaceManager.AssetsProperties.RetrieveAllBlockstatesFiles(); });
                await RefreshBlockstatesListView(BlockstatesSearchTextbox.Text);
            }
        }
        #endregion

        #region Models section
        /// <summary>
        /// Refresh models listview
        /// </summary>
        /// <param name="filterText">Text filtering - Leave empty for no filter</param>
        private async Task RefreshModelsListView(string filterText)
        {
            if (ModelsLoadingStackPanel != null)
            {
                // Manage cancellation token
                if (modelsTokenSource != null) modelsTokenSource.Cancel();
                modelsTokenSource = new CancellationTokenSource();
                var cancellationToken = modelsTokenSource.Token;

                ModelsLoadingStackPanel.Visibility = Visibility.Visible;

                // Run it async
                try
                {
                    await Task.Run(() =>
                    {
                        // Clear content
                        Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background, new Action(() => ModelsListView.Items.Clear()));
                        Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background, new Action(() => ModelsFileCountTextblock.Text = "0"));

                        // Foreach models file
                        foreach (string fileIn in _workspaceManager.AssetsProperties.ModelsFiles)
                        {
                            // Check if cancellation have been requested
                            if (cancellationToken.IsCancellationRequested) return;

                            if (!string.IsNullOrWhiteSpace(filterText))
                            {
                                if (Path.GetFileName(fileIn).Contains(filterText))
                                {
                                    if (fileIn.Contains("item"))
                                    {
                                        Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background, new Action(() => ModelsListView.Items.Add(new FileEntry(fileIn, FontAwesomeIcon.Diamond))));
                                    }
                                    else if (fileIn.Contains("block"))
                                    {
                                        Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background, new Action(() => ModelsListView.Items.Add(new FileEntry(fileIn, FontAwesomeIcon.Cube))));
                                    }
                                    else
                                    {
                                        Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background, new Action(() => ModelsListView.Items.Add(new FileEntry(fileIn, FontAwesomeIcon.FileCodeOutline))));
                                    }
                                    Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background, new Action(() => ModelsFileCountTextblock.Text = int.Parse(ModelsFileCountTextblock.Text) + 1 + ""));
                                }
                            }
                            else
                            {
                                if (fileIn.Contains("item"))
                                {
                                    Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background, new Action(() => ModelsListView.Items.Add(new FileEntry(fileIn, FontAwesomeIcon.Diamond))));
                                }
                                else if (fileIn.Contains("block"))
                                {
                                    Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background, new Action(() => ModelsListView.Items.Add(new FileEntry(fileIn, FontAwesomeIcon.Cube))));
                                }
                                else
                                {
                                    Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background, new Action(() => ModelsListView.Items.Add(new FileEntry(fileIn, FontAwesomeIcon.FileCodeOutline))));
                                }
                                Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background, new Action(() => ModelsFileCountTextblock.Text = int.Parse(ModelsFileCountTextblock.Text) + 1 + ""));
                            }
                        }

                        // Update count on home section
                        Dispatcher.BeginInvoke(() => { ModelsInfoDisplay.InfoContent = _workspaceManager.AssetsProperties.ModelsFiles.Count.ToString(); });

                    }, cancellationToken);
                }
                catch (OperationCanceledException)
                {

                }

                ModelsLoadingStackPanel.Visibility = Visibility.Hidden;
            }
        }

        /// <summary>
        /// Function called when files are dropped into blockstates listview
        /// </summary>
        private async void ModelsListViewDrop(object sender, DragEventArgs e)
        {
            // Check if this is files
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // Get all files
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                // Show importation dialog
                new ImportFilesDialog(files, "models", _workspaceManager.WorkspaceProperties.WorkspacePath, _workspaceManager.ModProperties.ModID).ShowDialog();

                await Task.Run(() => { _workspaceManager.AssetsProperties.RetrieveAllModelsFiles(); });
                await RefreshModelsListView(ModelsSearchTextbox.Text);
            }
        }
        #endregion

        #region Textures section
        /// <summary>
        /// Refresh models listview
        /// </summary>
        /// <param name="filterText">Text filtering - Leave empty for no filter</param>
        private async Task RefreshTexturesListView(string filterText)
        {
            if (TexturesLoadingStackPanel != null)
            {
                // Manage cancellation token
                if (texturesTokenSource != null) texturesTokenSource.Cancel();
                texturesTokenSource = new CancellationTokenSource();
                var cancellationToken = texturesTokenSource.Token;

                TexturesLoadingStackPanel.Visibility = Visibility.Visible;

                // Run it async
                try
                {
                    await Task.Run(() =>
                    {
                        // Clear content
                        Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background, new Action(() => TexturesListView.Items.Clear()));
                        Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background, new Action(() => TexturesFileCountTextblock.Text = "0"));

                        // Foreach blockstates file
                        foreach (string fileIn in _workspaceManager.AssetsProperties.TexturesFiles)
                        {
                            // Check if cancellation have been requested
                            if (cancellationToken.IsCancellationRequested) return;

                            if (!string.IsNullOrWhiteSpace(filterText))
                            {
                                if (Path.GetFileName(fileIn).Contains(filterText))
                                {
                                    Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background, new Action(() => TexturesListView.Items.Add(new FileEntry(fileIn, FontAwesomeIcon.Image))));
                                    Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background, new Action(() => TexturesFileCountTextblock.Text = int.Parse(TexturesFileCountTextblock.Text) + 1 + ""));
                                }
                            }
                            else
                            {
                                Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background, new Action(() => TexturesListView.Items.Add(new FileEntry(fileIn, FontAwesomeIcon.Image))));
                                Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background, new Action(() => TexturesFileCountTextblock.Text = int.Parse(TexturesFileCountTextblock.Text) + 1 + ""));
                            }
                        }

                        // Update count on home section
                        Dispatcher.BeginInvoke(() => { TexturesInfoDisplay.InfoContent = _workspaceManager.AssetsProperties.TexturesFiles.Count.ToString(); });

                    }, cancellationToken);
                }
                catch (OperationCanceledException)
                {

                }

                TexturesLoadingStackPanel.Visibility = Visibility.Hidden;
            }
        }

        /// <summary>
        /// Function called when files are dropped into blockstates listview
        /// </summary>
        private async void TexturesListViewDrop(object sender, DragEventArgs e)
        {
            // Check if this is files
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // Get all files
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                // Show importation dialog
                new ImportFilesDialog(files, "textures", _workspaceManager.WorkspaceProperties.WorkspacePath, _workspaceManager.ModProperties.ModID).ShowDialog();

                await Task.Run(() => { _workspaceManager.AssetsProperties.RetrieveAllTexturesFiles(); });
                await RefreshTexturesListView(TexturesSearchTextbox.Text);
            }
        }
        #endregion

        #region Translations section
        /// <summary>
        /// Refresh models listview
        /// </summary>
        private async Task RefreshTranslationsList()
        {
            TranslationsLoadingStackPanel.Visibility = Visibility.Visible;

            // Clearing listbox
            TranslationsFilesListBox.Items.Clear();
            List<string> fileList = Directory.EnumerateFiles(Path.Combine(_workspaceManager.WorkspaceProperties.WorkspacePath, "src\\main\\resources\\assets", _workspaceManager.ModProperties.ModID, "lang")).ToList();

            // Run it async
            await Task.Run(() =>
            {
                fileList.ForEach(element => Application.Current.Dispatcher.Invoke(() => TranslationsFilesListBox.Items.Add(Path.GetFileName(element))));
                if (Application.Current.Dispatcher.Invoke(() => TranslationsFilesListBox.Items.Count > 0)) Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, new Action(() => TranslationsFilesListBox.SelectedIndex = 0));
            });

            TranslationsLoadingStackPanel.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Function called when TranslationFileListBox selection is updated
        /// </summary>
        private async void TranslationsFilesListBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TranslationsLoadingStackPanel.Visibility = Visibility.Visible;

            // Run it async
            await Task.Run(() =>
            {
                if (Application.Current.Dispatcher.Invoke(() => TranslationsFilesListBox.SelectedItem != null) && File.Exists(Path.Combine(_workspaceManager.WorkspaceProperties.WorkspacePath, "src\\main\\resources\\assets", _workspaceManager.ModProperties.ModID, "lang", Application.Current.Dispatcher.Invoke(() => TranslationsFilesListBox.SelectedItem.ToString()))))
                {
                    Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, new Action(() => TranslationsTextEditor.Text = File.ReadAllText(Path.Combine(_workspaceManager.WorkspaceProperties.WorkspacePath, "src\\main\\resources\\assets", _workspaceManager.ModProperties.ModID, "lang", TranslationsFilesListBox.SelectedItem.ToString()))));
                    Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, new Action(() => TranslationsDeleteButton.IsEnabled = true));
                }
                else
                {
                    Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, new Action(() => TranslationsTextEditor.Text = ""));
                    Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, new Action(() => TranslationsDeleteButton.IsEnabled = false));
                }
            });

            TranslationsLoadingStackPanel.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Save changes to file
        /// </summary>
        private void TranslationsTextEditorTextChanged(object sender, TextChangedEventArgs e)
        {
            if (TranslationsFilesListBox.SelectedItem != null)
            {
                File.WriteAllText(Path.Combine(_workspaceManager.WorkspaceProperties.WorkspacePath, "src\\main\\resources\\assets", _workspaceManager.ModProperties.ModID, "lang", TranslationsFilesListBox.SelectedItem.ToString()), TranslationsTextEditor.Text);
            }
        }

        /// <summary>
        /// Function called when translations file add button is clicked
        /// </summary>
        private async void TranslationsAddButtonClick(object sender, RoutedEventArgs e)
        {
            new AddTranslationFileDialog(Path.Combine(_workspaceManager.WorkspaceProperties.WorkspacePath, "src\\main\\resources\\assets", _workspaceManager.ModProperties.ModID, "lang")).ShowDialog();
            await RefreshTranslationsList();
        }

        /// <summary>
        /// Function called when translations file remove button is clicked
        /// </summary>
        private async void TranslationsDeleteButtonClick(object sender, RoutedEventArgs e)
        {
            string filePath = Path.Combine(_workspaceManager.WorkspaceProperties.WorkspacePath, "src\\main\\resources\\assets", _workspaceManager.ModProperties.ModID, "lang", TranslationsFilesListBox.SelectedItem.ToString());

            if (TranslationsFilesListBox.SelectedItem != null && File.Exists(filePath))
            {
                MessageBoxResult res = MessageBox.Show(UITextTranslator.getTranslation("project_explorer.translations.alert.delete"), "Forge Modding Helper", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (res == MessageBoxResult.Yes)
                {
                    File.Delete(filePath);
                    await RefreshTranslationsList();
                }
            }
        }
        #endregion

        #region Exportation section
        /// <summary>
        /// Event called when "Export" button is clicked
        /// </summary>
        private async void ExportationButtonClick(object sender, RoutedEventArgs e)
        {
            string destinationFilePath = Path.Combine(_workspaceManager.WorkspaceProperties.WorkspacePath, "fmh", "versions", _workspaceManager.ModProperties.ModID + "-" + _workspaceManager.ModProperties.ModVersion + ".jar");

            // Show warning if the version has been already builded
            if (File.Exists(destinationFilePath))
            {
                if (MessageBox.Show(UITextTranslator.getTranslation("project_explorer.export.error.file_already_exist"), "Forge Modding Helper", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                    return;
            }

            // Configure console and run build task
            this.ModExportationConsoleControl.FontSize = 10;
            this.ModExportationConsoleControl.ClearOutput();
            this.ModExportationConsoleControl.StartProcess("cmd.exe", "/k \"cd /d " + _workspaceManager.WorkspaceProperties.WorkspacePath + "\"");
            this.ModExportationConsoleControl.WriteInput("gradlew build --no-daemon & exit", Color.FromRgb(255, 240, 0), true);
            this.SideBarExportationProgressBar.Visibility = Visibility.Visible;

            // Wait for task's end
            await Task.Run(() =>
            {
                while (this.ModExportationConsoleControl.ProcessInterface.IsProcessRunning) { }

                // Manage versions history
                ManageVersionHistoryAfterExportation();
            });

            // Hide progressBar
            this.SideBarExportationProgressBar.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Manage version history after exportation
        /// </summary>
        private async void ManageVersionHistoryAfterExportation()
        {
            string ouputFilePath = Path.Combine(_workspaceManager.WorkspaceProperties.WorkspacePath, "build", "libs", string.Concat(_workspaceManager.ModProperties.ModID, "-", _workspaceManager.ModProperties.ModVersion, ".jar"));
            string destinationFilePath = Path.Combine(_workspaceManager.WorkspaceProperties.WorkspacePath, "fmh", "versions", string.Concat(_workspaceManager.ModProperties.ModID, "-", _workspaceManager.ModProperties.ModVersion, ".jar"));

            // If the output file is not found, show error and stop process here
            if (!File.Exists(ouputFilePath))
            {
                this.ModExportationConsoleControl.WriteOutput(string.Concat("\n", UITextTranslator.getTranslation("project_explorer.export.error.output_file_not_found")), Color.FromRgb(255, 0, 0));
                return;
            }

            // Refresh mod versions history data
            _workspaceManager.ModVersionsHistory.ReadVersionsHistory();

            // Deleting previous generated build for this mod version if necessary
            if (File.Exists(destinationFilePath))
            {
                this.ModExportationConsoleControl.WriteOutput(string.Concat("\n", UITextTranslator.getTranslation("project_explorer.export.info.deleting_previous_file")), Color.FromRgb(255, 240, 0));
                File.Delete(destinationFilePath);
                _workspaceManager.ModVersionsHistory.RemoveVersionFromHistory(_workspaceManager.ModProperties.ModVersion);
            }

            // Moving file to fmh/versions folder
            this.ModExportationConsoleControl.WriteOutput(string.Concat("\n", UITextTranslator.getTranslation("project_explorer.export.info.moving_generated_file")), Color.FromRgb(255, 240, 0));
            if (!Directory.Exists(Path.Combine(_workspaceManager.WorkspaceProperties.WorkspacePath, "fmh", "versions")))
                Directory.CreateDirectory(Path.Combine(_workspaceManager.WorkspaceProperties.WorkspacePath, "fmh", "versions"));
            File.Move(ouputFilePath, destinationFilePath);

            this.ModExportationConsoleControl.WriteOutput(string.Concat("\n", UITextTranslator.getTranslation("project_explorer.export.info.updating_versions_history")), Color.FromRgb(255, 240, 0));

            _workspaceManager.ModVersionsHistory.AddVersionToHistory(_workspaceManager.ModProperties.ModVersion, _workspaceManager.ModProperties.ModMinecraftVersion, DateTime.Now, _workspaceManager.ModProperties.ModID + "-" + _workspaceManager.ModProperties.ModVersion + ".jar");
            _workspaceManager.ModVersionsHistory.WriteData();

            // Refresh mod data
            Dispatcher.Invoke(() => RefreshInterfaceModInfos());

            this.ModExportationConsoleControl.WriteOutput(string.Concat("\n", UITextTranslator.getTranslation("project_explorer.export.info.exportation_success")), Color.FromRgb(0, 255, 0));
        }
        #endregion

        #region UI functions
        /// <summary>
        /// Refresh all UI components who contains mod infos
        /// </summary>
        private async void RefreshInterfaceModInfos()
        {
            // Mod logo (side bar, mod settings)
            if (File.Exists(Path.Combine(_workspaceManager.WorkspaceProperties.WorkspacePath, @"src\main\resources\logo.png")))
            {
                using (var stream = File.OpenRead(Path.Combine(_workspaceManager.WorkspaceProperties.WorkspacePath, @"src\main\resources\logo.png")))
                {
                    var image = new BitmapImage();
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.StreamSource = stream;
                    image.EndInit();
                    this.HomeModLogoImage.Source = image;
                    this.ModSettingsModLogoImage.Source = image;
                    this.HomeModLogoImageBorder.Visibility = Visibility.Visible;
                }
            }
            else
            {
                this.ModSettingsModLogoImage.Source = new BitmapImage(new Uri("/FMH.Resources;component/Pictures/icon.png", UriKind.Relative));
                this.HomeModLogoImageBorder.Visibility = Visibility.Collapsed;
            }

            // Home section
            this.HomeModNameTextblock.Text = _workspaceManager.ModProperties.ModName;
            this.HomeWorkspaceFolderTextblock.Text = _workspaceManager.WorkspaceProperties.WorkspacePath;
            this.TexturesInfoDisplay.InfoContent = _workspaceManager.AssetsProperties.TexturesFiles.Count.ToString();
            this.ModelsInfoDisplay.InfoContent = _workspaceManager.AssetsProperties.ModelsFiles.Count.ToString();
            this.JavaFilesInfoDisplay.InfoContent = _workspaceManager.SourceCodeProperties.JavaFiles.Count.ToString();
            this.LinesCountInfoDisplay.InfoContent = _workspaceManager.SourceCodeProperties.CodeLinesCount.ToString();
            this.ModVersionInfoDisplay.InfoContent = _workspaceManager.ModProperties.ModVersion;
            this.MinecraftVersionInfoDisplay.InfoContent = _workspaceManager.ModProperties.ModMinecraftVersion;
            this.ForgeVersionInfoDisplay.InfoContent = _workspaceManager.ModProperties.ModAPIVersion;
            this.MappingsVersionInfoDisplay.InfoContent = _workspaceManager.ModProperties.ModMappingsVersion;

            // Mod settings section
            this.ModSettingsModNameTextbox.Text = _workspaceManager.ModProperties.ModName;
            this.ModSettingsModDescriptionTextbox.Text = _workspaceManager.ModProperties.ModDescription;
            this.ModSettingsModAuthorsTextbox.Text = _workspaceManager.ModProperties.ModAuthors;
            this.ModSettingsModLicenseTextbox.Text = _workspaceManager.ModProperties.ModLicense;
            this.ModSettingsModCreditsTextbox.Text = _workspaceManager.ModProperties.ModCredits;
            this.ModSettingsModWebsiteTextbox.Text = _workspaceManager.ModProperties.ModWebsite;
            this.ModSettingsModBugTrackerURLTextbox.Text = _workspaceManager.ModProperties.ModIssueTracker;
            this.ModSettingsModUpdateJsonURLTextbox.Text = _workspaceManager.ModProperties.ModUpdateJSONURL;
            this.ModSettingsModVersionTextbox.Text = _workspaceManager.ModProperties.ModVersion;
            this.ModSettingsMinecraftVersionTextbox.Text = _workspaceManager.ModProperties.ModMinecraftVersion;
            this.ModSettingsForgeVersionTextbox.Text = _workspaceManager.ModProperties.ModAPIVersion;
            this.ModSettingsMappingsVersionTextbox.Text = _workspaceManager.ModProperties.ModMappingsVersion;
            this.ModSettingsModidTextbox.Text = _workspaceManager.ModProperties.ModID;
            this.ModSettingsModgroupTextbox.Text = _workspaceManager.ModProperties.ModGroup;

            // Mod exportation section
            this.ModNameExportationRecapTextBlock.Text = _workspaceManager.ModProperties.ModName;
            this.ModVersionExportationRecapTextBlock.Text = _workspaceManager.ModProperties.ModVersion;
            this.ModAuthorsExportationRecapTextBlock.Text = _workspaceManager.ModProperties.ModAuthors;
            this.ForgeVersionExportationRecapTextBlock.Text = _workspaceManager.ModProperties.ModAPIVersion;
            this.MinecraftVersionExportationRecapTextBlock.Text = _workspaceManager.ModProperties.ModMinecraftVersion;
            this.MappingsVersionExportationRecapTextBlock.Text = _workspaceManager.ModProperties.ModMappingsVersion;

            // Mod history
            await Task.Run(() => { _workspaceManager.ModVersionsHistory.ReadVersionsHistory(); });
            HomeModVersionsHistoryListView.ItemsSource = _workspaceManager.ModVersionsHistory.VersionHistory.OrderByDescending(v => v.VersionDateTime);
        }

        /// <summary>
        /// Function called when mouse left button is pressed on side bar buttons
        /// </summary>
        private void SideBarButtonMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Grid SelectedButton = sender as Grid;

            if (SelectedButton != null)
            {
                if (SelectedButton.Tag != null)
                {
                    // Set selected section tag 
                    currentSectionOpenedTag = SelectedButton.Tag.ToString();

                    // Reset all buttons border
                    SideBarHomeButtonBorder.Background = null;
                    SideBarModSettingsButtonBorder.Background = null;
                    SideBarBlockstatesButtonBorder.Background = null;
                    SideBarModelsButtonBorder.Background = null;
                    SideBarTexturesButtonBorder.Background = null;
                    SideBarTranslationsButtonBorder.Background = null;
                    SideBarExportationButtonBorder.Background = null;

                    // Hide all grid
                    HomeGrid.Visibility = Visibility.Hidden;
                    ModSettingsGrid.Visibility = Visibility.Hidden;
                    BlockstatesGrid.Visibility = Visibility.Hidden;
                    ModelsGrid.Visibility = Visibility.Hidden;
                    TexturesGrid.Visibility = Visibility.Hidden;
                    TranslationsGrid.Visibility = Visibility.Hidden;
                    ExportationGrid.Visibility = Visibility.Hidden;

                    // Set selected border
                    Border SelectedBorder = (Border)this.FindName("SideBar" + SelectedButton.Tag + "ButtonBorder");
                    if (SelectedBorder != null)
                    {
                        SelectedBorder.Background = (Brush)App.Current.FindResource("BorderColor");
                    }

                    // Display Grid
                    Grid SelectedGrid = (Grid)this.FindName(SelectedButton.Tag + "Grid");
                    if (SelectedGrid != null)
                    {
                        SelectedGrid.Visibility = Visibility.Visible;
                    }

                }
            }
        }

        /// <summary>
        /// Function called when mouse left button is pressed on Options button in the side bar
        /// </summary>
        private void SideBarOptionsButtonMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            new OptionWindow().ShowDialog();

            // Reload translations
            UITextTranslator.LoadTranslationFile(OptionsFile.GetCurrentLanguage());
            UITextTranslator.UpdateComponentsTranslations(this.MainGrid);
            this.Title = UITextTranslator.getTranslation("project_explorer.title");

            // Reload font color if needed of mod settings statut label
            if (!string.Equals(((SolidColorBrush)ModSettingsStatusLabel.Foreground).Color.ToString(), "#FFFF0000"))
                this.ModSettingsStatusLabel.Foreground = (Brush)App.Current.FindResource("FontColorPrimary");

            // Reload color for the selected border
            Border SelectedBorder = (Border)this.FindName("SideBar" + currentSectionOpenedTag + "ButtonBorder");
            if (SelectedBorder != null)
            {
                SelectedBorder.Background = (Brush)App.Current.FindResource("BorderColor");
            }
        }

        /// <summary>
        /// Function called whan text in blockstates / models / textures search textboxes is changed
        /// </summary>
        private async void SearchTextboxTextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox senderTextbox = sender as TextBox;

            // Check if sender is not null
            if (senderTextbox != null)
            {
                // If this is the blockstates search textbox
                if (senderTextbox.Name.Contains("Blockstates"))
                {
                    await RefreshBlockstatesListView(senderTextbox.Text.Trim());
                }
                // If this is the models search textbox
                else if (senderTextbox.Name.Contains("Models"))
                {
                    await RefreshModelsListView(senderTextbox.Text.Trim());
                }
                // If this is the textures search textbox
                else if (senderTextbox.Name.Contains("Textures"))
                {
                    await RefreshTexturesListView(senderTextbox.Text.Trim());
                }
            }
        }

        /// <summary>
        /// Function called by blockstates / models / textures delete buttons
        /// </summary>
        private async void DeleteButtonClick(object sender, RoutedEventArgs e)
        {
            Button senderButton = sender as Button;

            // Check if sender is not null
            if (senderButton != null)
            {
                // If this is the blockstates delete button
                if (senderButton.Name.Contains("Blockstates"))
                {
                    // Check selected items count
                    if (BlockstatesListView.SelectedItems.Count > 0)
                    {
                        // Display confirmation message
                        MessageBoxResult result = MessageBox.Show(UITextTranslator.getTranslation("project_explorer.alert.delete").Replace("%N", BlockstatesListView.SelectedItems.Count.ToString()), "Forge Modding Helper", MessageBoxButton.YesNo, MessageBoxImage.Question);

                        // If user say Yes
                        if (result == MessageBoxResult.Yes)
                        {
                            // Delete all selected items
                            foreach (FileEntry element in BlockstatesListView.SelectedItems)
                            {
                                // Move the file to the recycle bin
                                FileSystem.DeleteFile(element.FilePath, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin, UICancelOption.DoNothing);
                            }

                            // Refresh blockstates list
                            await Task.Run(() => { _workspaceManager.AssetsProperties.RetrieveAllBlockstatesFiles(); });
                            await RefreshBlockstatesListView(BlockstatesSearchTextbox.Text);
                        }
                    }
                }
                // If this is the models delete button
                else if (senderButton.Name.Contains("Models"))
                {
                    // Check selected items count
                    if (ModelsListView.SelectedItems.Count > 0)
                    {
                        // Display confirmation message
                        MessageBoxResult result = MessageBox.Show(UITextTranslator.getTranslation("project_explorer.alert.delete").Replace("%N", ModelsListView.SelectedItems.Count.ToString()), "Forge Modding Helper", MessageBoxButton.YesNo, MessageBoxImage.Question);

                        // If user say Yes
                        if (result == MessageBoxResult.Yes)
                        {
                            // Delete all selected items
                            foreach (FileEntry element in ModelsListView.SelectedItems)
                            {
                                // Move the file to the recycle bin
                                FileSystem.DeleteFile(element.FilePath, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin, UICancelOption.DoNothing);
                            }

                            // Refresh models list
                            await Task.Run(() => { _workspaceManager.AssetsProperties.RetrieveAllModelsFiles(); });
                            await RefreshModelsListView(ModelsSearchTextbox.Text);
                        }
                    }
                }
                // If this is the textures delete button
                else if (senderButton.Name.Contains("Textures"))
                {
                    // Check selected items count
                    if (TexturesListView.SelectedItems.Count > 0)
                    {
                        // Display confirmation message
                        MessageBoxResult result = MessageBox.Show(UITextTranslator.getTranslation("project_explorer.alert.delete").Replace("%N", TexturesListView.SelectedItems.Count.ToString()), "Forge Modding Helper", MessageBoxButton.YesNo, MessageBoxImage.Question);

                        // If user say Yes
                        if (result == MessageBoxResult.Yes)
                        {
                            // Delete all selected items
                            foreach (FileEntry element in TexturesListView.SelectedItems)
                            {
                                // Move the file to the recycle bin
                                FileSystem.DeleteFile(element.FilePath, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin, UICancelOption.DoNothing);
                            }

                            // Refresh textures list
                            await Task.Run(() => { _workspaceManager.AssetsProperties.RetrieveAllTexturesFiles(); });
                            await RefreshTexturesListView(TexturesSearchTextbox.Text);
                        }
                    }
                }
            }
        }

        private void EditButtonClick(object sender, RoutedEventArgs e)
        {
            Button senderButton = sender as Button;

            // Check if sender is not null
            if (senderButton != null)
            {
                // If this is the blockstates edit button
                if (senderButton.Name.Contains("Blockstates"))
                {
                    // Check selected items count
                    if (BlockstatesListView.SelectedItems.Count == 1)
                    {
                        new JsonEditor(((FileEntry)BlockstatesListView.SelectedItem).FilePath).ShowDialog();
                    }
                }
                // If this is the models edit button
                else if (senderButton.Name.Contains("Models"))
                {
                    // Check selected items count
                    if (ModelsListView.SelectedItems.Count == 1)
                    {
                        new JsonEditor(((FileEntry)ModelsListView.SelectedItem).FilePath).ShowDialog();
                    }
                }
                // If this is the textures edit button
                else if (senderButton.Name.Contains("Textures"))
                {
                    // Check selected items count
                    if (TexturesListView.SelectedItems.Count == 1)
                    {
                        new JsonEditor(((FileEntry)TexturesListView.SelectedItem).FilePath).ShowDialog();
                    }
                }
            }
        }

        /// <summary>
        /// Function called by blockstates / models / textures rename buttons
        /// </summary>
        private async void RenameButtonClick(object sender, RoutedEventArgs e)
        {
            Button senderButton = sender as Button;

            // Check if sender is not null
            if (senderButton != null)
            {
                // If this is the blockstates rename button
                if (senderButton.Name.Contains("Blockstates"))
                {
                    // Check selected items count
                    if (BlockstatesListView.SelectedItems.Count == 1)
                    {
                        // Show rename dialog
                        new RenameDialog(((FileEntry)BlockstatesListView.SelectedItem).FilePath).ShowDialog();
                        // Refresh blockstates list
                        await Task.Run(() => { _workspaceManager.AssetsProperties.RetrieveAllBlockstatesFiles(); });
                        await RefreshBlockstatesListView(BlockstatesSearchTextbox.Text);
                    }
                }
                // If this is the models rename button
                else if (senderButton.Name.Contains("Models"))
                {
                    // Check selected items count
                    if (ModelsListView.SelectedItems.Count == 1)
                    {
                        // Show rename dialog
                        new RenameDialog(((FileEntry)ModelsListView.SelectedItem).FilePath).ShowDialog();
                        // Refresh models list
                        await Task.Run(() => { _workspaceManager.AssetsProperties.RetrieveAllModelsFiles(); });
                        await RefreshModelsListView(ModelsSearchTextbox.Text);
                    }
                }
                // If this is the textures rename button
                else if (senderButton.Name.Contains("Textures"))
                {
                    // Check selected items count
                    if (TexturesListView.SelectedItems.Count == 1)
                    {
                        // Show rename dialog
                        new RenameDialog(((FileEntry)TexturesListView.SelectedItem).FilePath).ShowDialog();
                        // Refresh textures list
                        await Task.Run(() => { _workspaceManager.AssetsProperties.RetrieveAllTexturesFiles(); });
                        await RefreshTexturesListView(TexturesSearchTextbox.Text);
                    }
                }
            }
        }

        /// <summary>
        /// Function called by blockstates / models / textures refresh button
        /// </summary>
        private async void RefreshButtonClick(object sender, RoutedEventArgs e)
        {
            Button senderButton = sender as Button;

            // Check if sender is not null
            if (senderButton != null)
            {
                // If this is the blockstates refresh button
                if (senderButton.Name.Contains("Blockstates"))
                {
                    // Refresh blockstates listView content
                    await Task.Run(() => { _workspaceManager.AssetsProperties.RetrieveAllBlockstatesFiles(); });
                    await RefreshBlockstatesListView(BlockstatesSearchTextbox.Text);
                }
                // If this is the models refresh button
                else if (senderButton.Name.Contains("Models"))
                {
                    // Refresh models listView content
                    await Task.Run(() => { _workspaceManager.AssetsProperties.RetrieveAllModelsFiles(); });
                    await RefreshModelsListView(ModelsSearchTextbox.Text);
                }
                // If this is the textures refresh button
                else if (senderButton.Name.Contains("Textures"))
                {
                    // Refresh textures listView content
                    await Task.Run(() => { _workspaceManager.AssetsProperties.RetrieveAllTexturesFiles(); });
                    await RefreshTexturesListView(TexturesSearchTextbox.Text);
                }
            }
        }

        /// <summary>
        /// Function called by blockstates / models / textures importation button
        /// </summary>
        private async void ImportationButtonClick(object sender, RoutedEventArgs e)
        {
            Button senderButton = sender as Button;

            // Check if sender is not null
            if (senderButton != null)
            {
                // File dialog
                OpenFileDialog filesDialog = new OpenFileDialog();
                filesDialog.Title = UITextTranslator.getTranslation("file_dialog.import.title");
                filesDialog.Filter = UITextTranslator.getTranslation("file_dialog.import.filter") + " (*.*)|*.*";
                filesDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                filesDialog.Multiselect = true;
                filesDialog.ShowDialog();

                // Check if there are files
                if (filesDialog.FileNames.Length > 0)
                {
                    // If this is the blockstates importation button
                    if (senderButton.Name.Contains("Blockstates"))
                    {
                        // Show importation dialog
                        new ImportFilesDialog(filesDialog.FileNames, "blockstates", _workspaceManager.WorkspaceProperties.WorkspacePath, _workspaceManager.ModProperties.ModID).ShowDialog();

                        // Refresh blockstates listView content
                        await Task.Run(() => { _workspaceManager.AssetsProperties.RetrieveAllBlockstatesFiles(); });
                        await RefreshBlockstatesListView(BlockstatesSearchTextbox.Text);
                    }
                    // If this is the models importation button
                    else if (senderButton.Name.Contains("Models"))
                    {
                        // Show importation dialog
                        new ImportFilesDialog(filesDialog.FileNames, "models", _workspaceManager.WorkspaceProperties.WorkspacePath, _workspaceManager.ModProperties.ModID).ShowDialog();

                        // Refresh models listView content
                        await Task.Run(() => { _workspaceManager.AssetsProperties.RetrieveAllModelsFiles(); });
                        await RefreshModelsListView(ModelsSearchTextbox.Text);
                    }
                    // If this is the textures importation button
                    else if (senderButton.Name.Contains("Textures"))
                    {
                        // Show importation dialog
                        new ImportFilesDialog(filesDialog.FileNames, "textures", _workspaceManager.WorkspaceProperties.WorkspacePath, _workspaceManager.ModProperties.ModID).ShowDialog();

                        // Refresh textures listView content
                        await Task.Run(() => { _workspaceManager.AssetsProperties.RetrieveAllTexturesFiles(); });
                        await RefreshTexturesListView(TexturesSearchTextbox.Text);
                    }
                }
            }
        }

        /// <summary>
        /// Founction called whan the selection is changed in blockstates / models / textures listViews
        /// </summary>
        private void ListViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListView senderListView = sender as ListView;

            // Check if sender is not null
            if (senderListView != null)
            {
                // If this is the blockstates listview
                if (senderListView.Name.Contains("Blockstates"))
                {
                    // Enable buttons if at least 1 item is selected
                    if (senderListView.SelectedItems.Count > 0)
                    {
                        // Enable delete button
                        BlockstatesDeleteButton.IsEnabled = true;

                        // If only 1 item is selected, enable rename and edit button
                        if (senderListView.SelectedItems.Count == 1)
                        {
                            BlockstatesRenameButton.IsEnabled = true;
                            BlockstatesEditButton.IsEnabled = true;
                        }
                        else
                        {
                            BlockstatesRenameButton.IsEnabled = false;
                            BlockstatesEditButton.IsEnabled = false;
                        }
                    }
                    // Disable buttons
                    else
                    {
                        BlockstatesDeleteButton.IsEnabled = false;
                        BlockstatesRenameButton.IsEnabled = false;
                        BlockstatesEditButton.IsEnabled = false;
                    }
                }
                // If this is the models listview
                else if (senderListView.Name.Contains("Models"))
                {
                    // Enable buttons if at least 1 item is selected
                    if (senderListView.SelectedItems.Count > 0)
                    {
                        // Enable delete button
                        ModelsDeleteButton.IsEnabled = true;
                        // If only 1 item is selected, enable rename button
                        if (senderListView.SelectedItems.Count == 1)
                        {
                            ModelsRenameButton.IsEnabled = true;
                            ModelsEditButton.IsEnabled = true;
                        }
                        else
                        {
                            ModelsRenameButton.IsEnabled = false;
                            ModelsEditButton.IsEnabled = false;
                        }
                    }
                    // Disable buttons
                    else
                    {
                        ModelsDeleteButton.IsEnabled = false;
                        ModelsRenameButton.IsEnabled = false;
                        ModelsEditButton.IsEnabled = false;
                    }
                }
                // If this is the textures listview
                else if (senderListView.Name.Contains("Textures"))
                {
                    // Enable buttons if at least 1 item is selected
                    if (senderListView.SelectedItems.Count > 0)
                    {
                        // Enable delete button
                        TexturesDeleteButton.IsEnabled = true;
                        // If only 1 item is selected, enable rename button
                        if (senderListView.SelectedItems.Count == 1)
                        {
                            TexturesRenameButton.IsEnabled = true;
                        }
                        else
                        {
                            TexturesRenameButton.IsEnabled = false;
                        }
                    }
                    // Disable buttons
                    else
                    {
                        TexturesDeleteButton.IsEnabled = false;
                        TexturesRenameButton.IsEnabled = false;
                    }
                }
            }
        }

        /// <summary>
        /// Function called when a grid is displayed
        /// </summary>
        private async void GridIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Grid senderGrid = sender as Grid;

            // Check if sender is not null
            if (senderGrid != null && isClosing == false)
            {
                // If visiblity is changed to Visible
                if (senderGrid.Visibility == Visibility.Visible)
                {
                    // Specific action for each grid
                    // Blockstates grid
                    if (senderGrid.Name.Contains("Blockstates"))
                    {
                        // If no content in blockstates listView
                        if (BlockstatesListView.Items.Count == 0)
                        {
                            // Refresh blockstates listView content
                            await RefreshBlockstatesListView(BlockstatesSearchTextbox.Text);
                        }
                    }
                    // Models grid
                    else if (senderGrid.Name.Contains("Models"))
                    {
                        // If no content in models listView
                        if (ModelsListView.Items.Count == 0)
                        {
                            // Refresh models listView content
                            await RefreshModelsListView(ModelsSearchTextbox.Text);
                        }
                    }
                    // Textures grid
                    else if (senderGrid.Name.Contains("Textures"))
                    {
                        // If no content in models listView
                        if (TexturesListView.Items.Count == 0)
                        {
                            // Refresh models listView content
                            await RefreshTexturesListView(TexturesSearchTextbox.Text);
                        }
                    }
                    // Translations grid
                    else if (senderGrid.Name.Contains("Translations"))
                    {
                        // If no content in models listView
                        if (TranslationsFilesListBox.Items.Count == 0)
                        {
                            // Refresh models listView content
                            await RefreshTranslationsList();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Fonction called before window closing
        /// used to prevent closing
        /// </summary>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Displaying confirmation message
            MessageBoxResult msgResult = MessageBox.Show(UITextTranslator.getTranslation("project_explorer.alert.close"), "Forge Modding Helper", MessageBoxButton.YesNo, MessageBoxImage.Question);

            // If yes
            if (msgResult == MessageBoxResult.Yes)
            {
                // Set isClosing to true
                isClosing = true;

                // Configuring welcome window
                WelcomeWindow welcomeWindow = new WelcomeWindow();

                // Write workspacedata
                WorkspaceManagerHelper.WriteWorkspaceData(_workspaceManager);
                // Generate project file
                WorkspaceManagerHelper.WriteProjectFile(_workspaceManager.WorkspaceProperties);

                // Refresh recent project list
                LastWorkspaces.RefreshData();

                // Update welcome UI depending on the presence of recent projects or not
                LastWorkspaces.ReadData();
                if (LastWorkspaces.LastWorkspacesData.Count > 0)
                {
                    welcomeWindow.label_no_workspace_found.Visibility = Visibility.Hidden;
                    welcomeWindow.listbox_recent_workspaces.ItemsSource = LastWorkspaces.LastWorkspacesProjectFile;
                }
                else
                {
                    welcomeWindow.label_no_workspace_found.Visibility = Visibility.Visible;
                }

                // Cleaning Project Explorer window
                this.BlockstatesListView.Items.Clear();
                this.ModelsListView.Items.Clear();
                this.TexturesListView.Items.Clear();
                this.TranslationsFilesListBox.Items.Clear();
                this.TranslationsTextEditor.Clear();

                // Windows management
                welcomeWindow.Show();
            }
            // If no
            else
            {
                // Cancel closing
                e.Cancel = true;
            }
        }
        #endregion
    }
}
