using FMH.Core.Utils.UI;
using FontAwesome.WPF;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FMH.Core.UI.Controls
{
    public partial class SideBarMenuButton : UserControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty IconProperty;
        public static readonly DependencyProperty TextProperty;
        public static readonly DependencyProperty TextTranslationKeyProperty;
        public static readonly DependencyProperty IsSelectedProperty;
        public static readonly DependencyProperty WidthModeProperty;

        [Description("Icon"), Category("Common Properties")]
        public FontAwesomeIcon Icon
        {
            get
            {
                return (FontAwesomeIcon)GetValue(IconProperty);
            }
            set
            {
                SetValue(IconProperty, value);
                OnPropertyChanged("Icon");
            }
        }

        [Description("The text displayed in the button"), Category("Common Properties")]
        public string Text
        {
            get
            {
                return (string)GetValue(TextProperty);
            }
            set
            {
                SetValue(TextProperty, value);
                OnPropertyChanged("Text");
            }
        }

        [Description("The translation key for text displayed in the button"), Category("Common Properties")]
        public string TextTranslationKey
        {
            get
            {
                return (string)GetValue(TextTranslationKeyProperty);
            }
            set
            {
                SetValue(TextTranslationKeyProperty, value);
                OnPropertyChanged("TextTranslationKey");
            }
        }

        [Description("Is the button currently selected ?"), Category("Common Properties")]
        public bool IsSelected
        {
            get
            {
                return (bool)GetValue(IsSelectedProperty);
            }
            set
            {
                SetValue(IsSelectedProperty, value);
                OnPropertyChanged("IsSelected");
            }
        }

        [Description("Width mode of the button"), Category("Common Properties")]
        public WidthMode WidthMode
        {
            get
            {
                return (WidthMode)GetValue(WidthModeProperty);
            }
            set
            {
                SetValue(WidthModeProperty, value);
                OnWidthModeChanged();
                OnPropertyChanged("WidthMode");
            }
        }

        static SideBarMenuButton()
        {
            IconProperty = DependencyProperty.Register("IconProperty", typeof(FontAwesomeIcon), typeof(Image));
            TextProperty = DependencyProperty.Register("TextProperty", typeof(string), typeof(UserControl));
            TextTranslationKeyProperty = DependencyProperty.Register("TextTranslationKeyProperty", typeof(string), typeof(UserControl));
            IsSelectedProperty = DependencyProperty.Register("IsSelectedProperty", typeof(bool), typeof(UserControl));
            WidthModeProperty = DependencyProperty.Register("WidthModeProperty", typeof(WidthMode), typeof(UserControl));
        }

        public SideBarMenuButton()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private void OnWidthModeChanged()
        {
            switch(this.WidthMode)
            {
                case WidthMode.Compact:
                    this.TextBlock.Visibility = Visibility.Collapsed;
                    this.ContentGrid.Margin = new Thickness(0, 0, 5, 0);
                    break;
                default:
                    this.TextBlock.Visibility = Visibility.Visible;
                    this.ContentGrid.Margin = new Thickness(0, 0, 10, 0);
                    break;
            }

        }

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
