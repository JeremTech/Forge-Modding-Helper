using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using Forge_Modding_Helper_3.Files;
using Forge_Modding_Helper_3.Utils;
using Newtonsoft.Json;
using Path = System.IO.Path;

namespace Forge_Modding_Helper_3.Windows
{
    /// <summary>
    /// Logique d'interaction pour OptionWindow.xaml
    /// </summary>
    public partial class OptionWindow : Window
    {
        // Language files
        Dictionary<string, string> languagesFilesList = new Dictionary<string, string>();

        // Themes files
        Dictionary<string, string> themesFilesList = new Dictionary<string, string>();

        public OptionWindow()
        {
            InitializeComponent();
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            // Reading option file
            OptionsFile.ReadDataFile();

            // Listing available language files
            foreach(string fileName in UITextTranslator.getAvailableLanguagesFileNameList())
            {
                string jsonContent = File.ReadAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Resources", "Languages", fileName + ".json"));
                TranslationFile file = JsonConvert.DeserializeObject<TranslationFile>(jsonContent);
                languagesFilesList.Add(fileName, file.name);
            }

            // Filling language combobox
            ui_language_comboBox.ItemsSource = languagesFilesList;
            string currentLanguage;
            languagesFilesList.TryGetValue(OptionsFile.GetCurrentLanguage(), out currentLanguage);
            ui_language_comboBox.SelectedItem = new KeyValuePair<string, string>(OptionsFile.GetCurrentLanguage(), currentLanguage);

            // Listing available themes files
            foreach (string fileName in UIUtils.GetAllAvailableThemesFiles())
            {
                string jsonContent = File.ReadAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Resources", "Themes", fileName + ".json"));
                ThemeFile file = JsonConvert.DeserializeObject<ThemeFile>(jsonContent);

                if(file.name != null) themesFilesList.Add(fileName, file.name);
            }

            // Filling themes combobox
            ui_theme_comboBox.ItemsSource = themesFilesList;
            string currentTheme;
            themesFilesList.TryGetValue(OptionsFile.GetCurrentTheme(), out currentTheme);
            ui_theme_comboBox.SelectedItem = new KeyValuePair<string, string>(OptionsFile.GetCurrentTheme(), currentTheme);

            // Set project scan options checkboxes
            CountBlankCodeLines_CheckBox.IsChecked = OptionsFile.GetCountBlankCodeLinesOption();
            CountCodeLinesAtProjectStartup_CheckBox.IsChecked = OptionsFile.GetCountCodeLinesAtProjectOpeningOption();

            // Loadings translations
            UITextTranslator.LoadTranslationFile(OptionsFile.GetCurrentLanguage());
            UITextTranslator.UpdateComponentsTranslations(this.main_grid);
            UITextTranslator.UpdateComponentsTranslations(this.general_groupbox_grid);
            UITextTranslator.UpdateComponentsTranslations(this.project_scan_groupbox_grid);
            this.Title = UITextTranslator.getTranslation("options.title");
        }

        private void ui_language_comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(ui_language_comboBox.SelectedItem != null)
            {
                OptionsFile.SetCurrentLanguage(((KeyValuePair<string, string>)ui_language_comboBox.SelectedItem).Key);
                UITextTranslator.LoadTranslationFile(OptionsFile.GetCurrentLanguage());
                UITextTranslator.UpdateComponentsTranslations(this.main_grid);
                this.Title = UITextTranslator.getTranslation("options.title");
            }
        }

        private void ui_theme_comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ui_theme_comboBox.SelectedItem != null)
            {
                OptionsFile.SetCurrentTheme(((KeyValuePair<string, string>)ui_theme_comboBox.SelectedItem).Key);
                App.LoadThemeFile(OptionsFile.GetCurrentTheme());
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            OptionsFile.SetCountBlankCodeLinesOption(CountBlankCodeLines_CheckBox.IsChecked.Value);
            OptionsFile.SetCountCodeLinesAtProjectOpeningOption(CountCodeLinesAtProjectStartup_CheckBox.IsChecked.Value);
            OptionsFile.WriteDataFile();
        }

        private void about_button_Click(object sender, RoutedEventArgs e)
        {
            new AboutWindow().ShowDialog();
        }

        private void report_bug_button_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/JeremTech/Forge-Modding-Helper/issues/new?assignees=&labels=&template=bug_report.md&title=");
        }

    }
}
