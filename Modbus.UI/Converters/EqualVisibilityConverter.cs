using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Stormbus.UI.Converters
{
    public class EqualVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Visibility.Collapsed;

            return value.Equals(System.Convert.ToInt32(parameter)) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}