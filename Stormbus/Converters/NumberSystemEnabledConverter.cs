using System;
using System.Globalization;
using System.Windows.Data;
using Stormbus.UI.Enums;

namespace Stormbus.UI.Converters
{
    public class NumberSystemEnabledConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DataType dataType) return !(dataType == DataType.Float || dataType == DataType.Double);

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}