using System;
using System.Windows;

namespace Stormbus.UI
{
    public class ResultItemModel : DependencyObject, ICloneable
    {
        public static readonly DependencyProperty AddressProperty = DependencyProperty.Register(
            nameof(Address), typeof(ushort), typeof(ResultItemModel));

        public static readonly DependencyProperty DisplayValueProperty = DependencyProperty.Register(
            nameof(DisplayValue), typeof(string), typeof(ResultItemModel), new PropertyMetadata(default(string)));

        public ushort Address
        {
            get => (ushort) GetValue(AddressProperty);
            set => SetValue(AddressProperty, value);
        }

        public string DisplayValue
        {
            get => (string) GetValue(DisplayValueProperty);
            set => SetValue(DisplayValueProperty, value);
        }

        public object Value { get; set; }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}