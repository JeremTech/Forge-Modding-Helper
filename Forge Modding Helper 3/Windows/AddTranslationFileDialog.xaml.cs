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
using System.Windows.Shapes;
using Forge_Modding_Helper_3.Utils;
using Path = System.IO.Path;

namespace Forge_Modding_Helper_3.Windows
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
        }

        private void name_textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (add_button != null)
            {
                if (!string.IsNullOrWhiteSpace(name_textBox.Text) && name_textBox.Text.EndsWith(".json"))
                {
                    add_button.IsEnabled = true;
                }
                else
                {
                    add_button.IsEnabled = false;
                }
            }
        }

        private void add_button_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(Path.Combine(this.langPath, this.name_textBox.Text)))
            {
                MessageBoxResult res = MessageBox.Show(UITextTranslator.getTranslation("workspace_manager.lang.add.error"), "Forge Modding Helper", MessageBoxButton.YesNo, MessageBoxImage.Question);

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
