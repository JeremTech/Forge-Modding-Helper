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

namespace Forge_Modding_Helper_3
{
    public partial class AssistantCreator : Window
    {
        private double total_step = 5.0;
        private double step = 0.0;
        private string folder = "";
        private string versions_url = "https://files.minecraftforge.net/maven/net/minecraftforge/forge/promotions_slim.json";
        private readonly BackgroundWorker background_thread = new BackgroundWorker();
        private ForgeVersionsUtils versions = new ForgeVersionsUtils();
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
            updateStep(0.0);
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

        public void updateStep(double stepIn)
        {
            step_progressbar.Value = (stepIn / this.total_step) * 100;
            step_label.Content = "Etape " + stepIn + " sur " + this.total_step;
        }

        #region Background Worker
        private void Background_thread_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            forge_versions_grid.ClearValue(EffectProperty);
            loading_label.Visibility = Visibility.Hidden;
            loading_progressbar.Visibility = Visibility.Hidden;

            if (!string.IsNullOrWhiteSpace(versions.getLatestForgeVersion(mod_infos["minecraft_version"])))
            {
                this.latest_forge_version_label.Content = versions.getLatestForgeVersion(mod_infos["minecraft_version"]);
                this.latest_forge_version_grid.Visibility = Visibility.Visible;
            }
            else
            {
                this.latest_forge_version_grid.Visibility = Visibility.Hidden;
            }

            if (!string.IsNullOrWhiteSpace(versions.getRecommendedForgeVersion(mod_infos["minecraft_version"])))
            {
                this.recommended_forge_version_label.Content = versions.getRecommendedForgeVersion(mod_infos["minecraft_version"]);
                this.recommended_forge_version_grid.Visibility = Visibility.Visible;
            }
            else
            {
                this.recommended_forge_version_grid.Visibility = Visibility.Hidden;
            }

            this.mod_infos["forge_version"] = versions.getLatestForgeVersion(mod_infos["minecraft_version"]);
        }

        private void Background_thread_DoWork(object sender, DoWorkEventArgs e)
        {
            string content = "";
            using (WebClient client = new WebClient())
            {
                content = client.DownloadString(versions_url);
            }

            versions = JsonConvert.DeserializeObject<ForgeVersionsUtils>(content);
        }
        #endregion

        #region Cancel button
        private void Cancel_button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        #endregion

        #region Next Button
        private void Next_button_Click(object sender, RoutedEventArgs e)
        {
            switch (this.step)
            {
                case 0:
                    {
                        if (!string.IsNullOrWhiteSpace(this.folder))
                        {
                            first_grid.Visibility = Visibility.Hidden;
                            missing_infos_label.Visibility = Visibility.Hidden;
                            second_grid.Visibility = Visibility.Visible;
                            previous_button.IsEnabled = true;
                            this.step++;
                            this.updateStep(this.step);
                        }
                        else
                        {
                            missing_infos_label.Visibility = Visibility.Visible;
                        }
                        break;
                    }
                case 1:
                    {
                        if (!String.IsNullOrEmpty(this.mod_infos["mod_name"]) && !String.IsNullOrEmpty(this.mod_infos["mod_authors"]) && !String.IsNullOrEmpty(this.mod_infos["mod_version"]))
                        {
                            missing_infos_label.Visibility = Visibility.Hidden;
                            second_grid.Visibility = Visibility.Hidden;
                            third_grid.Visibility = Visibility.Visible;
                            missing_infos_label.Visibility = Visibility.Hidden;
                            this.step++;
                            this.updateStep(this.step);
                        }
                        else
                        {
                            missing_infos_label.Visibility = Visibility.Visible;
                        }
                        break;
                    }
                case 2:
                    {
                        if (!String.IsNullOrWhiteSpace(this.mod_infos["mod_id"]) && !String.IsNullOrWhiteSpace(this.mod_infos["mod_group"]) && !String.IsNullOrWhiteSpace(this.mod_infos["minecraft_version"]) && !String.IsNullOrWhiteSpace(this.mod_infos["forge_version"]))
                        {
                            missing_infos_label.Visibility = Visibility.Hidden;
                            third_grid.Visibility = Visibility.Hidden;
                            fourth_grid.Visibility = Visibility.Visible;
                            this.step++;
                            this.updateStep(this.step);
                        }
                        else
                        {
                            missing_infos_label.Visibility = Visibility.Visible;
                        }
                        break;
                    }
                case 3:
                    {
                        fourth_grid.Visibility = Visibility.Hidden;
                        fith_grid.Visibility = Visibility.Visible;
                        this.step++;
                        this.updateStep(this.step);
                        missing_infos_label.Visibility = Visibility.Hidden;
                        break;
                    }
                case 4:
                    {
                        // Saving in recent workspaces
                        RecentWorkspaces.RecentWorkspacesList.Add(new Workspace(this.mod_infos["mod_name"], this.mod_infos["minecraft_version"], this.folder, this.mod_infos["mod_description"], DateTime.Now));
                        RecentWorkspaces.WriteDataFile();

                        // UI components
                        fith_grid.Visibility = Visibility.Hidden;
                        generation_grid.Visibility = Visibility.Visible;
                        this.step++;
                        this.updateStep(this.step);
                        missing_infos_label.Visibility = Visibility.Hidden;
                        next_button.IsEnabled = false;
                        previous_button.IsEnabled = false;
                        step_label.Visibility = Visibility.Hidden;

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
                        // UI components
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
        private void Previous_button_Click(object sender, RoutedEventArgs e)
        {
            switch (step)
            {
                case 1:
                    previous_button.IsEnabled = false;
                    first_grid.Visibility = Visibility.Visible;
                    second_grid.Visibility = Visibility.Hidden;
                    missing_infos_label.Visibility = Visibility.Hidden;
                    this.step--;
                    this.updateStep(this.step);
                    break;
                case 2:
                    second_grid.Visibility = Visibility.Visible;
                    third_grid.Visibility = Visibility.Hidden;
                    missing_infos_label.Visibility = Visibility.Hidden;
                    this.step--;
                    this.updateStep(this.step);
                    break;
                case 3:
                    third_grid.Visibility = Visibility.Visible;
                    fourth_grid.Visibility = Visibility.Hidden;
                    missing_infos_label.Visibility = Visibility.Hidden;
                    this.step--;
                    this.updateStep(this.step);
                    break;
                case 4:
                    fourth_grid.Visibility = Visibility.Visible;
                    fith_grid.Visibility = Visibility.Hidden;
                    missing_infos_label.Visibility = Visibility.Hidden;
                    this.step--;
                    this.updateStep(this.step);
                    break;
            }
        }
        #endregion

        #region Generation
        private void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            double bytesIn = double.Parse(e.BytesReceived.ToString());
            double totalBytes = double.Parse(e.TotalBytesToReceive.ToString());
            double percentage = bytesIn / totalBytes * 100;

            progress_progressbar.Value = int.Parse(Math.Truncate(percentage).ToString());
        }

        private void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            update_progress(0, "Extraction de l'archive...");
            ZipFile.ExtractToDirectory(this.folder + @"\mdk.zip", this.folder);

            if (File.Exists(this.folder + @"\mdk.zip"))
            {
                update_progress(0, "Supression de l'archive...");
                File.Delete(this.folder + @"\mdk.zip");
            }

            generate_folders();
            generate_files();

            update_progress(100, "Création de l'espace de travail terminé.");
            this.next_button.IsEnabled = true;
        }

        private void generate_folders()
        {
            if(code_packages_checkBox.IsChecked == true)
            {
                update_progress(0, "Supression des packages exemple dans \"" + this.folder + @"\src\main\java" + "\"...");
                
                File.Delete(this.folder + @"\src\main\java\com\example\examplemod\ExampleMod.java");
                Directory.Delete(this.folder + @"\src\main\java\com\example\examplemod");
                Directory.Delete(this.folder + @"\src\main\java\com\example");
                Directory.Delete(this.folder + @"\src\main\java\com");

                update_progress(0, "Création des packages de code dans \"" + this.folder + @"\src\main\java" + "\"...");
                Directory.CreateDirectory(this.folder + @"\src\main\java\" + this.mod_infos["mod_group"].Replace(".", @"\"));
            }

            if(assets_packages_checkBox.IsChecked == true)
            {
                update_progress(0, "Création des dossiers de textures dans \"" + this.folder + @"\src\main\assets" + "\"...");
                Directory.CreateDirectory(this.folder + @"\src\main\resources\assets\" + this.mod_infos["mod_id"] + @"\textures\block");
                Directory.CreateDirectory(this.folder + @"\src\main\resources\assets\" + this.mod_infos["mod_id"] + @"\textures\item");
                Directory.CreateDirectory(this.folder + @"\src\main\resources\assets\" + this.mod_infos["mod_id"] + @"\textures\gui");
                Directory.CreateDirectory(this.folder + @"\src\main\resources\assets\" + this.mod_infos["mod_id"] + @"\textures\entity");
                Directory.CreateDirectory(this.folder + @"\src\main\resources\assets\" + this.mod_infos["mod_id"] + @"\textures\models");

                update_progress(0, "Création des dossiers de modèles dans \"" + this.folder + @"\src\main\assets" + "\"...");
                Directory.CreateDirectory(this.folder + @"\src\main\resources\assets\" + this.mod_infos["mod_id"] + @"\models\block");
                Directory.CreateDirectory(this.folder + @"\src\main\resources\assets\" + this.mod_infos["mod_id"] + @"\models\item");

                update_progress(0, "Création du dossier de blockstates dans \"" + this.folder + @"\src\main\assets" + "\"...");
                Directory.CreateDirectory(this.folder + @"\src\main\resources\assets\" + this.mod_infos["mod_id"] + @"\blockstates");

                update_progress(0, "Création du dossier de fichiers de traduction dans \"" + this.folder + @"\src\main\assets" + "\"...");
                Directory.CreateDirectory(this.folder + @"\src\main\resources\assets\" + this.mod_infos["mod_id"] + @"\lang");
            }
        }

        private void generate_files()
        {
            if(fr_lang_file_checkBox.IsChecked == true)
            {
                if(!Directory.Exists(this.folder + @"\src\main\resources\assets\" + this.mod_infos["mod_id"] + @"\lang"))
                {
                    update_progress(0, "Création du dossier de fichiers de traduction dans \"" + this.folder + @"\src\main\assets" + "\"...");
                    Directory.CreateDirectory(this.folder + @"\src\main\resources\assets\" + this.mod_infos["mod_id"] + @"\lang");
                }

                update_progress(0, "Création du dossier du fichier de traduction \"fr_fr.json\"...");
                File.WriteAllText(this.folder + @"\src\main\resources\assets\" + this.mod_infos["mod_id"] + @"\lang\fr_fr.json", "{" + Environment.NewLine + Environment.NewLine + "}");
            }

            if(build_gradle_checkBox.IsChecked == true)
            {
                update_progress(0, "Configuration du fichier \"build.gradle\" dans \"" + this.folder + "\"...");
                BuildGradle build_gradle = new BuildGradle(this.mod_infos, this.folder);
                build_gradle.generateFile();
            }

            if(mod_toml_checkBox.IsChecked == true)
            {
                update_progress(0, "Configuration du fichier \"mods.toml\" dans \"" + this.folder + @"\src\main\resources\META-INF\""...");
                ModToml mod_toml = new ModToml(this.mod_infos, this.folder);
                mod_toml.generateFile();
            }

            if(!string.IsNullOrEmpty(this.mod_infos["mod_logo"]))
            {
                update_progress(0, "Copie du logo du mod...");

                if (File.Exists(this.mod_infos["mod_logo"]))
                {
                    File.Copy(this.mod_infos["mod_logo"], this.folder + @"\src\main\resources\logo.png");
                }
                else
                {
                    update_progress(0, "ERREUR : Le logo du mod n'a pas pu être copié : \"Fichier inaccessible ou supprimé.\"");
                }
            }

        }

        private void update_progress(int progress, string statut)
        {
            progress_progressbar.Value = progress;
            progress_label.Content = statut;
            progress_listBox.Items.Add(statut);
        }
        #endregion

        #region Messages
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

            textBox.Text = textBox.Text.deleteStartEndSpaces();
        }
        #endregion

        #region ComboBox Forge Version
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BlurEffect effect = new BlurEffect();
            effect.Radius = 20;
            forge_versions_grid.Effect = effect;
            loading_label.Visibility = Visibility.Visible;
            loading_progressbar.Visibility = Visibility.Visible;
            mod_infos["minecraft_version"] = (string) ((ComboBoxItem)forge_version_comboBox.SelectedItem).Content;
            Console.WriteLine(mod_infos["minecraft_version"]);
            background_thread.RunWorkerAsync();
        }
        #endregion

        #region Radiobutton Forge version
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
        private void Browse_logo_button_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog fileDialog = new Microsoft.Win32.OpenFileDialog();
            fileDialog.RestoreDirectory = true;
            fileDialog.Title = "Choisissez un logo pour votre mod";
            fileDialog.DefaultExt = "png";
            fileDialog.Filter = "png files (*.png)|*.png";
            fileDialog.CheckFileExists = true;
            fileDialog.CheckPathExists = true;
            fileDialog.ShowDialog();

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

            this.mod_infos["mod_logo"] = this.logo_path_textbox.Text;
        }
        #endregion

        #region Browse directory button
        private void button_browse_directory_Click(object sender, RoutedEventArgs e)
        {
            // Allow user to select workspace output directory
            using (var fbd = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = fbd.ShowDialog();

                if (!string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    this.textbox_directory.Text = fbd.SelectedPath;

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
                else if(string.IsNullOrWhiteSpace(fbd.SelectedPath) && string.IsNullOrWhiteSpace(this.folder))
                {
                    this.label_invalid_workspace_folder.Visibility = Visibility.Hidden;
                    this.folder = "";
                    this.textbox_directory.Text = "";
                }
            }
        }
        #endregion

        #region Finish button
        private void finish_button_Click(object sender, RoutedEventArgs e)
        {
            new WorkspaceManager().Show();
            this.Close();
        }
        #endregion

        #region Prevent Window closing
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (step < total_step)
            {
                MessageBoxResult action = MessageBox.Show(this, "Souhaitez-vous vraiment annuler la création de l'espace de travail ?", "Forge Modding Helper", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (action == MessageBoxResult.No)
                {
                    e.Cancel = true;
                }
            }
        }
        #endregion

    }
}
