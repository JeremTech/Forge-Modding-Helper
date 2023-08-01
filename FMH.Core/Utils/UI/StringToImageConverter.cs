using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.IO;
using FontAwesome.WPF;

namespace FMH.Core.Utils.UI
{
    internal class StringToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            object result = null;
            var path = value as string;

            if (!string.IsNullOrEmpty(path) && File.Exists(path) && Path.GetExtension(path).Equals(".png"))
            {
                using (var stream = File.OpenRead(path))
                {
                    var image = new BitmapImage();
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.StreamSource = stream;
                    image.EndInit();
                    result = image;
                }
            }
            else
            {
                result = ImageAwesome.CreateImageSource(FontAwesomeIcon.FileCodeOutline, Brushes.White);
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
