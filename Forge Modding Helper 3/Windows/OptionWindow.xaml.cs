using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using Forge_Modding_Helper_3.Files;
using Forge_Modding_Helper_3.Utils;

namespace Forge_Modding_Helper_3.Windows
{
    /// <summary>
    /// Logique d'interaction pour OptionWindow.xaml
    /// </summary>
    public partial class OptionWindow : Window
    {
        public OptionWindow()
        {
            InitializeComponent();
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            // Loadings translations
            UITextTranslator.LoadTranslationFile(OptionsFile.getCurrentLanguage());
            UITextTranslator.UpdateComponentsTranslations(this.main_grid);

            // Reading option file
            OptionsFile.ReadDataFile();

            ui_language_comboBox.ItemsSource = UITextTranslator.getAvailableLanguagesList();
            ui_language_comboBox.SelectedItem = OptionsFile.getCurrentLanguage();
        }

        private void ui_language_comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OptionsFile.setCurrentLanguage(ui_language_comboBox.SelectedItem.ToString());
            UITextTranslator.LoadTranslationFile(OptionsFile.getCurrentLanguage());
            UITextTranslator.UpdateComponentsTranslations(this.main_grid);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            OptionsFile.WriteDataFile();
        }
    }
}
