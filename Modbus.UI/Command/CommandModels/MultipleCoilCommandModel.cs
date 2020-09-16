using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using Stormbus.UI.Command.CommandData;
using Stormbus.UI.Configuration;
using Stormbus.UI.Helper;
using Stormbus.UI.ViewModels;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace Stormbus.UI.Command.CommandModels
{
    /// <summary>
    ///     Model for function 15
    /// </summary>
    public class MultipleCoilCommandModel : CommandModelBase
    {
        private ushort _count;

        public MultipleCoilCommandModel(ushort address, ushort count) 
            : base(address)
        {
            _count = count;
            Items = CommandHelper.GenerateSignalModelList<bool>(Address, Count);
        }

        /// <summary>
        ///     Number of signals in the command
        /// </summary>
        public ushort Count
        {
            get => _count;
            set
            {
                if (value <= 0)
                    value = 1;
                _count = value;
                CountChanged(value, _count);
            }
        }

        /// <summary>
        ///     Signals for editing in the command
        /// </summary>
        public ObservableCollection<ResultItemModel> Items { get; set; }

        public override CommandDataBase GetCommandData()
        {
            CommandDataBase commandData = null;
            if (Items.Count > 1)
                commandData = new MultipleCoilCommandData()
                {
                    Address = Address,
                    Values = Items.Select(i => (bool)i.Value).ToArray()
                };
            if (Items.Count == 1)
                commandData = new SingleCoilCommandData()
                {
                    Address = Address,
                    Value = (bool)Items[0].Value
                };

            return commandData;
        }

        protected override void AddressChanged(ushort newValue, ushort oldValue)
        {
            CommandHelper.UpdateItemsByAddressChanged(Items, newValue, oldValue);
        }

        /// <summary>
        ///    Updates the list of signals for the command
        /// </summary>
        private void CountChanged(ushort newValue, ushort oldValue)
        {
            CommandHelper.UpdateItemsByCountChanged<bool>(Items, newValue, oldValue, Address);
        }
    }
}