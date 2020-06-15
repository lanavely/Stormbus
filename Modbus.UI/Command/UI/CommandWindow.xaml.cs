using System.Collections.Generic;
using System.Windows;
using NModbus;
using Stormbus.UI.Command.CommandData;
using Stormbus.UI.Configuration;

namespace Stormbus.UI.Command.UI
{
    /// <summary>
    ///     Interaction logic for CommandWindow.xaml
    /// </summary>
    public partial class CommandWindow : Window
    {
        public static readonly DependencyProperty CommandDataProperty = DependencyProperty.Register(
            nameof(CommandData), typeof(CommandDataBase), typeof(CommandWindow),
            new PropertyMetadata(default(CommandDataBase)));

        private readonly ConfigurationSettingsModel _configurationSettings;

        public CommandWindow(List<ResultItemModel> items, ConfigurationSettingsModel configurationSettings)
        {
            _configurationSettings = configurationSettings;
            switch (configurationSettings.Function)
            {
                case ModbusFunctionCodes.ReadCoils:
                    if (items.Count == 1)
                    {
                        //CommandData = new SingleCoilCommandData(items[0].Address, (bool)items[0].Value);
                    }

                    if (items.Count > 1)
                    {
                        //CommandData = new MultipleCoilCommandData(items[0].Address, (ushort)items[0].Value,
                        //    (List<ResultItemModel>)((new List<ResultItemModel>(items)).Clone()));
                    }

                    break;
                case ModbusFunctionCodes.ReadHoldingRegisters:
                    //CommandData = new SingleRegisterCommandData(items[0].Address, (ushort)items[0].Value);
                    break;
            }

            InitializeComponent();
        }

        public CommandDataBase CommandData
        {
            get => (CommandDataBase) GetValue(CommandDataProperty);
            set => SetValue(CommandDataProperty, value);
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}