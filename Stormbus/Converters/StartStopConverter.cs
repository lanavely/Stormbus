using System;
using System.Globalization;
using System.Windows.Data;
using Stormbus.UI.Enums;

namespace Stormbus.UI.Converters
{
    public class StartStopConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is WorkingStates workingState)) return value;
            return workingState == WorkingStates.Stopped ? @"Start" : @"Stop";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}