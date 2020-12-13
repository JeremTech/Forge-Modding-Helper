using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Forge_Modding_Helper_3.Files;
using Forge_Modding_Helper_3.Utils;
using Forge_Modding_Helper_3.Windows;
using Microsoft.VisualBasic.FileIO;
using Newtonsoft.Json;
using Image = System.Drawing.Image;
using Path = System.Windows.Shapes.Path;

namespace Forge_Modding_Helper_3
{
    /// <summary>
    /// Logique d'interaction pour WorkspaceManager.xaml
    /// </summary>
    public partial class WorkspaceManager : Window
    {
        // Project path
        private string path = "";

        // Mod infos storage
        private Dictionary<string, string> modInfos = new Dictionary<string, string>
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
            {"forge_version", ""},
            {"mappings_version", ""}
        };

        // Blockstates list storage
        private List<BlockStates> blockstatesList = new List<BlockStates>();

        // Textures list storage
        private List<Texture> texturesList = new List<Texture>();

        // Models list storage
        private List<Model> modelsList = new List<Model>();

        // java file list storage
        private List<String> javaFileList = new List<string>();

        public WorkspaceManager(string path)
        {
            this.path = path;
            InitializeComponent();

            UITextTranslator.LoadTranslationFile(OptionsFile.getCurrentLanguage());
            UITextTranslator.UpdateComponentsTranslations(this.main_grid);
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            // Read mod_infos.json file
            string jsonContent = File.ReadAllText(System.IO.Path.Combine(path, "fmh", "mod_infos.json"));
            modInfos = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonContent);

            // Read blockstates_list.json file
            jsonContent = File.ReadAllText(System.IO.Path.Combine(path, "fmh", "blockstates_list.json"));
            JsonConvert.DeserializeObject<List<String>>(jsonContent).ForEach(element => blockstatesList.Add(new BlockStates(element)));

            // Read textures_list.json file
            jsonContent = File.ReadAllText(System.IO.Path.Combine(path, "fmh", "textures_list.json"));
            JsonConvert.DeserializeObject<List<String>>(jsonContent).ForEach(element => texturesList.Add(new Texture(element)));

            // Read models_list.json file
            jsonContent = File.ReadAllText(System.IO.Path.Combine(path, "fmh", "models_list.json"));
            JsonConvert.DeserializeObject<List<string>>(jsonContent).ForEach(element => modelsList.Add(new Model(element)));

            // Read java_files_list.json file
            jsonContent = File.ReadAllText(System.IO.Path.Combine(path, "fmh", "java_files_list.json"));
            javaFileList = JsonConvert.DeserializeObject<List<string>>(jsonContent);

            updateUI();
        }

        private void updateUI()
        {
            this.label_mod_name.Content = modInfos["mod_name"];
            this.label_home_minecraft_version.Content = modInfos["minecraft_version"];
            this.label_minecraft_version.Content = "Minecraft " + modInfos["minecraft_version"];
            this.label_home_forge_version.Content = modInfos["forge_version"];
            this.label_forge_version.Content = "Forge " + modInfos["forge_version"];
            this.label_home_mappings_version.Content = modInfos["mappings_version"];
            this.label_home_textures_number.Content = texturesList.Count;
            this.label_home_models_number.Content = modelsList.Count;
            this.label_home_javafiles_number.Content = javaFileList.Count;

            string[] desc = modInfos["mod_description"].Split('\r');
            if (desc.Count() != 0) this.label_mod_description.Content = desc[0];
            else this.label_mod_description.Content = modInfos["mod_description"];
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult msgResult = MessageBox.Show(UITextTranslator.getTranslation("workspace_manager.alert.close"), "Forge Modding Helper", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (msgResult == MessageBoxResult.Yes)
            {
                WelcomeWindow welcomeWindow = new WelcomeWindow();

                // Update welcome UI depending on the presence of recent projects or not
                if (RecentWorkspaces.ReadDataFile())
                {
                    welcomeWindow.label_no_workspace_found.Visibility = Visibility.Hidden;
                    welcomeWindow.listbox_recent_workspaces.ItemsSource = RecentWorkspaces.RecentWorkspacesList;
                }
                else
                {
                    welcomeWindow.label_no_workspace_found.Visibility = Visibility.Visible;
                }

                // Windows management
                welcomeWindow.Show();
            }
            else
            {
                e.Cancel = true;
            }
            
        }

        private void button_menu_click(object sender, RoutedEventArgs e)
        {
            if (sender is Button senderButton)
            {
                // Update menu border
                this.home_button_border.Background = null;
                this.mod_toml_button_border.Background = null;
                this.blockstates_button_border.Background = null;
                this.models_button_border.Background = null;
                this.textures_button_border.Background = null;
                this.lang_button_border.Background = null;
                this.build_button_border.Background = null;

                // Update grids
                this.home_grid.Visibility = Visibility.Hidden;
                this.mod_settings_grid.Visibility = Visibility.Hidden;
                this.blockstates_grid.Visibility = Visibility.Hidden;
                this.models_grid.Visibility = Visibility.Hidden;
                this.textures_grid.Visibility = Visibility.Hidden;
                this.language_grid.Visibility = Visibility.Hidden;
                this.exportation_grid.Visibility = Visibility.Hidden;

                String tag = senderButton.Tag.ToString();

                if (tag.Contains("home"))
                {
                    this.home_button_border.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 116, 255));
                    this.home_grid.Visibility = Visibility.Visible;
                }
                else if (tag.Contains("mod_settings"))
                {
                    this.mod_toml_button_border.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 116, 255));
                    this.mod_settings_grid.Visibility = Visibility.Visible;
                    updateModSettingsSection();
                }
                else if (tag.Contains("blockstates"))
                {
                    this.blockstates_button_border.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 116, 255));
                    this.blockstates_grid.Visibility = Visibility.Visible;
                    listView_blockstates.ItemsSource = blockstatesList;
                }
                else if (tag.Contains("models"))
                {
                    this.models_button_border.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 116, 255));
                    this.models_grid.Visibility = Visibility.Visible;
                    listView_models.ItemsSource = modelsList;

                    List<string> models_folders = new List<string>();
                    models_folders.Add("--");
                    foreach (string modelsFolder in Directory.GetDirectories(System.IO.Path.Combine(path, "src\\main\\resources\\assets", modInfos["mod_id"], "models")))
                    {
                        models_folders.Add(System.IO.Path.GetFileName(modelsFolder));
                    }
                    models_folder_comboBox.ItemsSource = models_folders;
                    models_folder_comboBox.SelectedIndex = 0;
                }
                else if (tag.Contains("textures"))
                {
                    this.textures_button_border.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 116, 255));
                    this.textures_grid.Visibility = Visibility.Visible;
                    listView_textures.ItemsSource = texturesList;

                    List<string> textures_folders = new List<string>();
                    textures_folders.Add("--");
                    foreach (string texturesFolder in Directory.GetDirectories(System.IO.Path.Combine(path, "src\\main\\resources\\assets", modInfos["mod_id"], "textures")))
                    {
                        textures_folders.Add(System.IO.Path.GetFileName(texturesFolder));
                    }
                    textures_folder_comboBox.ItemsSource = textures_folders;
                    textures_folder_comboBox.SelectedIndex = 0;
                }
                else if (tag.Contains("translations"))
                {
                    this.lang_button_border.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 116, 255));
                    this.language_grid.Visibility = Visibility.Visible;
                    RefreshTranslationFilesList();
                }
                else if (tag.Contains("export"))
                {
                    this.build_button_border.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 116, 255));
                    this.exportation_grid.Visibility = Visibility.Visible;
                    updateModExportSection();
                }
            }
        }

        #region Mod settings section controls events
        // Temporary variable for logo path
        string new_logo_path = "";

        public void updateModSettingsSection() 
        {
            this.mod_settings_name_textbox.Text = modInfos["mod_name"];
            this.mod_settings_authors_textbox.Text = modInfos["mod_authors"];
            this.mod_settings_version_textbox.Text = modInfos["mod_version"];
            this.mod_settings_modid_textbox.Text = modInfos["mod_id"];
            this.mod_settings_group_textbox.Text = modInfos["mod_group"];
            this.mod_settings_credits_textbox.Text = modInfos["mod_credits"];
            this.mod_settings_website_textbox.Text = modInfos["display_url"];
            this.mod_settings_bug_tracker_textbox.Text = modInfos["issue_tracker"];
            this.mod_settings_update_json_textbox.Text = modInfos["update_json"];
            this.mod_settings_description_textbox.Text = modInfos["mod_description"];
            this.mod_settings_minecraft_version_textbox.Text = modInfos["minecraft_version"];
            this.mod_settings_forge_version_textbox.Text = modInfos["forge_version"];
            this.mod_settings_mappings_version_textbox.Text = modInfos["mappings_version"];
            this.mod_settings_license_textbox.Text = modInfos["mod_license"];

            if (File.Exists(System.IO.Path.Combine(path, @"src\main\resources\logo.png")))
            {
                using (var stream = File.OpenRead(System.IO.Path.Combine(path, @"src\main\resources\logo.png")))
                {
                    var image = new BitmapImage();
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.StreamSource = stream;
                    image.EndInit();
                    this.mod_logo_image.Source = image;
                }
            }
            else
            {
                this.mod_logo_image.Source = null;
            }
        }

        private void update_mod_settings_button_Click(object sender, RoutedEventArgs e)
        {
            modInfos["mod_name"] = this.mod_settings_name_textbox.Text;
            modInfos["mod_authors"] = this.mod_settings_authors_textbox.Text;
            modInfos["mod_version"] = this.mod_settings_version_textbox.Text;
            modInfos["mod_id"] = this.mod_settings_modid_textbox.Text;
            modInfos["mod_group"] = this.mod_settings_group_textbox.Text;
            modInfos["mod_credits"] = this.mod_settings_credits_textbox.Text;
            modInfos["display_url"] = this.mod_settings_website_textbox.Text;
            modInfos["issue_tracker"] = this.mod_settings_bug_tracker_textbox.Text;
            modInfos["update_json"] = this.mod_settings_update_json_textbox.Text;
            modInfos["mod_description"] = this.mod_settings_description_textbox.Text;
            modInfos["minecraft_version"] = this.mod_settings_minecraft_version_textbox.Text;
            modInfos["forge_version"] = this.mod_settings_forge_version_textbox.Text;
            modInfos["mappings_version"] = this.mod_settings_mappings_version_textbox.Text;
            modInfos["mod_license"] = this.mod_settings_license_textbox.Text;

            if (!string.IsNullOrWhiteSpace(new_logo_path))
            {
                if (File.Exists(new_logo_path))
                {
                    if (File.Exists(System.IO.Path.Combine(path, @"src\main\resources\logo.png"))) File.Delete(System.IO.Path.Combine(path, @"src\main\resources\logo.png"));
                    File.Copy(new_logo_path, this.path + @"\src\main\resources\logo.png");
                    this.modInfos["mod_logo"] = new_logo_path;
                }
            }

            new ModToml(this.modInfos, this.path).generateFile();
            new BuildGradle(this.modInfos, this.path).generateFile();
            updateUI();
        }

        private void browse_mod_logo_Click(object sender, RoutedEventArgs e)
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
                using (var stream = File.OpenRead(fileDialog.FileName))
                {
                    var image = new BitmapImage();
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.StreamSource = stream;
                    image.EndInit();
                    this.mod_logo_image.Source = image;
                }

                new_logo_path = fileDialog.FileName;
            }
            else if (File.Exists(System.IO.Path.Combine(path, @"src\main\resources\logo.png")))
            {
                using (var stream = File.OpenRead(System.IO.Path.Combine(path, @"src\main\resources\logo.png")))
                {
                    var image = new BitmapImage();
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.StreamSource = stream;
                    image.EndInit();
                    this.mod_logo_image.Source = image;
                }

                new_logo_path = null;
            }
            else
            {
                this.mod_logo_image.Source = null;
                new_logo_path = null;
            }
        }
        #endregion

        #region Blockstates section controls events

        private void listView_blockstates_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            blockstates_delete_button.IsEnabled = false;
            blockstates_rename_button.IsEnabled = false;

            if (listView_blockstates.SelectedItems.Count > 0)
            {
                blockstates_delete_button.IsEnabled = true;

                if (listView_blockstates.SelectedItems.Count == 1)
                {
                    blockstates_rename_button.IsEnabled = true;
                }
            }
        }

        private void blockstates_search_textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(blockstates_search_textBox.Text))
                listView_blockstates.ItemsSource = blockstatesList.Where(item => item.FileName.Contains(blockstates_search_textBox.Text));
            else
                listView_blockstates.ItemsSource = blockstatesList;
        }

        private void blockstates_delete_button_Click(object sender, RoutedEventArgs e)
        {
            if (listView_blockstates.SelectedItems.Count > 0)
            {
                MessageBoxResult msgResult = MessageBox.Show(UITextTranslator.getTranslation("workspace_manager.alert.delete").Replace("%N", listView_blockstates.SelectedItems.Count.ToString()), "Forge Modding Helper", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (msgResult == MessageBoxResult.Yes)
                {
                    foreach (BlockStates element in listView_blockstates.SelectedItems)
                    {
                        // Move the file to the recycle bin
                        FileSystem.DeleteFile(element.FilePath, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin, UICancelOption.DoNothing);
                    }
                }

                RefreshBlockStatesList();
            }
        }

        private void blockstates_rename_button_Click(object sender, RoutedEventArgs e)
        {
            if (listView_blockstates.SelectedItems.Count == 1)
            {
                new RenameDialog(((BlockStates) listView_blockstates.SelectedItem).FilePath).ShowDialog();
                RefreshBlockStatesList();
            }
        }

        private void blockstates_reload_button_Click(object sender, RoutedEventArgs e)
        {
            RefreshBlockStatesList();
        }
        #endregion

        #region Models section controls events
        private void models_folder_comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            listView_models.ItemsSource = null;

            if (!string.IsNullOrWhiteSpace(models_search_textBox.Text))
            {
                if (models_folder_comboBox.SelectedIndex == 0)
                    listView_models.ItemsSource = modelsList.Where(item => item.FileName.Contains(models_search_textBox.Text) && item.FileName.Contains(models_search_textBox.Text));
                else
                    listView_models.ItemsSource = modelsList.Where(item => item.FileName.Contains(models_search_textBox.Text) && System.IO.Path.GetFileName(System.IO.Path.GetDirectoryName(item.FilePath)).Contains((string)models_folder_comboBox.SelectedItem));
            }
            else
            {
                if (models_folder_comboBox.SelectedIndex == 0)
                    listView_models.ItemsSource = modelsList;
                else
                    listView_models.ItemsSource = modelsList.Where(item => System.IO.Path.GetFileName(System.IO.Path.GetDirectoryName(item.FilePath)).Contains((string)models_folder_comboBox.SelectedItem));
            }
        }

        private void models_search_textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(models_search_textBox.Text))
            {
                if (models_folder_comboBox.SelectedIndex != 0)
                    listView_models.ItemsSource = modelsList.Where(item => item.FileName.Contains(models_search_textBox.Text) && System.IO.Path.GetFileName(System.IO.Path.GetDirectoryName(item.FilePath)).Contains((string)models_folder_comboBox.SelectedItem));
                else
                    listView_models.ItemsSource = modelsList.Where(item => item.FileName.Contains(models_search_textBox.Text));
            }
            else
            {
                if (models_folder_comboBox.SelectedIndex != 0)
                    listView_models.ItemsSource = modelsList.Where(item => System.IO.Path.GetFileName(System.IO.Path.GetDirectoryName(item.FilePath)).Contains((string)models_folder_comboBox.SelectedItem));
                else
                    listView_models.ItemsSource = modelsList;
            }
        }

        private void listView_models_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            models_delete_button.IsEnabled = false;
            models_rename_button.IsEnabled = false;

            if (listView_models.SelectedItems.Count > 0)
            {
                models_delete_button.IsEnabled = true;

                if (listView_models.SelectedItems.Count == 1)
                {
                    models_rename_button.IsEnabled = true;
                }
            }
        }

        private void models_rename_button_Click(object sender, RoutedEventArgs e)
        {
            if (listView_models.SelectedItems.Count == 1)
            {
                new RenameDialog(((Model)listView_models.SelectedItem).FilePath).ShowDialog();
                RefreshModelsList();
            }
        }

        private void models_delete_button_Click(object sender, RoutedEventArgs e)
        {
            if (listView_models.SelectedItems.Count > 0)
            {
                MessageBoxResult msgResult = MessageBox.Show(UITextTranslator.getTranslation("workspace_manager.alert.delete").Replace("%N", listView_models.SelectedItems.Count.ToString()), "Forge Modding Helper", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (msgResult == MessageBoxResult.Yes)
                {
                    foreach (Model element in listView_models.SelectedItems)
                    {
                        // Move the file to the recycle bin
                        FileSystem.DeleteFile(element.FilePath, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin, UICancelOption.DoNothing);
                    }
                }

                RefreshModelsList();
            }
        }

        private void models_reload_button_Click(object sender, RoutedEventArgs e)
        {
            RefreshModelsList();
        }
        #endregion

        #region Textures section controls events
        private void textures_folder_comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            listView_textures.ItemsSource = null;

            if (!string.IsNullOrWhiteSpace(textures_search_textBox.Text))
            {
                if (textures_folder_comboBox.SelectedIndex == 0)
                    listView_textures.ItemsSource = texturesList.Where(item => item.FileName.Contains(textures_search_textBox.Text));
                else
                    listView_textures.ItemsSource = texturesList.Where(item => item.FileName.Contains(textures_search_textBox.Text) && System.IO.Path.GetFileName(System.IO.Path.GetDirectoryName(item.FilePath)).Contains((string)textures_folder_comboBox.SelectedItem));
            }
            else
            {
                if (textures_folder_comboBox.SelectedIndex == 0)
                    listView_textures.ItemsSource = texturesList;
                else
                    listView_textures.ItemsSource = texturesList.Where(item => System.IO.Path.GetFileName(System.IO.Path.GetDirectoryName(item.FilePath)).Contains((string)textures_folder_comboBox.SelectedItem));
            }
        }

        private void textures_search_textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(textures_search_textBox.Text))
            {
                if (textures_folder_comboBox.SelectedIndex != 0)
                    listView_textures.ItemsSource = texturesList.Where(item => item.FileName.Contains(textures_search_textBox.Text) && System.IO.Path.GetFileName(System.IO.Path.GetDirectoryName(item.FilePath)).Contains((string)textures_folder_comboBox.SelectedItem));
                else
                    listView_textures.ItemsSource = texturesList.Where(item => item.FileName.Contains(textures_search_textBox.Text));
            }
            else
            {
                if (textures_folder_comboBox.SelectedIndex != 0)
                    listView_textures.ItemsSource = texturesList.Where(item => System.IO.Path.GetFileName(System.IO.Path.GetDirectoryName(item.FilePath)).Contains((string)textures_folder_comboBox.SelectedItem));
                else
                    listView_textures.ItemsSource = texturesList;
            }
        }

        private void listView_textures_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            textures_delete_button.IsEnabled = false;
            textures_rename_button.IsEnabled = false;

            if (listView_textures.SelectedItems.Count > 0)
            {
                textures_delete_button.IsEnabled = true;

                if (listView_textures.SelectedItems.Count == 1)
                {
                    textures_rename_button.IsEnabled = true;
                }
            }
        }

        private void textures_rename_button_Click(object sender, RoutedEventArgs e)
        {
            if (listView_textures.SelectedItems.Count == 1)
            {
                new RenameDialog(((Texture)listView_textures.SelectedItem).FilePath).ShowDialog();
                RefreshTexturesList();
            }
        }

        private void textures_delete_button_Click(object sender, RoutedEventArgs e)
        {
            if (listView_textures.SelectedItems.Count > 0)
            {
                MessageBoxResult msgResult = MessageBox.Show(UITextTranslator.getTranslation("workspace_manager.alert.delete").Replace("%N", listView_textures.SelectedItems.Count.ToString()), "Forge Modding Helper", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (msgResult == MessageBoxResult.Yes)
                {
                    foreach (Texture element in listView_textures.SelectedItems)
                    {
                        // Move the file to the recycle bin
                        FileSystem.DeleteFile(element.FilePath, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin, UICancelOption.DoNothing);
                    }
                }

                RefreshTexturesList();
            }
        }

        private void textures_reload_button_Click(object sender, RoutedEventArgs e)
        {
            RefreshTexturesList();
        }
        #endregion

        #region Translation files section controls events
        private void language_files_listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (language_files_listBox.SelectedItem != null && File.Exists(System.IO.Path.Combine(path, "src\\main\\resources\\assets", modInfos["mod_id"], "lang", language_files_listBox.SelectedItem.ToString())))
            {
                language_textEditor.Text = File.ReadAllText(System.IO.Path.Combine(path, "src\\main\\resources\\assets", modInfos["mod_id"], "lang", language_files_listBox.SelectedItem.ToString()));
                translation_delete_file_button.IsEnabled = true;
            }
            else
            {
                language_textEditor.Text = "";
                translation_delete_file_button.IsEnabled = false;
            }
        }

        private void language_textEditor_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (language_files_listBox.SelectedItem != null)
            {
                File.WriteAllText(System.IO.Path.Combine(path, "src\\main\\resources\\assets", modInfos["mod_id"], "lang", language_files_listBox.SelectedItem.ToString()), language_textEditor.Text);
            }
        }

        private void translation_delete_file_button_Click(object sender, RoutedEventArgs e)
        {
            string filePath = System.IO.Path.Combine(path, "src\\main\\resources\\assets", modInfos["mod_id"], "lang", language_files_listBox.SelectedItem.ToString());

            if (language_files_listBox.SelectedItem != null && File.Exists(filePath))
            {
                MessageBoxResult res = MessageBox.Show(UITextTranslator.getTranslation("workspace_manager.lang.delete"), "Forge Modding Helper", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (res == MessageBoxResult.Yes)
                {
                    File.Delete(filePath);
                    RefreshTranslationFilesList();
                }
            }
        }

        private void translation_add_file_button_Click(object sender, RoutedEventArgs e)
        {
            new AddTranslationFileDialog(System.IO.Path.Combine(path, "src\\main\\resources\\assets", modInfos["mod_id"], "lang")).ShowDialog();
            RefreshTranslationFilesList();
        }
        #endregion

        #region Mod export section controls events
        public void updateModExportSection()
        {
            this.mod_export_name_label.Content = modInfos["mod_name"];
            this.mod_export_authors_label.Content = modInfos["mod_authors"];
            this.mod_export_version_label.Content = modInfos["mod_version"];
            this.mod_export_minecraft_version_label.Content = modInfos["minecraft_version"];
            this.mod_export_forge_version_label.Content = modInfos["forge_version"];

            if (File.Exists(System.IO.Path.Combine(path, @"src\main\resources\logo.png")))
            {
                using (var stream = File.OpenRead(System.IO.Path.Combine(path, @"src\main\resources\logo.png")))
                {
                    var image = new BitmapImage();
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.StreamSource = stream;
                    image.EndInit();
                    this.mod_export_logo_image.Source = image;
                }
            }
            else
            {
                this.mod_export_logo_image.Source = null;
            }
        }

        private void mod_export_button_Click(object sender, RoutedEventArgs e)
        {
            this.mod_export_console.FontSize = 10;
            this.mod_export_console.StartProcess("cmd.exe", string.Empty);
            this.mod_export_console.WriteInput("cd " + this.path, System.Windows.Media.Color.FromRgb(255, 240, 0), false);
            this.mod_export_console.WriteInput("gradlew build", System.Windows.Media.Color.FromRgb(255, 240, 0), true);
        }
        #endregion

        #region Integrated Project Scan
        /// <summary>
        /// Refresh blockstates section by reload blockstates files list
        /// </summary>
        private void RefreshBlockStatesList()
        {
            new ProjectScanWindow(this.path, false).ShowDialog();

            string jsonContent = File.ReadAllText(System.IO.Path.Combine(path, "fmh", "blockstates_list.json"));
            blockstatesList.Clear();
            JsonConvert.DeserializeObject<List<String>>(jsonContent).ForEach(element => blockstatesList.Add(new BlockStates(element)));

            listView_blockstates.ItemsSource = null;

            if (!string.IsNullOrWhiteSpace(blockstates_search_textBox.Text))
                listView_blockstates.ItemsSource = blockstatesList.Where(item => item.FileName.Contains(blockstates_search_textBox.Text));
            else
                listView_blockstates.ItemsSource = blockstatesList;
        }

        /// <summary>
        /// Refresh models section by reload models files list
        /// </summary>
        private void RefreshModelsList()
        {
            new ProjectScanWindow(this.path, false).ShowDialog();

            string jsonContent = File.ReadAllText(System.IO.Path.Combine(path, "fmh", "models_list.json"));
            modelsList.Clear();
            JsonConvert.DeserializeObject<List<string>>(jsonContent).ForEach(element => modelsList.Add(new Model(element)));

            listView_models.ItemsSource = null;
            if (!string.IsNullOrWhiteSpace(models_search_textBox.Text))
            {
                if (models_folder_comboBox.SelectedIndex == 0)
                    listView_models.ItemsSource = modelsList.Where(item => item.FileName.Contains(models_search_textBox.Text));
                else
                    listView_models.ItemsSource = modelsList.Where(item => item.FileName.Contains(models_search_textBox.Text) && System.IO.Path.GetFileName(System.IO.Path.GetDirectoryName(item.FilePath)).Contains((string)models_folder_comboBox.SelectedItem));
            }
            else
            {
                if (models_folder_comboBox.SelectedIndex == 0)
                    listView_models.ItemsSource = modelsList;
                else
                    listView_models.ItemsSource = modelsList.Where(item => System.IO.Path.GetFileName(System.IO.Path.GetDirectoryName(item.FilePath)).Contains((string)models_folder_comboBox.SelectedItem));
            }
        }

        /// <summary>
        /// Refresh texture section by reload textures files list
        /// </summary>
        private void RefreshTexturesList()
        {
            new ProjectScanWindow(this.path, false).ShowDialog();

            string jsonContent = File.ReadAllText(System.IO.Path.Combine(path, "fmh", "textures_list.json"));
            texturesList.Clear();
            JsonConvert.DeserializeObject<List<string>>(jsonContent).ForEach(element => texturesList.Add(new Texture(element)));

            listView_textures.ItemsSource = null;
            if (!string.IsNullOrWhiteSpace(textures_search_textBox.Text))
            {
                if (textures_folder_comboBox.SelectedIndex == 0)
                    listView_textures.ItemsSource = texturesList.Where(item => item.FileName.Contains(textures_search_textBox.Text));
                else
                    listView_textures.ItemsSource = texturesList.Where(item => item.FileName.Contains(textures_search_textBox.Text) && System.IO.Path.GetFileName(System.IO.Path.GetDirectoryName(item.FilePath)).Contains((string)textures_folder_comboBox.SelectedItem));
            }
            else
            {
                if (models_folder_comboBox.SelectedIndex == 0)
                    listView_textures.ItemsSource = texturesList;
                else
                    listView_textures.ItemsSource = texturesList.Where(item => System.IO.Path.GetFileName(System.IO.Path.GetDirectoryName(item.FilePath)).Contains((string)textures_folder_comboBox.SelectedItem));
            }
        }

        /// <summary>
        /// Refresh translation section by reload translation files list
        /// </summary>
        private void RefreshTranslationFilesList()
        {
            language_files_listBox.Items.Clear();

            List<String> fileList = Directory.EnumerateFiles(System.IO.Path.Combine(path, "src\\main\\resources\\assets", modInfos["mod_id"], "lang")).ToList();
            fileList.ForEach(element => language_files_listBox.Items.Add(System.IO.Path.GetFileName(element)));

            if (language_files_listBox.Items.Count > 0)
                language_files_listBox.SelectedIndex = 0;
        }
        #endregion
    }

    ///// <summary>
    ///// Class representing blockstates file
    ///// </summary>
    public class BlockStates
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }

        public BlockStates(string filePath)
        {
            this.FilePath = filePath;
            this.FileName = System.IO.Path.GetFileNameWithoutExtension(filePath);
        }
    }

    ///// <summary>
    ///// Class representing model file
    ///// </summary>
    public class Model
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public BitmapImage Icon { get; set; }

        public Model(string filePath)
        {
            this.FilePath = filePath;
            this.FileName = System.IO.Path.GetFileNameWithoutExtension(filePath);
            Stream imgStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Forge_Modding_Helper_3.Resources.Pictures.models_icon.png");

            if (FilePath.Contains("item"))
                imgStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Forge_Modding_Helper_3.Resources.Pictures.item_icon.png");
            else if (FilePath.Contains("block"))
                imgStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Forge_Modding_Helper_3.Resources.Pictures.block_icon.png");

            BitmapImage logo = new BitmapImage();
            logo.BeginInit();
            logo.StreamSource = imgStream;
            logo.EndInit();
            imgStream.Close();
            this.Icon = logo;
        }
    }

    /// <summary>
    /// Class representing texture file
    /// </summary>
    public class Texture
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }

        public string FileImagePath { get; set; }

        public Texture(string filePath)
        {
            this.FilePath = filePath;
            this.FileName = System.IO.Path.GetFileNameWithoutExtension(filePath);

            if (FileName.Contains(".png"))
                FileImagePath = @"/Forge Modding Helper 3;component/Resources/Pictures/code_file_icon.png";
            else
                FileImagePath = FilePath;
        }
    }

    /// <summary>
    /// Class used to not lock opened external pictures in a listview
    /// Only for WPF binding converter
    /// </summary>
    public class StringToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            object result = null;
            var path = value as string;

            if (!string.IsNullOrEmpty(path) && path.Contains("/Forge Modding Helper 3;component"))
                result = path;

            else if (!string.IsNullOrEmpty(path) && File.Exists(path))
            {
                using (var stream = File.OpenRead(path))
                {
                    var image = new BitmapImage();
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.StreamSource = stream;
                    image.EndInit();
                    result = image;
                }
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
