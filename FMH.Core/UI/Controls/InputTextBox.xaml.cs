using FMH.Core.Utils.UI;
using FontAwesome.WPF;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FMH.Core.UI.Controls
{
    public partial class InputTextBox : UserControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty InputTextProperty = DependencyProperty.Register("InputText", typeof(string), typeof(InputTextBox), new PropertyMetadata(default(string), new PropertyChangedCallback(OnInputTextChanged)));
        public static readonly DependencyProperty PlaceHolderTextProperty = DependencyProperty.Register("PlaceHolderText", typeof(string), typeof(InputTextBox));
        public static readonly DependencyProperty PlaceHolderTextTranslationKeyProperty = DependencyProperty.Register("PlaceHolderTextTranslationKey", typeof(string), typeof(InputTextBox));
        public static readonly DependencyProperty IsMultilineProperty = DependencyProperty.Register("IsMultiline", typeof(bool), typeof(InputTextBox), new PropertyMetadata(false));

        #region Dependency properties
        [Description("The text writed by the user"), Category("Common Properties")]
        public string InputText
        {
            get
            {
                return (string)GetValue(InputTextProperty);
            }
            set
            {
                SetValue(InputTextProperty, value);
                OnPropertyChanged("InputText");
                OnPropertyChanged("PlaceHolderVisibility");
            }
        }

        [Description("The text displayed as a placeholder"), Category("Common Properties")]
        public string PlaceHolderText
        {
            get
            {
                return (string)GetValue(PlaceHolderTextProperty);
            }
            set
            {
                SetValue(PlaceHolderTextProperty, value);
                OnPropertyChanged("PlaceHolderText");
            }
        }

        [Description("The translation key for the text displayed as a placeholder"), Category("Common Properties")]
        public string PlaceHolderTextTranslationKey
        {
            get
            {
                return (string)GetValue(PlaceHolderTextTranslationKeyProperty);
            }
            set
            {
                SetValue(PlaceHolderTextTranslationKeyProperty, value);
                OnPropertyChanged("PlaceHolderTextTranslationKey");
            }
        }

        [Description("Is a multiline textbox ?"), Category("Common Properties")]
        public bool IsMultiline
        {
            get
            {
                return (bool)GetValue(IsMultilineProperty);
            }
            set
            {
                SetValue(IsMultilineProperty, value);
                OnPropertyChanged("IsMultiline");
                OnPropertyChanged("InputTextVerticalAlignement");
            }
        }
        #endregion

        #region Properties
        public Visibility PlaceHolderVisibility
        {
            get
            {
                if(string.IsNullOrEmpty(InputText)) 
                    return Visibility.Visible;

                return Visibility.Hidden;
            }
        }

        public VerticalAlignment InputTextVerticalAlignement
        {
            get
            {
                if (IsMultiline)
                    return VerticalAlignment.Top;

                return VerticalAlignment.Center;
            }
        }

        public Visibility StatusIconVisibility
        {
            get
            {
                if (_currentInputStatus != InputStatus.None)
                    return Visibility.Visible;

                return Visibility.Collapsed;
            }
        }

        public Brush StatusColorBrush
        {
            get
            {
                if(_currentInputStatus == InputStatus.Error)
                    return (Brush)App.Current.FindResource("BorderErrorColor");

                return (Brush)App.Current.FindResource("BorderColor");
            }
        }

        public FontAwesomeIcon StatusIcon
        {
            get
            {
                if (_currentInputStatus == InputStatus.Error)
                    return FontAwesomeIcon.TimesCircle;

                return FontAwesomeIcon.None;
            }
        }

        public Thickness StatusBorderThickness
        {
            get
            {
                if(_currentInputStatus != InputStatus.None)
                    return new Thickness { Bottom = 2, Left = 2, Right = 2, Top = 2 };

                return new Thickness { Bottom = 1, Left = 1, Right = 1, Top = 1 };
            }
        }

        private InputStatus _currentInputStatus;
        #endregion

        public InputTextBox()
        {
            InitializeComponent();
            _currentInputStatus = InputStatus.None;
        }

        public void UpdateInputStatus(InputStatus newInputStatus)
        {
            _currentInputStatus = newInputStatus;

            OnPropertyChanged("StatusIconVisibility");
            OnPropertyChanged("StatusColorBrush");
            OnPropertyChanged("StatusIcon");
            OnPropertyChanged("StatusBorderThickness");
        }

        #region Callbacks
        private static void OnInputTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var uc = d as InputTextBox;
            var value = e.NewValue;

            uc.SetValue(InputTextProperty, value);
            uc.OnPropertyChanged("InputText");
            uc.OnPropertyChanged("PlaceHolderVisibility");
            uc.UpdateInputStatus(InputStatus.None);
        }
        #endregion

        #region INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
