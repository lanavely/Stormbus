using System.Configuration;
using System.Windows;
using Stormbus.UI.Command.CommandData;
using Stormbus.UI.Configuration;
using Stormbus.UI.ViewModels;

namespace Stormbus.UI.Command.CommandModels
{
    /// <summary>
    ///     Model for function 5
    /// </summary>
    public class SingleCoilCommandModel : CommandModelBase
    {
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            nameof(Value), typeof(bool), typeof(SingleCoilCommandModel), new PropertyMetadata(default(bool)));

        public SingleCoilCommandModel(ushort address) 
            : base(address)
        {
        }

        /// <summary>
        ///     Signal for editing in the command
        /// </summary>
        public bool Value
        {
            get => (bool) GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public override CommandDataBase GetCommandData()
        {
            return new SingleCoilCommandData()
            {
                Address = Address,
                Value = Value
            };
        }
    }
}