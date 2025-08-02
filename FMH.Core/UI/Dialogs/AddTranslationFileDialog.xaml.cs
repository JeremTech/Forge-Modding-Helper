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
    /// Logique d'interaction pour AddTranslationFileDialog.xaml
    /// </summary>
    public partial class AddTranslationFileDialog : Window, INotifyPropertyChanged
    {
        private string langPath = "";

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
        /// Add command
        /// </summary>
        private ICommand _addCommand;
        public ICommand AddCommand
        {
            get
            {
                return _addCommand;
            }
            set
            {
                _addCommand = value;
                OnPropertyChanged();
            }
        }
        #endregion

        public AddTranslationFileDialog(string langFolderPath)
        {
            // Initialize commands
            this.CancelCommand = new RelayCommand(Cancel);
            this.AddCommand = new RelayCommand(Add);

            InitializeComponent();
            this.langPath = langFolderPath;

            // Load translations
            UITextTranslator.LoadTranslationFile(OptionsFile.GetCurrentLanguage());
            UITextTranslator.UpdateComponentsTranslations(this.MainGrid);
            this.Title = UITextTranslator.GetTranslation("dialog.translation.add.title");
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

        #region Commands functions
        public void Cancel()
        {
            this.Close();
        }

        public void Add()
        {
            if (File.Exists(Path.Combine(this.langPath, this.name_textBox.Text)))
            {
                MessageBoxResult res = MessageBox.Show(UITextTranslator.GetTranslation("dialog.translation.add.alert.error"), "Forge Modding Helper", MessageBoxButton.YesNo, MessageBoxImage.Question);

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
