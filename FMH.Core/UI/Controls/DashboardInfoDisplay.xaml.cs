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
    public partial class DashboardInfoDisplay : UserControl
    {
        public static readonly DependencyProperty InfoColorProperty;
        public static readonly DependencyProperty InfoTitleProperty;
        public static readonly DependencyProperty InfoContentProperty;
        public static readonly DependencyProperty InfoImageSourceProperty;

        [Description("The text displayed at the top of the control"), Category("Common Properties")]
        public string InfoTitle
        {
            get
            {
                return (string)GetValue(InfoTitleProperty);
            }
            set
            {
                SetValue(InfoTitleProperty, value);
            }
        }

        [Description("The text displayed at the bottom of the control"), Category("Common Properties")]
        public string InfoContent
        {
            get
            {
                return (string)GetValue(InfoContentProperty);
            }
            set
            {
                SetValue(InfoContentProperty, value);
            }
        }

        [Description("The color behind the icon")]
        public Brush InfoColor
        {
            get
            {
                return (Brush)GetValue(InfoColorProperty);
            }
            set
            {
                SetValue(InfoColorProperty, value);
            }
        }

        [Description("Icon"), Category("Common Properties")]
        public ImageSource InfoImageSource
        {
            get
            {
                return (ImageSource)GetValue(InfoImageSourceProperty);
            }
            set
            {
                SetValue(InfoImageSourceProperty, value);
            }
        }

        static DashboardInfoDisplay()
        {
            InfoColorProperty = DependencyProperty.Register("InfoColor", typeof(Brush), typeof(UserControl));
            InfoTitleProperty = DependencyProperty.Register("InfoTitle", typeof(string), typeof(UserControl));
            InfoContentProperty = DependencyProperty.Register("InfoContent", typeof(string), typeof(UserControl));
            InfoImageSourceProperty = DependencyProperty.Register("InfoImageSource", typeof(ImageSource), typeof(Image));
        }

        public DashboardInfoDisplay()
        {
            InitializeComponent();
            this.DataContext = this;
            this.InfoColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#000000"));
        }
    }
}
