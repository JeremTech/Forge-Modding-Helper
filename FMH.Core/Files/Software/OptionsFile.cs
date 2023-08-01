using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FMH.Core.Files.Software
{
    public class OptionsFile
    {
        // Data-file path
        private static string FilePath = Path.Combine(App.getApplicationDataDirectory(), "options.json");
        // Data object with default values
        private static OptionsData data = new OptionsData();

        // Selected language
        private static string language = "en_us";
        // Selected theme
        private static string theme = "dark";
        // Count blank code lines ?
        private static bool countBlankCodeLines = false;
        // Count code lines at project startup ?
        private static bool countCodeLinesAtProjectOpening = true;

        /// <summary>
        /// Get current language
        /// </summary>
        /// <returns>Current language file name without extension</returns>
        public static string GetCurrentLanguage()
        {
            return language;
        }

        /// <summary>
        /// Set current language
        /// </summary>
        /// <returns>Set language file name (without extension)</returns>
        public static void SetCurrentLanguage(string lang)
        {
            language = lang;
            data.Language = lang;
        }

        /// <summary>
        /// Get current theme
        /// </summary>
        /// <returns>Current theme file name without extension</returns>
        public static string GetCurrentTheme()
        {
            return theme;
        }

        /// <summary>
        /// Set current theme
        /// </summary>
        /// <returns>Set theme file name (without extension)</returns>
        public static void SetCurrentTheme(string themeIn)
        {
            theme = themeIn;
            data.Theme = themeIn;
        }

        /// <summary>
        /// Get CountBlankCodeLines option
        /// </summary>
        public static bool GetCountBlankCodeLinesOption()
        {
            return countBlankCodeLines;
        }

        /// <summary>
        /// Set CountBlankCodeLines option
        /// </summary>
        /// <param name="_countBlankCodeLines">New value</param>
        public static void SetCountBlankCodeLinesOption(bool _countBlankCodeLines)
        {
            countBlankCodeLines = _countBlankCodeLines;
        }

        /// <summary>
        /// Get CountCodeLinesAtProjectOpening option
        /// </summary>
        public static bool GetCountCodeLinesAtProjectOpeningOption()
        {
            return countCodeLinesAtProjectOpening;
        }

        /// <summary>
        /// Set CountCodeLinesAtProjectOpening option
        /// </summary>
        /// <param name="_countCodeLinesAtProjectOpening">New value</param>
        public static void SetCountCodeLinesAtProjectOpeningOption(bool _countCodeLinesAtProjectOpening)
        {
            countCodeLinesAtProjectOpening = _countCodeLinesAtProjectOpening;
        }

        /// <summary>
        /// Read the data file 
        /// </summary>
        public static void ReadDataFile()
        {
            if (File.Exists(FilePath))
            {
                string jsonContent = File.ReadAllText(FilePath);
                OptionsData jsonContentFormatted = JsonConvert.DeserializeObject<OptionsData>(jsonContent);
                data = jsonContentFormatted;

                if (!string.IsNullOrEmpty(jsonContentFormatted.Language))
                    language = jsonContentFormatted.Language;

                if (!string.IsNullOrEmpty(jsonContentFormatted.Theme))
                    theme = jsonContentFormatted.Theme;

                if (jsonContentFormatted.CountBlankCodeLines.HasValue)
                    countBlankCodeLines = jsonContentFormatted.CountBlankCodeLines.Value;

                if (jsonContentFormatted.CountCodeLinesAtProjectOpening.HasValue)
                    countCodeLinesAtProjectOpening = jsonContentFormatted.CountCodeLinesAtProjectOpening.Value;
            }
        }

        /// <summary>
        /// Write the data file 
        /// </summary>
        public static void WriteDataFile()
        {
            if (!string.IsNullOrEmpty(language) && !string.IsNullOrEmpty(theme))
            {
                OptionsData data = new OptionsData(language, theme, countBlankCodeLines, countCodeLinesAtProjectOpening);
                string jsonContent = JsonConvert.SerializeObject(data, Formatting.Indented);
                File.WriteAllText(FilePath, jsonContent);
            }
        }
    }

    public class OptionsData
    {
        /// <summary>
        /// Define software language
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// Define software theme
        /// </summary>
        public string Theme { get; set; }

        /// <summary>
        /// Define if blank lines must be counted or not
        /// </summary>
        public bool? CountBlankCodeLines { get; set; }

        /// <summary>
        /// Define if the number of code lines is calculated at project opening
        /// </summary>
        public bool? CountCodeLinesAtProjectOpening { get; set; }

        /// <summary>
        /// Default constructor which set values to default
        /// </summary>
        public OptionsData()
        {
            this.Language = "en_us";
            this.Theme = "dark";
            this.CountBlankCodeLines = false;
            this.CountCodeLinesAtProjectOpening = true;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="_language">Software language</param>
        /// <param name="_theme">Software theme</param>
        /// <param name="_countBlankCodeLines">Count blank code lines or not</param>
        /// <param name="_countCodeLinesAtProjectOpening">Count code lines at project opening or not</param>
        public OptionsData(string _language, string _theme, bool _countBlankCodeLines, bool _countCodeLinesAtProjectOpening)
        {
            this.Language = _language;
            this.Theme = _theme;
            this.CountBlankCodeLines = _countBlankCodeLines;
            this.CountCodeLinesAtProjectOpening = _countCodeLinesAtProjectOpening;
        }
    }
}
