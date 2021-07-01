using FontAwesome.WPF;
using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Forge_Modding_Helper_3.Utils
{
    /// <summary>
    /// Class used to not lock opened external pictures in a listview
    /// Only for WPF binding converter
    /// </summary>
    public class StringToImageConverter : IValueConverter
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
