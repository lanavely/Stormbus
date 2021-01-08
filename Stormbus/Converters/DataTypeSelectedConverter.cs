using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Stormbus.UI.Enums;

namespace Stormbus.UI.Converters
{
    public class DataTypeSelectedConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is string tag && values[1] is DataType dataType)
                return tag == dataType.ToString() ? Visibility.Visible : Visibility.Collapsed;

            return values;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}