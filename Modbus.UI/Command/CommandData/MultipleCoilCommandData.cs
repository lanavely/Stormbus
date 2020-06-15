using System.Collections.Generic;

namespace Stormbus.UI.Command.CommandData
{
    public class MultipleCoilCommandData : CommandDataBase
    {
        public MultipleCoilCommandData(ushort address, ushort count, List<ResultItemModel> value) : base(address)
        {
            Count = count;
            Value = value;
        }

        public ushort Count { get; set; }
        public List<ResultItemModel> Value { get; set; }
    }
}