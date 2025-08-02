using CommunityToolkit.Mvvm.Input;
using FMH.Core.Files.Software;
using FMH.Core.Utils.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace FMH.Core.UI.Dialogs
{
    /// <summary>
    /// Logique d'interaction pour RenameDialog.xaml
    /// </summary>
    public partial class RenameDialog : Window, INotifyPropertyChanged
    {
        private string filePath;

        // Events
        public event PropertyChangedEventHandler? PropertyChanged;

        #region Commands
        /// <summary>
        /// Cancel command
        /// </summary>
        private ICommand _cancelCommand;
        public ICommand CancelCommand
        {
            get
            {
                return _cancelCommand;
            }
            set
            {
                _cancelCommand = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Rename command
        /// </summary>
        private ICommand _renameCommand;
        public ICommand RenameCommand
        {
            get
            {
                return _renameCommand;
            }
            set
            {
                _renameCommand = value;
                OnPropertyChanged();
            }
        }
        #endregion

        public RenameDialog(string filePath)
        {
            // Initializing commands
            this.CancelCommand = new RelayCommand(Cancel);
            this.RenameCommand = new RelayCommand(Rename);

            InitializeComponent();

            // Loading translations
            UITextTranslator.LoadTranslationFile(OptionsFile.GetCurrentLanguage());
            UITextTranslator.UpdateComponentsTranslations(this.MainGrid);
            this.Title = UITextTranslator.GetTranslation("dialog.rename.title");

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

        #region Commands functions
        public void Cancel()
        {
            this.Close();
        }

        public void Rename()
        {
            File.Move(this.filePath, Path.Combine(this.filePath.Replace(Path.GetFileName(filePath), ""), this.new_name_textBox.Text));
            this.Close();
        }
        #endregion

        #region Interfaces implementations
        public void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
