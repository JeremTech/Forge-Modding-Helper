using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using FontAwesome.WPF;

namespace FMH.Core.UI.Controls
{
    /// <summary>
    /// Interaction logic for StandardButton.xaml
    /// </summary>
    public partial class StandardButton : UserControl, ICommandSource, INotifyPropertyChanged
    {
        public static readonly DependencyProperty LeftIconProperty = DependencyProperty.Register("LeftIcon", typeof(FontAwesomeIcon), typeof(Image), new PropertyMetadata(FontAwesomeIcon.None));
        public static readonly DependencyProperty RightIconProperty = DependencyProperty.Register("RightIcon", typeof(FontAwesomeIcon), typeof(Image), new PropertyMetadata(FontAwesomeIcon.None));
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(StandardButton));
        public static readonly DependencyProperty TextTranslationKeyProperty = DependencyProperty.Register("TextTranslationKey", typeof(string), typeof(StandardButton));
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(StandardButton));
        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register("CommandParameter", typeof(object), typeof(StandardButton));
        public static readonly DependencyProperty CommandTargetProperty = DependencyProperty.Register("CommandTarget", typeof(IInputElement), typeof(StandardButton));

        #region Properties
        [Description("Left icon"), Category("Common Properties")]
        public FontAwesomeIcon LeftIcon
        {
            get
            {
                return (FontAwesomeIcon)GetValue(LeftIconProperty);
            }
            set
            {
                SetValue(LeftIconProperty, value);
                OnPropertyChanged("LeftIcon");
            }
        }

        [Description("Right icon"), Category("Common Properties")]
        public FontAwesomeIcon RightIcon
        {
            get
            {
                return (FontAwesomeIcon)GetValue(RightIconProperty);
            }
            set
            {
                SetValue(RightIconProperty, value);
                OnPropertyChanged("RightIcon");
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
        #endregion

        public StandardButton() : base()
        {
            InitializeComponent();
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
