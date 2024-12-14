using FontAwesome.WPF;
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

namespace FMH.Core.UI.Controls
{
    public partial class ImageRadioButton : RadioButton, INotifyPropertyChanged
    {
        public static readonly DependencyProperty ImageProperty = DependencyProperty.Register("Image", typeof(string), typeof(Image));
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(RadioButton));
        public static readonly DependencyProperty TextTranslationKeyProperty = DependencyProperty.Register("TextTranslationKey", typeof(string), typeof(RadioButton));

        [Description("Image"), Category("Common Properties")]
        public string Image
        {
            get
            {
                return (string)GetValue(ImageProperty);
            }
            set
            {
                SetValue(ImageProperty, value);
                OnPropertyChanged("Image");
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

        public ImageRadioButton()
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
