using System.Windows;

namespace Stormbus.UI.Command.CommandData
{
    public class SingleCoilCommandData : CommandDataBase
    {
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            nameof(Value), typeof(bool), typeof(SingleCoilCommandData), new PropertyMetadata(default(bool)));

        public SingleCoilCommandData(ushort address, bool value) : base(address)
        {
            Value = value;
        }

        public bool Value
        {
            get => (bool) GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }
    }
}