using FMH.Core.Utils.UI;
using FontAwesome.WPF;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FMH.Core.UI.Controls
{
    public partial class SideBarMenuButton : UserControl, ICommandSource, INotifyPropertyChanged
    {
        public static readonly DependencyProperty IconProperty = DependencyProperty.Register("Icon", typeof(FontAwesomeIcon), typeof(Image));
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(SideBarMenuButton));
        public static readonly DependencyProperty TextTranslationKeyProperty = DependencyProperty.Register("TextTranslationKey", typeof(string), typeof(SideBarMenuButton));
        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), typeof(SideBarMenuButton));
        public static readonly DependencyProperty WidthModeProperty = DependencyProperty.Register("WidthMode", typeof(WidthMode), typeof(SideBarMenuButton));
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(SideBarMenuButton));
        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register("CommandParameter", typeof(object), typeof(SideBarMenuButton));
        public static readonly DependencyProperty CommandTargetProperty = DependencyProperty.Register("CommandTarget", typeof(IInputElement), typeof(SideBarMenuButton));


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

        [Description("Command of the button"), Category("Common Properties")]
        public ICommand Command
        {
            get
            {
                return (ICommand)GetValue(CommandProperty);
            }
            set
            {
                SetValue(CommandProperty, value);
                OnPropertyChanged("Command");
            }
        }

        [Description("Command parameter"), Category("Common Properties")]
        public object CommandParameter
        {
            get 
            {
                return GetValue(CommandParameterProperty);
            }
            set
            {
                SetValue(CommandParameterProperty, value);
                OnPropertyChanged("CommandParameter");
            }
        }

        [Description("Command target"), Category("Common Properties")]
        public IInputElement CommandTarget
        {
            get
            {
                return (IInputElement)GetValue(CommandTargetProperty);
            }
            set
            {
                SetValue(CommandTargetProperty, value);
                OnPropertyChanged("CommandTarget");
            }
        }

        public SideBarMenuButton() : base()
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
