using System;
using System.Collections.Generic;
using System.Linq;
using Stormbus.UI.Command.CommandData;
using Stormbus.UI.Configuration;
using Stormbus.UI.ViewModels;

namespace Stormbus.UI.Command.CommandModels
{
    /// <summary>
    ///     Model for function 15
    /// </summary>
    public class MultipleCoilCommandModel : CommandModelBase
    {
        private ViewModel _viewModel;

        public MultipleCoilCommandModel(ushort address, ushort count, List<ResultItemModel> selectedItems, ViewModel viewModel) 
            : base(address, viewModel.ConfigurationSettings)
        {
            _viewModel = viewModel;
            _count = count;
            _selectedItems = selectedItems;
            CountChanged(count, 0);
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
                CountChanged(value, _count);
                _count = value;
            }
        }

        private List<ResultItemModel> _selectedItems;
        private ushort _count;

        /// <summary>
        ///     Signals for editing in the command
        /// </summary>
        public List<ResultItemModel> Values { get; set; } = new List<ResultItemModel>();

        public override CommandDataBase GetCommandData()
        {
            CommandDataBase commandData = null;
            if (Values.Count > 1)
                commandData = new MultipleCoilCommandData()
                {
                    Address = Address,
                    Values = Values.Select(i => (bool)i.Value).ToArray()
                };
            if (Values.Count == 1)
                commandData = new SingleCoilCommandData()
                {
                    Address = Address,
                    Value = (bool)Values[0].Value
                };

            return commandData;
        }

        protected override void AddressChanged(ushort newValue, ushort oldValue)
        {
            if (newValue > oldValue)
            {
                var removeCount = newValue - oldValue; // getting the number of signals that need to be removed from the beginning and added later
                removeCount = removeCount > Values.Count ? Values.Count : removeCount; // validate
                Values.RemoveRange(0, removeCount); 
                var addingStartAddress = oldValue + Count > newValue ? newValue : oldValue + newValue; // the offset address for adding signals to the list
                
                Values.AddRange(Enumerable.Range(addingStartAddress, removeCount).Select(i => new ResultItemModel {Address = Convert.ToUInt16(i), Value = default(bool)}));
            }

            if (newValue < oldValue)
            {
                //var removeCount = oldValue - newValue;
                //removeCount = removeCount > Values.Count ? Values.Count : removeCount;
                //Values.RemoveRange(Values.Count - removeCount, removeCount);
                //Values.InsertRange(0, Enumerable.Range(0, oldValue - newValue).Select(i => default(bool)));
            }
        }

        /// <summary>
        ///    Updates the list of signals for the command
        /// </summary>
        private void CountChanged(ushort newValue, ushort oldValue)
        {
            if (newValue > oldValue)
            {
                var rangeStartIndex = Address - ConfigurationSettings.StartAddress + oldValue;
                var deltaCount = newValue - oldValue;
                var fillDefaultCount = 0;
                if (rangeStartIndex + deltaCount > _selectedItems.Count)
                {
                    fillDefaultCount = rangeStartIndex + deltaCount - _selectedItems.Count;
                }
                Values.AddRange(_selectedItems.GetRange(rangeStartIndex, deltaCount - fillDefaultCount));
                Values.AddRange(Enumerable.Range(0, fillDefaultCount).Select(i => new ResultItemModel()));//TODO: to finish the assignment address
            }

            if (newValue < oldValue)
            {
                Values.RemoveRange(newValue, oldValue - newValue);
            }
        }
    }
}