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
using FMH.Core.Files.Software;
using FMH.Core.Utils.UI;

namespace FMH.Core.UI.Dialogs
{
    /// <summary>
    /// Logique d'interaction pour AddTranslationFileDialog.xaml
    /// </summary>
    public partial class AddTranslationFileDialog : Window
    {
        private string langPath = "";

        public AddTranslationFileDialog(string langFolderPath)
        {
            InitializeComponent();
            this.langPath = langFolderPath;

            // Load translations
            UITextTranslator.LoadTranslationFile(OptionsFile.GetCurrentLanguage());
            UITextTranslator.UpdateComponentsTranslations(this.MainGrid);
            this.Title = UITextTranslator.getTranslation("dialog.translation.add.title");
        }

        private void name_textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (AddButton != null)
            {
                if (!string.IsNullOrWhiteSpace(name_textBox.Text) && name_textBox.Text.EndsWith(".json"))
                {
                    AddButton.IsEnabled = true;
                }
                else
                {
                    AddButton.IsEnabled = false;
                }
            }
        }

        private void add_button_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(Path.Combine(this.langPath, this.name_textBox.Text)))
            {
                MessageBoxResult res = MessageBox.Show(UITextTranslator.getTranslation("dialog.translation.add.alert.error"), "Forge Modding Helper", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (res == MessageBoxResult.Yes)
                {
                    File.WriteAllText(Path.Combine(this.langPath, this.name_textBox.Text), "{\n}");
                    this.Close();
                }
            }
            else
            {
                File.WriteAllText(Path.Combine(this.langPath, this.name_textBox.Text), "{\n}");
                this.Close();
            }
        }

        private void cancel_button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
