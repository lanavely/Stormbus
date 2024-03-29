﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using NModbus;
using Stormbus.UI.Command.CommandModels;
using Stormbus.UI.ViewModels;

namespace Stormbus.UI.Command.UI
{
    /// <summary>
    ///     Interaction logic for CommandWindow.xaml
    /// </summary>
    public partial class CommandWindow
    {
        public static readonly DependencyProperty CommandModelProperty = DependencyProperty.Register(
            nameof(CommandModel), typeof(CommandModelBase), typeof(CommandWindow),
            new PropertyMetadata(default(CommandModelBase)));

        private readonly ViewModel _viewModel;

        public CommandWindow(List<ResultItemModel> selectedItems, ViewModel model)
        {
            _viewModel = model;
            var address = selectedItems.FirstOrDefault()?.Address ?? 0;
            var count = Convert.ToUInt16(selectedItems.Count > 0 ? selectedItems.Count : 1);

            // get view model depending on function and signal count
            switch (_viewModel.ConfigurationSettings.Function)
            {
                case ModbusFunctionCodes.ReadCoils:
                    if (count == 1)
                        CommandModel = new SingleCoilCommandModel(address, _viewModel.ConfigurationSettings);

                    if (count > 1)
                        CommandModel = new MultipleCoilCommandModel(address, count, _viewModel.ConfigurationSettings);

                    break;
                case ModbusFunctionCodes.ReadHoldingRegisters:
                    if (count == 1)
                        CommandModel = new SingleRegisterCommandModel(address, _viewModel.ConfigurationSettings);

                    if (count > 1)
                        CommandModel =
                            new MultipleRegisterCommandModel(address, count, _viewModel.ConfigurationSettings);

                    break;
            }

            InitializeComponent();
        }

        public CommandModelBase CommandModel
        {
            get => (CommandModelBase) GetValue(CommandModelProperty);
            set => SetValue(CommandModelProperty, value);
        }

        private void SendButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (CommandModel != null)
                _viewModel.ModbusClient.ExecuteCommandAsync(CommandModel.GetCommandData());
            Close();
        }
    }
}