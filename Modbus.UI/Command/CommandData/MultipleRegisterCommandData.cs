using System.Collections.Generic;

namespace Stormbus.UI.Command.CommandData
{
    public class MultipleRegisterCommandData : CommandDataBase
    {
        public MultipleRegisterCommandData(ushort address, ushort count, List<ushort> value) : base(address)
        {
            Count = count;
            Value = value;
        }

        public ushort Count { get; set; }
        public List<ushort> Value { get; set; }
    }
}