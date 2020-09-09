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
        private ViewModel _viewModel;

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            nameof(Value), typeof(bool), typeof(SingleCoilCommandModel), new PropertyMetadata(default(bool)));

        public SingleCoilCommandModel(ushort address, bool value, ViewModel viewModel) 
            : base(address, viewModel.ConfigurationSettings)
        {
            _viewModel = viewModel;
            Value = value;
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

        protected override void AddressChanged(ushort newValue, ushort oldValue)
        {
        }
    }
}