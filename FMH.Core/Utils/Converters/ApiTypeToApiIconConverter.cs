using FMH.Workspace.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace FMH.Core.Utils.Converters
{
    public class ApiTypeToApiIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var apiType = (ModAPIType)value;

            switch (apiType)
            {
                case ModAPIType.Forge:
                    return new BitmapImage(new Uri("pack://application:,,,/FMH.Resources;component/Icons/forge_icon.png"));
                default:
                    return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
