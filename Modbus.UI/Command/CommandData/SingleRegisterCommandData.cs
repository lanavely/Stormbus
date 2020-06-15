namespace Stormbus.UI.Command.CommandData
{
    public class SingleRegisterCommandData : CommandDataBase
    {
        public SingleRegisterCommandData(ushort address, ushort value) : base(address)
        {
            Value = value;
        }

        public ushort Value { get; set; }
    }
}