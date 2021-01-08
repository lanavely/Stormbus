using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Stormbus.UI.Command.CommandData;
using Stormbus.UI.Configuration;
using Stormbus.UI.Converters;
using Stormbus.UI.Helper;

namespace Stormbus.UI.Command.CommandModels
{
    public class MultipleRegisterCommandModel : CommandModelBase
    {
        private ushort _count;

        public MultipleRegisterCommandModel(ushort address, ushort count, ConfigurationSettingsModel settings)
            : base(address, settings)
        {
            Count = count;
            Items = CommandHelper.GenerateSignalModelList<ushort>(Address, Count);
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
            var registers = new List<ushort>();
            foreach (var item in Items)
            {
                registers.AddRange(ModbusDataTypesConverter.ConvertToRegisters(item, Settings));
            }

            if (registers.Count == 1)
                return new SingleRegisterCommandData
                {
                    Address = Address,
                    Value = registers[0]
                };
            if (registers.Count > 1)
                return  new MultipleRegisterCommandData
                {
                    Address = Address,
                    Values = registers.ToArray()
                };
            

            return null;
        }

        protected override void AddressChanged(ushort newValue, ushort oldValue)
        {
            CommandHelper.UpdateItemsByAddressChanged(Items, newValue, oldValue);
        }

        /// <summary>
        ///     Updates the list of signals for the command
        /// </summary>
        private void CountChanged(ushort newValue, ushort oldValue)
        {
            CommandHelper.UpdateItemsByCountChanged<ushort>(Items, newValue, oldValue, Address);
        }
    }
}