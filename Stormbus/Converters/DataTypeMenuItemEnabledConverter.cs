using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace Stormbus.UI.Converters
{
    public class DataTypeMenuItemEnabledConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is byte function)
            {
                var functionsWithDataType = new byte[] {3, 4, 6, 16};
                return functionsWithDataType.Contains(function);
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}