using System;
using System.Collections.Generic;
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
using Forge_Modding_Helper_3.Utils;
using Forge_Modding_Helper_3.Windows;
using Microsoft.VisualBasic.FileIO;
using Newtonsoft.Json;
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
        private List<String> texturesList = new List<string>();

        // Models list storage
        private List<Model> modelsList = new List<Model>();

        // java file list storage
        private List<String> javaFileList = new List<string>();

        public WorkspaceManager(string path)
        {
            this.path = path;
            InitializeComponent();
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
            texturesList = JsonConvert.DeserializeObject<List<string>>(jsonContent);

            // Read models_list.json file
            jsonContent = File.ReadAllText(System.IO.Path.Combine(path, "fmh", "models_list.json"));
            JsonConvert.DeserializeObject<List<string>>(jsonContent).ForEach(element => modelsList.Add(new Model(element)));

            // Read java_files_list.json file
            jsonContent = File.ReadAllText(System.IO.Path.Combine(path, "fmh", "java_files_list.json"));
            javaFileList = JsonConvert.DeserializeObject<List<string>>(jsonContent);

            #region Updating UI

            // Mod infos
            this.label_mod_name.Content = modInfos["mod_name"];
            this.label_mod_description.Content = modInfos["mod_description"];
            this.label_home_minecraft_version.Content = modInfos["minecraft_version"];
            this.label_minecraft_version.Content = "Minecraft " + modInfos["minecraft_version"];
            this.label_home_forge_version.Content = modInfos["forge_version"];
            this.label_forge_version.Content = "Forge " + modInfos["forge_version"];
            this.label_home_mappings_version.Content = modInfos["mappings_version"];
            this.label_home_textures_number.Content = texturesList.Count;
            this.label_home_models_number.Content = modelsList.Count;
            this.label_home_javafiles_number.Content = javaFileList.Count;

            #endregion
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
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

                String tag = senderButton.Tag.ToString();

                if (tag.Contains("home"))
                {
                    this.home_button_border.Background = new SolidColorBrush(Color.FromRgb(0, 116, 255));
                    this.home_grid.Visibility = Visibility.Visible;
                }
                else if (tag.Contains("mod_settings"))
                {
                    this.mod_toml_button_border.Background = new SolidColorBrush(Color.FromRgb(0, 116, 255));
                    this.mod_settings_grid.Visibility = Visibility.Visible;
                }
                else if (tag.Contains("blockstates"))
                {
                    this.blockstates_button_border.Background = new SolidColorBrush(Color.FromRgb(0, 116, 255));
                    this.blockstates_grid.Visibility = Visibility.Visible;
                    listView_blockstates.ItemsSource = blockstatesList;
                }
                else if (tag.Contains("models"))
                {
                    this.models_button_border.Background = new SolidColorBrush(Color.FromRgb(0, 116, 255));
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
                    this.textures_button_border.Background = new SolidColorBrush(Color.FromRgb(0, 116, 255));
                }
                else if (tag.Contains("translations"))
                {
                    this.lang_button_border.Background = new SolidColorBrush(Color.FromRgb(0, 116, 255));
                }
                else if (tag.Contains("export"))
                {
                    this.build_button_border.Background = new SolidColorBrush(Color.FromRgb(0, 116, 255));
                }
            }
        }

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
                listView_models.ItemsSource = modelsList;
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

        #region Integrated Project Scan
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
        #endregion
    }

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
}
