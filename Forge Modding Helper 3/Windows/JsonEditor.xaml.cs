using Forge_Modding_Helper_3.Files;
using Forge_Modding_Helper_3.Utils;
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

namespace Forge_Modding_Helper_3.Windows
{
    public partial class JsonEditor : Window
    {
        // File path
        private string filePath;
        // Save status
        private bool savedStatus = true;
        // Initial content
        private string initialContent;

        public JsonEditor(string _filePath)
        {
            InitializeComponent();
            filePath = _filePath;

            // Load translations
            UITextTranslator.LoadTranslationFile(OptionsFile.getCurrentLanguage());
            UITextTranslator.UpdateComponentsTranslations(this.MainGrid);
            this.Title = UITextTranslator.getTranslation("json_editor.title") + " - " + System.IO.Path.GetFileName(_filePath);
            this.EditMenuSave.Header = UITextTranslator.getTranslation("json_editor.menu.save");
            this.EditMenuCopy.Header = UITextTranslator.getTranslation("json_editor.menu.copy");
            this.EditMenuPaste.Header = UITextTranslator.getTranslation("json_editor.menu.paste");
            this.EditMenuCut.Header = UITextTranslator.getTranslation("json_editor.menu.cut");
        }

        private void Window_Loaded(object sender, EventArgs e)
        {
            // Read file
            initialContent = File.ReadAllText(filePath);
            // Saved status initialization
            savedStatus = true;
            // Set TextEditor content
            TextEditor.Text = initialContent;
            // Give focus at TextEditor control
            TextEditor.Focus();
            // Update modification status
            UpdateModificationsStatus();
        }

        private void UpdateModificationsStatus()
        {
            if(savedStatus == true)
            {
                StatusLabel.Content = UITextTranslator.getTranslation("json_editor.modifications_saved");
                StatusImage.Source = new BitmapImage(new Uri("/Forge Modding Helper 3;component/Resources/Pictures/check.png", UriKind.Relative));
            }
            else
            {
                StatusLabel.Content = UITextTranslator.getTranslation("json_editor.modifications_unsaved");
                StatusImage.Source = new BitmapImage(new Uri("/Forge Modding Helper 3;component/Resources/Pictures/uncheck.png", UriKind.Relative));
            }
        }

        private void TextEditor_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(TextEditor.Text != initialContent)
            {
                savedStatus = false;
            }
            else
            {
                savedStatus = true;
            }

            // Update modification status
            UpdateModificationsStatus();
        }

        private void EditMenuSave_Click(object sender, RoutedEventArgs e)
        {
            File.WriteAllText(filePath, TextEditor.Text);

            // Update modification status
            savedStatus = true;
            UpdateModificationsStatus();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Check if there are unsaved modifications
            if(savedStatus == false)
            {
                // Displaying confirmation message
                MessageBoxResult msgResult = MessageBox.Show(UITextTranslator.getTranslation("json_editor.alert.close"), "Forge Modding Helper", MessageBoxButton.YesNo, MessageBoxImage.Question);

                // If yes
                if (msgResult == MessageBoxResult.Yes)
                {
                }
                // If no
                else
                {
                    e.Cancel = true;
                }
            }
        }
    }
}
