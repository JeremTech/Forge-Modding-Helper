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
using Path = System.Windows.Shapes.Path;

namespace Forge_Modding_Helper_3.Windows
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

            this.filePath = filePath;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.current_name_label.Text = System.IO.Path.GetFileName(filePath);
            this.new_name_textBox.Text = System.IO.Path.GetFileName(filePath);
        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (rename_button != null)
            {
                if (!string.IsNullOrWhiteSpace(this.new_name_textBox.Text) && this.new_name_textBox.Text != System.IO.Path.GetFileName(filePath))
                    rename_button.IsEnabled = true;
                else
                    rename_button.IsEnabled = false;
            }
        }

        private void cancel_button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void rename_button_Click(object sender, RoutedEventArgs e)
        {
            File.Move(this.filePath, System.IO.Path.Combine(this.filePath.Replace(System.IO.Path.GetFileName(filePath), ""), this.new_name_textBox.Text));
            this.Close();
        }
    }
}
