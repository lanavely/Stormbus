using Stormbus.UI.Command.CommandData;

namespace Stormbus.UI.Command.CommandModels
{
    public class SingleRegisterCommandModel : CommandModelBase
    {
        public SingleRegisterCommandModel(ushort address)
            : base(address)
        {
        }

        public ushort Value { get; set; }


        public override CommandDataBase GetCommandData()
        {
            return new SingleRegisterCommandData {Address = Address, Value = Value};
        }
    }
}