using System.Windows;
using Stormbus.UI.Command.CommandData;
using Stormbus.UI.Configuration;

namespace Stormbus.UI.Command.CommandModels
{
    public abstract class CommandModelBase : DependencyObject
    {
        public static readonly DependencyProperty AddressProperty = DependencyProperty.Register(
            nameof(Address), typeof(ushort), typeof(CommandModelBase),
            new FrameworkPropertyMetadata(default(ushort),
                (d, e) => { (d as CommandModelBase)?.AddressChanged((ushort) e.NewValue, (ushort) e.OldValue); }));

        protected CommandModelBase(ushort address, ConfigurationSettingsModel settings)
        {
            Address = address;
            Settings = settings;
        }

        public ConfigurationSettingsModel Settings { get; }
        
        /// <summary>
        ///     Command address
        /// </summary>
        public ushort Address
        {
            get => (ushort) GetValue(AddressProperty);
            set => SetValue(AddressProperty, value);
        }

        /// <summary>
        ///     Returns the data needed to execute the command by modbus client
        /// </summary>
        public abstract CommandDataBase GetCommandData();

        /// <summary>
        ///     Updates the list of signals for the command
        /// </summary>
        protected virtual void AddressChanged(ushort newValue, ushort oldValue)
        {
            // do nothing
        }
    }
}