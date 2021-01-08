using Stormbus.UI.Command.CommandData;
using Stormbus.UI.Command.CommandHelpers;
using Stormbus.UI.Configuration;
using Stormbus.UI.Converters;

namespace Stormbus.UI.Command.CommandModels
{
    public class SingleRegisterCommandModel : CommandModelBase
    {
        private object _value;

        public SingleRegisterCommandModel(ushort address, ConfigurationSettingsModel settings)
            : base(address, settings)
        {
            Value = DataTypeHelper.GetDefaultValue(settings.DataType);
        }

        public object Value
        {
            get => _value;
            set
            {
                if (_value == null || !(value is string strValue))
                {
                    _value = value;
                    return;
                }

                _value = DataTypeHelper.ConvertToType(_value.GetType(), strValue);
            }
        }

        public override CommandDataBase GetCommandData()
        {
            var registers = ModbusDataTypesConverter.ConvertToRegisters(Value, Settings);
            if (registers.Length == 1)
                return new SingleRegisterCommandData
                {
                    Address = Address,
                    Value = registers[0]
                };
            if (registers.Length > 1)
                return  new MultipleRegisterCommandData
                {
                    Address = Address,
                    Values = registers
                };

            return null;
        }
    }
}