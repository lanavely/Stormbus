using System.Windows;

namespace Stormbus.UI.Command.CommandData
{
    public abstract class CommandDataBase : DependencyObject
    {
        public static readonly DependencyProperty AddressProperty = DependencyProperty.Register(
            nameof(Address), typeof(ushort), typeof(CommandDataBase), new PropertyMetadata(default(ushort)));

        protected CommandDataBase(ushort address)
        {
            Address = address;
        }

        public ushort Address
        {
            get => (ushort) GetValue(AddressProperty);
            set => SetValue(AddressProperty, value);
        }
    }
}