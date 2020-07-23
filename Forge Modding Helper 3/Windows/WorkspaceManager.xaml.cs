using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        private List<String> modelsList = new List<String>();

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

            // Read textures_list.json file
            jsonContent = File.ReadAllText(System.IO.Path.Combine(path, "fmh", "blockstates_list.json"));
            JsonConvert.DeserializeObject<List<String>>(jsonContent).ForEach(element => blockstatesList.Add(new BlockStates(element)));

            // Read textures_list.json file
            jsonContent = File.ReadAllText(System.IO.Path.Combine(path, "fmh", "textures_list.json"));
            texturesList = JsonConvert.DeserializeObject<List<string>>(jsonContent);

            // Read models_list.json file
            jsonContent = File.ReadAllText(System.IO.Path.Combine(path, "fmh", "models_list.json"));
            modelsList = JsonConvert.DeserializeObject<List<string>>(jsonContent);

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

        private void blockstates_search_textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(!string.IsNullOrWhiteSpace(blockstates_search_textBox.Text))
                listView_blockstates.ItemsSource = blockstatesList.Where(item => item.FileName.Contains(blockstates_search_textBox.Text));
            else
                listView_blockstates.ItemsSource = blockstatesList;
        }
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
}
