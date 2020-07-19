﻿using System;
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
        /// <param name="localization">Name of the target language file (without <c>.json</c> extension)</param>
        public static void LoadTranslationFile(String localization)
        {

            string resourceName = "Forge_Modding_Helper_3.Resources.Languages." + localization + ".json";
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);

            using (StreamReader reader = new StreamReader(stream))
            {
                string fileContent = reader.ReadToEnd();
                translationFile = JsonConvert.DeserializeObject<TranslationFile>(fileContent);
            }
        }

        public static void UpdateComponentsTranslations(DependencyObject window)
        {
            // Updating labels
            IEnumerable<Label> labels = UIUtils.FindVisualChildren<Label>(window);

            foreach (Label label in labels)
            {
                if (label.Tag != null)
                {
                    label.Content = getTranslation((String) label.Tag).FormateTranslationText();
                }
            }

            // Updating buttons
            IEnumerable<Button> buttons = UIUtils.FindVisualChildren<Button>(window);

            foreach (Button button in buttons)
            {
                if (button.Tag != null)
                    button.Content = getTranslation((String)button.Tag).FormateTranslationText();
            }

            // Updating checkboxes text
            IEnumerable<CheckBox> checkboxes = UIUtils.FindVisualChildren<CheckBox>(window);

            foreach (CheckBox checkbox in checkboxes)
            {
                if (checkbox.Tag != null)
                    checkbox.Content = getTranslation((String)checkbox.Tag);
            }

            // Updating groups header
            IEnumerable<GroupBox> groups = UIUtils.FindVisualChildren<GroupBox>(window);

            foreach (GroupBox group in groups)
            {
                if (group.Tag != null)
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
    }

    public class TranslationFile
    {
        public String name { get; set; }
        public Dictionary<String, String> entries { get; set; }
    }
}