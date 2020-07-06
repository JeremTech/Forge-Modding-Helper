using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using Forge_Modding_Helper_3.Files;
using Forge_Modding_Helper_3.Objects;
using Forge_Modding_Helper_3.Utils;
using Forge_Modding_Helper_3.Windows;

namespace Forge_Modding_Helper_3
{
    public partial class AssistantCreator : Window
    {
        // Number of step of the Assistant Creator
        private double total_step = 5.0;
        // Current step
        private double step = 0.0;
        // Output folder
        private string folder = "";
        // Url to json to check last forge versions
        private string versions_url = "https://files.minecraftforge.net/maven/net/minecraftforge/forge/promotions_slim.json";
        // Background Worker to retrieve forge versions
        private readonly BackgroundWorker background_thread = new BackgroundWorker();
        // Forge Version manager
        private ForgeVersionsUtils versions = new ForgeVersionsUtils();
        // Mod infos storage
        private Dictionary<string, string> mod_infos = new Dictionary<string, string>
        {
            {"mod_name", ""},
            {"mod_authors", ""},
            {"mod_version", ""},
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

        public AssistantCreator()
        {
            InitializeComponent();

            // Reset step display
            updateStep(0.0);

            // Initialize background thread
            background_thread.DoWork += Background_thread_DoWork;
            background_thread.RunWorkerCompleted += Background_thread_RunWorkerCompleted;

            // Hiding all grids except the first one
            first_grid.Visibility = Visibility.Visible;
            second_grid.Visibility = Visibility.Hidden;
            third_grid.Visibility = Visibility.Hidden;
            fourth_grid.Visibility = Visibility.Hidden;
            fith_grid.Visibility = Visibility.Hidden;
            generation_grid.Visibility = Visibility.Hidden;
            finish_grid.Visibility = Visibility.Hidden;
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            // Loadings translations
            UITextTranslator.LoadTranslationFile("french");
            UITextTranslator.UpdateComponentsTranslations(this.main_grid);
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

        #region Background Worker
        /// <summary>
        /// Function executed when the forge version retrieval background thread finished his tasks
        /// </summary>
        private void Background_thread_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // Update Forge version selection UI
            forge_versions_grid.ClearValue(EffectProperty);
            loading_label.Visibility = Visibility.Hidden;
            loading_progressbar.Visibility = Visibility.Hidden;

            // Check if a latest version is found
            if (!string.IsNullOrWhiteSpace(versions.getLatestForgeVersion(mod_infos["minecraft_version"])))
            {
                this.latest_forge_version_label.Content = versions.getLatestForgeVersion(mod_infos["minecraft_version"]);
                this.latest_forge_version_grid.Visibility = Visibility.Visible;
            }
            else
            {
                this.latest_forge_version_grid.Visibility = Visibility.Hidden;
            }

            // Check if a recommended version is found
            if (!string.IsNullOrWhiteSpace(versions.getRecommendedForgeVersion(mod_infos["minecraft_version"])))
            {
                this.recommended_forge_version_label.Content = versions.getRecommendedForgeVersion(mod_infos["minecraft_version"]);
                this.recommended_forge_version_grid.Visibility = Visibility.Visible;
            }
            else
            {
                this.recommended_forge_version_grid.Visibility = Visibility.Hidden;
            }

            // Set selected version to the latest version
            this.mod_infos["forge_version"] = versions.getLatestForgeVersion(mod_infos["minecraft_version"]);
        }

        /// <summary>
        /// Function executed when the forge version retrieval background thread is call
        /// </summary>
        private void Background_thread_DoWork(object sender, DoWorkEventArgs e)
        {
            string content = "";

            using (WebClient client = new WebClient())
            {
                // Retrieve forge versions json content
                content = client.DownloadString(versions_url);
            }

            // Convert content to ForgeVersionsUtils object
            versions = JsonConvert.DeserializeObject<ForgeVersionsUtils>(content);
        }
        #endregion

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
                        if (!String.IsNullOrEmpty(this.mod_infos["mod_name"]) && !String.IsNullOrEmpty(this.mod_infos["mod_authors"]) && !String.IsNullOrEmpty(this.mod_infos["mod_version"]))
                        {
                            // Update UI components
                            missing_infos_label.Visibility = Visibility.Hidden;
                            second_grid.Visibility = Visibility.Hidden;
                            third_grid.Visibility = Visibility.Visible;
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
                case 2:
                    {
                        // Check if mandatory step's infos are completed
                        if (!String.IsNullOrWhiteSpace(this.mod_infos["mod_id"]) && !String.IsNullOrWhiteSpace(this.mod_infos["mod_group"]) && !String.IsNullOrWhiteSpace(this.mod_infos["minecraft_version"]) && !String.IsNullOrWhiteSpace(this.mod_infos["forge_version"]))
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
                        // Update UI components
                        fourth_grid.Visibility = Visibility.Hidden;
                        fith_grid.Visibility = Visibility.Visible;
                        missing_infos_label.Visibility = Visibility.Hidden;

                        // Increase and update step number
                        this.step++;
                        this.updateStep(this.step);
                        break;
                    }
                case 4:
                    {
                        // Saving in recent workspaces
                        RecentWorkspaces.RecentWorkspacesList.Add(new Workspace(this.mod_infos["mod_name"], this.mod_infos["minecraft_version"], this.folder, this.mod_infos["mod_description"], DateTime.Now));
                        RecentWorkspaces.WriteDataFile();

                        // Update UI components
                        fith_grid.Visibility = Visibility.Hidden;
                        generation_grid.Visibility = Visibility.Visible;
                        missing_infos_label.Visibility = Visibility.Hidden;
                        next_button.IsEnabled = false;
                        previous_button.IsEnabled = false;
                        step_label.Visibility = Visibility.Hidden;

                        // Increase and update step number
                        this.step++;
                        this.updateStep(this.step);

                        // Forge download
                        update_progress(0, "Téléchargement de Forge " + this.mod_infos["minecraft_version"] + " - " + this.mod_infos["forge_version"] + " en cours...");

                        WebClient client = new WebClient();
                        client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
                        client.DownloadFileCompleted += new AsyncCompletedEventHandler(client_DownloadFileCompleted);
                        client.DownloadFileAsync(new Uri("https://files.minecraftforge.net/maven/net/minecraftforge/forge/" + this.mod_infos["minecraft_version"] + "-" + this.mod_infos["forge_version"] + "/forge-" + this.mod_infos["minecraft_version"] + "-" + this.mod_infos["forge_version"] + "-mdk.zip"), this.folder + @"\mdk.zip");

                        break;
                    }
                case 5:
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
                    fith_grid.Visibility = Visibility.Hidden;
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
            if(code_packages_checkBox.IsChecked == true)
            {
                update_progress(0, UITextTranslator.getTranslation("assistant_creator.progress.deleting_example") + " \"" + this.folder + @"\src\main\java" + "\"...");
                
                File.Delete(this.folder + @"\src\main\java\com\example\examplemod\ExampleMod.java");
                Directory.Delete(this.folder + @"\src\main\java\com\example\examplemod");
                Directory.Delete(this.folder + @"\src\main\java\com\example");
                Directory.Delete(this.folder + @"\src\main\java\com");

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
            // Generate blank fr language file
            if (fr_lang_file_checkBox.IsChecked == true)
            {
                if(!Directory.Exists(this.folder + @"\src\main\resources\assets\" + this.mod_infos["mod_id"] + @"\lang"))
                {
                    update_progress(0, UITextTranslator.getTranslation("assistant_creator.progress.creating_lang_folder") + " \"" + this.folder + @"\src\main\assets" + "\"...");
                    Directory.CreateDirectory(this.folder + @"\src\main\resources\assets\" + this.mod_infos["mod_id"] + @"\lang");
                }

                update_progress(0, UITextTranslator.getTranslation("assistant_creator.progress.creating_lang_file"));
                File.WriteAllText(this.folder + @"\src\main\resources\assets\" + this.mod_infos["mod_id"] + @"\lang\fr_fr.json", "{" + Environment.NewLine + Environment.NewLine + "}");
            }

            // Generate build.gradle file
            if (build_gradle_checkBox.IsChecked == true)
            {
                update_progress(0, UITextTranslator.getTranslation("assistant_creator.progress.configurate_build_gradle_file") + " \"" + this.folder + "\"...");
                BuildGradle build_gradle = new BuildGradle(this.mod_infos, this.folder);
                build_gradle.generateFile();
            }

            // Generate mod.toml file
            if (mod_toml_checkBox.IsChecked == true)
            {
                update_progress(0, UITextTranslator.getTranslation("assistant_creator.progress.configurate_toml_file") + " \"" + this.folder + @"\src\main\resources\META-INF\""...");
                ModToml mod_toml = new ModToml(this.mod_infos, this.folder);
                mod_toml.generateFile();
            }

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
                textBox.Text = textBox.Text.RemoveSpecialCharacters();

                this.mod_infos[textBox.Tag.ToString()] = textBox.Text;
                Image img = (Image)this.FindName(textBox.Tag + "_image");

                if (img != null)
                {
                    if (textBox.Text.isTextValid())
                    {
                        img.Source = new BitmapImage(new Uri("/Forge Modding Helper 3;component/Resources/Pictures/check.png", UriKind.Relative));
                    }
                    else
                    {
                        img.Source = new BitmapImage(new Uri("/Forge Modding Helper 3;component/Resources/Pictures/uncheck.png", UriKind.Relative));
                    }
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

        #region ComboBox Forge Version
        /// <summary>
        /// Function called when the selected minecraft version change
        /// </summary>
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Update UI component
            BlurEffect effect = new BlurEffect();
            effect.Radius = 20;
            forge_versions_grid.Effect = effect;
            loading_label.Visibility = Visibility.Visible;
            loading_progressbar.Visibility = Visibility.Visible;
            mod_infos["minecraft_version"] = (string) ((ComboBoxItem)forge_version_comboBox.SelectedItem).Content;

            // Starting forge version checker
            background_thread.RunWorkerAsync();
        }
        #endregion

        #region Radiobutton Forge version
        /// <summary>
        /// Function called when the user change the forge version selection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void forge_version_radiobutton_Checked(object sender, RoutedEventArgs e)
        {
            if(latest_radiobutton.IsChecked == true)
            {
                this.mod_infos["forge_version"] = versions.getLatestForgeVersion(this.mod_infos["minecraft_version"]);
            }
            else
            {
                this.mod_infos["forge_version"] = versions.getRecommendedForgeVersion(this.mod_infos["minecraft_version"]);

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
            if(!String.IsNullOrWhiteSpace(fileDialog.FileName))
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
            new ProjectScanWindow(this.folder).Show();
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
            }
        }
        #endregion

        
    }
}
