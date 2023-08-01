using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Windows.Controls;
using System.Windows;
using FMH.Core.Files.Software;
using FMH.Core.UI.Controls;

namespace FMH.Core.Utils.UI
{
    public class UITextTranslator
    {
        private static TranslationFile translationFile;

        /// <summary>
        /// Reading translation file
        /// </summary>
        /// <param name="localization">Name of the language file without extension</param>
        public static void LoadTranslationFile(string localization)
        {
            string directoryPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Languages");

            // Check if Languages directory exist
            if (!Directory.Exists(directoryPath))
            {
                MessageBox.Show("\"" + directoryPath + "\" not found.\nUnable to load translations.\n\nThe application will close.", "Forge Modding Helper", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            }

            List<string> languagesFilesList = Directory.GetFiles(directoryPath).ToList();

            // If no translation files is available
            if (languagesFilesList.Count == 0)
            {
                // Error and quit
                MessageBox.Show("No translations files found.\nUnable to load translations.\n\nThe application will close.", "Forge Modding Helper", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            }

            // Search language file
            foreach (string filePath in languagesFilesList)
            {
                string fileContent = File.ReadAllText(filePath);
                if (Path.GetFileNameWithoutExtension(filePath) == localization) translationFile = JsonConvert.DeserializeObject<TranslationFile>(fileContent);
            }

            // If no file has found, we display error message and set to en_us file
            if (translationFile == null)
            {
                MessageBox.Show("\"" + localization + ".json\" translation file not found.\nUnable to load translations.\n\nLanguage will be reset to default (en_us).", "Forge Modding Helper", MessageBoxButton.OK, MessageBoxImage.Error);
                localization = "en_us";
                string fileContent = File.ReadAllText(Path.Combine(directoryPath, "en_us.json"));
                translationFile = JsonConvert.DeserializeObject<TranslationFile>(fileContent);

                // Rewrite option files
                OptionsFile.SetCurrentLanguage("en_us");
                OptionsFile.WriteDataFile();
            }
        }

        public static void UpdateComponentsTranslations(DependencyObject window)
        {
            // Updating labels
            IEnumerable<Label> labels = UIUtils.FindVisualChildren<Label>(window);

            foreach (Label label in labels)
            {
                if (label.Tag != null && label.Tag is string && !string.IsNullOrWhiteSpace(label.Tag.ToString()))
                {
                    label.Content = getTranslation((String)label.Tag).FormateTranslationText();
                }
            }

            // Updating TextBlocks
            IEnumerable<TextBlock> textblocks = UIUtils.FindVisualChildren<TextBlock>(window);

            foreach (TextBlock textblock in textblocks)
            {
                if (textblock.Tag != null && textblock.Tag is string && !string.IsNullOrWhiteSpace(textblock.Tag.ToString()))
                {
                    textblock.Text = getTranslation((String)textblock.Tag).FormateTranslationText();
                }
            }

            // Updating buttons
            IEnumerable<Button> buttons = UIUtils.FindVisualChildren<Button>(window);

            foreach (Button button in buttons)
            {
                if (button.Tag != null && button.Tag is string && !string.IsNullOrWhiteSpace(button.Tag.ToString()))
                    button.Content = getTranslation((String)button.Tag).FormateTranslationText();
            }

            // Updating checkboxes text
            IEnumerable<CheckBox> checkboxes = UIUtils.FindVisualChildren<CheckBox>(window);

            foreach (CheckBox checkbox in checkboxes)
            {
                if (checkbox.Tag != null && checkbox.Tag is string && !string.IsNullOrWhiteSpace(checkbox.Tag.ToString()))
                    checkbox.Content = getTranslation((String)checkbox.Tag);
            }

            // Updating groups header
            IEnumerable<GroupBox> groups = UIUtils.FindVisualChildren<GroupBox>(window);

            foreach (GroupBox group in groups)
            {
                if (group.Tag != null && group.Tag is string && !string.IsNullOrWhiteSpace(group.Tag.ToString()))
                    group.Header = getTranslation((String)group.Tag);

                // Translate inner components
                UpdateComponentsTranslations(group);
            }

            // Updating info display controls
            IEnumerable<DashboardInfoDisplay> infoDisplays = UIUtils.FindVisualChildren<DashboardInfoDisplay>(window);

            foreach (DashboardInfoDisplay infoDisplay in infoDisplays)
            {
                if (infoDisplay.Tag != null && infoDisplay.Tag is string && !string.IsNullOrWhiteSpace(infoDisplay.Tag.ToString()))
                    infoDisplay.InfoTitle = getTranslation((String)infoDisplay.Tag);
            }

            // Updating context menus items header from listbox
            foreach (var listbox in UIUtils.FindVisualChildren<ListBox>(window))
            {
                if (listbox.ContextMenu != null)
                {
                    foreach (MenuItem contextMenuItem in listbox.ContextMenu.Items)
                    {
                        if (contextMenuItem.Tag != null && contextMenuItem.Tag is string && !string.IsNullOrWhiteSpace(contextMenuItem.Tag.ToString()))
                            contextMenuItem.Header = getTranslation((String)contextMenuItem.Tag);
                    }
                }
            }
        }

        public static String getTranslation(string translationKey)
        {
            try
            {
                String result;
                translationFile.entries.TryGetValue(translationKey, out result);
                return result;
            }
            catch (ArgumentNullException e)
            {
                Debug.Print(e.Message);
                return translationKey;
            }
        }

        public static List<String> getAvailableLanguagesFileNameList()
        {
            List<String> languagesList = new List<string>();

            string directoryPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Languages");

            if (!Directory.Exists(directoryPath))
            {
                MessageBox.Show("Forge Modding Helper", "\"" + directoryPath + "\" not found.\nUnable to load translations.\n\nThe application will close.");
                Application.Current.Shutdown();
            }

            List<string> languagesFilesList = Directory.GetFiles(directoryPath).ToList();

            foreach (string filePath in languagesFilesList)
            {
                languagesList.Add(Path.GetFileNameWithoutExtension(filePath));
            }

            return languagesList;
        }
    }
}
