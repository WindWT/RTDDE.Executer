using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace RTDDE.Executer.Util
{
    public class HexcolorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string hex = value as string;
            if (string.IsNullOrEmpty(hex)) {
                return null;
            }
            try {
                //no way to avoid exception
                return (SolidColorBrush)new BrushConverter().ConvertFromString(hex);
            }
            catch (Exception) {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
