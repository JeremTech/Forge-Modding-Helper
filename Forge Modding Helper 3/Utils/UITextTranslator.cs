using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;

namespace Forge_Modding_Helper_3.Utils
{
    /// <summary>
    /// This class help to display the correct text in UI Components
    /// </summary>
    public class UITextTranslator
    {
        private static TranslationFile translationFile;

        /// <summary>
        /// Reading translation file
        /// </summary>
        /// <param name="localization">Name of the language in the file</param>
        public static void LoadTranslationFile(String localization)
        {
            string directoryName = "Forge_Modding_Helper_3.Resources.Languages";
            List<string> languagesFilesList = Assembly.GetExecutingAssembly().GetManifestResourceNames().Where(element => element.StartsWith(directoryName) && element.EndsWith(".json")).ToList();

            foreach (string filePath in languagesFilesList)
            {
                Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(filePath);

                using (StreamReader reader = new StreamReader(stream))
                {
                    string fileContent = reader.ReadToEnd();
                    if(JsonConvert.DeserializeObject<TranslationFile>(fileContent).name == localization)
                        translationFile = JsonConvert.DeserializeObject<TranslationFile>(fileContent);
                }
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
                    label.Content = getTranslation((String) label.Tag).FormateTranslationText();
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
            catch(ArgumentNullException e)
            {
                Debug.Print(e.Message);
                return translationKey;
            }
        }

        public static List<String> getAvailableLanguagesList()
        {
            List<String> languagesList = new List<string>();

            string directoryName = "Forge_Modding_Helper_3.Resources.Languages";
            List<string> languagesFilesList = Assembly.GetExecutingAssembly().GetManifestResourceNames().Where(element => element.StartsWith(directoryName) && element.EndsWith(".json")).ToList();

            foreach (string filePath in languagesFilesList)
            {
                Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(filePath);

                using (StreamReader reader = new StreamReader(stream))
                {
                    string fileContent = reader.ReadToEnd();
                    languagesList.Add(JsonConvert.DeserializeObject<TranslationFile>(fileContent).name);
                }
            }

            return languagesList;
        }
    }

    public class TranslationFile
    {
        public String name { get; set; }
        public Dictionary<String, String> entries { get; set; }
    }
}
