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
    /// Logique d'interaction pour RenameDialog.xaml
    /// </summary>
    public partial class RenameDialog : Window
    {
        private string filePath;

        public RenameDialog(string filePath)
        {
            InitializeComponent();

            // Loading translations
            UITextTranslator.LoadTranslationFile(OptionsFile.GetCurrentLanguage());
            UITextTranslator.UpdateComponentsTranslations(this.MainGrid);
            this.Title = UITextTranslator.getTranslation("dialog.rename.title");

            this.filePath = filePath;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.current_name_label.Text = Path.GetFileName(filePath);
            this.new_name_textBox.Text = Path.GetFileName(filePath);
        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (RenameButton != null)
            {
                if (!string.IsNullOrWhiteSpace(this.new_name_textBox.Text) && this.new_name_textBox.Text != Path.GetFileName(filePath))
                    RenameButton.IsEnabled = true;
                else
                    RenameButton.IsEnabled = false;
            }
        }

        private void cancel_button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void rename_button_Click(object sender, RoutedEventArgs e)
        {
            File.Move(this.filePath, Path.Combine(this.filePath.Replace(Path.GetFileName(filePath), ""), this.new_name_textBox.Text));
            this.Close();
        }
    }
}
