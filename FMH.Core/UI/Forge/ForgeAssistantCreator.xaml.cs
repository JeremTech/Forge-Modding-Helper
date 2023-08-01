using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using FMH.Core.Utils;
using FMH.Workspace.Data;
using FMH.Workspace.WorkspaceManager;
using FMH.Core.Utils.UI;
using FMH.Core.Files.Software;
using McVersionsLib.Forge;
using FMH.Core.Objects;
using FMH.Core.UI.Common;

namespace FMH.Core.UI.Forge
{
    /// <summary>
    /// Logique d'interaction pour ForgeAssistantCreator.xaml
    /// </summary>
    public partial class ForgeAssistantCreator : Window
    {
        // Number of step of the Assistant Creator
        private double total_step = 6.0;
        // Current step
        private double step = 0.0;
        // Output folder
        private string folder = "";
        // Temp vars
        private List<String> AvailableForgeVersions = new List<string>();
        private string RecommendedForgeVersion = "";
        private string LatestForgeVersion = "";
        // Mod infos storage
        private Dictionary<string, string> mod_infos = new Dictionary<string, string>
        {
            {"mod_name", ""},
            {"mod_authors", ""},
            {"mod_version", ""},
            {"mod_license", ""},
            {"mod_description", ""},
            {"mod_id", ""},
            {"mod_group", ""},
            {"mod_logo", ""},
            {"mod_credits", ""},
            {"display_url", ""},
            {"issue_tracker", ""},
            {"update_json", ""},
            {"minecraft_version", ""},
            {"forge_version", ""}
        };

        // Workspace manager
        private IWorkspaceManager _workspaceManager;

        public ForgeAssistantCreator()
        {
            InitializeComponent();

            // Reset step display
            updateStep(0.0);

            // Hiding all grids except the first one
            first_grid.Visibility = Visibility.Visible;
            second_grid.Visibility = Visibility.Hidden;
            third_grid.Visibility = Visibility.Hidden;
            fourth_grid.Visibility = Visibility.Hidden;
            fifth_grid.Visibility = Visibility.Hidden;
            sixth_grid.Visibility = Visibility.Hidden;
            generation_grid.Visibility = Visibility.Hidden;
            finish_grid.Visibility = Visibility.Hidden;
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            // Loadings translations
            UITextTranslator.LoadTranslationFile(OptionsFile.GetCurrentLanguage());
            UITextTranslator.UpdateComponentsTranslations(this.main_grid);
            label_welcome_output_directory.Text = UITextTranslator.getTranslation("assistant_creator.label.welcome.output_directory_message");
            this.Title = UITextTranslator.getTranslation("assistant_creator.title");

            // Adding supported Minecraft versions to the step 4 ComboBox
            App.getSupportedMinecraftVersions().Reverse();
            forge_version_comboBox.ItemsSource = App.getSupportedMinecraftVersions();
        }

        /// <summary>
        /// Update step display
        /// </summary>
        /// <param name="stepIn">New step number</param>
        public void updateStep(double stepIn)
        {
            step_progressbar.Value = (stepIn / this.total_step) * 100;
            step_label.Content = UITextTranslator.getTranslation("assistant_creator.step") + " " + stepIn + " / " + this.total_step;
        }

        #region Cancel button
        /// <summary>
        /// Function called when the user click on the cancel button
        /// </summary>
        private void Cancel_button_Click(object sender, RoutedEventArgs e)
        {

            this.Close();
        }
        #endregion

        #region Next Button
        /// <summary>
        /// Function called when the user click on the next button
        /// </summary>
        private void Next_button_Click(object sender, RoutedEventArgs e)
        {
            // Depending on the step number
            switch (this.step)
            {
                case 0:
                    {
                        // Check if mandatory step's infos are completed
                        if (!string.IsNullOrWhiteSpace(this.folder))
                        {
                            // Update UI components
                            first_grid.Visibility = Visibility.Hidden;
                            missing_infos_label.Visibility = Visibility.Hidden;
                            second_grid.Visibility = Visibility.Visible;
                            previous_button.IsEnabled = true;

                            // Increase and update step number
                            this.step++;
                            this.updateStep(this.step);
                        }
                        else
                        {
                            // Display missing infos message
                            missing_infos_label.Visibility = Visibility.Visible;
                        }
                        break;
                    }
                case 1:
                    {
                        // Check if mandatory step's infos are completed
                        if (!String.IsNullOrEmpty(this.mod_infos["mod_name"]) && !String.IsNullOrEmpty(this.mod_infos["mod_authors"]) && !String.IsNullOrEmpty(this.mod_infos["mod_version"]) && !String.IsNullOrEmpty(this.mod_infos["mod_license"]))
                        {
                            // Update UI components
                            missing_infos_label.Visibility = Visibility.Hidden;
                            second_grid.Visibility = Visibility.Hidden;
                            third_grid.Visibility = Visibility.Visible;
                            missing_infos_label.Visibility = Visibility.Hidden;

                            // Suggest ModID if no ModID is already writed
                            if (string.IsNullOrWhiteSpace(this.mod_id_textbox.Text))
                            {
                                this.mod_id_textbox.Text = StringUtils.CreateModIDFromModName(this.mod_infos["mod_name"]);
                            }

                            // Suggest ModGroup if no ModGroup is already writed
                            if (string.IsNullOrWhiteSpace(this.mod_group_textbox.Text))
                            {
                                this.mod_group_textbox.Text = StringUtils.CreateModGroupFromModIDAndAuthor(this.mod_infos["mod_id"], this.mod_infos["mod_authors"]);
                            }

                            // Increase and update step number
                            this.step++;
                            this.updateStep(this.step);
                        }
                        else
                        {
                            // Display missing infos message
                            missing_infos_label.Visibility = Visibility.Visible;
                        }
                        break;
                    }
                case 2:
                    {
                        // Check if mandatory step's infos are completed
                        if (!String.IsNullOrWhiteSpace(this.mod_infos["mod_id"]) && !String.IsNullOrWhiteSpace(this.mod_infos["mod_group"]))
                        {
                            // Update UI components
                            missing_infos_label.Visibility = Visibility.Hidden;
                            third_grid.Visibility = Visibility.Hidden;
                            fourth_grid.Visibility = Visibility.Visible;

                            // Increase and update step number
                            this.step++;
                            this.updateStep(this.step);
                        }
                        else
                        {
                            // Display missing infos message
                            missing_infos_label.Visibility = Visibility.Visible;
                        }
                        break;
                    }
                case 3:
                    {
                        if (!String.IsNullOrWhiteSpace(this.mod_infos["minecraft_version"]) && !String.IsNullOrWhiteSpace(this.mod_infos["forge_version"]))
                        {
                            // Setup adapted workspace manager
                            _workspaceManager = WorkspaceManagerHelper.GetWorkspaceManager(this.mod_infos["minecraft_version"], folder);
                            _workspaceManager.AssetsProperties.SetModId(this.mod_infos["mod_id"]);

                            // If no generator have been found, critical error and application exit
                            if (_workspaceManager == null)
                            {
                                MessageBox.Show(UITextTranslator.getTranslation("assistant_creator.alert.no_generator"));
                                App.Current.Shutdown();
                            }

                            // Update UI components
                            fourth_grid.Visibility = Visibility.Hidden;
                            fifth_grid.Visibility = Visibility.Visible;
                            missing_infos_label.Visibility = Visibility.Hidden;

                            // Increase and update step number
                            this.step++;
                            this.updateStep(this.step);
                        }
                        else
                        {
                            // Display missing infos message
                            missing_infos_label.Visibility = Visibility.Visible;
                        }

                        break;
                    }
                case 4:
                    {
                        // Update UI components
                        fifth_grid.Visibility = Visibility.Hidden;
                        sixth_grid.Visibility = Visibility.Visible;
                        missing_infos_label.Visibility = Visibility.Hidden;

                        // Increase and update step number
                        this.step++;
                        this.updateStep(this.step);
                        break;
                    }
                case 5:
                    {
                        // Save mod data
                        _workspaceManager.ModProperties.ModName = this.mod_infos["mod_name"];
                        _workspaceManager.ModProperties.ModAuthors = this.mod_infos["mod_authors"];
                        _workspaceManager.ModProperties.ModVersion = this.mod_infos["mod_version"];
                        _workspaceManager.ModProperties.ModLicense = this.mod_infos["mod_license"];
                        _workspaceManager.ModProperties.ModDescription = this.mod_infos["mod_description"];
                        _workspaceManager.ModProperties.ModID = this.mod_infos["mod_id"];
                        _workspaceManager.ModProperties.ModGroup = this.mod_infos["mod_group"];
                        _workspaceManager.ModProperties.ModLogo = this.mod_infos["mod_logo"];
                        _workspaceManager.ModProperties.ModCredits = this.mod_infos["mod_credits"];
                        _workspaceManager.ModProperties.ModWebsite = this.mod_infos["display_url"];
                        _workspaceManager.ModProperties.ModIssueTracker = this.mod_infos["issue_tracker"];
                        _workspaceManager.ModProperties.ModUpdateJSONURL = this.mod_infos["update_json"];
                        _workspaceManager.ModProperties.ModMinecraftVersion = this.mod_infos["minecraft_version"];
                        _workspaceManager.ModProperties.ModAPIVersion = this.mod_infos["forge_version"].Split('-')[1];

                        _workspaceManager.WorkspaceProperties.MCVersion = this.mod_infos["minecraft_version"];
                        _workspaceManager.WorkspaceProperties.ModAPI = ModAPIType.Forge;
                        _workspaceManager.WorkspaceProperties.SoftwareVersion = App.GetApplicationVersionCompact();
                        _workspaceManager.WorkspaceProperties.APIVersion = this.mod_infos["forge_version"];
                        _workspaceManager.WorkspaceProperties.LastOpened = DateTime.Now;

                        // Update UI components
                        sixth_grid.Visibility = Visibility.Hidden;
                        generation_grid.Visibility = Visibility.Visible;
                        missing_infos_label.Visibility = Visibility.Hidden;
                        next_button.IsEnabled = false;
                        previous_button.IsEnabled = false;
                        step_label.Visibility = Visibility.Hidden;

                        // Increase and update step number
                        this.step++;
                        this.updateStep(this.step);

                        // Forge download
                        update_progress(0, UITextTranslator.getTranslation("assistant_creator.progress.downloading_forge") + this.mod_infos["forge_version"] + "...");

                        WebClient client = new WebClient();
                        client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
                        client.DownloadFileCompleted += new AsyncCompletedEventHandler(client_DownloadFileCompleted);

                        string dlLink = McForgeUtils.BuildMinecraftForgeMDKDownloadLink(this.mod_infos["forge_version"]);
                        try
                        {
                            client.DownloadFileAsync(new Uri(dlLink), this.folder + @"\mdk.zip");
                        }
                        catch (VersionNotFoundException)
                        {
                            update_progress(0, UITextTranslator.getTranslation("assistant_creator.progress.downloading_forge.error") + this.mod_infos["forge_version"] + " !");
                        }

                        // Saving in recent workspaces
                        LastWorkspaces.LastWorkspacesData.Add(new WorkspaceEntry(this.folder, DateTime.Now, _workspaceManager.WorkspaceProperties.ModAPI, _workspaceManager.WorkspaceProperties.MCVersion));
                        LastWorkspaces.WriteData();

                        break;
                    }
                case 6:
                    {
                        // Update UI components
                        generation_grid.Visibility = Visibility.Hidden;
                        finish_grid.Visibility = Visibility.Visible;
                        next_button.IsEnabled = false;
                        cancel_button.IsEnabled = false;
                        cancel_button.Visibility = Visibility.Hidden;
                        finish_button.Visibility = Visibility.Visible;

                        break;
                    }
            }
        }
        #endregion

        #region Previous button
        /// <summary>
        /// Function called when the user click on the previous button
        /// </summary>
        private void Previous_button_Click(object sender, RoutedEventArgs e)
        {
            // Depending on the step number
            switch (step)
            {
                case 1:
                    {
                        // Update UI components
                        previous_button.IsEnabled = false;
                        first_grid.Visibility = Visibility.Visible;
                        second_grid.Visibility = Visibility.Hidden;
                        missing_infos_label.Visibility = Visibility.Hidden;

                        // Decrease and update step number
                        this.step--;
                        this.updateStep(this.step);
                        break;
                    }
                case 2:
                    {
                        // Update UI components
                        second_grid.Visibility = Visibility.Visible;
                        third_grid.Visibility = Visibility.Hidden;
                        missing_infos_label.Visibility = Visibility.Hidden;

                        // Decrease and update step number
                        this.step--;
                        this.updateStep(this.step);
                        break;
                    }
                case 3:
                    {
                        // Update UI components
                        third_grid.Visibility = Visibility.Visible;
                        fourth_grid.Visibility = Visibility.Hidden;
                        missing_infos_label.Visibility = Visibility.Hidden;

                        // Decrease and update step number
                        this.step--;
                        this.updateStep(this.step);
                        break;
                    }
                case 4:
                    {
                        // Update UI components
                        fourth_grid.Visibility = Visibility.Visible;
                        fifth_grid.Visibility = Visibility.Hidden;
                        missing_infos_label.Visibility = Visibility.Hidden;

                        // Decrease and update step number
                        this.step--;
                        this.updateStep(this.step);
                        break;
                    }
                case 5:
                    {
                        // Update UI components
                        fifth_grid.Visibility = Visibility.Visible;
                        sixth_grid.Visibility = Visibility.Hidden;
                        missing_infos_label.Visibility = Visibility.Hidden;

                        // Decrease and update step number
                        this.step--;
                        this.updateStep(this.step);
                        break;
                    }
            }
        }
        #endregion

        #region Generation
        /// <summary>
        /// Function called when the forge download progress change
        /// </summary>
        private void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            double bytesIn = double.Parse(e.BytesReceived.ToString());
            double totalBytes = double.Parse(e.TotalBytesToReceive.ToString());
            double percentage = bytesIn / totalBytes * 100;

            // Update progressbar value with the download percentage
            progress_progressbar.Value = int.Parse(Math.Truncate(percentage).ToString());
        }

        /// <summary>
        /// Function called when the forge download is completed
        /// </summary>
        private void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            // Extract the downloaded archive
            update_progress(0, UITextTranslator.getTranslation("assistant_creator.progress.extract_archive"));
            ZipFile.ExtractToDirectory(this.folder + @"\mdk.zip", this.folder);

            // Check if the mdk.zip is always in the directory
            if (File.Exists(this.folder + @"\mdk.zip"))
            {
                // Delete mdk.zip file
                update_progress(0, UITextTranslator.getTranslation("assistant_creator.progress.delete_archive"));
                File.Delete(this.folder + @"\mdk.zip");
            }

            // Creation of workspaces/assets folders
            generate_folders();

            // Creation of files
            generate_files();

            // End actions
            update_progress(100, UITextTranslator.getTranslation("assistant_creator.progress.finish"));
            this.next_button.IsEnabled = true;
        }

        /// <summary>
        /// This function generate all mod folders depend of the user choices
        /// </summary>
        private void generate_folders()
        {
            // Generate code directories
            if (code_packages_checkBox.IsChecked == true)
            {
                update_progress(0, UITextTranslator.getTranslation("assistant_creator.progress.deleting_example") + " \"" + this.folder + @"\src\main\java" + "\"...");
                Directory.Delete(this.folder + @"\src\main\java\com", true);

                update_progress(0, UITextTranslator.getTranslation("assistant_creator.progress.creating_code_package") + " \"" + this.folder + @"\src\main\java" + "\"...");
                Directory.CreateDirectory(this.folder + @"\src\main\java\" + this.mod_infos["mod_group"].Replace(".", @"\"));
            }

            // Generate assets directories
            if (assets_packages_checkBox.IsChecked == true)
            {
                update_progress(0, UITextTranslator.getTranslation("assistant_creator.progress.creating_textures_folders") + " \"" + this.folder + @"\src\main\assets" + "\"...");
                Directory.CreateDirectory(this.folder + @"\src\main\resources\assets\" + this.mod_infos["mod_id"] + @"\textures\block");
                Directory.CreateDirectory(this.folder + @"\src\main\resources\assets\" + this.mod_infos["mod_id"] + @"\textures\item");
                Directory.CreateDirectory(this.folder + @"\src\main\resources\assets\" + this.mod_infos["mod_id"] + @"\textures\gui");
                Directory.CreateDirectory(this.folder + @"\src\main\resources\assets\" + this.mod_infos["mod_id"] + @"\textures\entity");
                Directory.CreateDirectory(this.folder + @"\src\main\resources\assets\" + this.mod_infos["mod_id"] + @"\textures\models");

                update_progress(0, UITextTranslator.getTranslation("assistant_creator.progress.creating_models_folders") + " \"" + this.folder + @"\src\main\assets" + "\"...");
                Directory.CreateDirectory(this.folder + @"\src\main\resources\assets\" + this.mod_infos["mod_id"] + @"\models\block");
                Directory.CreateDirectory(this.folder + @"\src\main\resources\assets\" + this.mod_infos["mod_id"] + @"\models\item");

                update_progress(0, UITextTranslator.getTranslation("assistant_creator.progress.creating_blockstates_folder") + " \"" + this.folder + @"\src\main\assets" + "\"...");
                Directory.CreateDirectory(this.folder + @"\src\main\resources\assets\" + this.mod_infos["mod_id"] + @"\blockstates");

                update_progress(0, UITextTranslator.getTranslation("assistant_creator.progress.creating_lang_folder") + " \"" + this.folder + @"\src\main\assets" + "\"...");
                Directory.CreateDirectory(this.folder + @"\src\main\resources\assets\" + this.mod_infos["mod_id"] + @"\lang");
            }
        }

        /// <summary>
        /// This function generate all mod files depend of the user choices
        /// </summary>
        private void generate_files()
        {
            // Generate build.gradle file
            if (build_gradle_checkBox.IsChecked == true)
            {
                update_progress(0, UITextTranslator.getTranslation("assistant_creator.progress.configurate_build_gradle_file") + " \"" + this.folder + "\"...");
                _workspaceManager.WriteBuildGradle();
                _workspaceManager.WriteGradleProperties();
            }

            // Generate mod.toml file
            if (mod_toml_checkBox.IsChecked == true)
            {
                update_progress(0, UITextTranslator.getTranslation("assistant_creator.progress.configurate_toml_file") + " \"" + this.folder + @"\src\main\resources\META-INF\""...");
                _workspaceManager.WriteModToml();
            }

            // Generate project file
            WorkspaceManagerHelper.WriteProjectFile(_workspaceManager.WorkspaceProperties);

            // Copy mod logo (if gave by the user)
            if (!string.IsNullOrEmpty(this.mod_infos["mod_logo"]))
            {
                update_progress(0, UITextTranslator.getTranslation("assistant_creator.progress.copy_mod_logo"));

                if (File.Exists(this.mod_infos["mod_logo"]))
                {
                    File.Copy(this.mod_infos["mod_logo"], this.folder + @"\src\main\resources\logo.png");
                }
                else
                {
                    update_progress(0, UITextTranslator.getTranslation("assistant_creator.progress.copy_mod_logo_error"));
                }
            }

            // Write workspace data
            WorkspaceManagerHelper.WriteWorkspaceData(_workspaceManager);
        }

        /// <summary>
        /// Update the generation progress
        /// </summary>
        /// <param name="progress">Value to display in the generation progressbar</param>
        /// <param name="statut">Text to display under the progressbar and in the history list</param>
        private void update_progress(int progress, string statut)
        {
            progress_progressbar.Value = progress;
            progress_label.Content = statut;
            progress_listBox.Items.Add(statut);
        }
        #endregion

        #region Messages
        /// <summary>
        /// Show an error message 
        /// </summary>
        /// <param name="error">Error message</param>
        public void showErrorMessageAndShutdown(string error)
        {
            MessageBox.Show("Une erreur est survenue.\n\"" + error + "\"\nL'application va se fermer.", "Forge Modding Helper", MessageBoxButton.OK, MessageBoxImage.Error);
            System.Windows.Application.Current.Shutdown();
        }
        #endregion

        #region TextBoxes events
        private void TextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (textBox != null)
            {
                this.mod_infos[textBox.Tag.ToString()] = textBox.Text;
                Image img = (Image)this.FindName(textBox.Tag + "_image");

                if (img != null)
                {
                    if (textBox.Text.isTextValid())
                    {
                        img.Source = new BitmapImage(new Uri("/FMH.Resources;component/Pictures/check.png", UriKind.Relative));
                    }
                    else
                    {
                        img.Source = new BitmapImage(new Uri("/FMH.Resources;component/Pictures/uncheck.png", UriKind.Relative));
                    }
                }

                if (missing_infos_label != null)
                {
                    missing_infos_label.Visibility = Visibility.Hidden;
                }
            }
        }

        private void Optional_TextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (textBox != null)
            {
                this.mod_infos[textBox.Tag.ToString()] = textBox.Text;
            }

        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            textBox.Text = textBox.Text.Trim(' ');
        }
        #endregion

        #region API Version selection related functions
        /// <summary>
        /// Function called when the selected minecraft version change
        /// </summary>
        private async void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Update UI component
            BlurEffect effect = new BlurEffect();
            effect.Radius = 20;
            forge_version_list.Effect = effect;
            loading_versions_progressbar.Visibility = Visibility.Visible;
            mod_infos["minecraft_version"] = (string)(forge_version_comboBox.SelectedItem);

            // Retrieve Forge versions
            await RetrieveAvailableForgeVersion(mod_infos["minecraft_version"]);

            // Clear then fill list
            forge_version_list.Items.Clear();

            foreach (string version in AvailableForgeVersions)
            {
                // If this is the recommended version
                if (version.Equals(mod_infos["minecraft_version"] + "-" + RecommendedForgeVersion))
                {
                    // Display a small star icon before version text
                    forge_version_list.Items.Insert(0, new { Version = version, Image = new BitmapImage(new Uri("/FMH.Resources;component/Pictures/star_icon.png", UriKind.Relative)) });
                }
                // Else if this is the latest version
                else if (version.Equals(mod_infos["minecraft_version"] + "-" + LatestForgeVersion))
                {
                    // Display a small bug icon before version text
                    forge_version_list.Items.Insert(0, new { Version = version, Image = new BitmapImage(new Uri("/FMH.Resources;component/Pictures/bug_icon.png", UriKind.Relative)) });
                }
                else
                {
                    // No icon before version text
                    forge_version_list.Items.Insert(0, new { Version = version, Image = (BitmapImage)null });
                }
            }

            // Update UI
            forge_version_list.ClearValue(EffectProperty);
            loading_versions_progressbar.Visibility = Visibility.Hidden;
            minecraft_version_image.Source = new BitmapImage(new Uri("/FMH.Resources;component/Pictures/check.png", UriKind.Relative));
        }

        /// <summary>
        /// Retrieve all forge versions informations (using McModAPIVersions)
        /// </summary>
        /// <param name="mcVersion">Targetted Minecraft version</param>
        private async Task RetrieveAvailableForgeVersion(string mcVersion)
        {
            // Run it async
            await Task.Factory.StartNew(() =>
            {
                try
                {
                    // Retrieve all available Forge version for the wanted Minecraft version
                    AvailableForgeVersions = McForgeVersions.GetAllMinecraftForgeVersions(mcVersion, true);
                    LatestForgeVersion = McForgeVersions.GetLatestMinecraftForgeVersion(mcVersion, true);
                    RecommendedForgeVersion = McForgeVersions.GetRecommendedMinecraftForgeVersion(mcVersion, true);
                }
                catch (VersionNotFoundException)
                {
                    MessageBox.Show(UITextTranslator.getTranslation("assistant_creator.alert.no_forge_version"), "Forge Modding Helper", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (WebException)
                {
                    // If we can't retrieve remote informations 
                    MessageBox.Show(UITextTranslator.getTranslation("assistant_creator.alert.no_connection"), "Forge Modding Helper", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            });
        }

        /// <summary>
        /// Function called when the user change version selection
        /// </summary>
        private void forge_version_list_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Convert sender as listbox
            ListBox listBox = sender as ListBox;

            // If successfull convertion and selected item not null
            if (listBox != null && listBox.SelectedItem != null)
            {
                // Retrieve version value from anonymous type
                string version = (string)listBox.SelectedItem.GetType().GetProperty("Version").GetValue(listBox.SelectedItem, null);
                selected_version_label.Content = version;
                this.mod_infos["forge_version"] = version;
            }
        }
        #endregion

        #region Browse logo
        /// <summary>
        /// Function called when the user click on the browse button for the mod logo
        /// </summary>
        private void Browse_logo_button_Click(object sender, RoutedEventArgs e)
        {
            // Create and configure FileDialog
            Microsoft.Win32.OpenFileDialog fileDialog = new Microsoft.Win32.OpenFileDialog();
            fileDialog.RestoreDirectory = true;
            fileDialog.Title = "Choisissez un logo pour votre mod";
            fileDialog.DefaultExt = "png";
            fileDialog.Filter = "png files (*.png)|*.png";
            fileDialog.CheckFileExists = true;
            fileDialog.CheckPathExists = true;
            // Display the FileDialog
            fileDialog.ShowDialog();

            // Check the user selection
            if (!String.IsNullOrWhiteSpace(fileDialog.FileName))
            {
                this.logo_path_textbox.Text = fileDialog.FileName;
                this.mod_logo_image.Source = new BitmapImage(new Uri(fileDialog.FileName));
            }
            else
            {
                this.logo_path_textbox.Text = "";
                this.mod_logo_image.Source = null;
            }

            // Update logo path
            this.mod_infos["mod_logo"] = this.logo_path_textbox.Text;
        }
        #endregion

        #region Browse directory button
        /// <summary>
        /// Function called when the user click on the browse button for the output directory
        /// </summary>
        private void button_browse_directory_Click(object sender, RoutedEventArgs e)
        {
            // Allow user to select workspace output directory
            using (var fbd = new System.Windows.Forms.FolderBrowserDialog())
            {
                // Display FolderDialog
                System.Windows.Forms.DialogResult result = fbd.ShowDialog();

                // Check the user selection
                if (!string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    this.textbox_directory.Text = fbd.SelectedPath;

                    // Check if the selected output directory is not a Forge Workspace
                    if (!DirectoryUtils.CheckFolderIsForgeWorkspace(fbd.SelectedPath))
                    {
                        this.folder = fbd.SelectedPath;
                        this.label_invalid_workspace_folder.Visibility = Visibility.Hidden;
                        missing_infos_label.Visibility = Visibility.Hidden;
                    }
                    else
                    {
                        this.folder = "";
                        this.label_invalid_workspace_folder.Visibility = Visibility.Visible;
                    }
                }
                // If the selected path is null and the current folder too
                else if (string.IsNullOrWhiteSpace(fbd.SelectedPath) && string.IsNullOrWhiteSpace(this.folder))
                {
                    this.label_invalid_workspace_folder.Visibility = Visibility.Hidden;
                    this.folder = "";
                    this.textbox_directory.Text = "";
                }
            }
        }
        #endregion

        #region Finish button
        /// <summary>
        /// Function called when the user click on the finish button
        /// </summary>
        private void finish_button_Click(object sender, RoutedEventArgs e)
        {
            new ForgeProjectExplorer(_workspaceManager.WorkspaceProperties.WorkspacePath);
            this.Close();
        }
        #endregion

        #region Prevent Window closing
        /// <summary>
        /// Function called before window closing
        /// </summary>
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            // If the current step is not the final step
            if (step < total_step)
            {
                // Display a closing confirmation
                MessageBoxResult action = MessageBox.Show(this, UITextTranslator.getTranslation("assistant_creator.close_message"), "Forge Modding Helper", MessageBoxButton.YesNo, MessageBoxImage.Question);

                // If the user refuse
                if (action == MessageBoxResult.No)
                {
                    // We cancel the event / the window closing
                    e.Cancel = true;
                }
                else
                {
                    WelcomeWindow welcomeWindow = new WelcomeWindow();

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

                    // Windows management
                    welcomeWindow.Show();
                }
            }
        }

        #endregion

        private void mod_license_help_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start("https://choosealicense.com/");
        }
    }
}
