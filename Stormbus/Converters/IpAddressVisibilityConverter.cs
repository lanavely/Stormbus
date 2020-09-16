using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Stormbus.UI.Containers;

namespace Stormbus.UI.Converters
{
    public class IpAddressVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string connectionType)
                return connectionType == ConnectionType.Serial ? Visibility.Collapsed : Visibility.Visible;

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}